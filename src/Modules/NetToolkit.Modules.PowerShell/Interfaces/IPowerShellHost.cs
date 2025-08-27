using System.Collections.ObjectModel;

namespace NetToolkit.Modules.PowerShell.Interfaces;

/// <summary>
/// The omnipotent PowerShell host - like wielding Thor's hammer, but for scripts!
/// </summary>
public interface IPowerShellHost
{
    /// <summary>
    /// Execute a script with the fury of a thousand servers - async because we're civilized
    /// </summary>
    Task<PowerShellExecutionResult> ExecuteAsync(string script, Dictionary<string, object>? parameters = null);
    
    /// <summary>
    /// Execute a single command - perfect for quick incantations
    /// </summary>
    Task<PowerShellExecutionResult> ExecuteCommandAsync(string command);
    
    /// <summary>
    /// Toggle SSH mode - because sometimes you need to talk to distant machines
    /// </summary>
    Task<bool> ToggleSshModeAsync(bool enabled, SshConnectionInfo? connectionInfo = null);
    
    /// <summary>
    /// Get the output stream - like a crystal ball for terminal watchers
    /// </summary>
    IAsyncEnumerable<PowerShellOutputChunk> GetOutputStreamAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get command history - because we all need to remember our glorious past
    /// </summary>
    ObservableCollection<string> GetCommandHistory();
    
    /// <summary>
    /// Clear the terminal - a fresh start, like dawn after a long debugging night
    /// </summary>
    Task ClearAsync();
    
    /// <summary>
    /// Get available cmdlets - for those "what can I do?" moments
    /// </summary>
    Task<List<string>> GetAvailableCommandsAsync();
    
    /// <summary>
    /// Get current session state - know thy terminal, know thyself
    /// </summary>
    PowerShellSessionState GetSessionState();
}

/// <summary>
/// The result of PowerShell execution - success, failure, or "it's complicated"
/// </summary>
public class PowerShellExecutionResult
{
    public bool Success { get; set; }
    public string Output { get; set; } = string.Empty;
    public string ErrorOutput { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public int ExitCode { get; set; }
    public List<PowerShellError> Errors { get; set; } = new();
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

/// <summary>
/// Output chunk for streaming - because good things come in small, digestible pieces
/// </summary>
public class PowerShellOutputChunk
{
    public string Content { get; set; } = string.Empty;
    public PowerShellOutputType Type { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Color { get; set; } = "#FFFFFF"; // Default to white, because we're classy
}

/// <summary>
/// Types of PowerShell output - for those who like their data categorized
/// </summary>
public enum PowerShellOutputType
{
    Information,
    Warning, 
    Error,
    Debug,
    Verbose,
    Progress,
    Command // The command being executed
}

/// <summary>
/// PowerShell error details - when things go sideways, we need the details
/// </summary>
public class PowerShellError
{
    public string Message { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string FullyQualifiedErrorId { get; set; } = string.Empty;
    public string ScriptStackTrace { get; set; } = string.Empty;
    public int LineNumber { get; set; }
    public int ColumnNumber { get; set; }
}

/// <summary>
/// Session state - keeping track of where we are in the PowerShell universe
/// </summary>
public class PowerShellSessionState
{
    public bool IsConnected { get; set; }
    public bool IsSshMode { get; set; }
    public string CurrentLocation { get; set; } = string.Empty;
    public string ExecutionPolicy { get; set; } = string.Empty;
    public Dictionary<string, object> Variables { get; set; } = new();
    public List<string> LoadedModules { get; set; } = new();
    public string HostName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}

/// <summary>
/// SSH connection information - for when PowerShell needs to travel
/// </summary>
public class SshConnectionInfo
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 22;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PrivateKeyPath { get; set; } = string.Empty;
    public int Timeout { get; set; } = 30000;
    public string KnownHostsPath { get; set; } = string.Empty;
}