namespace NetToolkit.Modules.Education.Events;

/// <summary>
/// Event fired when a user achieves routing riddle mastery (completes Module 5 with high scores)
/// Unlocks advanced network topology visualization and routing optimization across NetToolkit modules
/// </summary>
public class RoutingRiddleMasteredEvent
{
    /// <summary>
    /// User who achieved routing mastery
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// Overall Module 5 completion score
    /// </summary>
    public double OverallScore { get; init; }

    /// <summary>
    /// Routing mastery level achieved
    /// </summary>
    public required string MasteryLevel { get; init; } // "Path Seeker", "OSPF Oracle", "Routing Riddle Master", etc.

    /// <summary>
    /// Routing protocols and concepts mastered
    /// </summary>
    public List<string> MasteredProtocols { get; init; } = new();

    /// <summary>
    /// Advanced 3D topology features to unlock
    /// </summary>
    public List<string> TopologyFeatures { get; init; } = new();

    /// <summary>
    /// Network scanning enhancements for routing discovery
    /// </summary>
    public List<string> ScanningEnhancements { get; init; } = new();

    /// <summary>
    /// PowerShell routing automation scripts to enable
    /// </summary>
    public List<string> AutomationScripts { get; init; } = new();

    /// <summary>
    /// Security routing features and protections
    /// </summary>
    public List<string> SecurityFeatures { get; init; } = new();

    /// <summary>
    /// SSH tunneling and routing capabilities
    /// </summary>
    public List<string> SSHRoutingCapabilities { get; init; } = new();

    /// <summary>
    /// Network troubleshooting and diagnostic tools
    /// </summary>
    public List<string> DiagnosticTools { get; init; } = new();

    /// <summary>
    /// When mastery was achieved
    /// </summary>
    public DateTime AchievedAt { get; init; }

    /// <summary>
    /// Badges awarded for this routing mastery achievement
    /// </summary>
    public List<string> BadgesAwarded { get; init; } = new();

    /// <summary>
    /// Cosmic riddle mastery message celebrating the achievement
    /// </summary>
    public required string CosmicRiddleMessage { get; init; }

    /// <summary>
    /// Custom routing configurations created during learning
    /// </summary>
    public List<string> CustomRoutingConfigs { get; init; } = new();

    /// <summary>
    /// Integration data for cross-module routing enhancements
    /// </summary>
    public Dictionary<string, object> IntegrationData { get; init; } = new();

    /// <summary>
    /// Whether user qualifies as a network pathfinding mentor
    /// </summary>
    public bool QualifiesAsPathfindingMentor { get; init; }

    /// <summary>
    /// Advanced certification-level features unlocked
    /// </summary>
    public List<string> CertificationFeatures { get; init; } = new();
}