namespace NetToolkit.Modules.Education.Events;

/// <summary>
/// Event fired when an IP-focused lesson is completed
/// Enables cross-module integration for IP address related functionality
/// </summary>
public class IPLessonCompletedEvent
{
    /// <summary>
    /// User who completed the IP lesson
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// ID of the completed IP lesson
    /// </summary>
    public required int LessonId { get; init; }

    /// <summary>
    /// Module 3 lesson identifier
    /// </summary>
    public required int ModuleId { get; init; }

    /// <summary>
    /// IP lesson number (1-20 for Module 3)
    /// </summary>
    public required int LessonNumber { get; init; }

    /// <summary>
    /// Quiz score percentage (0-100)
    /// </summary>
    public double QuizScore { get; init; }

    /// <summary>
    /// IP addresses or concepts covered in this lesson
    /// </summary>
    public List<string> IPConcepts { get; init; } = new();

    /// <summary>
    /// Suggested PowerShell scripts related to this IP lesson
    /// </summary>
    public List<string> SuggestedScripts { get; init; } = new();

    /// <summary>
    /// IP addresses to scan/analyze (if applicable)
    /// </summary>
    public List<string> IPAddressesToScan { get; init; } = new();

    /// <summary>
    /// When the lesson was completed
    /// </summary>
    public DateTime CompletedAt { get; init; }

    /// <summary>
    /// Whether this lesson qualifies for cross-module integration
    /// </summary>
    public bool EnablesIntegration { get; init; }

    /// <summary>
    /// Integration recommendations for other modules
    /// </summary>
    public Dictionary<string, object> IntegrationData { get; init; } = new();
}