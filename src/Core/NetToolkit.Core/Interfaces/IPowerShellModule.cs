namespace NetToolkit.Core.Interfaces;

public interface IPowerShellModule : IModule
{
    Task<PowerShellResult> ExecuteScriptAsync(string script, Dictionary<string, object>? parameters = null);
    Task<PowerShellResult> ExecuteCommandAsync(string command);
    Task<List<string>> GetAvailableCommandsAsync();
    Task<string> FormatScriptWithParametersAsync(string scriptTemplate, Dictionary<string, object> parameters);
}

public class PowerShellResult
{
    public bool Success { get; set; }
    public string Output { get; set; } = string.Empty;
    public string ErrorOutput { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public int ExitCode { get; set; }
}