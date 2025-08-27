using NetToolkit.Modules.ScannerAndTopography.Models;

namespace NetToolkit.Modules.ScannerAndTopography.Interfaces;

/// <summary>
/// Configuration utility interface - the digital scepter of network command
/// Where node configurations bend to the will of administrative wizardry
/// </summary>
public interface IConfigurationUtility
{
    /// <summary>
    /// Configure network node with specified command - wielding administrative power
    /// </summary>
    /// <param name="nodeId">Target node identifier</param>
    /// <param name="command">Configuration command to execute</param>
    /// <returns>Configuration result - the chronicle of digital transformation</returns>
    Task<ConfigurationResult> ConfigureNodeAsync(string nodeId, ConfigCommand command, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get available configuration commands for node type - the arsenal of possibilities
    /// </summary>
    /// <param name="nodeType">Device type (router, server, workstation, etc.)</param>
    /// <returns>List of applicable commands - the toolkit of network mastery</returns>
    List<ConfigCommand> GetAvailableCommands(string nodeType);
    
    /// <summary>
    /// Validate configuration command before execution - wisdom before action
    /// </summary>
    /// <param name="command">Command to validate</param>
    /// <param name="targetNode">Target network node</param>
    /// <returns>Validation result with warnings and recommendations</returns>
    Task<ValidationResult> ValidateCommandAsync(ConfigCommand command, NetworkNode targetNode, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Generate PowerShell script for configuration - translating will to execution
    /// </summary>
    /// <param name="command">Configuration command</param>
    /// <param name="node">Target network node</param>
    /// <returns>PowerShell script ready for execution - the incantation of change</returns>
    string GeneratePowerShellScript(ConfigCommand command, NetworkNode node);
    
    /// <summary>
    /// Execute configuration and monitor progress - the conductor of digital symphony
    /// </summary>
    /// <param name="nodeId">Target node</param>
    /// <param name="command">Configuration command</param>
    /// <param name="progressCallback">Progress reporting callback</param>
    /// <returns>Execution result with detailed logs - the epic of transformation</returns>
    Task<ConfigurationResult> ExecuteWithProgressAsync(string nodeId, ConfigCommand command, 
        IProgress<string>? progressCallback = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Configuration command model - the blueprint of digital intentions
/// </summary>
public class ConfigCommand
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ConfigCommandType Type { get; set; }
    public Dictionary<string, string> Parameters { get; set; } = new();
    public string Script { get; set; } = string.Empty;
    public bool RequiresElevation { get; set; } = false;
    public bool RequiresReboot { get; set; } = false;
    public List<string> Prerequisites { get; set; } = new();
}

/// <summary>
/// Configuration result model - the chronicle of executed intentions
/// </summary>
public class ConfigurationResult
{
    public string CommandId { get; set; } = string.Empty;
    public string NodeId { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Output { get; set; } = string.Empty;
    public string ErrorDetails { get; set; } = string.Empty;
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
    public TimeSpan Duration { get; set; }
    public List<string> Warnings { get; set; } = new();
}

/// <summary>
/// Validation result model - the wisdom preceding action
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
    public string ValidationMessage { get; set; } = string.Empty;
}

/// <summary>
/// Configuration command types - the categories of digital mastery
/// </summary>
public enum ConfigCommandType
{
    NetworkInterface,   // IP, subnet, gateway configuration
    Security,          // Firewall, access control
    Performance,       // QoS, bandwidth settings
    Monitoring,        // SNMP, logging configuration
    Maintenance,       // Updates, patches, restarts
    Discovery,         // Network scanning, topology updates
    Troubleshooting,   // Diagnostic commands, connectivity tests
    Backup,           // Configuration backup and restore
    Custom            // User-defined scripts
}