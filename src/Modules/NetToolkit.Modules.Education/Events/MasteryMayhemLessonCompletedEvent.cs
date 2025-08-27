namespace NetToolkit.Modules.Education.Events;

/// <summary>
/// Event fired when a mastery mayhem lesson is completed
/// Enables ultimate cross-module integration and comprehensive project automation
/// </summary>
public class MasteryMayhemLessonCompletedEvent
{
    /// <summary>
    /// User who completed the mastery mayhem lesson
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// ID of the completed mastery lesson
    /// </summary>
    public required int LessonId { get; init; }

    /// <summary>
    /// Module 10 lesson identifier
    /// </summary>
    public required int ModuleId { get; init; }

    /// <summary>
    /// Mastery lesson number (1-20 for Module 10)
    /// </summary>
    public required int LessonNumber { get; init; }

    /// <summary>
    /// Quiz score percentage (0-100)
    /// </summary>
    public double QuizScore { get; init; }

    /// <summary>
    /// Mastery concepts synthesized in this lesson
    /// </summary>
    public List<string> MasteryConcepts { get; init; } = new();

    /// <summary>
    /// Cross-module knowledge areas integrated
    /// </summary>
    public List<string> IntegratedKnowledgeAreas { get; init; } = new();

    /// <summary>
    /// Advanced project templates unlocked
    /// </summary>
    public List<string> ProjectTemplates { get; init; } = new();

    /// <summary>
    /// Comprehensive automation scripts enabled
    /// </summary>
    public List<string> AutomationCapabilities { get; init; } = new();

    /// <summary>
    /// Ultimate troubleshooting techniques mastered
    /// </summary>
    public List<string> TroubleshootingMastery { get; init; } = new();

    /// <summary>
    /// Advanced security implementations unlocked
    /// </summary>
    public List<string> SecurityImplementations { get; init; } = new();

    /// <summary>
    /// Enterprise-level architecture patterns mastered
    /// </summary>
    public List<string> ArchitecturePatterns { get; init; } = new();

    /// <summary>
    /// Professional certification pathways opened
    /// </summary>
    public List<string> CertificationPathways { get; init; } = new();

    /// <summary>
    /// When the lesson was completed
    /// </summary>
    public DateTime CompletedAt { get; init; }

    /// <summary>
    /// Whether this lesson enables comprehensive project automation
    /// </summary>
    public bool EnablesComprehensiveAutomation { get; init; }

    /// <summary>
    /// Whether this lesson unlocks enterprise architecture consulting
    /// </summary>
    public bool UnlocksEnterpriseConsulting { get; init; }

    /// <summary>
    /// Whether this lesson enables advanced security auditing
    /// </summary>
    public bool EnablesAdvancedSecurity { get; init; }

    /// <summary>
    /// Whether this lesson unlocks professional-level capabilities
    /// </summary>
    public bool UnlocksProfessionalLevel { get; init; }

    /// <summary>
    /// Whether this lesson enables real-world project implementation
    /// </summary>
    public bool EnablesRealWorldProjects { get; init; }

    /// <summary>
    /// Integration data for ultimate cross-module functionality
    /// </summary>
    public Dictionary<string, object> UltimateIntegrationData { get; init; } = new();

    /// <summary>
    /// Witty mastery mayhem feedback message for the achievement
    /// </summary>
    public string MasteryMayhemFeedback { get; init; } = string.Empty;
}