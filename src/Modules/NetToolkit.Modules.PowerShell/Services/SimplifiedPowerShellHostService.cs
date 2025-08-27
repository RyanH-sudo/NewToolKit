using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Runtime.CompilerServices;
using NetToolkit.Modules.PowerShell.Interfaces;
using NetToolkit.Core.Interfaces;

namespace NetToolkit.Modules.PowerShell.Services;

/// <summary>
/// Simplified PowerShell Host Service - Core terminal functionality without complex dependencies
/// The essential magic - stripped of complexity but retaining the power!
/// </summary>
public class SimplifiedPowerShellHostService : IPowerShellHost, IDisposable
{
    private readonly ILoggerWrapper _logger;
    private readonly ITerminalEventPublisher _eventPublisher;
    private readonly ObservableCollection<string> _commandHistory;
    private readonly SemaphoreSlim _executionSemaphore;
    private readonly CancellationTokenSource _cancellationTokenSource;
    
    // PowerShell execution infrastructure - the beating heart
    private System.Management.Automation.PowerShell? _powerShell;
    private Runspace? _runspace;
    private bool _isInitialized;
    
    // SSH mode (simplified - just a flag for now)
    private bool _sshModeEnabled;
    
    // Output streaming
    private readonly Queue<PowerShellOutputChunk> _outputQueue;
    
    public SimplifiedPowerShellHostService(ILoggerWrapper logger, ITerminalEventPublisher eventPublisher)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        _commandHistory = new ObservableCollection<string>();
        _executionSemaphore = new SemaphoreSlim(1, 1);
        _cancellationTokenSource = new CancellationTokenSource();
        _outputQueue = new Queue<PowerShellOutputChunk>();
        
