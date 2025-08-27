namespace NetToolkit.Modules.Education.Events;

/// <summary>
/// Event fired when specific IP quiz questions are answered
/// Enables real-time IP address discovery and validation
/// </summary>
public class IPShenanigansQuizEvent
{
    /// <summary>
    /// User taking the IP quiz
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// Lesson ID for the IP quiz
    /// </summary>
    public required int LessonId { get; init; }

    /// <summary>
    /// Question that was answered
    /// </summary>
    public required string QuestionText { get; init; }

    /// <summary>
    /// User's answer to the question
    /// </summary>
    public required string UserAnswer { get; init; }

    /// <summary>
    /// Whether the answer was correct
    /// </summary>
    public bool IsCorrect { get; init; }

    /// <summary>
    /// IP addresses mentioned in the question/answer
    /// </summary>
    public List<string> IPAddressesInQuestion { get; init; } = new();

    /// <summary>
    /// Network concepts being tested
    /// </summary>
    public List<string> NetworkConcepts { get; init; } = new();

    /// <summary>
    /// Suggested real-world validation commands
    /// </summary>
    public List<string> ValidationCommands { get; init; } = new();

    /// <summary>
    /// Whether this answer triggers network discovery
    /// </summary>
    public bool TriggerNetworkDiscovery { get; init; }

    /// <summary>
    /// Scanner integration data (subnets to scan, etc.)
    /// </summary>
    public Dictionary<string, object> ScannerData { get; init; } = new();

    /// <summary>
    /// PowerShell scripts to demonstrate the concept
    /// </summary>
    public List<string> DemonstrationScripts { get; init; } = new();

    /// <summary>
    /// When the quiz answer was submitted
    /// </summary>
    public DateTime AnsweredAt { get; init; }

    /// <summary>
    /// Witty feedback for the answer
    /// </summary>
    public string WittyFeedback { get; init; } = string.Empty;
}