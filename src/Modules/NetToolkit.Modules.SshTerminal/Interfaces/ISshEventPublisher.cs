using NetToolkit.Core.Interfaces;
using NetToolkit.Modules.SshTerminal.Models;

namespace NetToolkit.Modules.SshTerminal.Interfaces;

/// <summary>
/// SSH event publisher interface - the herald of digital terminal proclamations
/// Where connection events cascade through the modular cosmos with urgent tidings
/// </summary>
public interface ISshEventPublisher : IEventBus
{
    /// <summary>
    /// Publish connection established event - announcing successful digital bridge construction
    /// </summary>
    /// <param name="sessionId">Newly connected session identifier</param>
    /// <param name="connectionParams">Connection parameters used</param>
    /// <param name="connectionDuration">Time taken to establish connection</param>
    Task PublishConnectionEstablishedAsync(string sessionId, ConnectionParams connectionParams, TimeSpan connectionDuration);
    
    /// <summary>
    /// Publish data received event - heralding the arrival of digital consciousness streams
    /// </summary>
    /// <param name="sessionId">Source session identifier</param>
    /// <param name="data">Received data content</param>
    /// <param name="byteCount">Number of bytes received</param>
    /// <param name="connectionType">Type of connection</param>
    Task PublishDataReceivedAsync(string sessionId, string data, int byteCount, ConnectionType connectionType);
    
    /// <summary>
    /// Publish command executed event - chronicling digital incantations and their outcomes
    /// </summary>
    /// <param name="sessionId">Execution session identifier</param>
    /// <param name="command">Executed command</param>
    /// <param name="response">Command response</param>
    /// <param name="success">Execution success status</param>
    /// <param name="executionTime">Time taken to execute</param>
    Task PublishCommandExecutedAsync(string sessionId, string command, string? response, bool success, TimeSpan executionTime);
    
    /// <summary>
    /// Publish error encountered event - sounding alarms for digital tribulations
    /// </summary>
    /// <param name="sessionId">Affected session identifier</param>
    /// <param name="errorMessage">Description of the error</param>
    /// <param name="exception">Exception details if available</param>
    /// <param name="context">Contextual information about the error</param>
    Task PublishErrorEncounteredAsync(string sessionId, string errorMessage, Exception? exception = null, string? context = null);
    
    /// <summary>
    /// Publish device detected event - announcing discovery of new hardware companions
    /// </summary>
    /// <param name="device">Detected device information</param>
    /// <param name="deviceType">Type of detected device</param>
    Task PublishDeviceDetectedAsync(DeviceInfo device, DeviceType deviceType);
    
    /// <summary>
    /// Publish device lost event - lamenting the departure of hardware companions
    /// </summary>
    /// <param name="device">Lost device information</param>
    /// <param name="deviceType">Type of lost device</param>
    Task PublishDeviceLostAsync(DeviceInfo device, DeviceType deviceType);
    
    /// <summary>
    /// Publish session status change event - documenting the lifecycle of digital connections
    /// </summary>
    /// <param name="sessionId">Affected session identifier</param>
    /// <param name="oldStatus">Previous connection status</param>
    /// <param name="newStatus">New connection status</param>
    /// <param name="statusMessage">Optional status message</param>
    Task PublishSessionStatusChangedAsync(string sessionId, ConnectionStatus oldStatus, ConnectionStatus newStatus, string? statusMessage = null);
    
    /// <summary>
    /// Publish authentication attempt event - recording access attempts to digital realms
    /// </summary>
    /// <param name="sessionId">Session identifier</param>
    /// <param name="username">Username used for authentication</param>
    /// <param name="authMethod">Authentication method used</param>
    /// <param name="success">Authentication success status</param>
    /// <param name="host">Target host for network connections</param>
    Task PublishAuthenticationAttemptAsync(string sessionId, string username, string authMethod, bool success, string? host = null);
    
    /// <summary>
    /// Publish terminal output event - broadcasting terminal display updates for UI synchronization
    /// </summary>
    /// <param name="sessionId">Source session identifier</param>
    /// <param name="coloredOutput">Formatted terminal output</param>
    /// <param name="lineNumber">Output line number</param>
    Task PublishTerminalOutputAsync(string sessionId, ColoredOutput coloredOutput, int lineNumber);
    
    /// <summary>
    /// Publish file transfer event - chronicling data movement across digital realms
    /// </summary>
    /// <param name="sessionId">Transfer session identifier</param>
    /// <param name="fileName">Name of transferred file</param>
    /// <param name="direction">Transfer direction (upload/download)</param>
    /// <param name="byteCount">Number of bytes transferred</param>
    /// <param name="success">Transfer success status</param>
    Task PublishFileTransferAsync(string sessionId, string fileName, TransferDirection direction, long byteCount, bool success);
    
    /// <summary>
    /// Publish terminal configuration change event - announcing customization updates
    /// </summary>
    /// <param name="sessionId">Affected session identifier</param>
    /// <param name="configChange">Description of configuration change</param>
    /// <param name="newConfig">New terminal configuration</param>
    Task PublishTerminalConfigChangedAsync(string sessionId, string configChange, TerminalConfig newConfig);
    
    /// <summary>
    /// Publish session metrics event - sharing performance and usage statistics
    /// </summary>
    /// <param name="sessionId">Session identifier</param>
    /// <param name="metrics">Session metrics dictionary</param>
    Task PublishSessionMetricsAsync(string sessionId, Dictionary<string, object> metrics);
    
    /// <summary>
    /// Publish hardware integration event - announcing successful device connections
    /// </summary>
    /// <param name="sessionId">Session identifier</param>
    /// <param name="deviceInfo">Connected device information</param>
    /// <param name="connectionMethod">Method used for connection</param>
    /// <param name="success">Integration success status</param>
    Task PublishHardwareIntegrationAsync(string sessionId, DeviceInfo deviceInfo, string connectionMethod, bool success);
    
    /// <summary>
    /// Publish security alert event - warning of potential security concerns
    /// </summary>
    /// <param name="sessionId">Affected session identifier</param>
    /// <param name="alertType">Type of security alert</param>
    /// <param name="message">Alert message</param>
    /// <param name="severity">Alert severity level</param>
    Task PublishSecurityAlertAsync(string sessionId, SecurityAlertType alertType, string message, AlertSeverity severity);
    
    /// <summary>
    /// Publish AI insight event - sharing intelligent observations and suggestions
    /// </summary>
    /// <param name="sessionId">Session identifier</param>
    /// <param name="insightType">Type of AI insight</param>
    /// <param name="insight">AI-generated insight or suggestion</param>
    /// <param name="confidence">Confidence level of the insight</param>
    Task PublishAiInsightAsync(string sessionId, AiInsightType insightType, string insight, float confidence);
}

/// <summary>
/// Transfer direction enumeration
/// </summary>
public enum TransferDirection
{
    Upload,
    Download
}

/// <summary>
/// Security alert type enumeration
/// </summary>
public enum SecurityAlertType
{
    WeakAuthentication,
    SuspiciousActivity,
    UnencryptedConnection,
    PrivilegeEscalation,
    UnauthorizedAccess
}

/// <summary>
/// Alert severity enumeration
/// </summary>
public enum AlertSeverity
{
    Low,
    Medium,
    High,
    Critical
}

/// <summary>
/// AI insight type enumeration
/// </summary>
public enum AiInsightType
{
    CommandSuggestion,
    ErrorSolution,
    PerformanceOptimization,
    SecurityRecommendation,
    ConfigurationTip
}