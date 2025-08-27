using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.IO.Ports;
using Microsoft.Extensions.Logging;
using Renci.SshNet;
using Polly;
// using Polly.Extensions.Http; // Not needed for this implementation
using NetToolkit.Modules.SshTerminal.Models;
using NetToolkit.Modules.SshTerminal.Interfaces;

namespace NetToolkit.Modules.SshTerminal.Services;

/// <summary>
/// SSH terminal service - the omniscient conductor of digital connection symphonies
/// Where terminal dreams become reality and hardware whispers become flowing conversations
/// </summary>
public class SshTerminalService : ISshTerminalHost, IDisposable
{
    private readonly ILogger<SshTerminalService> _logger;
    private readonly ISshEventPublisher _eventPublisher;
    private readonly IEmulationEngine _emulationEngine;
    
    // Active connections and their states
    private readonly ConcurrentDictionary<string, SessionHandle> _activeSessions = new();
    private readonly ConcurrentDictionary<string, SshClient> _sshConnections = new();
    private readonly ConcurrentDictionary<string, SerialPort> _serialConnections = new();
    private readonly ConcurrentDictionary<string, TerminalBuffer> _terminalBuffers = new();
    private readonly ConcurrentDictionary<string, CancellationTokenSource> _sessionCancellations = new();
    
    // Resilience policies
    private readonly IAsyncPolicy _retryPolicy;
    private readonly IAsyncPolicy _circuitBreakerPolicy;
    
    private readonly SemaphoreSlim _connectionSemaphore = new(10, 10); // Max 10 concurrent connections
    private volatile bool _disposed;
    
