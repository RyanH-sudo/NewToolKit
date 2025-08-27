namespace NetToolkit.Modules.Education.Events;

/// <summary>
/// Event fired when a user achieves mastery in protocol alchemy (Module 9)
/// Signifies comprehensive understanding of advanced protocol mixing and network engineering
/// </summary>
public class ProtocolAlchemyMasteredEvent
{
    /// <summary>
    /// User who achieved protocol alchemy mastery
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// Module 9 identifier
    /// </summary>
    public required int ModuleId { get; init; }

    /// <summary>
    /// Overall mastery score percentage
    /// </summary>
    public double OverallScore { get; init; }

    /// <summary>
    /// Number of lessons completed with excellence (>= 85%)
    /// </summary>
    public int ExcellentLessons { get; init; }

    /// <summary>
    /// Total time spent mastering protocol alchemy
    /// </summary>
    public TimeSpan TotalTimeSpent { get; init; }

    /// <summary>
    /// Alchemy mastery level achieved
    /// </summary>
    public string MasteryLevel { get; init; } = string.Empty;

    /// <summary>
    /// List of protocol families mastered
    /// </summary>
    public List<string> MasteredProtocolFamilies { get; init; } = new();

    /// <summary>
    /// Advanced alchemical skills unlocked
    /// </summary>
    public List<string> UnlockedAlchemicalSkills { get; init; } = new();

    /// <summary>
    /// Protocol architecture patterns mastered
    /// </summary>
    public List<string> ArchitecturePatterns { get; init; } = new();

    /// <summary>
    /// Network engineering certification pathways unlocked
    /// </summary>
    public List<string> CertificationPathways { get; init; } = new();

    /// <summary>
    /// Advanced troubleshooting capabilities gained
    /// </summary>
    public List<string> TroubleshootingCapabilities { get; init; } = new();

    /// <summary>
    /// Performance optimization techniques mastered
    /// </summary>
    public List<string> OptimizationTechniques { get; init; } = new();

    /// <summary>
    /// When protocol alchemy mastery was achieved
    /// </summary>
    public DateTime AchievedAt { get; init; }

    /// <summary>
    /// Whether this enables expert-level protocol features
    /// </summary>
    public bool EnablesExpertProtocolFeatures { get; init; }

    /// <summary>
    /// Whether this unlocks network architecture consulting capabilities
    /// </summary>
    public bool UnlocksArchitectureConsulting { get; init; }

    /// <summary>
    /// Whether this enables advanced multiprotocol orchestration
    /// </summary>
    public bool EnablesAdvancedOrchestration { get; init; }

    /// <summary>
    /// Whether this unlocks protocol research and development features
    /// </summary>
    public bool UnlocksProtocolResearch { get; init; }

    /// <summary>
    /// Cosmic alchemical congratulatory message for the supreme achievement
    /// </summary>
    public string CosmicAlchemicalCongratulations { get; init; } = string.Empty;

    /// <summary>
    /// Advanced integration opportunities unlocked by protocol alchemy mastery
    /// </summary>
    public Dictionary<string, object> MasteryIntegrations { get; init; } = new();
}