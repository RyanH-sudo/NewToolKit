using NetToolkit.Modules.Education.Models;

namespace NetToolkit.Modules.Education.Interfaces;

/// <summary>
/// Slide generator interface - the mystical creator of educational visual experiences
/// Where learning content transforms into captivating slides with cosmic artistry
/// </summary>
public interface ISlideGenerator
{
    /// <summary>
    /// Generate image slide with SkiaSharp graphics
    /// </summary>
    /// <param name="title">Slide title</param>
    /// <param name="description">Content description for image generation</param>
    /// <param name="style">Image style preference</param>
    /// <param name="width">Image width in pixels</param>
    /// <param name="height">Image height in pixels</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Generated image as byte array</returns>
    Task<byte[]> GenerateImageSlideAsync(
        string title, 
        string description, 
        ImageStyle style = ImageStyle.Cartoon,
        int width = 800, 
        int height = 600,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Generate text slide with hover tips and formatting
    /// </summary>
    /// <param name="title">Slide title</param>
    /// <param name="content">Main text content</param>
    /// <param name="tips">Hover tips to embed</param>
    /// <param name="style">Text formatting style</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Formatted HTML content</returns>
    Task<string> GenerateTextSlideAsync(
        string title,
        string content,
        List<HoverTip> tips,
        TextStyle style = TextStyle.Educational,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Generate quiz slide with multiple choice options
    /// </summary>
    /// <param name="question">Quiz question data</param>
    /// <param name="style">Quiz presentation style</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Interactive quiz slide data</returns>
    Task<QuizSlideData> GenerateQuizSlideAsync(
        QuizQuestion question,
        QuizStyle style = QuizStyle.Gamified,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Generate interactive Three.js slide with 3D models
    /// </summary>
    /// <param name="title">Slide title</param>
    /// <param name="modelType">Type of 3D model to create</param>
    /// <param name="interactionData">Interaction parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Three.js scene configuration</returns>
    Task<ThreeJsSlideData> GenerateThreeJsSlideAsync(
        string title,
        ModelType modelType,
        Dictionary<string, object> interactionData,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Generate introduction slide for lesson start
    /// </summary>
    /// <param name="lessonTitle">Lesson title</param>
    /// <param name="objectives">Learning objectives</param>
    /// <param name="estimatedMinutes">Estimated completion time</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Introduction slide data</returns>
    Task<Slide> GenerateIntroductionSlideAsync(
        string lessonTitle,
        List<string> objectives,
        int estimatedMinutes,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Generate summary slide for lesson conclusion
    /// </summary>
    /// <param name="lessonTitle">Lesson title</param>
    /// <param name="keyPoints">Key learning points</param>
    /// <param name="nextSteps">What comes next</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Summary slide data</returns>
    Task<Slide> GenerateSummarySlideAsync(
        string lessonTitle,
        List<string> keyPoints,
        List<string> nextSteps,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Generate network diagram slide (specific to Module 1)
    /// </summary>
    /// <param name="diagramType">Type of network diagram</param>
    /// <param name="nodes">Network nodes to display</param>
    /// <param name="connections">Node connections</param>
    /// <param name="annotations">Explanatory annotations</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Network diagram image</returns>
    Task<byte[]> GenerateNetworkDiagramAsync(
        NetworkDiagramType diagramType,
        List<NetworkNode> nodes,
        List<NetworkConnection> connections,
        List<DiagramAnnotation> annotations,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Generate animated slide sequence for complex concepts
    /// </summary>
    /// <param name="concept">Concept to animate</param>
    /// <param name="steps">Animation steps</param>
    /// <param name="duration">Total animation duration in seconds</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Animation slide data</returns>
    Task<AnimationSlideData> GenerateAnimatedSlideAsync(
        string concept,
        List<AnimationStep> steps,
        int duration,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Generate slide with embedded hover tips
    /// </summary>
    /// <param name="baseSlide">Base slide content</param>
    /// <param name="tips">Tips to embed</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Enhanced slide with interactive tips</returns>
    Task<Slide> EmbedHoverTipsAsync(
        Slide baseSlide,
        List<HoverTip> tips,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Validate slide content for educational quality
    /// </summary>
    /// <param name="slide">Slide to validate</param>
    /// <param name="targetAudience">Target audience level</param>
    /// <returns>Validation results and suggestions</returns>
    Task<SlideValidationResult> ValidateSlideAsync(Slide slide, DifficultyLevel targetAudience);
}

/// <summary>
/// Image generation styles
/// </summary>
public enum ImageStyle
{
    Cartoon,
    Technical,
    Minimalist,
    Detailed,
    Humorous,
    Professional
}

/// <summary>
/// Text formatting styles
/// </summary>
public enum TextStyle
{
    Educational,
    Conversational,
    Technical,
    Playful,
    Formal
}

/// <summary>
/// Quiz presentation styles
/// </summary>
public enum QuizStyle
{
    Traditional,
    Gamified,
    Interactive,
    Timed,
    Progressive
}

/// <summary>
/// 3D model types for Three.js slides
/// </summary>
public enum ModelType
{
    NetworkTopology,
    OSILayers,
    DataFlow,
    DeviceStructure,
    ProtocolStack,
    SecurityModel
}

/// <summary>
/// Network diagram types
/// </summary>
public enum NetworkDiagramType
{
    Star,
    Mesh,
    Bus,
    Ring,
    Hybrid,
    Tree,
    OSIModel,
    PacketFlow
}

/// <summary>
/// Quiz slide data structure
/// </summary>
public class QuizSlideData
{
    public string QuestionId { get; set; } = string.Empty;
    public string Html { get; set; } = string.Empty;
    public Dictionary<string, object> InteractionData { get; set; } = new();
    public List<string> AnswerOptions { get; set; } = new();
    public int CorrectAnswerIndex { get; set; }
    public string FeedbackHtml { get; set; } = string.Empty;
}

/// <summary>
/// Three.js slide data structure
/// </summary>
public class ThreeJsSlideData
{
    public string SceneId { get; set; } = string.Empty;
    public string SceneJson { get; set; } = string.Empty;
    public Dictionary<string, object> Controls { get; set; } = new();
    public List<string> Scripts { get; set; } = new();
    public string HtmlWrapper { get; set; } = string.Empty;
}

/// <summary>
/// Network node for diagrams
/// </summary>
public class NetworkNode
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public NodeType Type { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public string Color { get; set; } = "#007ACC";
    public Dictionary<string, object> Properties { get; set; } = new();
}

/// <summary>
/// Network connection for diagrams
/// </summary>
public class NetworkConnection
{
    public string FromNodeId { get; set; } = string.Empty;
    public string ToNodeId { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public ConnectionType Type { get; set; }
    public string Color { get; set; } = "#666666";
    public float Thickness { get; set; } = 1.0f;
}

/// <summary>
/// Diagram annotation
/// </summary>
public class DiagramAnnotation
{
    public string Id { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public float X { get; set; }
    public float Y { get; set; }
    public string Color { get; set; } = "#333333";
    public AnnotationType Type { get; set; }
}

/// <summary>
/// Animation step data
/// </summary>
public class AnimationStep
{
    public int StepNumber { get; set; }
    public string Description { get; set; } = string.Empty;
    public int DurationMs { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new();
}

/// <summary>
/// Animation slide data
/// </summary>
public class AnimationSlideData
{
    public string AnimationId { get; set; } = string.Empty;
    public List<AnimationStep> Steps { get; set; } = new();
    public string HtmlContent { get; set; } = string.Empty;
    public string CssStyles { get; set; } = string.Empty;
    public string JavaScript { get; set; } = string.Empty;
}

/// <summary>
/// Slide validation result
/// </summary>
public class SlideValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Issues { get; set; } = new();
    public List<string> Suggestions { get; set; } = new();
    public int ReadabilityScore { get; set; }
    public DifficultyLevel EstimatedDifficulty { get; set; }
    public List<string> RequiredImprovements { get; set; } = new();
}

/// <summary>
/// Node types for network diagrams
/// </summary>
public enum NodeType
{
    Computer,
    Router,
    Switch,
    Hub,
    Server,
    Firewall,
    AccessPoint,
    Modem,
    Cloud,
    Internet
}

/// <summary>
/// Connection types for network diagrams
/// </summary>
public enum ConnectionType
{
    Ethernet,
    Wireless,
    Fiber,
    Serial,
    Virtual,
    Logical
}

/// <summary>
/// Annotation types
/// </summary>
public enum AnnotationType
{
    Label,
    Callout,
    Warning,
    Tip,
    Description
}