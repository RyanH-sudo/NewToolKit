using Microsoft.Extensions.Logging;
using NetToolkit.Core.Interfaces;
using NetToolkit.Modules.SshTerminal.Models;
using NetToolkit.Modules.SshTerminal.Interfaces;

namespace NetToolkit.Modules.SshTerminal.Services;

/// <summary>
/// SSH event publisher service - the herald of digital terminal proclamations
/// Where connection events cascade through the modular cosmos with urgent tidings
/// </summary>
public class SshEventPublisher : ISshEventPublisher
{
    private readonly ILogger<SshEventPublisher> _logger;
    private readonly IEventBus _eventBus;
    
    public SshEventPublisher(ILogger<SshEventPublisher> logger, IEventBus eventBus)
    {
        _logger = logger;
        _eventBus = eventBus;
        
        _logger.LogInformation("📡 SSH Event Publisher initialized - ready to broadcast terminal tidings across digital realms!");
    }
    
    #region ISshEventPublisher Implementation
    
    /// <summary>
    /// Publish connection established event
    /// </summary>
    public async Task PublishConnectionEstablishedAsync(string sessionId, ConnectionParams connectionParams, TimeSpan connectionDuration)
    {
        try
        {
            var eventData = new
            {
                SessionId = sessionId,
                ConnectionType = connectionParams.Type.ToString(),
                Target = GetConnectionTarget(connectionParams),
                Username = connectionParams.Username,
                ConnectionDurationMs = connectionDuration.TotalMilliseconds,
                Timestamp = DateTime.UtcNow,
                DisplayName = connectionParams.DisplayName,
                AutoReconnect = connectionParams.AutoReconnect,
                EnableLogging = connectionParams.EnableLogging
            };
            
            await _eventBus.PublishAsync(new { EventName = "ssh.connection.established", Data = eventData });
            
            _logger.LogDebug("🌉 Published connection established event for session {SessionId} to {Target}",
                sessionId, GetConnectionTarget(connectionParams));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error publishing connection established event for session {SessionId}", sessionId);
        }
    }
    
