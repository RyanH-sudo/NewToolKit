using System.ComponentModel;
using System.Drawing;
using System.Text.Json.Serialization;

namespace NetToolkit.Modules.AiOrb.Models;

/// <summary>
/// API configuration for AI services
/// Contains encrypted credentials and service settings
/// </summary>
public class ApiConfig
{
    public string Provider { get; set; } = "OpenAI";
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.openai.com";
    public string Model { get; set; } = "gpt-4";
    public int MaxTokens { get; set; } = 2000;
    public double Temperature { get; set; } = 0.7;
    public int TimeoutSeconds { get; set; } = 30;
    public bool EnableCaching { get; set; } = true;
    public int RateLimitPerMinute { get; set; } = 60;
    public Dictionary<string, string> CustomHeaders { get; set; } = new();
    public Dictionary<string, object> ProviderSpecificSettings { get; set; } = new();
}

/// <summary>
/// Chat context for more informed AI responses
/// </summary>
public class ChatContext
{
    public string SessionId { get; set; } = Guid.NewGuid().ToString();
    public string UserRole { get; set; } = "NetworkEngineer";
    public List<string> RecentCommands { get; set; } = new();
    public string CurrentModule { get; set; } = string.Empty;
    public Dictionary<string, object> EnvironmentInfo { get; set; } = new();
    public List<ChatMessage> ConversationHistory { get; set; } = new();
}

/// <summary>
/// Individual chat message
/// </summary>
public class ChatMessage
{
    public string Role { get; set; } = "user"; // user, assistant, system
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// OCR analysis result containing extracted text and AI insights
/// </summary>
public class OcrAnalysisResult
{
    public string ExtractedText { get; set; } = string.Empty;
    public double ConfidenceScore { get; set; }
    public string AiAnalysis { get; set; } = string.Empty;
    public List<NetworkingInsight> Insights { get; set; } = new();
    public List<ActionSuggestion> Suggestions { get; set; } = new();
    public TimeSpan ProcessingTime { get; set; }
    public Dictionary<string, object> TechnicalData { get; set; } = new();
}

/// <summary>
/// Types of OCR analysis that can be performed
/// </summary>
public enum OcrAnalysisType
{
    General,
    NetworkDiagram,
    ErrorMessage,
    Configuration,
    LogFile,
    CommandOutput,
    Documentation
}

/// <summary>
/// Networking-specific insight extracted from OCR
/// </summary>
public class NetworkingInsight
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Priority { get; set; }
    public List<string> RelatedConcepts { get; set; } = new();
    public string EducationalTip { get; set; } = string.Empty;
}

/// <summary>
/// Action suggestion based on analysis
/// </summary>
public class ActionSuggestion
{
    public string Action { get; set; } = string.Empty;
    public string Rationale { get; set; } = string.Empty;
    public string CommandSuggestion { get; set; } = string.Empty;
    public int Priority { get; set; }
    public List<string> Prerequisites { get; set; } = new();
    public string EstimatedTime { get; set; } = string.Empty;
}

/// <summary>
/// Code suggestion from CLI co-pilot analysis
/// </summary>
public class CodeSuggestion
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Language { get; set; } = "powershell";
    public SuggestionType Type { get; set; }
    public int ConfidenceLevel { get; set; }
    public List<string> Prerequisites { get; set; } = new();
    public string ExpectedOutcome { get; set; } = string.Empty;
    public List<string> RiskFactors { get; set; } = new();
    public Dictionary<string, object> Parameters { get; set; } = new();
}

/// <summary>
/// Types of code suggestions
/// </summary>
public enum SuggestionType
{
    Fix,
    Enhancement,
    Alternative,
    Automation,
    Optimization,
    Troubleshooting,
    Documentation
}

/// <summary>
/// CLI context for co-pilot analysis
/// </summary>
public class CliContext
{
    public string CurrentDirectory { get; set; } = string.Empty;
    public string Shell { get; set; } = "powershell";
    public string LastCommand { get; set; } = string.Empty;
    public int ExitCode { get; set; }
    public List<string> CommandHistory { get; set; } = new();
    public Dictionary<string, string> EnvironmentVariables { get; set; } = new();
    public string WorkingModule { get; set; } = string.Empty;
}

/// <summary>
/// Current status of the AI orb system
/// </summary>
public class OrbStatus
{
    public bool IsConfigured { get; set; }
    public bool ApiConnected { get; set; }
    public bool OcrReady { get; set; }
    public bool OverlayActive { get; set; }
    public string CurrentMode { get; set; } = "Idle";
    public DateTime LastActivity { get; set; }
    public Dictionary<string, object> HealthMetrics { get; set; } = new();
    public List<string> ActiveFeatures { get; set; } = new();
}

/// <summary>
/// Configuration validation result
/// </summary>
public class ConfigValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
}

