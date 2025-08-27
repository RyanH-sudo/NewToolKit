namespace NetToolkit.Modules.Education.Events;

/// <summary>
/// Event fired when a routing-focused lesson is completed
/// Enables cross-module integration for network topology visualization and path optimization
/// </summary>
public class RoutingLessonCompletedEvent
{
    /// <summary>
    /// User who completed the routing lesson
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// ID of the completed routing lesson
    /// </summary>
    public required int LessonId { get; init; }

    /// <summary>
    /// Module 5 lesson identifier
    /// </summary>
    public required int ModuleId { get; init; }

    /// <summary>
    /// Routing lesson number (1-20 for Module 5)
    /// </summary>
    public required int LessonNumber { get; init; }

    /// <summary>
    /// Quiz score percentage (0-100)
    /// </summary>
    public double QuizScore { get; init; }

    /// <summary>
    /// Routing concepts mastered in this lesson
    /// </summary>
    public List<string> RoutingConcepts { get; init; } = new();

    /// <summary>
    /// PowerShell routing scripts to demonstrate or execute
    /// </summary>
    public List<string> RoutingScripts { get; init; } = new();

    /// <summary>
    /// Network topology enhancements to visualize
    /// </summary>
    public List<string> TopologyEnhancements { get; init; } = new();

    /// <summary>
    /// Routing security considerations unlocked
    /// </summary>
    public List<string> SecurityConsiderations { get; init; } = new();

    /// <summary>
    /// Troubleshooting tools and techniques enabled
    /// </summary>
    public List<string> TroubleshootingTools { get; init; } = new();

    /// <summary>
    /// When the lesson was completed
    /// </summary>
    public DateTime CompletedAt { get; init; }

    /// <summary>
    /// Whether this lesson enables 3D topology visualization
    /// </summary>
    public bool Enables3DTopology { get; init; }

    /// <summary>
    /// Whether this lesson unlocks advanced routing features
    /// </summary>
    public bool UnlocksAdvancedRouting { get; init; }

    /// <summary>
    /// Network protocols and standards covered
    /// </summary>
    public List<string> ProtocolsCovered { get; init; } = new();

    /// <summary>
    /// Integration data for cross-module functionality
    /// </summary>
    public Dictionary<string, object> IntegrationData { get; init; } = new();

    /// <summary>
    /// Witty routing riddle feedback message
    /// </summary>
    public string RiddleFeedback { get; init; } = string.Empty;
}