    public SshTerminalService(
        ILogger<SshTerminalService> logger,
        ISshEventPublisher eventPublisher,
        IEmulationEngine emulationEngine)
    {
        _logger = logger;
        _eventPublisher = eventPublisher;
        _emulationEngine = emulationEngine;
        
        // Configure resilience policies with cosmic wisdom
        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    _logger.LogWarning("🔄 Connection retry attempt {RetryCount} after {Delay}ms - persistence conquers adversity!",
                        retryCount, timespan.TotalMilliseconds);
                });
        
        // Note: Circuit breaker pattern implementation simplified for this demo
        _circuitBreakerPolicy = _retryPolicy; // Using retry policy as fallback
        
        _logger.LogInformation("🚀 SSH Terminal Service initialized - ready to bridge realms with digital elegance!");
    }
    
    #region ISshTerminalHost Implementation
    
    /// <summary>
    /// Connect to remote or local device - the grand ceremony of digital bridge construction
    /// </summary>
    public async Task<SessionHandle> ConnectAsync(ConnectionParams parameters, CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(SshTerminalService));
        
        var sessionHandle = new SessionHandle
        {
            SessionId = parameters.SessionId,
            Type = parameters.Type,
            Status = ConnectionStatus.Connecting,
            DisplayName = parameters.DisplayName
        };
        
        _activeSessions[parameters.SessionId] = sessionHandle;
        _terminalBuffers[parameters.SessionId] = new TerminalBuffer();
        
        var cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _sessionCancellations[parameters.SessionId] = cancellationSource;
        
        _logger.LogInformation("🌉 Initiating {ConnectionType} connection to {Target} - digital bridge construction begins!",
            parameters.Type, GetConnectionTarget(parameters));
        
        var startTime = DateTime.Now;
        
        try
        {
            await _connectionSemaphore.WaitAsync(cancellationToken);
            
            sessionHandle.Status = ConnectionStatus.Connecting;
            await _eventPublisher.PublishSessionStatusChangedAsync(
                parameters.SessionId, ConnectionStatus.Disconnected, ConnectionStatus.Connecting);
            
            // Establish connection based on type
            var success = parameters.Type switch
            {
                ConnectionType.SSH => await ConnectSshAsync(parameters, cancellationSource.Token),
                ConnectionType.Serial => await ConnectSerialAsync(parameters, cancellationSource.Token),
                ConnectionType.Bluetooth => await ConnectBluetoothAsync(parameters, cancellationSource.Token),
                ConnectionType.Telnet => await ConnectTelnetAsync(parameters, cancellationSource.Token),
                _ => throw new NotSupportedException($"Connection type {parameters.Type} not supported")
            };
            
            if (success)
            {
                sessionHandle.Status = ConnectionStatus.Connected;
                sessionHandle.ConnectedAt = DateTime.Now;
                sessionHandle.LastActivity = DateTime.Now;
                
                var connectionDuration = DateTime.Now - startTime;
                
                await _eventPublisher.PublishConnectionEstablishedAsync(
                    parameters.SessionId, parameters, connectionDuration);
                
                // Start data monitoring
                _ = Task.Run(() => MonitorSessionDataAsync(parameters.SessionId, cancellationSource.Token), cancellationSource.Token);
                
                _logger.LogInformation("✨ {ConnectionType} connection established to {Target} in {Duration:F2}s - digital harmony achieved!",
                    parameters.Type, GetConnectionTarget(parameters), connectionDuration.TotalSeconds);
                
                ConnectionEstablished?.Invoke(this, new ConnectionEstablishedEvent
                {
                    SessionId = parameters.SessionId,
                    ConnectionParams = parameters,
                    ConnectionDuration = connectionDuration
                });
                
                return sessionHandle;
            }
            else
            {
                sessionHandle.Status = ConnectionStatus.Failed;
                await _eventPublisher.PublishSessionStatusChangedAsync(
                    parameters.SessionId, ConnectionStatus.Connecting, ConnectionStatus.Failed);
                
                throw new InvalidOperationException($"Failed to establish {parameters.Type} connection");
            }
        }
        catch (OperationCanceledException)
        {
            sessionHandle.Status = ConnectionStatus.Timeout;
            _logger.LogWarning("⏰ Connection to {Target} was cancelled or timed out - time's arrow defeated our quest!",
                GetConnectionTarget(parameters));
            
            await _eventPublisher.PublishSessionStatusChangedAsync(
                parameters.SessionId, ConnectionStatus.Connecting, ConnectionStatus.Timeout);
            
            throw;
        }
        catch (Exception ex)
        {
            sessionHandle.Status = ConnectionStatus.Failed;
            _logger.LogError(ex, "💥 Failed to connect to {Target} - digital obstacles block our path!",
                GetConnectionTarget(parameters));
            
            await _eventPublisher.PublishErrorEncounteredAsync(
                parameters.SessionId, ex.Message, ex, "Connection establishment");
            
            ErrorEncountered?.Invoke(this, new ErrorEncounteredEvent
            {
                SessionId = parameters.SessionId,
                ErrorMessage = ex.Message,
                Exception = ex,
                Context = "Connection establishment"
            });
            
            throw;
        }
        finally
        {
            _connectionSemaphore.Release();
        }
    }
    
    /// <summary>
    /// Send command to active session - dispatching digital incantations
    /// </summary>
    public async Task<bool> SendCommandAsync(string sessionId, string command, CancellationToken cancellationToken = default)
    {
        if (!_activeSessions.TryGetValue(sessionId, out var session))
        {
            _logger.LogWarning("❓ Attempted to send command to unknown session {SessionId}", sessionId);
            return false;
        }
        
        var startTime = DateTime.Now;
        
        try
        {
            var success = session.Type switch
            {
                ConnectionType.SSH => await SendSshCommandAsync(sessionId, command, cancellationToken),
                ConnectionType.Serial => await SendSerialCommandAsync(sessionId, command, cancellationToken),
                ConnectionType.Bluetooth => await SendBluetoothCommandAsync(sessionId, command, cancellationToken),
                ConnectionType.Telnet => await SendTelnetCommandAsync(sessionId, command, cancellationToken),
                _ => false
            };
            
            var executionTime = DateTime.Now - startTime;
            
            // Update session activity
            session.LastActivity = DateTime.Now;
            
            // Add to history
            await _emulationEngine.AddCommandToHistoryAsync(sessionId, command, null, success);
            
            // Publish command execution event
            await _eventPublisher.PublishCommandExecutedAsync(sessionId, command, null, success, executionTime);
            
            CommandExecuted?.Invoke(this, new CommandExecutedEvent
            {
                SessionId = sessionId,
                Command = command,
                Success = success,
                ExecutionTime = executionTime
            });
            
            if (success)
            {
                _logger.LogDebug("⚡ Command executed in session {SessionId}: {Command} ({Duration:F2}ms)",
                    sessionId, command.Length > 50 ? command[..50] + "..." : command, executionTime.TotalMilliseconds);
            }
            
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error executing command in session {SessionId}: {Command}", sessionId, command);
            
            await _eventPublisher.PublishErrorEncounteredAsync(sessionId, ex.Message, ex, "Command execution");
            
            return false;
        }
    }
    
    /// <summary>
    /// Send raw data to session - pure byte transmission
    /// </summary>
    public async Task<int> SendRawDataAsync(string sessionId, byte[] data, CancellationToken cancellationToken = default)
    {
        if (!_activeSessions.TryGetValue(sessionId, out var session))
        {
            return 0;
        }
        
        try
        {
            var bytesTransmitted = session.Type switch
            {
                ConnectionType.SSH => await SendSshRawDataAsync(sessionId, data, cancellationToken),
                ConnectionType.Serial => await SendSerialRawDataAsync(sessionId, data, cancellationToken),
                ConnectionType.Bluetooth => await SendBluetoothRawDataAsync(sessionId, data, cancellationToken),
                ConnectionType.Telnet => await SendTelnetRawDataAsync(sessionId, data, cancellationToken),
                _ => 0
            };
            
            session.LastActivity = DateTime.Now;
            
            _logger.LogTrace("📡 Transmitted {ByteCount} bytes to session {SessionId} - raw data flowing through digital channels!",
                bytesTransmitted, sessionId);
            
            return bytesTransmitted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error sending raw data to session {SessionId}", sessionId);
            return 0;
        }
    }
    
    /// <summary>
    /// Get output stream from session - the flowing river of digital consciousness
    /// </summary>
    public async IAsyncEnumerable<ColoredOutput> GetOutputStreamAsync(string sessionId, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (!_terminalBuffers.TryGetValue(sessionId, out var buffer))
        {
            yield break;
        }
        
        var lastLineCount = 0;
        
        while (!cancellationToken.IsCancellationRequested && _activeSessions.ContainsKey(sessionId))
        {
            var currentLines = buffer.GetLines().ToList();
            
            // Yield new lines since last check
            if (currentLines.Count > lastLineCount)
            {
                for (int i = lastLineCount; i < currentLines.Count; i++)
                {
                    if (cancellationToken.IsCancellationRequested) yield break;
                    yield return currentLines[i];
                }
                
                lastLineCount = currentLines.Count;
            }
            
            // Brief pause to prevent excessive CPU usage
            try
            {
                await Task.Delay(50, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                yield break;
            }
        }
    }
    
    /// <summary>
    /// Get current terminal buffer contents
    /// </summary>
    public async Task<TerminalBuffer> GetTerminalBufferAsync(string sessionId, int lastLines = 0)
    {
        await Task.CompletedTask; // Async signature for future expansion
        
        if (!_terminalBuffers.TryGetValue(sessionId, out var buffer))
        {
            return new TerminalBuffer();
        }
        
        if (lastLines <= 0)
        {
            return buffer;
        }
        
        var lines = buffer.GetLines().TakeLast(lastLines);
        var newBuffer = new TerminalBuffer(lastLines);
        
        foreach (var line in lines)
        {
            newBuffer.AddLine(line);
        }
        
        return newBuffer;
    }
    
    /// <summary>
    /// Disconnect from session - graceful digital farewell
    /// </summary>
    public async Task<bool> DisconnectAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        if (!_activeSessions.TryGetValue(sessionId, out var session))
        {
            return false;
        }
        
        try
        {
            // Cancel session monitoring
            if (_sessionCancellations.TryGetValue(sessionId, out var cts))
            {
                cts.Cancel();
                _sessionCancellations.TryRemove(sessionId, out _);
            }
            
            var success = session.Type switch
            {
                ConnectionType.SSH => await DisconnectSshAsync(sessionId, cancellationToken),
                ConnectionType.Serial => await DisconnectSerialAsync(sessionId, cancellationToken),
                ConnectionType.Bluetooth => await DisconnectBluetoothAsync(sessionId, cancellationToken),
                ConnectionType.Telnet => await DisconnectTelnetAsync(sessionId, cancellationToken),
                _ => true
            };
            
            // Update session status
            session.Status = ConnectionStatus.Disconnected;
            await _eventPublisher.PublishSessionStatusChangedAsync(
                sessionId, ConnectionStatus.Connected, ConnectionStatus.Disconnected);
            
            // Clean up resources
            _activeSessions.TryRemove(sessionId, out _);
            _terminalBuffers.TryRemove(sessionId, out _);
            
            _logger.LogInformation("👋 Session {SessionId} disconnected gracefully - digital farewell complete!",
                sessionId);
            
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error disconnecting session {SessionId}", sessionId);
            return false;
        }
    }
    
    /// <summary>
    /// Get all active sessions
    /// </summary>
    public async Task<List<SessionHandle>> GetActiveSessionsAsync()
    {
        await Task.CompletedTask;
        return _activeSessions.Values.ToList();
    }
    
    /// <summary>
    /// Get session state
    /// </summary>
    public async Task<SessionState?> GetSessionStateAsync(string sessionId)
    {
        await Task.CompletedTask;
        
        if (!_activeSessions.TryGetValue(sessionId, out var session))
        {
            return null;
        }
        
        var history = await _emulationEngine.GetCommandHistoryAsync(sessionId, 100);
        
        return new SessionState
        {
            SessionId = sessionId,
            Status = session.Status,
            ConnectedAt = session.ConnectedAt,
            LastActivity = session.LastActivity,
            CommandHistory = history,
            Metadata = session.Properties
        };
    }
    
    /// <summary>
    /// Reconnect session
    /// </summary>
    public async Task<bool> ReconnectSessionAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        // Implementation would restore connection parameters and reconnect
        await Task.CompletedTask;
        
        _logger.LogInformation("🔄 Reconnection requested for session {SessionId} - digital resurrection begins!",
            sessionId);
        
        // This is a placeholder - actual implementation would restore connection params
        return false;
    }
    
    /// <summary>
    /// Close all sessions
    /// </summary>
    public async Task<int> CloseAllSessionsAsync(CancellationToken cancellationToken = default)
    {
        var sessions = _activeSessions.Keys.ToList();
        var closedCount = 0;
        
        foreach (var sessionId in sessions)
        {
            try
            {
                if (await DisconnectAsync(sessionId, cancellationToken))
                {
                    closedCount++;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "⚠️ Error closing session {SessionId} during bulk closure", sessionId);
            }
        }
        
        _logger.LogInformation("🧹 Closed {ClosedCount} of {TotalCount} sessions - digital cleanup complete!",
            closedCount, sessions.Count);
        
        return closedCount;
    }
    
    #endregion
    
    #region Events
    
    public event EventHandler<DataReceivedEvent>? DataReceived;
    public event EventHandler<ConnectionEstablishedEvent>? ConnectionEstablished;
    public event EventHandler<CommandExecutedEvent>? CommandExecuted;
    public event EventHandler<ErrorEncounteredEvent>? ErrorEncountered;
    
    #endregion
    
    #region Connection Type Implementations
    
    /// <summary>
    /// Establish SSH connection
    /// </summary>
    private async Task<bool> ConnectSshAsync(ConnectionParams parameters, CancellationToken cancellationToken)
    {
        try
        {
            var connectionInfo = CreateSshConnectionInfo(parameters);
            var sshClient = new SshClient(connectionInfo);
            
            await Task.Run(() => sshClient.Connect(), cancellationToken);
            
            if (sshClient.IsConnected)
            {
                _sshConnections[parameters.SessionId] = sshClient;
                
                await _eventPublisher.PublishAuthenticationAttemptAsync(
                    parameters.SessionId, parameters.Username ?? "unknown", "password", true, parameters.Host);
                
                return true;
            }
            
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 SSH connection failed to {Host}:{Port}", parameters.Host, parameters.Port);
            
            await _eventPublisher.PublishAuthenticationAttemptAsync(
                parameters.SessionId, parameters.Username ?? "unknown", "password", false, parameters.Host);
            
            return false;
        }
    }
    
    /// <summary>
    /// Establish Serial connection
    /// </summary>
    private async Task<bool> ConnectSerialAsync(ConnectionParams parameters, CancellationToken cancellationToken)
    {
        try
        {
            var serialPort = new SerialPort(
                parameters.SerialPort!,
                parameters.BaudRate,
                parameters.Parity,
                parameters.DataBits,
                parameters.StopBits)
            {
                ReadTimeout = 5000,
                WriteTimeout = 5000
            };
            
            await Task.Run(() => serialPort.Open(), cancellationToken);
            
            if (serialPort.IsOpen)
            {
                _serialConnections[parameters.SessionId] = serialPort;
                
                // Set up data received event
                serialPort.DataReceived += (sender, e) =>
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            var data = serialPort.ReadExisting();
                            if (!string.IsNullOrEmpty(data))
                            {
                                await ProcessReceivedDataAsync(parameters.SessionId, data, ConnectionType.Serial);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "💥 Error reading serial data from session {SessionId}", parameters.SessionId);
                        }
                    });
                };
                
                return true;
            }
            
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Serial connection failed to {Port}", parameters.SerialPort);
            return false;
        }
    }
    
    /// <summary>
    /// Establish Bluetooth connection (placeholder implementation)
    /// </summary>
    private async Task<bool> ConnectBluetoothAsync(ConnectionParams parameters, CancellationToken cancellationToken)
    {
        await Task.Delay(100, cancellationToken); // Simulate connection time
        
        _logger.LogWarning("📡 Bluetooth connection to {Address} - Implementation pending for full Bluetooth RFCOMM support!",
            parameters.BluetoothAddress);
        
        // Placeholder - actual implementation would use Windows.Devices.Bluetooth
        return false;
    }
    
    /// <summary>
    /// Establish Telnet connection (simplified implementation)
    /// </summary>
    private async Task<bool> ConnectTelnetAsync(ConnectionParams parameters, CancellationToken cancellationToken)
    {
        await Task.Delay(100, cancellationToken); // Simulate connection time
        
        _logger.LogWarning("📞 Telnet connection to {Host}:{Port} - Implementation pending for full Telnet protocol support!",
            parameters.Host, parameters.Port);
        
        // Placeholder - actual implementation would use TcpClient
        return false;
    }
    
    #endregion
    
    #region Command Sending Implementations
    
    private async Task<bool> SendSshCommandAsync(string sessionId, string command, CancellationToken cancellationToken)
    {
        if (!_sshConnections.TryGetValue(sessionId, out var sshClient) || !sshClient.IsConnected)
        {
            return false;
        }
        
        try
        {
            using var sshCommand = sshClient.CreateCommand(command);
            var result = await Task.Run(() => sshCommand.Execute(), cancellationToken);
            
            if (!string.IsNullOrEmpty(result))
            {
                await ProcessReceivedDataAsync(sessionId, result, ConnectionType.SSH);
            }
            
            if (!string.IsNullOrEmpty(sshCommand.Error))
            {
                await ProcessReceivedDataAsync(sessionId, sshCommand.Error, ConnectionType.SSH);
            }
            
            return sshCommand.ExitStatus == 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error executing SSH command in session {SessionId}", sessionId);
            return false;
        }
    }
    
    private async Task<bool> SendSerialCommandAsync(string sessionId, string command, CancellationToken cancellationToken)
    {
        if (!_serialConnections.TryGetValue(sessionId, out var serialPort) || !serialPort.IsOpen)
        {
            return false;
        }
        
        try
        {
            await Task.Run(() => serialPort.WriteLine(command), cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error sending serial command in session {SessionId}", sessionId);
            return false;
        }
    }
    
    private async Task<bool> SendBluetoothCommandAsync(string sessionId, string command, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        // Placeholder implementation
        return false;
    }
    
    private async Task<bool> SendTelnetCommandAsync(string sessionId, string command, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        // Placeholder implementation
        return false;
    }
    
    #endregion
    
    #region Raw Data Sending Implementations
    
    private async Task<int> SendSshRawDataAsync(string sessionId, byte[] data, CancellationToken cancellationToken)
    {
        if (!_sshConnections.TryGetValue(sessionId, out var sshClient) || !sshClient.IsConnected)
        {
            return 0;
        }
        
        try
        {
            // SSH.NET doesn't have direct raw data sending - this would require a shell stream
            var command = Encoding.UTF8.GetString(data);
            return await SendSshCommandAsync(sessionId, command, cancellationToken) ? data.Length : 0;
        }
        catch
        {
            return 0;
        }
    }
    
    private async Task<int> SendSerialRawDataAsync(string sessionId, byte[] data, CancellationToken cancellationToken)
    {
        if (!_serialConnections.TryGetValue(sessionId, out var serialPort) || !serialPort.IsOpen)
        {
            return 0;
        }
        
        try
        {
            await Task.Run(() => serialPort.Write(data, 0, data.Length), cancellationToken);
            return data.Length;
        }
        catch
        {
            return 0;
        }
    }
    
    private async Task<int> SendBluetoothRawDataAsync(string sessionId, byte[] data, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        // Placeholder implementation
        return 0;
    }
    
    private async Task<int> SendTelnetRawDataAsync(string sessionId, byte[] data, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        // Placeholder implementation
        return 0;
    }
    
    #endregion
    
    #region Disconnection Implementations
    
    private async Task<bool> DisconnectSshAsync(string sessionId, CancellationToken cancellationToken)
    {
        if (_sshConnections.TryRemove(sessionId, out var sshClient))
        {
            try
            {
                await Task.Run(() =>
                {
                    if (sshClient.IsConnected)
                        sshClient.Disconnect();
                    sshClient.Dispose();
                }, cancellationToken);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Error disconnecting SSH session {SessionId}", sessionId);
                return false;
            }
        }
        
        return true;
    }
    
    private async Task<bool> DisconnectSerialAsync(string sessionId, CancellationToken cancellationToken)
    {
        if (_serialConnections.TryRemove(sessionId, out var serialPort))
        {
            try
            {
                await Task.Run(() =>
                {
                    if (serialPort.IsOpen)
                        serialPort.Close();
                    serialPort.Dispose();
                }, cancellationToken);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Error disconnecting serial session {SessionId}", sessionId);
                return false;
            }
        }
        
        return true;
    }
    
    private async Task<bool> DisconnectBluetoothAsync(string sessionId, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        // Placeholder implementation
        return true;
    }
    
    private async Task<bool> DisconnectTelnetAsync(string sessionId, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        // Placeholder implementation
        return true;
    }
    
    #endregion
    
    #region Helper Methods
    
    private ConnectionInfo CreateSshConnectionInfo(ConnectionParams parameters)
    {
        var authMethods = new List<AuthenticationMethod>();
        
        if (!string.IsNullOrEmpty(parameters.PrivateKeyPath) && File.Exists(parameters.PrivateKeyPath))
        {
            var keyFile = new PrivateKeyFile(parameters.PrivateKeyPath);
            authMethods.Add(new PrivateKeyAuthenticationMethod(parameters.Username!, keyFile));
        }
        
        if (!string.IsNullOrEmpty(parameters.Password))
        {
            authMethods.Add(new PasswordAuthenticationMethod(parameters.Username!, parameters.Password));
        }
        
        return new ConnectionInfo(parameters.Host!, parameters.Port, parameters.Username!, authMethods.ToArray())
        {
            Timeout = TimeSpan.FromSeconds(parameters.TimeoutSeconds)
        };
    }
    
    private string GetConnectionTarget(ConnectionParams parameters) => parameters.Type switch
    {
        ConnectionType.SSH => $"{parameters.Host}:{parameters.Port}",
        ConnectionType.Serial => $"{parameters.SerialPort}@{parameters.BaudRate}",
        ConnectionType.Bluetooth => $"BT:{parameters.BluetoothAddress}",
        ConnectionType.Telnet => $"{parameters.Host}:{parameters.Port}",
        _ => "Unknown Target"
    };
    
    private async Task ProcessReceivedDataAsync(string sessionId, string data, ConnectionType connectionType)
    {
        try
        {
            // Parse output through emulation engine
            var coloredOutputs = await _emulationEngine.ParseOutputAsync(data, sessionId);
            
            // Add to terminal buffer
            if (_terminalBuffers.TryGetValue(sessionId, out var buffer))
            {
                foreach (var output in coloredOutputs)
                {
                    buffer.AddLine(output);
                }
            }
            
            // Publish data received events
            await _eventPublisher.PublishDataReceivedAsync(sessionId, data, Encoding.UTF8.GetByteCount(data), connectionType);
            
            // Fire local event
            DataReceived?.Invoke(this, new DataReceivedEvent
            {
                SessionId = sessionId,
                Data = data,
                ByteCount = Encoding.UTF8.GetByteCount(data),
                ConnectionType = connectionType
            });
            
            // Update session activity
            if (_activeSessions.TryGetValue(sessionId, out var session))
            {
                session.LastActivity = DateTime.Now;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error processing received data for session {SessionId}", sessionId);
        }
    }
    
    private async Task MonitorSessionDataAsync(string sessionId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("👁️ Starting data monitoring for session {SessionId}", sessionId);
        
        try
        {
            while (!cancellationToken.IsCancellationRequested && _activeSessions.ContainsKey(sessionId))
            {
                // Monitor connection health and data flow
                if (_activeSessions.TryGetValue(sessionId, out var session))
                {
                    // Check for connection timeouts
                    var timeSinceActivity = DateTime.Now - session.LastActivity;
                    if (timeSinceActivity > TimeSpan.FromMinutes(30)) // 30 minute idle timeout
                    {
                        _logger.LogWarning("⏰ Session {SessionId} has been idle for {Minutes} minutes - considering timeout",
                            sessionId, timeSinceActivity.TotalMinutes);
                    }
                    
                    // Publish session metrics periodically
                    var metrics = new Dictionary<string, object>
                    {
                        ["uptime_minutes"] = (DateTime.Now - session.ConnectedAt).TotalMinutes,
                        ["last_activity_minutes_ago"] = timeSinceActivity.TotalMinutes,
                        ["status"] = session.Status.ToString()
                    };
                    
                    await _eventPublisher.PublishSessionMetricsAsync(sessionId, metrics);
                }
                
                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogDebug("🛑 Data monitoring cancelled for session {SessionId}", sessionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error in data monitoring for session {SessionId}", sessionId);
        }
    }
    
    #endregion
    
    #region IDisposable Implementation
    
    public void Dispose()
    {
        if (_disposed) return;
        
        _disposed = true;
        
        // Cancel all sessions
        foreach (var cts in _sessionCancellations.Values)
        {
            cts?.Cancel();
        }
        
        // Close all connections
        _ = Task.Run(async () =>
        {
            await CloseAllSessionsAsync();
        });
        
        _connectionSemaphore?.Dispose();
        
        _logger.LogInformation("🧹 SSH Terminal Service disposed - digital realm connections severed gracefully!");
    }
    
    #endregion
}