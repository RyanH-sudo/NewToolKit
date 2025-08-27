namespace NetToolkit.Modules.Education.Events;

/// <summary>
/// Event fired when a protocol alchemy lesson is completed
/// Enables cross-module integration for protocol mixing automation and advanced networking
/// </summary>
public class ProtocolAlchemyLessonCompletedEvent
{
    /// <summary>
    /// User who completed the protocol alchemy lesson
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// ID of the completed protocol mixing lesson
    /// </summary>
    public required int LessonId { get; init; }

    /// <summary>
    /// Module 9 lesson identifier
    /// </summary>
    public required int ModuleId { get; init; }

    /// <summary>
    /// Protocol alchemy lesson number (1-20 for Module 9)
    /// </summary>
    public required int LessonNumber { get; init; }

    /// <summary>
    /// Quiz score percentage (0-100)
    /// </summary>
    public double QuizScore { get; init; }

    /// <summary>
    /// Protocol mixing concepts mastered in this lesson
    /// </summary>
    public List<string> ProtocolConcepts { get; init; } = new();

    /// <summary>
    /// Advanced protocol recipes unlocked
    /// </summary>
    public List<string> ProtocolRecipes { get; init; } = new();

    /// <summary>
    /// Network automation scripts using protocol combinations
    /// </summary>
    public List<string> AutomationScripts { get; init; } = new();

    /// <summary>
    /// Security enhancements for protocol mixing
    /// </summary>
    public List<string> SecurityEnhancements { get; init; } = new();

    /// <summary>
    /// Protocol troubleshooting techniques unlocked
    /// </summary>
    public List<string> TroubleshootingTechniques { get; init; } = new();

    /// <summary>
    /// Cross-layer integration patterns discovered
    /// </summary>
    public List<string> IntegrationPatterns { get; init; } = new();

    /// <summary>
    /// Performance optimization strategies for protocol mixes
    /// </summary>
    public List<string> OptimizationStrategies { get; init; } = new();

    /// <summary>
    /// When the lesson was completed
    /// </summary>
    public DateTime CompletedAt { get; init; }

    /// <summary>
    /// Whether this lesson enables advanced protocol automation
    /// </summary>
    public bool EnablesAdvancedAutomation { get; init; }

    /// <summary>
    /// Whether this lesson unlocks multiprotocol architecture features
    /// </summary>
    public bool UnlocksMultiprotocolArchitecture { get; init; }

    /// <summary>
    /// Whether this lesson enables protocol security analysis
    /// </summary>
    public bool EnablesProtocolSecurity { get; init; }

    /// <summary>
    /// Whether this lesson unlocks network performance optimization
    /// </summary>
    public bool UnlocksPerformanceOptimization { get; init; }

    /// <summary>
    /// Integration data for cross-module functionality
    /// </summary>
    public Dictionary<string, object> IntegrationData { get; init; } = new();

    /// <summary>
    /// Witty alchemical feedback message for the achievement
    /// </summary>
    public string AlchemicalFeedback { get; init; } = string.Empty;
}