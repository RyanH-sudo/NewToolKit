namespace NetToolkit.Modules.Education.Events;

/// <summary>
/// Event fired when a wireless lesson is completed with high scores
/// Enables cross-module integration for wireless networking automation, security, and topology visualization
/// </summary>
public class WirelessLessonCompletedEvent
{
    /// <summary>
    /// User who completed the wireless lesson
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// ID of the completed lesson
    /// </summary>
    public int LessonId { get; init; }

    /// <summary>
    /// Module ID (should be 7 for Wireless Wonders)
    /// </summary>
    public int ModuleId { get; init; }

    /// <summary>
    /// Lesson number within the module (1-20)
    /// </summary>
    public int LessonNumber { get; init; }

    /// <summary>
    /// Quiz score achieved (0-100)
    /// </summary>
    public double QuizScore { get; init; }

    /// <summary>
    /// When the lesson was completed
    /// </summary>
    public DateTime CompletedAt { get; init; }

    /// <summary>
    /// Whether this completion score enables cross-module integrations
    /// </summary>
    public bool EnablesIntegration { get; init; }

    /// <summary>
    /// Wireless concepts mastered in this lesson
    /// </summary>
    public List<string> WirelessConcepts { get; init; } = new();

    /// <summary>
    /// Suggested PowerShell scripts for wireless automation
    /// </summary>
    public List<string> SuggestedWirelessScripts { get; init; } = new();

    /// <summary>
    /// Wireless networks to scan/analyze (SSIDs, frequencies, etc.)
    /// </summary>
    public List<string> WirelessNetworksToAnalyze { get; init; } = new();

    /// <summary>
    /// Security scanning recommendations for wireless networks
    /// </summary>
    public List<string> WirelessSecurityScans { get; init; } = new();

    /// <summary>
    /// SSH configurations for wireless access points and devices
    /// </summary>
    public List<string> WirelessSSHCapabilities { get; init; } = new();

    /// <summary>
    /// 3D topology features to enable for wireless visualization
    /// </summary>
    public List<string> WirelessTopologyFeatures { get; init; } = new();

    /// <summary>
    /// IoT and smart device wireless protocols mastered
    /// </summary>
    public List<string> IoTProtocolsLearned { get; init; } = new();

    /// <summary>
    /// Frequency bands and wireless standards understood
    /// </summary>
    public List<string> FrequencyBandKnowledge { get; init; } = new();

    /// <summary>
    /// Wireless troubleshooting techniques learned
    /// </summary>
    public List<string> TroubleshootingTechniques { get; init; } = new();

    /// <summary>
    /// Mesh networking and advanced wireless concepts
    /// </summary>
    public List<string> AdvancedWirelessConcepts { get; init; } = new();

    /// <summary>
    /// NFC, Bluetooth, and short-range wireless capabilities
    /// </summary>
    public List<string> ShortRangeWirelessSkills { get; init; } = new();

    /// <summary>
    /// 5G, 6G, and future wireless technology awareness
    /// </summary>
    public List<string> FutureWirelessTechnologies { get; init; } = new();

    /// <summary>
    /// Custom wireless configurations created during learning
    /// </summary>
    public List<string> CustomWirelessConfigs { get; init; } = new();

    /// <summary>
    /// Integration data for cross-module wireless enhancements
    /// </summary>
    public Dictionary<string, object> WirelessIntegrationData { get; init; } = new();

    /// <summary>
    /// Wireless mastery level achieved in this lesson
    /// </summary>
    public string WirelessMasteryLevel { get; init; } = "";

    /// <summary>
    /// Cosmic wireless achievement message celebrating the completion
    /// </summary>
    public string WirelessAchievementMessage { get; init; } = "";

    /// <summary>
    /// PowerShell wireless cmdlets recommended for practice
    /// </summary>
    public List<string> RecommendedWirelessCmdlets { get; init; } = new();

    /// <summary>
    /// Security vulnerability scanning targets for wireless networks
    /// </summary>
    public List<string> WirelessVulnerabilityTargets { get; init; } = new();

    /// <summary>
    /// Network topology wireless visualization data
    /// </summary>
    public Dictionary<string, object> WirelessTopologyData { get; init; } = new();
}