namespace NetToolkit.Modules.Education.Events;

/// <summary>
/// Event fired when a user achieves mastery in cloud computing (Module 8)
/// Signifies comprehensive understanding of cloud technologies and architectures
/// </summary>
public class CloudMasteryAchievedEvent
{
    /// <summary>
    /// User who achieved cloud mastery
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// Module 8 identifier
    /// </summary>
    public required int ModuleId { get; init; }

    /// <summary>
    /// Overall mastery score percentage
    /// </summary>
    public double OverallScore { get; init; }

    /// <summary>
    /// Number of lessons completed with excellence (>= 80%)
    /// </summary>
    public int ExcellentLessons { get; init; }

    /// <summary>
    /// Total time spent conquering the cloud domain
    /// </summary>
    public TimeSpan TotalTimeSpent { get; init; }

    /// <summary>
    /// Cloud mastery level achieved
    /// </summary>
    public string MasteryLevel { get; init; } = string.Empty;

    /// <summary>
    /// List of cloud platforms mastered
    /// </summary>
    public List<string> MasteredPlatforms { get; init; } = new();

    /// <summary>
    /// Advanced cloud skills unlocked
    /// </summary>
    public List<string> UnlockedSkills { get; init; } = new();

    /// <summary>
    /// Cloud architecture patterns understood
    /// </summary>
    public List<string> ArchitecturePatterns { get; init; } = new();

    /// <summary>
    /// Certification pathways suggested
    /// </summary>
    public List<string> CertificationPathways { get; init; } = new();

    /// <summary>
    /// When cloud mastery was achieved
    /// </summary>
    public DateTime AchievedAt { get; init; }

    /// <summary>
    /// Whether this enables expert-level cloud features
    /// </summary>
    public bool EnablesExpertFeatures { get; init; }

    /// <summary>
    /// Whether this unlocks cloud architecture consulting
    /// </summary>
    public bool UnlocksConsultingMode { get; init; }

    /// <summary>
    /// Whether this enables advanced multi-cloud orchestration
    /// </summary>
    public bool EnablesAdvancedOrchestration { get; init; }

    /// <summary>
    /// Cosmic congratulatory message for the supreme achievement
    /// </summary>
    public string CosmicCongratulations { get; init; } = string.Empty;

    /// <summary>
    /// Integration opportunities unlocked by cloud mastery
    /// </summary>
    public Dictionary<string, object> MasteryIntegrations { get; init; } = new();
}