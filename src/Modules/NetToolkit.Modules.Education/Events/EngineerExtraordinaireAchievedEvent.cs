namespace NetToolkit.Modules.Education.Events;

/// <summary>
/// Event fired when a user achieves Engineer Extraordinaire status (Module 10 completion)
/// Signifies ultimate mastery of all network engineering domains and comprehensive expertise
/// </summary>
public class EngineerExtraordinaireAchievedEvent
{
    /// <summary>
    /// User who achieved Engineer Extraordinaire status
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// Module 10 identifier
    /// </summary>
    public required int ModuleId { get; init; }

    /// <summary>
    /// Overall mastery score percentage across all modules
    /// </summary>
    public double OverallMasteryScore { get; init; }

    /// <summary>
    /// Number of lessons completed with excellence (>= 90%)
    /// </summary>
    public int ExcellentLessons { get; init; }

    /// <summary>
    /// Total time spent achieving complete mastery
    /// </summary>
    public TimeSpan TotalMasteryTime { get; init; }

    /// <summary>
    /// Engineer Extraordinaire level achieved
    /// </summary>
    public string ExtraordinaireLevel { get; init; } = string.Empty;

    /// <summary>
    /// All network engineering domains mastered
    /// </summary>
    public List<string> MasteredDomains { get; init; } = new();

    /// <summary>
    /// Ultimate engineering skills unlocked
    /// </summary>
    public List<string> UltimateEngineeringSkills { get; init; } = new();

    /// <summary>
    /// Comprehensive project capabilities gained
    /// </summary>
    public List<string> ProjectCapabilities { get; init; } = new();

    /// <summary>
    /// Advanced troubleshooting and diagnostic expertise
    /// </summary>
    public List<string> DiagnosticExpertise { get; init; } = new();

    /// <summary>
    /// Enterprise architecture and design mastery
    /// </summary>
    public List<string> ArchitectureMastery { get; init; } = new();

    /// <summary>
    /// Professional certification readiness achieved
    /// </summary>
    public List<string> CertificationReadiness { get; init; } = new();

    /// <summary>
    /// Industry leadership capabilities unlocked
    /// </summary>
    public List<string> LeadershipCapabilities { get; init; } = new();

    /// <summary>
    /// When Engineer Extraordinaire status was achieved
    /// </summary>
    public DateTime AchievedAt { get; init; }

    /// <summary>
    /// Whether this enables industry-level consulting capabilities
    /// </summary>
    public bool EnablesIndustryConsulting { get; init; }

    /// <summary>
    /// Whether this unlocks advanced research and development opportunities
    /// </summary>
    public bool UnlocksAdvancedRnD { get; init; }

    /// <summary>
    /// Whether this enables comprehensive network architecture design
    /// </summary>
    public bool EnablesArchitectureDesign { get; init; }

    /// <summary>
    /// Whether this unlocks professional teaching and mentoring capabilities
    /// </summary>
    public bool UnlocksTeachingMentoring { get; init; }

    /// <summary>
    /// Whether this enables industry leadership and innovation roles
    /// </summary>
    public bool EnablesIndustryLeadership { get; init; }

    /// <summary>
    /// Comprehensive badge collection achieved
    /// </summary>
    public List<string> BadgeCollection { get; init; } = new();

    /// <summary>
    /// Ultimate cosmic congratulatory message for achieving Engineer Extraordinaire status
    /// </summary>
    public string UltimateCosmicCongratulations { get; init; } = string.Empty;

    /// <summary>
    /// Ultimate mastery integrations unlocked by Engineer Extraordinaire achievement
    /// </summary>
    public Dictionary<string, object> UltimateMasteryIntegrations { get; init; } = new();
}