/// <summary>
/// OCR text extraction result
/// </summary>
public class OcrTextResult
{
    public string Text { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public List<TextRegion> Regions { get; set; } = new();
    public TimeSpan ProcessingTime { get; set; }
    public string Language { get; set; } = "eng";
}

/// <summary>
/// Text region within OCR result
/// </summary>
public class TextRegion
{
    public Rectangle BoundingBox { get; set; }
    public string Text { get; set; } = string.Empty;
    public double Confidence { get; set; }
}

/// <summary>
/// Screen region for screenshot capture
/// </summary>
public class ScreenRegion
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public bool IsFullScreen => X == 0 && Y == 0 && Width == 0 && Height == 0;
}

/// <summary>
/// Text analysis result for extracted OCR text
/// </summary>
public class TextAnalysisResult
{
    public string Summary { get; set; } = string.Empty;
    public List<NetworkingInsight> Insights { get; set; } = new();
    public List<ActionSuggestion> Suggestions { get; set; } = new();
    public Dictionary<string, int> KeywordFrequency { get; set; } = new();
    public string SentimentAnalysis { get; set; } = "Neutral";
}

/// <summary>
/// Terminal output parsing result
/// </summary>
public class TerminalParseResult
{
    public bool HasErrors { get; set; }
    public bool HasWarnings { get; set; }
    public List<string> ErrorMessages { get; set; } = new();
    public List<string> WarningMessages { get; set; } = new();
    public List<string> SuccessIndicators { get; set; } = new();
    public string CommandType { get; set; } = string.Empty;
    public Dictionary<string, string> ParsedData { get; set; } = new();
    public List<ImprovementOpportunity> Opportunities { get; set; } = new();
}

/// <summary>
/// Improvement opportunity identified in CLI output
/// </summary>
public class ImprovementOpportunity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int ImpactScore { get; set; }
    public string SuggestedAction { get; set; } = string.Empty;
}

/// <summary>
/// Execution result for running suggested code
/// </summary>
public class ExecutionResult
{
    public bool Success { get; set; }
    public string Output { get; set; } = string.Empty;
    public string ErrorOutput { get; set; } = string.Empty;
    public int ExitCode { get; set; }
    public TimeSpan ExecutionTime { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Proactive suggestion for workflow improvement
/// </summary>
public class ProactiveSuggestion
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Priority { get; set; }
    public List<string> Benefits { get; set; } = new();
    public CodeSuggestion? CodeSuggestion { get; set; }
}

/// <summary>
/// Script template for network tasks
/// </summary>
public class ScriptTemplate
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Language { get; set; } = "powershell";
    public string Template { get; set; } = string.Empty;
    public List<ScriptParameter> Parameters { get; set; } = new();
    public List<string> RequiredModules { get; set; } = new();
    public string Category { get; set; } = string.Empty;
}

/// <summary>
/// Parameter for script template
/// </summary>
public class ScriptParameter
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = "string";
    public bool Required { get; set; }
    public string DefaultValue { get; set; } = string.Empty;
    public List<string> ValidValues { get; set; } = new();
}

/// <summary>
/// Network task types for script generation
/// </summary>
public enum NetworkTaskType
{
    NetworkScan,
    SecurityAudit,
    ConfigBackup,
    PerformanceMonitoring,
    TroubleshootingDiagnostic,
    Automation,
    Documentation,
    Compliance
}

/// <summary>
/// Visual state of the orb overlay
/// </summary>
public class OrbVisualState
{
    public bool IsGlowing { get; set; }
    public bool IsPulsing { get; set; }
    public Color PrimaryColor { get; set; } = Color.Blue;
    public Color AccentColor { get; set; } = Color.White;
    public double Opacity { get; set; } = 0.8;
    public int Size { get; set; } = 64;
    public string Animation { get; set; } = "None";
    public Dictionary<string, object> CustomProperties { get; set; } = new();
}

/// <summary>
/// Display information about the orb
/// </summary>
public class OrbDisplayInfo
{
    public Point Position { get; set; }
    public bool IsVisible { get; set; }
    public bool IsDraggable { get; set; }
    public OrbVisualState VisualState { get; set; } = new();
    public DateTime LastInteraction { get; set; }
}

/// <summary>
/// Event arguments for orb click events
/// </summary>
public class OrbClickEventArgs : EventArgs
{
    public Point ClickPosition { get; set; }
    public MouseButton Button { get; set; }
    public int ClickCount { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Event arguments for orb position changes
/// </summary>
public class OrbPositionChangedEventArgs : EventArgs
{
    public Point OldPosition { get; set; }
    public Point NewPosition { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Event arguments for orb state changes
/// </summary>
public class OrbStateChangedEventArgs : EventArgs
{
    public OrbVisualState OldState { get; set; } = new();
    public OrbVisualState NewState { get; set; } = new();
    public string ChangeReason { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Mouse button enumeration
/// </summary>
public enum MouseButton
{
    Left,
    Right,
    Middle,
    X1,
    X2
}