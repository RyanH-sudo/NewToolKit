namespace NetToolkit.Core.Interfaces;

public interface ISshModule : IModule
{
    Task<SshConnection> ConnectAsync(SshConnectionInfo connectionInfo);
    Task<SshResult> ExecuteCommandAsync(string connectionId, string command);
    Task DisconnectAsync(string connectionId);
    Task<List<SshConnection>> GetActiveConnectionsAsync();
    Task<bool> TestConnectionAsync(SshConnectionInfo connectionInfo);
}

public class SshConnectionInfo
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 22;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PrivateKeyPath { get; set; } = string.Empty;
    public int Timeout { get; set; } = 30000;
}

public class SshConnection
{
    public string Id { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Username { get; set; } = string.Empty;
    public bool IsConnected { get; set; }
    public DateTime ConnectedAt { get; set; }
}

public class SshResult
{
    public bool Success { get; set; }
    public string Output { get; set; } = string.Empty;
    public string ErrorOutput { get; set; } = string.Empty;
    public int ExitCode { get; set; }
    public TimeSpan Duration { get; set; }
}