        InitializeAsync().ConfigureAwait(false);
        _logger.LogInfo("Simplified PowerShell Host Service initialized - Basic digital magic ready! ⚡");
    }
    
    private async Task InitializeAsync()
    {
        try
        {
            // Create initial session state
            var initialSessionState = InitialSessionState.CreateDefault();
            initialSessionState.ExecutionPolicy = Microsoft.PowerShell.ExecutionPolicy.RemoteSigned;
            
            // Create runspace
            _runspace = RunspaceFactory.CreateRunspace(initialSessionState);
            _runspace.Open();
            
            // Create PowerShell instance
            _powerShell = System.Management.Automation.PowerShell.Create();
            _powerShell.Runspace = _runspace;
            
            _isInitialized = true;
            _logger.LogInfo("PowerShell runtime initialized successfully! 🔥");
            
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize PowerShell runtime!");
            throw;
        }
    }
    
    public async Task<PowerShellExecutionResult> ExecuteAsync(string script, Dictionary<string, object>? parameters = null)
    {
        if (!_isInitialized)
            throw new InvalidOperationException("PowerShell host not initialized!");
        
        var scriptId = Guid.NewGuid().ToString();
        var startTime = DateTime.UtcNow;
        
        _commandHistory.Add(script);
        
        await _eventPublisher.PublishScriptStartedAsync(new ScriptExecutionStartedEvent
        {
            ScriptId = scriptId,
            ScriptName = script.Length > 50 ? script[..47] + "..." : script,
            ScriptContent = script,
            Parameters = parameters ?? new(),
            IsSshMode = _sshModeEnabled
        });
        
        try
        {
            await _executionSemaphore.WaitAsync(_cancellationTokenSource.Token);
            
            var result = await ExecutePowerShellScriptAsync(script, parameters ?? new());
            var endTime = DateTime.UtcNow;
            result.Duration = endTime - startTime;
            
            await _eventPublisher.PublishScriptCompletedAsync(new ScriptExecutionCompletedEvent
            {
                ScriptId = scriptId,
                ScriptName = script.Length > 50 ? script[..47] + "..." : script,
                Success = result.Success,
                Output = result.Output,
                ErrorOutput = result.ErrorOutput,
                Duration = result.Duration,
                ExitCode = result.ExitCode,
                Errors = result.Errors
            });
            
            _logger.LogInfo("Script execution completed in {Duration}ms - {Status}", 
                          result.Duration.TotalMilliseconds, 
                          result.Success ? "SUCCESS" : "FAILED");
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Script execution failed catastrophically!");
            
            var errorResult = new PowerShellExecutionResult
            {
                Success = false,
                ErrorOutput = ex.Message,
                Duration = DateTime.UtcNow - startTime,
                ExitCode = -1
            };
            
            await _eventPublisher.PublishErrorOccurredAsync(new ErrorOccurredEvent
            {
                ErrorMessage = ex.Message,
                StackTrace = ex.StackTrace ?? string.Empty,
                ScriptId = scriptId,
                Severity = ErrorSeverity.Critical
            });
            
            return errorResult;
        }
        finally
        {
            _executionSemaphore.Release();
        }
    }
    
    public async Task<PowerShellExecutionResult> ExecuteCommandAsync(string command)
    {
        return await ExecuteAsync(command);
    }
    
    public async Task<bool> ToggleSshModeAsync(bool enabled, Interfaces.SshConnectionInfo? connectionInfo = null)
    {
        // Simplified SSH mode - just toggle the flag for now
        _sshModeEnabled = enabled;
        
        await _eventPublisher.PublishSshModeChangedAsync(new SshModeChangedEvent
        {
            Enabled = enabled,
            RemoteHost = connectionInfo?.Host,
            RemotePort = connectionInfo?.Port ?? 22,
            RemoteUser = connectionInfo?.Username,
            ConnectionSuccessful = true // Simplified - assume success
        });
        
        _logger.LogInfo("SSH mode {Action} - {Status}", 
                       enabled ? "enabled" : "disabled",
                       enabled ? "Portal opened!" : "Portal closed!");
        
        return true;
    }
    
    public async IAsyncEnumerable<PowerShellOutputChunk> GetOutputStreamAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (_outputQueue.Count > 0)
            {
                lock (_outputQueue)
                {
                    if (_outputQueue.Count > 0)
                    {
                        yield return _outputQueue.Dequeue();
                    }
                }
            }
            else
            {
                await Task.Delay(100, cancellationToken);
            }
        }
    }
    
    public ObservableCollection<string> GetCommandHistory()
    {
        return _commandHistory;
    }
    
    public async Task ClearAsync()
    {
        lock (_outputQueue)
        {
            _outputQueue.Clear();
        }
        
        _logger.LogInfo("Terminal cleared - Fresh digital slate! ✨");
        await Task.CompletedTask;
    }
    
    public async Task<List<string>> GetAvailableCommandsAsync()
    {
        if (_powerShell == null || !_isInitialized)
            return new List<string>();
        
        try
        {
            var commands = await Task.Run(() =>
            {
                _powerShell.Commands.Clear();
                _powerShell.AddCommand("Get-Command");
                var results = _powerShell.Invoke();
                
                return results
                    .Select(r => r?.ToString() ?? "")
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Take(100) // Limit to prevent performance issues
                    .OrderBy(name => name)
                    .ToList();
            });
            
            _logger.LogDebug("Retrieved {Count} available commands", commands.Count);
            return commands;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve available commands!");
            return new List<string>();
        }
    }
    
    public PowerShellSessionState GetSessionState()
    {
        return new PowerShellSessionState
        {
            IsConnected = _isInitialized,
            IsSshMode = _sshModeEnabled,
            CurrentLocation = Environment.CurrentDirectory,
            ExecutionPolicy = "RemoteSigned",
            HostName = _sshModeEnabled ? "Remote" : Environment.MachineName,
            UserName = Environment.UserName,
            Variables = new Dictionary<string, object>(),
            LoadedModules = new List<string>()
        };
    }
    
    private async Task<PowerShellExecutionResult> ExecutePowerShellScriptAsync(string script, Dictionary<string, object> parameters)
    {
        if (_powerShell == null)
            throw new InvalidOperationException("PowerShell instance not available!");
        
        var result = new PowerShellExecutionResult();
        var outputLines = new List<string>();
        var errorLines = new List<string>();
        
        try
        {
            _powerShell.Commands.Clear();
            _powerShell.AddScript(script);
            
            // Add parameters
            foreach (var param in parameters)
            {
                _powerShell.AddParameter(param.Key, param.Value);
            }
            
            // Execute
            var results = await Task.Run(() => _powerShell.Invoke());
            
            // Process results
            foreach (var obj in results)
            {
                var outputText = obj?.ToString() ?? string.Empty;
                outputLines.Add(outputText);
                
                // Add to output queue for streaming
                await PublishOutputChunk(outputText, PowerShellOutputType.Information);
            }
            
            // Process errors
            if (_powerShell.HadErrors)
            {
                foreach (var error in _powerShell.Streams.Error)
                {
                    var errorText = error.ToString();
                    errorLines.Add(errorText);
                    
                    result.Errors.Add(new PowerShellError
                    {
                        Message = error.Exception?.Message ?? errorText,
                        Category = error.CategoryInfo?.Category.ToString() ?? "Unknown",
                        FullyQualifiedErrorId = error.FullyQualifiedErrorId ?? string.Empty,
                        ScriptStackTrace = error.ScriptStackTrace ?? string.Empty,
                        LineNumber = error.InvocationInfo?.ScriptLineNumber ?? 0,
                        ColumnNumber = error.InvocationInfo?.OffsetInLine ?? 0
                    });
                    
                    await PublishOutputChunk(errorText, PowerShellOutputType.Error);
                }
            }
            
            result.Success = !_powerShell.HadErrors;
            result.Output = string.Join(Environment.NewLine, outputLines);
            result.ErrorOutput = string.Join(Environment.NewLine, errorLines);
            result.ExitCode = result.Success ? 0 : 1;
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PowerShell execution failed!");
            
            result.Success = false;
            result.ErrorOutput = ex.Message;
            result.ExitCode = -1;
            
            return result;
        }
    }
    
    private async Task PublishOutputChunk(string content, PowerShellOutputType outputType)
    {
        var chunk = new PowerShellOutputChunk
        {
            Content = content,
            Type = outputType,
            Color = GetColorForOutputType(outputType)
        };
        
        lock (_outputQueue)
        {
            _outputQueue.Enqueue(chunk);
        }
        
        await _eventPublisher.PublishOutputReceivedAsync(new OutputReceivedEvent
        {
            Content = content,
            OutputType = outputType,
            Color = chunk.Color,
            IsSshOutput = _sshModeEnabled
        });
    }
    
    private static string GetColorForOutputType(PowerShellOutputType outputType)
    {
        return outputType switch
        {
            PowerShellOutputType.Information => "#FFFFFF",
            PowerShellOutputType.Warning => "#FFA500",
            PowerShellOutputType.Error => "#FF4444",
            PowerShellOutputType.Debug => "#888888",
            PowerShellOutputType.Verbose => "#00FFFF",
            PowerShellOutputType.Progress => "#00FF00",
            PowerShellOutputType.Command => "#FFFF00",
            _ => "#FFFFFF"
        };
    }
    
    public void Dispose()
    {
        try
        {
            _cancellationTokenSource?.Cancel();
            _powerShell?.Dispose();
            _runspace?.Dispose();
            _executionSemaphore?.Dispose();
            _cancellationTokenSource?.Dispose();
            
            _logger.LogInfo("Simplified PowerShell Host Service disposed - Digital simplicity restored! 🌙");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during disposal!");
        }
    }
}