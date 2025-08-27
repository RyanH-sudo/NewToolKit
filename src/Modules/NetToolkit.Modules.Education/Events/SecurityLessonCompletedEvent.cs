namespace NetToolkit.Modules.Education.Events;

/// <summary>
/// Event fired when a security-focused lesson is completed
/// Enables cross-module integration for security scanning, PowerShell security scripts, and defensive measures
/// </summary>
public class SecurityLessonCompletedEvent
{
    /// <summary>
    /// User who completed the security lesson
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// ID of the completed security lesson
    /// </summary>
    public required int LessonId { get; init; }

    /// <summary>
    /// Module 6 lesson identifier
    /// </summary>
    public required int ModuleId { get; init; }

    /// <summary>
    /// Security lesson number (1-20 for Module 6)
    /// </summary>
    public required int LessonNumber { get; init; }

    /// <summary>
    /// Quiz score percentage (0-100)
    /// </summary>
    public double QuizScore { get; init; }

    /// <summary>
    /// Security concepts mastered in this lesson
    /// </summary>
    public List<string> SecurityConcepts { get; init; } = new();

    /// <summary>
    /// PowerShell security scripts to demonstrate or execute
    /// </summary>
    public List<string> SecurityScripts { get; init; } = new();

    /// <summary>
    /// Security scanning types to enable or demonstrate
    /// </summary>
    public List<string> SecurityScanTypes { get; init; } = new();

    /// <summary>
    /// Vulnerability assessment techniques unlocked
    /// </summary>
    public List<string> VulnerabilityTechniques { get; init; } = new();

    /// <summary>
    /// Network security enhancements to visualize
    /// </summary>
    public List<string> NetworkSecurityEnhancements { get; init; } = new();

    /// <summary>
    /// SSH security configurations enabled
    /// </summary>
    public List<string> SSHSecurityConfigs { get; init; } = new();

    /// <summary>
    /// When the lesson was completed
    /// </summary>
    public DateTime CompletedAt { get; init; }

    /// <summary>
    /// Whether this lesson enables advanced security scanning
    /// </summary>
    public bool EnablesAdvancedScanning { get; init; }

    /// <summary>
    /// Whether this lesson unlocks penetration testing features
    /// </summary>
    public bool UnlocksPentestingFeatures { get; init; }

    /// <summary>
    /// Security standards and compliance frameworks covered
    /// </summary>
    public List<string> ComplianceFrameworks { get; init; } = new();

    /// <summary>
    /// Integration data for cross-module security functionality
    /// </summary>
    public Dictionary<string, object> IntegrationData { get; init; } = new();

    /// <summary>
    /// Witty security shenanigan feedback message
    /// </summary>
    public string SecurityFeedback { get; init; } = string.Empty;

    /// <summary>
    /// Threat intelligence insights gained from lesson
    /// </summary>
    public List<string> ThreatIntelInsights { get; init; } = new();

    /// <summary>
    /// Security automation capabilities unlocked
    /// </summary>
    public List<string> AutomationCapabilities { get; init; } = new();
}