    /// <summary>
    /// Publish data received event
    /// </summary>
    public async Task PublishDataReceivedAsync(string sessionId, string data, int byteCount, ConnectionType connectionType)
    {
        try
        {
            var eventData = new
            {
                SessionId = sessionId,
                DataLength = data.Length,
                ByteCount = byteCount,
                ConnectionType = connectionType.ToString(),
                Timestamp = DateTime.UtcNow,
                HasAnsiCodes = data.Contains('\x1B'),
                LineCount = data.Split('\n').Length,
                Preview = data.Length > 100 ? data[..100] + "..." : data
            };
            
            await _eventBus.PublishAsync(new { EventName = "ssh.data.received", Data = eventData });
            
            _logger.LogTrace("📨 Published data received event for session {SessionId}: {ByteCount} bytes",
                sessionId, byteCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error publishing data received event for session {SessionId}", sessionId);
        }
    }
    
    /// <summary>
    /// Publish command executed event
    /// </summary>
    public async Task PublishCommandExecutedAsync(string sessionId, string command, string? response, bool success, TimeSpan executionTime)
    {
        try
        {
            var eventData = new
            {
                SessionId = sessionId,
                Command = command,
                CommandLength = command.Length,
                ResponseLength = response?.Length ?? 0,
                Success = success,
                ExecutionTimeMs = executionTime.TotalMilliseconds,
                Timestamp = DateTime.UtcNow,
                CommandCategory = CategorizeCommand(command),
                IsPrivileged = command.Contains("sudo") || command.StartsWith("su "),
                HasPipeline = command.Contains('|'),
                HasRedirection = command.Contains('>') || command.Contains('<')
            };
            
            await _eventBus.PublishAsync(new { EventName = "ssh.command.executed", Data = eventData });
            
            var status = success ? "✅" : "❌";
            _logger.LogDebug("{Status} Published command executed event for session {SessionId}: {Command} ({ExecutionTime:F2}ms)",
                status, sessionId, command.Length > 50 ? command[..50] + "..." : command, executionTime.TotalMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error publishing command executed event for session {SessionId}", sessionId);
        }
    }
    
    /// <summary>
    /// Publish error encountered event
    /// </summary>
    public async Task PublishErrorEncounteredAsync(string sessionId, string errorMessage, Exception? exception = null, string? context = null)
    {
        try
        {
            var eventData = new
            {
                SessionId = sessionId,
                ErrorMessage = errorMessage,
                ExceptionType = exception?.GetType().Name,
                StackTrace = exception?.StackTrace,
                Context = context,
                Timestamp = DateTime.UtcNow,
                Severity = DetermineErrorSeverity(exception),
                IsRecoverable = IsRecoverableError(exception)
            };
            
            await _eventBus.PublishAsync(new { EventName = "ssh.error.encountered", Data = eventData });
            
            _logger.LogWarning("⚠️ Published error event for session {SessionId}: {ErrorMessage} (Context: {Context})",
                sessionId, errorMessage, context ?? "Unknown");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error publishing error encountered event for session {SessionId}", sessionId);
        }
    }
    
    /// <summary>
    /// Publish device detected event
    /// </summary>
    public async Task PublishDeviceDetectedAsync(DeviceInfo device, DeviceType deviceType)
    {
        try
        {
            var eventData = new
            {
                DeviceId = device.Id,
                DeviceName = device.Name,
                DeviceType = deviceType.ToString(),
                IsAvailable = device.IsAvailable,
                Description = device.Description,
                Properties = device.Properties,
                Timestamp = DateTime.UtcNow,
                StatusIcon = device.GetStatusIcon()
            };
            
            await _eventBus.PublishAsync(new { EventName = "ssh.device.detected", Data = eventData });
            
            _logger.LogDebug("📱 Published device detected event: {DeviceName} ({DeviceType}) - {Status}",
                device.Name, deviceType, device.IsAvailable ? "Available" : "Unavailable");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error publishing device detected event for device {DeviceId}", device.Id);
        }
    }
    
    /// <summary>
    /// Publish device lost event
    /// </summary>
    public async Task PublishDeviceLostAsync(DeviceInfo device, DeviceType deviceType)
    {
        try
        {
            var eventData = new
            {
                DeviceId = device.Id,
                DeviceName = device.Name,
                DeviceType = deviceType.ToString(),
                LastSeen = DateTime.UtcNow,
                Properties = device.Properties,
                Timestamp = DateTime.UtcNow
            };
            
            await _eventBus.PublishAsync(new { EventName = "ssh.device.lost", Data = eventData });
            
            _logger.LogDebug("📵 Published device lost event: {DeviceName} ({DeviceType})",
                device.Name, deviceType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error publishing device lost event for device {DeviceId}", device.Id);
        }
    }
    
    /// <summary>
    /// Publish session status change event
    /// </summary>
    public async Task PublishSessionStatusChangedAsync(string sessionId, ConnectionStatus oldStatus, ConnectionStatus newStatus, string? statusMessage = null)
    {
        try
        {
            var eventData = new
            {
                SessionId = sessionId,
                OldStatus = oldStatus.ToString(),
                NewStatus = newStatus.ToString(),
                StatusMessage = statusMessage,
                Timestamp = DateTime.UtcNow,
                TransitionType = GetStatusTransitionType(oldStatus, newStatus),
                IsImprovement = IsStatusImprovement(oldStatus, newStatus)
            };
            
            await _eventBus.PublishAsync(new { EventName = "ssh.session.status.changed", Data = eventData });
            
            _logger.LogDebug("🔄 Published status change event for session {SessionId}: {OldStatus} → {NewStatus}",
                sessionId, oldStatus, newStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error publishing session status change event for session {SessionId}", sessionId);
        }
    }
    
    /// <summary>
    /// Publish authentication attempt event
    /// </summary>
    public async Task PublishAuthenticationAttemptAsync(string sessionId, string username, string authMethod, bool success, string? host = null)
    {
        try
        {
            var eventData = new
            {
                SessionId = sessionId,
                Username = username,
                AuthMethod = authMethod,
                Success = success,
                Host = host,
                Timestamp = DateTime.UtcNow,
                AuthType = DetermineAuthType(authMethod),
                SecurityLevel = GetAuthSecurityLevel(authMethod)
            };
            
            await _eventBus.PublishAsync(new { EventName = "ssh.authentication.attempt", Data = eventData });
            
            var status = success ? "✅" : "❌";
            _logger.LogDebug("{Status} Published authentication event for session {SessionId}: {Username}@{Host} via {AuthMethod}",
                status, sessionId, username, host ?? "unknown", authMethod);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error publishing authentication attempt event for session {SessionId}", sessionId);
        }
    }
    
    /// <summary>
    /// Publish terminal output event
    /// </summary>
    public async Task PublishTerminalOutputAsync(string sessionId, ColoredOutput coloredOutput, int lineNumber)
    {
        try
        {
            var eventData = new
            {
                SessionId = sessionId,
                Text = coloredOutput.Text,
                TextLength = coloredOutput.Text.Length,
                HasColor = coloredOutput.Foreground.HasValue || coloredOutput.Background.HasValue,
                IsBold = coloredOutput.IsBold,
                IsUnderline = coloredOutput.IsUnderline,
                IsItalic = coloredOutput.IsItalic,
                LineNumber = lineNumber,
                Timestamp = coloredOutput.Timestamp,
                XtermFormat = coloredOutput.ToXtermString()
            };
            
            await _eventBus.PublishAsync(new { EventName = "ssh.terminal.output", Data = eventData });
            
            _logger.LogTrace("📺 Published terminal output event for session {SessionId}: line {LineNumber}",
                sessionId, lineNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error publishing terminal output event for session {SessionId}", sessionId);
        }
    }
    
    /// <summary>
    /// Publish file transfer event
    /// </summary>
    public async Task PublishFileTransferAsync(string sessionId, string fileName, TransferDirection direction, long byteCount, bool success)
    {
        try
        {
            var eventData = new
            {
                SessionId = sessionId,
                FileName = fileName,
                Direction = direction.ToString(),
                ByteCount = byteCount,
                Success = success,
                Timestamp = DateTime.UtcNow,
                FileExtension = Path.GetExtension(fileName),
                FileSizeMB = Math.Round(byteCount / (1024.0 * 1024.0), 2),
                TransferType = DetermineTransferType(fileName)
            };
            
            await _eventBus.PublishAsync(new { EventName = "ssh.file.transfer", Data = eventData });
            
            var status = success ? "✅" : "❌";
            var arrow = direction == TransferDirection.Upload ? "⬆️" : "⬇️";
            _logger.LogDebug("{Status} {Arrow} Published file transfer event for session {SessionId}: {FileName} ({ByteCount:N0} bytes)",
                status, arrow, sessionId, fileName, byteCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error publishing file transfer event for session {SessionId}", sessionId);
        }
    }
    
    /// <summary>
    /// Publish terminal configuration change event
    /// </summary>
    public async Task PublishTerminalConfigChangedAsync(string sessionId, string configChange, TerminalConfig newConfig)
    {
        try
        {
            var eventData = new
            {
                SessionId = sessionId,
                ConfigChange = configChange,
                NewConfig = new
                {
                    newConfig.FontFamily,
                    newConfig.FontSize,
                    newConfig.DefaultForeground,
                    newConfig.DefaultBackground,
                    newConfig.BufferSize,
                    newConfig.HistorySize,
                    newConfig.EnableBell,
                    newConfig.EnableThreeJsEffects
                },
                Timestamp = DateTime.UtcNow,
                ConfigSummary = newConfig.GetConfigSummary()
            };
            
            await _eventBus.PublishAsync(new { EventName = "ssh.terminal.config.changed", Data = eventData });
            
            _logger.LogDebug("⚙️ Published terminal config change event for session {SessionId}: {ConfigChange}",
                sessionId, configChange);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error publishing terminal config change event for session {SessionId}", sessionId);
        }
    }
    
    /// <summary>
    /// Publish session metrics event
    /// </summary>
    public async Task PublishSessionMetricsAsync(string sessionId, Dictionary<string, object> metrics)
    {
        try
        {
            var eventData = new Dictionary<string, object>(metrics)
            {
                ["session_id"] = sessionId,
                ["timestamp"] = DateTime.UtcNow,
                ["metric_count"] = metrics.Count
            };
            
            await _eventBus.PublishAsync(eventData);
            
            _logger.LogTrace("📊 Published session metrics event for session {SessionId}: {MetricCount} metrics",
                sessionId, metrics.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error publishing session metrics event for session {SessionId}", sessionId);
        }
    }
    
    /// <summary>
    /// Publish hardware integration event
    /// </summary>
    public async Task PublishHardwareIntegrationAsync(string sessionId, DeviceInfo deviceInfo, string connectionMethod, bool success)
    {
        try
        {
            var eventData = new
            {
                SessionId = sessionId,
                DeviceId = deviceInfo.Id,
                DeviceName = deviceInfo.Name,
                DeviceType = deviceInfo.Type.ToString(),
                ConnectionMethod = connectionMethod,
                Success = success,
                Timestamp = DateTime.UtcNow,
                DeviceProperties = deviceInfo.Properties,
                IntegrationComplexity = GetIntegrationComplexity(deviceInfo.Type)
            };
            
            await _eventBus.PublishAsync(new { EventName = "ssh.hardware.integration", Data = eventData });
            
            var status = success ? "✅" : "❌";
            _logger.LogDebug("{Status} Published hardware integration event for session {SessionId}: {DeviceName} via {ConnectionMethod}",
                status, sessionId, deviceInfo.Name, connectionMethod);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error publishing hardware integration event for session {SessionId}", sessionId);
        }
    }
    
    /// <summary>
    /// Publish security alert event
    /// </summary>
    public async Task PublishSecurityAlertAsync(string sessionId, SecurityAlertType alertType, string message, AlertSeverity severity)
    {
        try
        {
            var eventData = new
            {
                SessionId = sessionId,
                AlertType = alertType.ToString(),
                Message = message,
                Severity = severity.ToString(),
                Timestamp = DateTime.UtcNow,
                RequiresImmedateAction = severity >= AlertSeverity.High,
                AlertCategory = GetSecurityAlertCategory(alertType)
            };
            
            await _eventBus.PublishAsync(new { EventName = "ssh.security.alert", Data = eventData });
            
            var severityIcon = severity switch
            {
                AlertSeverity.Critical => "🚨",
                AlertSeverity.High => "⚠️",
                AlertSeverity.Medium => "🔸",
                AlertSeverity.Low => "ℹ️",
                _ => "❓"
            };
            
            _logger.LogWarning("{SeverityIcon} Published security alert for session {SessionId}: {AlertType} - {Message}",
                severityIcon, sessionId, alertType, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error publishing security alert event for session {SessionId}", sessionId);
        }
    }
    
    /// <summary>
    /// Publish AI insight event
    /// </summary>
    public async Task PublishAiInsightAsync(string sessionId, AiInsightType insightType, string insight, float confidence)
    {
        try
        {
            var eventData = new
            {
                SessionId = sessionId,
                InsightType = insightType.ToString(),
                Insight = insight,
                Confidence = confidence,
                Timestamp = DateTime.UtcNow,
                ConfidenceLevel = GetConfidenceLevel(confidence),
                IsActionable = confidence > 0.7f
            };
            
            await _eventBus.PublishAsync(new { EventName = "ssh.ai.insight", Data = eventData });
            
            _logger.LogDebug("🤖 Published AI insight for session {SessionId}: {InsightType} (confidence: {Confidence:P0})",
                sessionId, insightType, confidence);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error publishing AI insight event for session {SessionId}", sessionId);
        }
    }
    
    #endregion
    
    #region IEventBus Delegation
    
    public async Task PublishAsync<T>(T eventData) where T : class
    {
        await _eventBus.PublishAsync(eventData);
    }
    
    public void Subscribe<T>(Func<T, Task> handler) where T : class
    {
        _eventBus.Subscribe(handler);
    }
    
    public void Unsubscribe<T>(Func<T, Task> handler) where T : class
    {
        _eventBus.Unsubscribe(handler);
    }
    
    #endregion
    
    #region Helper Methods
    
    private string GetConnectionTarget(ConnectionParams connectionParams) => connectionParams.Type switch
    {
        ConnectionType.SSH => $"{connectionParams.Host}:{connectionParams.Port}",
        ConnectionType.Serial => $"{connectionParams.SerialPort}@{connectionParams.BaudRate}",
        ConnectionType.Bluetooth => $"BT:{connectionParams.BluetoothAddress}",
        ConnectionType.Telnet => $"{connectionParams.Host}:{connectionParams.Port}",
        _ => "Unknown Target"
    };
    
    private string CategorizeCommand(string command)
    {
        var firstWord = command.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.ToLower() ?? "";
        
        return firstWord switch
        {
            "ls" or "dir" or "pwd" or "cd" => "Navigation",
            "cat" or "less" or "more" or "head" or "tail" => "FileViewing",
            "cp" or "mv" or "rm" or "mkdir" or "rmdir" => "FileManagement",
            "ps" or "top" or "htop" or "kill" or "killall" => "ProcessManagement",
            "grep" or "find" or "locate" or "which" => "Search",
            "chmod" or "chown" or "sudo" or "su" => "Permissions",
            "ssh" or "scp" or "rsync" or "curl" or "wget" => "Network",
            "git" or "svn" or "hg" => "VersionControl",
            "docker" or "kubectl" or "systemctl" or "service" => "ServiceManagement",
            "vim" or "nano" or "emacs" or "code" => "TextEditing",
            _ => "Other"
        };
    }
    
    private string DetermineErrorSeverity(Exception? exception) => exception switch
    {
        null => "Low",
        OperationCanceledException => "Low",
        TimeoutException => "Medium",
        UnauthorizedAccessException => "High",
        SystemException => "Critical",
        _ => "Medium"
    };
    
    private bool IsRecoverableError(Exception? exception) => exception switch
    {
        null => true,
        OperationCanceledException => true,
        TimeoutException => true,
        UnauthorizedAccessException => false,
        SystemException => false,
        _ => true
    };
    
    private string GetStatusTransitionType(ConnectionStatus oldStatus, ConnectionStatus newStatus)
    {
        return (oldStatus, newStatus) switch
        {
            (ConnectionStatus.Disconnected, ConnectionStatus.Connecting) => "Initialization",
            (ConnectionStatus.Connecting, ConnectionStatus.Connected) => "Success",
            (ConnectionStatus.Connected, ConnectionStatus.Reconnecting) => "Recovery",
            (ConnectionStatus.Reconnecting, ConnectionStatus.Connected) => "Restoration",
            (_, ConnectionStatus.Failed) => "Failure",
            (_, ConnectionStatus.Timeout) => "Timeout",
            (_, ConnectionStatus.Disconnected) => "Termination",
            _ => "StateChange"
        };
    }
    
    private bool IsStatusImprovement(ConnectionStatus oldStatus, ConnectionStatus newStatus)
    {
        var statusRank = new Dictionary<ConnectionStatus, int>
        {
            [ConnectionStatus.Failed] = 0,
            [ConnectionStatus.Timeout] = 1,
            [ConnectionStatus.Disconnected] = 2,
            [ConnectionStatus.Connecting] = 3,
            [ConnectionStatus.Reconnecting] = 3,
            [ConnectionStatus.Connected] = 4
        };
        
        return statusRank.GetValueOrDefault(newStatus, 0) > statusRank.GetValueOrDefault(oldStatus, 0);
    }
    
    private string DetermineAuthType(string authMethod) => authMethod.ToLower() switch
    {
        "password" => "Password",
        "publickey" or "privatekey" => "KeyBased",
        "keyboard-interactive" => "Interactive",
        "hostbased" => "HostBased",
        _ => "Unknown"
    };
    
    private string GetAuthSecurityLevel(string authMethod) => authMethod.ToLower() switch
    {
        "password" => "Medium",
        "publickey" or "privatekey" => "High",
        "keyboard-interactive" => "Medium",
        "hostbased" => "High",
        _ => "Unknown"
    };
    
    private string DetermineTransferType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLower();
        return extension switch
        {
            ".txt" or ".log" or ".conf" or ".cfg" => "Configuration",
            ".jpg" or ".png" or ".gif" or ".bmp" => "Image",
            ".mp4" or ".avi" or ".mov" or ".mkv" => "Video",
            ".mp3" or ".wav" or ".flac" => "Audio",
            ".zip" or ".tar" or ".gz" or ".7z" => "Archive",
            ".exe" or ".bin" or ".app" => "Executable",
            ".py" or ".js" or ".java" or ".cs" => "SourceCode",
            _ => "Generic"
        };
    }
    
    private string GetIntegrationComplexity(DeviceType deviceType) => deviceType switch
    {
        DeviceType.Serial => "Low",
        DeviceType.USB => "Medium",
        DeviceType.Network => "Medium",
        DeviceType.Bluetooth => "High",
        _ => "Unknown"
    };
    
    private string GetSecurityAlertCategory(SecurityAlertType alertType) => alertType switch
    {
        SecurityAlertType.WeakAuthentication => "Authentication",
        SecurityAlertType.UnencryptedConnection => "Encryption",
        SecurityAlertType.SuspiciousActivity => "Behavioral",
        SecurityAlertType.PrivilegeEscalation => "Access",
        SecurityAlertType.UnauthorizedAccess => "Access",
        _ => "General"
    };
    
    private string GetConfidenceLevel(float confidence) => confidence switch
    {
        >= 0.9f => "VeryHigh",
        >= 0.7f => "High",
        >= 0.5f => "Medium",
        >= 0.3f => "Low",
        _ => "VeryLow"
    };
    
    #endregion
}