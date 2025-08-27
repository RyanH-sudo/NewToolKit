using NetToolkit.Core.Interfaces;

namespace NetToolkit.Modules.PowerShell.Interfaces;

/// <summary>
/// Terminal event publisher - broadcasting the gospel of PowerShell across the application
/// </summary>
public interface ITerminalEventPublisher
{
    /// <summary>
    /// Publish script execution started event - let the world know we're cooking
    /// </summary>
    Task PublishScriptStartedAsync(ScriptExecutionStartedEvent eventData);
    
    /// <summary>
    /// Publish script execution completed event - victory or defeat, we tell all
    /// </summary>
    Task PublishScriptCompletedAsync(ScriptExecutionCompletedEvent eventData);
    
    /// <summary>
    /// Publish output chunk received - real-time streaming for the impatient
    /// </summary>
    Task PublishOutputReceivedAsync(OutputReceivedEvent eventData);
    
    /// <summary>
    /// Publish error occurred event - when things go boom, everyone should know
    /// </summary>
    Task PublishErrorOccurredAsync(ErrorOccurredEvent eventData);
    
    /// <summary>
    /// Publish SSH mode changed event - for when we switch dimensions
    /// </summary>
    Task PublishSshModeChangedAsync(SshModeChangedEvent eventData);
    
    /// <summary>
    /// Publish template generated event - when magic happens
    /// </summary>
    Task PublishTemplateGeneratedAsync(TemplateGeneratedEvent eventData);
}

/// <summary>
/// Base terminal event - the foundation of all terminal happenings
/// </summary>
public abstract class BaseTerminalEvent
{
    public string SessionId { get; set; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string UserName { get; set; } = Environment.UserName;
    public string MachineName { get; set; } = Environment.MachineName;
}

/// <summary>
/// Script execution started event - the beginning of magic
/// </summary>
public class ScriptExecutionStartedEvent : BaseTerminalEvent
{
    public string ScriptId { get; set; } = string.Empty;
    public string ScriptName { get; set; } = string.Empty;
    public string ScriptContent { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
    public string TemplateId { get; set; } = string.Empty;
    public bool IsSshMode { get; set; }
    public TemplateRisk RiskLevel { get; set; } = TemplateRisk.Low;
}

/// <summary>
/// Script execution completed event - the end of the journey
/// </summary>
public class ScriptExecutionCompletedEvent : BaseTerminalEvent
{
    public string ScriptId { get; set; } = string.Empty;
    public string ScriptName { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string Output { get; set; } = string.Empty;
    public string ErrorOutput { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public int ExitCode { get; set; }
    public List<PowerShellError> Errors { get; set; } = new();
    public TemplateRisk RiskLevel { get; set; } = TemplateRisk.Low;
    
    // Witty status messages based on outcome
    public string GetWittyStatusMessage()
    {
        return Success switch
        {
            true when Duration.TotalSeconds < 1 => "Script executed faster than you can say 'PowerShell'! ⚡",
            true when Duration.TotalSeconds < 5 => "Mission accomplished with style! 🎯",
            true => "Success! Like a fine wine, some scripts are worth the wait 🍷",
            false when Errors.Any(e => e.Message.Contains("access", StringComparison.OrdinalIgnoreCase)) => "Access denied! Even PowerShell has boundaries 🚫",
            false when Errors.Any(e => e.Message.Contains("not found", StringComparison.OrdinalIgnoreCase)) => "Command not found - it's playing hide and seek! 🙈",
            false => "Script stumbled over digital debris - fear not, we shall rise again! 💪"
        };
    }
}

/// <summary>
/// Output received event - real-time terminal goodness
/// </summary>
public class OutputReceivedEvent : BaseTerminalEvent
{
    public string Content { get; set; } = string.Empty;
    public PowerShellOutputType OutputType { get; set; }
    public string Color { get; set; } = "#FFFFFF";
    public string ScriptId { get; set; } = string.Empty;
    public bool IsSshOutput { get; set; }
}

/// <summary>
/// Error occurred event - when Murphy's Law kicks in
/// </summary>
public class ErrorOccurredEvent : BaseTerminalEvent
{
    public string ErrorMessage { get; set; } = string.Empty;
    public string ErrorCategory { get; set; } = string.Empty;
    public string StackTrace { get; set; } = string.Empty;
    public string ScriptId { get; set; } = string.Empty;
    public int LineNumber { get; set; }
    public int ColumnNumber { get; set; }
    public ErrorSeverity Severity { get; set; } = ErrorSeverity.Error;
    
    // Generate witty error message
    public string GetWittyErrorMessage()
    {
        return ErrorCategory.ToLowerInvariant() switch
        {
            "syntaxerror" => "Syntax error detected! The script grammar police are not amused 👮‍♀️",
            "parameternotfound" => "Parameter went AWOL! Check if it's hiding behind a typo 🔍",
            "accessdenied" => "Access denied! Looks like you need the secret handshake 🤝",
            "timeout" => "Script took a coffee break and never came back ☕",
            _ => $"Houston, we have a problem: {ErrorMessage} 🚀"
        };
    }
}

/// <summary>
/// SSH mode changed event - dimensional shift notification
/// </summary>
public class SshModeChangedEvent : BaseTerminalEvent
{
    public bool Enabled { get; set; }
    public string? RemoteHost { get; set; }
    public int RemotePort { get; set; }
    public string? RemoteUser { get; set; }
    public bool ConnectionSuccessful { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Template generated event - automation magic in action
/// </summary>
public class TemplateGeneratedEvent : BaseTerminalEvent
{
    public string TemplateId { get; set; } = string.Empty;
    public string TemplateName { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
    public string GeneratedScript { get; set; } = string.Empty;
    public TimeSpan GenerationTime { get; set; }
    public TemplateRisk RiskLevel { get; set; } = TemplateRisk.Low;
}

/// <summary>
/// Error severity levels - because not all errors are created equal
/// </summary>
public enum ErrorSeverity
{
    Info,
    Warning,
    Error,
    Critical,
    Fatal
}