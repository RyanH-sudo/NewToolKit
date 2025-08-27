namespace NetToolkit.Modules.Education.Events;

/// <summary>
/// Event fired when a user achieves IP mastery (completes Module 3 with high scores)
/// Unlocks advanced IP functionality across other NetToolkit modules
/// </summary>
public class IPMasteryUnlockedEvent
{
    /// <summary>
    /// User who achieved IP mastery
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// Overall Module 3 completion score
    /// </summary>
    public double OverallScore { get; init; }

    /// <summary>
    /// Mastery level achieved
    /// </summary>
    public required string MasteryLevel { get; init; } // "IP Initiate", "Address Architect", "Subnet Sorcerer", etc.

    /// <summary>
    /// IP skills unlocked for cross-module integration
    /// </summary>
    public List<string> UnlockedSkills { get; init; } = new();

    /// <summary>
    /// PowerShell Terminal Module integrations to enable
    /// </summary>
    public List<string> PowerShellIntegrations { get; init; } = new();

    /// <summary>
    /// Network Scanner Module features to unlock
    /// </summary>
    public List<string> ScannerFeatures { get; init; } = new();

    /// <summary>
    /// Security Module assessments to enable
    /// </summary>
    public List<string> SecurityAssessments { get; init; } = new();

    /// <summary>
    /// SSH Terminal Module configurations to unlock
    /// </summary>
    public List<string> SSHConfigurations { get; init; } = new();

    /// <summary>
    /// When mastery was achieved
    /// </summary>
    public DateTime AchievedAt { get; init; }

    /// <summary>
    /// Badges awarded for this mastery achievement
    /// </summary>
    public List<string> BadgesAwarded { get; init; } = new();

    /// <summary>
    /// Cosmic message for the achievement
    /// </summary>
    public required string CosmicMessage { get; init; }

    /// <summary>
    /// Additional data for module integrations
    /// </summary>
    public Dictionary<string, object> IntegrationData { get; init; } = new();
}