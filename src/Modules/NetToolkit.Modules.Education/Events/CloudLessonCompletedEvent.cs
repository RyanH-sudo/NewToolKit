namespace NetToolkit.Modules.Education.Events;

/// <summary>
/// Event fired when a cloud-focused lesson is completed
/// Enables cross-module integration for cloud automation, security, and infrastructure management
/// </summary>
public class CloudLessonCompletedEvent
{
    /// <summary>
    /// User who completed the cloud lesson
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// ID of the completed cloud lesson
    /// </summary>
    public required int LessonId { get; init; }

    /// <summary>
    /// Module 8 lesson identifier
    /// </summary>
    public required int ModuleId { get; init; }

    /// <summary>
    /// Cloud lesson number (1-20 for Module 8)
    /// </summary>
    public required int LessonNumber { get; init; }

    /// <summary>
    /// Quiz score percentage (0-100)
    /// </summary>
    public double QuizScore { get; init; }

    /// <summary>
    /// Cloud concepts mastered in this lesson
    /// </summary>
    public List<string> CloudConcepts { get; init; } = new();

    /// <summary>
    /// Cloud scripts to demonstrate or execute (Azure CLI, AWS CLI, GCP CLI)
    /// </summary>
    public List<string> CloudScripts { get; init; } = new();

    /// <summary>
    /// Azure automation opportunities unlocked
    /// </summary>
    public List<string> AzureAutomationOpportunities { get; init; } = new();

    /// <summary>
    /// AWS automation opportunities unlocked
    /// </summary>
    public List<string> AWSAutomationOpportunities { get; init; } = new();

    /// <summary>
    /// Google Cloud automation opportunities unlocked
    /// </summary>
    public List<string> GCPAutomationOpportunities { get; init; } = new();

    /// <summary>
    /// Cloud security enhancements available
    /// </summary>
    public List<string> SecurityEnhancements { get; init; } = new();

    /// <summary>
    /// Infrastructure as Code templates unlocked
    /// </summary>
    public List<string> IaCTemplates { get; init; } = new();

    /// <summary>
    /// Network topology patterns for cloud architecture
    /// </summary>
    public List<string> NetworkTopologyPatterns { get; init; } = new();

    /// <summary>
    /// When the lesson was completed
    /// </summary>
    public DateTime CompletedAt { get; init; }

    /// <summary>
    /// Whether this lesson enables cloud automation integration
    /// </summary>
    public bool EnablesCloudAutomation { get; init; }

    /// <summary>
    /// Whether this lesson unlocks multi-cloud management features
    /// </summary>
    public bool UnlocksMultiCloudManagement { get; init; }

    /// <summary>
    /// Whether this lesson enables cloud security scanning
    /// </summary>
    public bool EnablesCloudSecurity { get; init; }

    /// <summary>
    /// Whether this lesson unlocks hybrid cloud features
    /// </summary>
    public bool UnlocksHybridCloud { get; init; }

    /// <summary>
    /// Integration data for cross-module functionality
    /// </summary>
    public Dictionary<string, object> IntegrationData { get; init; } = new();

    /// <summary>
    /// Witty cloud conquest feedback message for the achievement
    /// </summary>
    public string WittyFeedback { get; init; } = string.Empty;
}