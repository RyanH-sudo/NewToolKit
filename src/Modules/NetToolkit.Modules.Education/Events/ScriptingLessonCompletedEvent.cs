namespace NetToolkit.Modules.Education.Events;

/// <summary>
/// Event fired when a scripting-focused lesson is completed
/// Enables cross-module integration for PowerShell automation and network scripting
/// </summary>
public class ScriptingLessonCompletedEvent
{
    /// <summary>
    /// User who completed the scripting lesson
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// ID of the completed scripting lesson
    /// </summary>
    public required int LessonId { get; init; }

    /// <summary>
    /// Module 4 lesson identifier
    /// </summary>
    public required int ModuleId { get; init; }

    /// <summary>
    /// Scripting lesson number (1-20 for Module 4)
    /// </summary>
    public required int LessonNumber { get; init; }

    /// <summary>
    /// Quiz score percentage (0-100)
    /// </summary>
    public double QuizScore { get; init; }

    /// <summary>
    /// Scripting concepts mastered in this lesson
    /// </summary>
    public List<string> ScriptingConcepts { get; init; } = new();

    /// <summary>
    /// PowerShell scripts to demonstrate or execute
    /// </summary>
    public List<string> DemonstrationScripts { get; init; } = new();

    /// <summary>
    /// Network automation opportunities unlocked
    /// </summary>
    public List<string> AutomationOpportunities { get; init; } = new();

    /// <summary>
    /// Security scripting enhancements available
    /// </summary>
    public List<string> SecurityEnhancements { get; init; } = new();

    /// <summary>
    /// When the lesson was completed
    /// </summary>
    public DateTime CompletedAt { get; init; }

    /// <summary>
    /// Whether this lesson enables PowerShell Terminal integration
    /// </summary>
    public bool EnablesPowerShellIntegration { get; init; }

    /// <summary>
    /// Whether this lesson unlocks network automation features
    /// </summary>
    public bool UnlocksNetworkAutomation { get; init; }

    /// <summary>
    /// Integration data for cross-module functionality
    /// </summary>
    public Dictionary<string, object> IntegrationData { get; init; } = new();

    /// <summary>
    /// Witty feedback message for the achievement
    /// </summary>
    public string WittyFeedback { get; init; } = string.Empty;

    /// <summary>
    /// Network automation opportunities identified for this lesson
    /// </summary>
    public List<string> NetworkAutomationOpportunities { get; init; } = new();
}