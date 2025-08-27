using NetToolkit.Modules.AiOrb.Models;

namespace NetToolkit.Modules.AiOrb.Events;

/// <summary>
/// Event fired when the AI Orb is activated
/// Signals to other modules that AI assistance is now available
/// </summary>
public class OrbActivatedEvent
{
    public string UserId { get; set; } = string.Empty;
    public DateTime ActivatedAt { get; set; } = DateTime.UtcNow;
    public ApiConfig Configuration { get; set; } = new();
    public List<string> AvailableFeatures { get; set; } = new();
    public string ActivationReason { get; set; } = "UserRequest";
}

/// <summary>
/// Event fired when AI chat response is generated
/// Allows other modules to react to AI conversations
/// </summary>
public class ChatResponseEvent
{
    public string SessionId { get; set; } = string.Empty;
    public string Prompt { get; set; } = string.Empty;
    public string Response { get; set; } = string.Empty;
    public TimeSpan ResponseTime { get; set; }
    public int TokensUsed { get; set; }
    public double ConfidenceScore { get; set; }
    public List<string> Keywords { get; set; } = new();
    public ChatContext Context { get; set; } = new();
}

/// <summary>
/// Event fired when OCR analysis is completed
/// Provides extracted text and analysis results to interested modules
/// </summary>
public class OcrAnalysisCompletedEvent
{
    public string AnalysisId { get; set; } = Guid.NewGuid().ToString();
    public OcrAnalysisResult Result { get; set; } = new();
    public byte[] OriginalImage { get; set; } = Array.Empty<byte>();
    public OcrAnalysisType AnalysisType { get; set; }
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Event fired when CLI co-pilot generates suggestions
/// Enables terminal modules to display or act on AI suggestions
/// </summary>
public class CoPilotSuggestionEvent
{
    public string SuggestionId { get; set; } = Guid.NewGuid().ToString();
    public CodeSuggestion Suggestion { get; set; } = new();
    public string SourceModule { get; set; } = string.Empty;
    public string TerminalOutput { get; set; } = string.Empty;
    public CliContext Context { get; set; } = new();
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Event fired when API configuration is updated
/// Notifies system of configuration changes
/// </summary>
public class ConfigurationUpdatedEvent
{
    public ApiConfig OldConfiguration { get; set; } = new();
    public ApiConfig NewConfiguration { get; set; } = new();
    public string UpdatedBy { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public List<string> ChangedFields { get; set; } = new();
    public string UpdateReason { get; set; } = string.Empty;
}

/// <summary>
/// Event fired when orb visual state changes
/// Useful for UI synchronization and status display
/// </summary>
public class OrbStateChangedEvent
{
    public OrbVisualState OldState { get; set; } = new();
    public OrbVisualState NewState { get; set; } = new();
    public string ChangeReason { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

/// <summary>
/// Event requesting AI assistance from other modules
/// Allows modules to request orb intervention for specific scenarios
/// </summary>
public class AiAssistanceRequestEvent
{
    public string RequestId { get; set; } = Guid.NewGuid().ToString();
    public string RequestingModule { get; set; } = string.Empty;
    public string AssistanceType { get; set; } = string.Empty; // Chat, OCR, CoPilot
    public string Context { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
    public int Priority { get; set; } = 1;
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Event providing proactive AI insights based on system events
/// The orb can analyze system-wide events and provide contextual assistance
/// </summary>
public class ProactiveInsightEvent
{
    public string InsightId { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Priority { get; set; }
    public List<ActionSuggestion> Suggestions { get; set; } = new();
    public string SourceEvent { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Event fired when script execution is requested through co-pilot
/// Allows secure execution of AI-suggested scripts
/// </summary>
public class ScriptExecutionRequestEvent
{
    public string ExecutionId { get; set; } = Guid.NewGuid().ToString();
    public CodeSuggestion Suggestion { get; set; } = new();
    public string TargetModule { get; set; } = string.Empty;
    public bool RequiresConfirmation { get; set; } = true;
    public Dictionary<string, object> ExecutionContext { get; set; } = new();
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Event fired when script execution completes
/// Provides execution results back to interested parties
/// </summary>
public class ScriptExecutionCompletedEvent
{
    public string ExecutionId { get; set; } = string.Empty;
    public ExecutionResult Result { get; set; } = new();
    public CodeSuggestion OriginalSuggestion { get; set; } = new();
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    public string ExecutedBy { get; set; } = string.Empty;
}

/// <summary>
/// Event fired when orb encounters an error
/// Provides error information for logging and user notification
/// </summary>
public class OrbErrorEvent
{
    public string ErrorId { get; set; } = Guid.NewGuid().ToString();
    public string ErrorType { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public Exception? Exception { get; set; }
    public string SourceComponent { get; set; } = string.Empty;
    public Dictionary<string, object> Context { get; set; } = new();
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public string Severity { get; set; } = "Warning";
}

/// <summary>
/// Event fired when orb provides educational tips
/// Integrates with education module for contextual learning
/// </summary>
public class EducationalTipEvent
{
    public string TipId { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int DifficultyLevel { get; set; } = 1;
    public List<string> RelatedConcepts { get; set; } = new();
    public string SourceContext { get; set; } = string.Empty;
    public DateTime ProvidedAt { get; set; } = DateTime.UtcNow;
}