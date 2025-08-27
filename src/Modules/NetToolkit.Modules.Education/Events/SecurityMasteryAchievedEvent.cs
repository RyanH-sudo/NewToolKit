namespace NetToolkit.Modules.Education.Events;

/// <summary>
/// Event fired when a user achieves security mastery (completes Module 6 with high scores)
/// Unlocks advanced security scanning, penetration testing, and cross-module security automation
/// </summary>
public class SecurityMasteryAchievedEvent
{
    /// <summary>
    /// User who achieved security mastery
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// Overall Module 6 completion score
    /// </summary>
    public double OverallScore { get; init; }

    /// <summary>
    /// Security mastery level achieved
    /// </summary>
    public required string MasteryLevel { get; init; } // "Security Sentinel", "Fortress Guardian", "Security Shenanigan Master", etc.

    /// <summary>
    /// Security domains and concepts mastered
    /// </summary>
    public List<string> MasteredSecurityDomains { get; init; } = new();

    /// <summary>
    /// Advanced security scanning capabilities to unlock
    /// </summary>
    public List<string> AdvancedScanningCapabilities { get; init; } = new();

    /// <summary>
    /// Penetration testing tools and techniques enabled
    /// </summary>
    public List<string> PentestingCapabilities { get; init; } = new();

    /// <summary>
    /// PowerShell security automation scripts to enable
    /// </summary>
    public List<string> SecurityAutomationScripts { get; init; } = new();

    /// <summary>
    /// Network topology security visualization features
    /// </summary>
    public List<string> TopologySecurityFeatures { get; init; } = new();

    /// <summary>
    /// Advanced SSH security and tunneling capabilities
    /// </summary>
    public List<string> SSHSecurityCapabilities { get; init; } = new();

    /// <summary>
    /// Incident response and forensics tools enabled
    /// </summary>
    public List<string> IncidentResponseTools { get; init; } = new();

    /// <summary>
    /// Compliance and governance capabilities unlocked
    /// </summary>
    public List<string> ComplianceCapabilities { get; init; } = new();

    /// <summary>
    /// When mastery was achieved
    /// </summary>
    public DateTime AchievedAt { get; init; }

    /// <summary>
    /// Badges awarded for this security mastery achievement
    /// </summary>
    public List<string> BadgesAwarded { get; init; } = new();

    /// <summary>
    /// Cosmic security mastery message celebrating the achievement
    /// </summary>
    public required string SecurityMasteryMessage { get; init; }

    /// <summary>
    /// Custom security configurations created during learning
    /// </summary>
    public List<string> CustomSecurityConfigs { get; init; } = new();

    /// <summary>
    /// Integration data for cross-module security enhancements
    /// </summary>
    public Dictionary<string, object> IntegrationData { get; init; } = new();

    /// <summary>
    /// Whether user qualifies as a security mentor
    /// </summary>
    public bool QualifiesAsSecurityMentor { get; init; }

    /// <summary>
    /// Advanced certification-level security features unlocked
    /// </summary>
    public List<string> CertificationSecurityFeatures { get; init; } = new();

    /// <summary>
    /// Threat hunting capabilities enabled
    /// </summary>
    public List<string> ThreatHuntingCapabilities { get; init; } = new();

    /// <summary>
    /// Zero Trust architecture features unlocked
    /// </summary>
    public List<string> ZeroTrustFeatures { get; init; } = new();

    /// <summary>
    /// Cloud security capabilities enabled
    /// </summary>
    public List<string> CloudSecurityCapabilities { get; init; } = new();
}