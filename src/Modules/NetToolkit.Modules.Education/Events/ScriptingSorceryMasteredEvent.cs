namespace NetToolkit.Modules.Education.Events;

/// <summary>
/// Event fired when a user achieves scripting sorcery mastery (completes Module 4 with high scores)
/// Unlocks advanced PowerShell automation across all NetToolkit modules
/// </summary>
public class ScriptingSorceryMasteredEvent
{
    /// <summary>
    /// User who achieved scripting mastery
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// Overall Module 4 completion score
    /// </summary>
    public double OverallScore { get; init; }

    /// <summary>
    /// Scripting mastery level achieved
    /// </summary>
    public required string MasteryLevel { get; init; } // "Script Apprentice", "Automation Archmage", "Script Sorcerer Supreme", etc.

    /// <summary>
    /// PowerShell skills unlocked for automation
    /// </summary>
    public List<string> UnlockedSkills { get; init; } = new();

    /// <summary>
    /// Advanced PowerShell Terminal features to enable
    /// </summary>
    public List<string> PowerShellFeatures { get; init; } = new();

    /// <summary>
    /// Network automation scripts to unlock
    /// </summary>
    public List<string> NetworkAutomationScripts { get; init; } = new();

    /// <summary>
    /// Security automation capabilities to enable
    /// </summary>
    public List<string> SecurityAutomationFeatures { get; init; } = new();

    /// <summary>
    /// SSH automation and remote scripting features
    /// </summary>
    public List<string> RemoteScriptingFeatures { get; init; } = new();

    /// <summary>
    /// Microsoft integration enhancements
    /// </summary>
    public List<string> MicrosoftIntegrations { get; init; } = new();

    /// <summary>
    /// When mastery was achieved
    /// </summary>
    public DateTime AchievedAt { get; init; }

    /// <summary>
    /// Badges awarded for this mastery achievement
    /// </summary>
    public List<string> BadgesAwarded { get; init; } = new();

    /// <summary>
    /// Cosmic message celebrating the achievement
    /// </summary>
    public required string CosmicMessage { get; init; }

    /// <summary>
    /// Custom PowerShell scripts created during the module
    /// </summary>
    public List<string> CustomScriptsCreated { get; init; } = new();

    /// <summary>
    /// Integration data for cross-module automation
    /// </summary>
    public Dictionary<string, object> IntegrationData { get; init; } = new();

    /// <summary>
    /// Whether user qualifies for advanced scripting mentor role
    /// </summary>
    public bool QualifiesForMentorRole { get; init; }
}