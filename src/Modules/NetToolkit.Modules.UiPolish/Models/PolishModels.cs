using System.ComponentModel;
using System.Text.Json.Serialization;

namespace NetToolkit.Modules.UiPolish.Models;

/// <summary>
/// Theme configuration for comprehensive UI styling
/// </summary>
public class ThemeConfig
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ColorPalette Colors { get; set; } = new();
    public Typography Typography { get; set; } = new();
    public EffectSettings Effects { get; set; } = new();
    public AnimationSettings Animations { get; set; } = new();
    public MaterialSettings Materials { get; set; } = new();
    public Dictionary<string, object> CustomProperties { get; set; } = new();
}

/// <summary>
/// Scandinavian-inspired color palette
/// </summary>
public class ColorPalette
{
    // Primary Scandinavian colors - midnight blue as base
    public string Primary { get; set; } = "#1a1a2e";           // Deep midnight blue
    public string PrimaryLight { get; set; } = "#16213e";      // Lighter midnight
    public string PrimaryDark { get; set; } = "#0d0d1a";       // Darker midnight
    
    // Secondary colors - silver and neon accents
    public string Secondary { get; set; } = "#c4c4c4";         // Pure silver
    public string SecondaryLight { get; set; } = "#e8e8e8";    // Light silver
    public string SecondaryDark { get; set; } = "#8c8c8c";     // Dark silver
    
    // Accent colors - cyberpunk neon
    public string Accent { get; set; } = "#00d4ff";            // Neon cyan
    public string AccentWarm { get; set; } = "#ff6b6b";        // Warm neon coral
    public string AccentCool { get; set; } = "#4ecdc4";        // Cool neon teal
    
    // Surface colors - for floating panels
    public string Surface { get; set; } = "#ffffff10";         // Semi-transparent white
    public string SurfaceVariant { get; set; } = "#ffffff18";  // Slightly more opaque
    public string Background { get; set; } = "#0a0a0f";       // Very dark blue-black
    
    // Text colors
    public string OnPrimary { get; set; } = "#ffffff";         // White text on primary
    public string OnSecondary { get; set; } = "#1a1a2e";       // Dark text on secondary  
    public string OnSurface { get; set; } = "#e0e0e0";         // Light text on surface
    public string OnBackground { get; set; } = "#ffffff";      // White text on background
    
    // Status colors
    public string Success { get; set; } = "#4caf50";           // Green
    public string Warning { get; set; } = "#ff9800";           // Orange
    public string Error { get; set; } = "#f44336";             // Red
    public string Info { get; set; } = "#2196f3";              // Blue
    
    // Holographic colors
    public string HologramPrimary { get; set; } = "#00ffff88"; // Cyan hologram
    public string HologramSecondary { get; set; } = "#ff00ff44"; // Magenta hologram
    public string GlowColor { get; set; } = "#00d4ff";         // Neon cyan glow
}

/// <summary>
/// Typography settings for elegant text rendering
/// </summary>
public class Typography
{
    public string PrimaryFont { get; set; } = "Segoe UI Variable";
    public string SecondaryFont { get; set; } = "Consolas";
    public string MonospaceFont { get; set; } = "Fira Code";
    
    public double BaseFontSize { get; set; } = 14.0;
    public double LineHeight { get; set; } = 1.4;
    public double LetterSpacing { get; set; } = 0.5;
    
    // Font weights
    public int LightWeight { get; set; } = 300;
    public int RegularWeight { get; set; } = 400;
    public int MediumWeight { get; set; } = 500;
    public int BoldWeight { get; set; } = 600;
    
    // Font sizes
    public double HeadingLarge { get; set; } = 32.0;
    public double HeadingMedium { get; set; } = 24.0;
    public double HeadingSmall { get; set; } = 18.0;
    public double BodyLarge { get; set; } = 16.0;
    public double BodyMedium { get; set; } = 14.0;
    public double BodySmall { get; set; } = 12.0;
    public double Caption { get; set; } = 10.0;
}

/// <summary>
/// Visual effect settings
/// </summary>
public class EffectSettings
{
    public bool EnableShadows { get; set; } = true;
    public bool EnableGlow { get; set; } = true;
    public bool EnableParticles { get; set; } = true;
    public bool EnableBlur { get; set; } = true;
    public bool EnableHolographics { get; set; } = true;
    
    public double ShadowOpacity { get; set; } = 0.3;
    public double GlowIntensity { get; set; } = 0.8;
    public int ParticleDensity { get; set; } = 50;
    public double BlurRadius { get; set; } = 8.0;
    public double HologramOpacity { get; set; } = 0.7;
    
    public string ShadowColor { get; set; } = "#000000";
    public string GlowColor { get; set; } = "#00d4ff";
    public string ParticleColor { get; set; } = "#ffffff";
}

/// <summary>
/// Animation configuration settings
/// </summary>
public class AnimationSettings
{
    public bool EnableAnimations { get; set; } = true;
    public bool EnableTransitions { get; set; } = true;
    public bool EnableHoverEffects { get; set; } = true;
    public bool EnableParallax { get; set; } = true;
    
    public int DefaultDuration { get; set; } = 300;
    public int FastDuration { get; set; } = 150;
    public int SlowDuration { get; set; } = 600;
    
    public EasingType DefaultEasing { get; set; } = EasingType.EaseOutQuart;
    public double AnimationScale { get; set; } = 1.0; // Global animation speed multiplier
}

/// <summary>
/// Material appearance settings
/// </summary>
public class MaterialSettings
{
    public bool EnableMetallicEffects { get; set; } = true;
    public bool EnableGlassEffects { get; set; } = true;
    public bool EnableReflections { get; set; } = true;
    
    public double Roughness { get; set; } = 0.1;
    public double Metallic { get; set; } = 0.8;
    public double Transparency { get; set; } = 0.15;
    public double RefractiveIndex { get; set; } = 1.5;
    
    public string MetallicColor { get; set; } = "#c4c4c4"; // Silver
    public string GlassColor { get; set; } = "#ffffff10";  // Semi-transparent white
}

/// <summary>
/// Component type enumeration
/// </summary>
public enum ComponentType
{
    Terminal,
    Topography,
    Dashboard,
    Scanner,
    Education,
    AiOrb,
    MicrosoftAdmin,
    Security,
    Network,
    SSH
}

/// <summary>
/// Three.js enhancement options
/// </summary>
public class ThreeJsOptions
{
    public bool EnableShaders { get; set; } = true;
    public bool EnableLighting { get; set; } = true;
    public bool EnablePostProcessing { get; set; } = true;
    public bool EnableAnimations { get; set; } = true;
    
    public Dictionary<string, object> ShaderUniforms { get; set; } = new();
    public List<string> RequiredLibraries { get; set; } = new();
    public PerformanceLevel PerformanceLevel { get; set; } = PerformanceLevel.High;
    
    public string VertexShader { get; set; } = string.Empty;
    public string FragmentShader { get; set; } = string.Empty;
    public Dictionary<string, object> CustomOptions { get; set; } = new();
}

/// <summary>
/// Floating panel style configuration
/// </summary>
public class FloatingPanelStyle
{
    public double BorderRadius { get; set; } = 12.0;
    public double Opacity { get; set; } = 0.95;
    public double BlurRadius { get; set; } = 20.0;
    public string BackgroundColor { get; set; } = "#ffffff10";
    public string BorderColor { get; set; } = "#ffffff20";
    public double BorderThickness { get; set; } = 1.0;
    
    public ShadowConfig Shadow { get; set; } = new();
    public bool EnableBackdropBlur { get; set; } = true;
    public bool EnableHoverEffects { get; set; } = true;
    public FloatingAnimation FloatingEffect { get; set; } = FloatingAnimation.Gentle;
}

/// <summary>
/// Shadow configuration
/// </summary>
public class ShadowConfig
{
    public double OffsetX { get; set; } = 0.0;
    public double OffsetY { get; set; } = 8.0;
    public double BlurRadius { get; set; } = 32.0;
    public double SpreadRadius { get; set; } = 0.0;
    public string Color { get; set; } = "#00000040";
    public bool Inset { get; set; } = false;
}

/// <summary>
/// Metallic effect types
/// </summary>
public enum MetallicType
{
    Chrome,
    Gold,
    Silver,
    Copper,
    Titanium,
    Holographic,
    Iridescent
}

/// <summary>
/// Particle system configuration
/// </summary>
public class ParticleSystemConfig
{
    public string ContainerId { get; set; } = string.Empty;
    public int ParticleCount { get; set; } = 100;
    public ParticleType Type { get; set; } = ParticleType.Floating;
    
    public ColorRange ColorRange { get; set; } = new();
    public SizeRange SizeRange { get; set; } = new();
    public SpeedRange SpeedRange { get; set; } = new();
    
    public bool EnableInteractivity { get; set; } = true;
    public bool EnableConnections { get; set; } = false;
    public double ConnectionDistance { get; set; } = 150.0;
    
    public Dictionary<string, object> CustomProperties { get; set; } = new();
}

/// <summary>
/// Color range for gradients and particles
/// </summary>
public class ColorRange
{
    public string StartColor { get; set; } = "#00d4ff";
    public string EndColor { get; set; } = "#ff6b6b";
    public List<string> IntermediateColors { get; set; } = new();
}

/// <summary>
/// Size range configuration
/// </summary>
public class SizeRange
{
    public double MinSize { get; set; } = 1.0;
    public double MaxSize { get; set; } = 5.0;
}

/// <summary>
/// Speed range configuration  
/// </summary>
public class SpeedRange
{
    public double MinSpeed { get; set; } = 0.1;
    public double MaxSpeed { get; set; } = 2.0;
}

/// <summary>
/// Current polish state information
/// </summary>
public class PolishState
{
    public string ActiveTheme { get; set; } = string.Empty;
    public List<string> AppliedEnhancements { get; set; } = new();
    public Dictionary<string, string> ActiveTips { get; set; } = new();
    public List<string> RunningAnimations { get; set; } = new();
    public PerformanceMetrics Performance { get; set; } = new();
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Performance metrics for polish operations
/// </summary>
public class PerformanceMetrics
{
    public double FrameRate { get; set; }
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public int ActiveAnimations { get; set; }
    public int LoadedTextures { get; set; }
    public double RenderTime { get; set; }
    public Dictionary<string, double> ComponentMetrics { get; set; } = new();
}

/// <summary>
/// Animation type enumeration
/// </summary>
public enum AnimationType
{
    FadeIn,
    FadeOut,
    SlideIn,
    SlideOut,
    ScaleUp,
    ScaleDown,
    RotateIn,
    RotateOut,
    Bounce,
    Pulse,
    Shake,
    Glow,
    Float,
    Wobble
}

/// <summary>
/// Easing function types
/// </summary>
public enum EasingType
{
    Linear,
    EaseIn,
    EaseOut,
    EaseInOut,
    EaseInQuad,
    EaseOutQuad,
    EaseInOutQuad,
    EaseInCubic,
    EaseOutCubic,
    EaseInOutCubic,
    EaseInQuart,
    EaseOutQuart,
    EaseInOutQuart,
    EaseInElastic,
    EaseOutElastic,
    EaseInOutElastic,
    EaseInBack,
    EaseOutBack,
    EaseInOutBack,
    EaseInBounce,
    EaseOutBounce,
    EaseInOutBounce
}

/// <summary>
/// Animation sequence definition
/// </summary>
public class AnimationSequence
{
    public string SequenceId { get; set; } = string.Empty;
    public List<AnimationStep> Steps { get; set; } = new();
    public bool Loop { get; set; } = false;
    public int LoopCount { get; set; } = -1; // -1 for infinite
    public double DelayBetweenLoops { get; set; } = 0.0;
}

/// <summary>
/// Individual animation step
/// </summary>
public class AnimationStep
{
    public string ElementId { get; set; } = string.Empty;
    public AnimationType Type { get; set; }
    public int Duration { get; set; } = 300;
    public int Delay { get; set; } = 0;
    public EasingType Easing { get; set; } = EasingType.EaseOutQuart;
    public Dictionary<string, object> Properties { get; set; } = new();
}

/// <summary>
/// Hover effect types
/// </summary>
public enum HoverEffectType
{
    Glow,
    Scale,
    Rotate,
    Pulse,
    Color,
    Shadow,
    Float,
    Shake,
    Bounce,
    Highlight
}

/// <summary>
/// Shader code container
/// </summary>
public class ShaderCode
{
    public string VertexShader { get; set; } = string.Empty;
    public string FragmentShader { get; set; } = string.Empty;
    public Dictionary<string, object> Uniforms { get; set; } = new();
    public List<string> Dependencies { get; set; } = new();
}

/// <summary>
/// Shader types
/// </summary>
public enum ShaderType
{
    Holographic,
    Metallic,
    Glass,
    Neon,
    Particle,
    Distortion,
    Glow,
    Chromatic,
    Noise,
    Gradient
}

/// <summary>
/// Holographic effect configuration
/// </summary>
public class HolographicConfig
{
    public double Intensity { get; set; } = 0.8;
    public double ChromaticAberration { get; set; } = 0.1;
    public double ScanlineFrequency { get; set; } = 0.005;
    public double FlickerRate { get; set; } = 0.1;
    public string BaseColor { get; set; } = "#00ffff";
    public bool EnableDistortion { get; set; } = true;
    public bool EnableNoise { get; set; } = true;
}

/// <summary>
/// Glow effect configuration
/// </summary>
public class GlowConfig
{
    public string Color { get; set; } = "#00d4ff";
    public double Intensity { get; set; } = 1.0;
    public double BlurRadius { get; set; } = 20.0;
    public double SpreadRadius { get; set; } = 5.0;
    public bool Animated { get; set; } = true;
    public double AnimationSpeed { get; set; } = 1.0;
}

/// <summary>
/// Glow effect result
/// </summary>
public class GlowEffect
{
    public string CssCode { get; set; } = string.Empty;
    public string JavaScriptCode { get; set; } = string.Empty;
    public List<string> RequiredClasses { get; set; } = new();
}

/// <summary>
/// Shader validation result
/// </summary>
public class ShaderValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public string ValidatedSource { get; set; } = string.Empty;
}

/// <summary>
/// Theme definition
/// </summary>
public class ThemeDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0.0";
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public ThemeConfig Config { get; set; } = new();
    public string PreviewImage { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
}

/// <summary>
/// Performance level enumeration
/// </summary>
public enum PerformanceLevel
{
    Low,
    Medium,
    High,
    Ultra
}

/// <summary>
/// Floating animation types
/// </summary>
public enum FloatingAnimation
{
    None,
    Gentle,
    Moderate,
    Pronounced
}

/// <summary>
/// Particle types
/// </summary>
public enum ParticleType
{
    Floating,
    Falling,
    Rising,
    Orbiting,
    Connecting,
    Exploding,
    Swirling
}

/// <summary>
/// Polish performance metrics
/// </summary>
public class PolishPerformanceMetrics
{
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public double FrameRate { get; set; }
    public double CpuUsage { get; set; }
    public double GpuUsage { get; set; }
    public long MemoryUsage { get; set; }
    public int ActiveAnimations { get; set; }
    public int LoadedShaders { get; set; }
    public int ParticleCount { get; set; }
    public Dictionary<string, object> CustomMetrics { get; set; } = new();
}

/// <summary>
/// Three.js scene configuration
/// </summary>
public class ThreeJsSceneConfig
{
    public string SceneId { get; set; } = string.Empty;
    public int MaxObjects { get; set; } = 1000;
    public int MaxLights { get; set; } = 8;
    public int MaxTextures { get; set; } = 50;
    public PerformanceLevel QualityLevel { get; set; } = PerformanceLevel.High;
    public bool EnableShadows { get; set; } = true;
    public bool EnablePostProcessing { get; set; } = true;
    public Dictionary<string, object> SceneProperties { get; set; } = new();
}

/// <summary>
/// Scene optimization result
/// </summary>
public class SceneOptimizationResult
{
    public ThreeJsSceneConfig OptimizedConfig { get; set; } = new();
    public List<string> AppliedOptimizations { get; set; } = new();
    public double EstimatedFrameRate { get; set; }
    public double EstimatedMemoryUsage { get; set; }
    public bool RequiresFallback { get; set; }
}

/// <summary>
/// Performance status
/// </summary>
public class PerformanceStatus
{
    public double CurrentFps { get; set; }
    public double AverageFps { get; set; }
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public PerformanceLevel RecommendedLevel { get; set; }
    public List<string> PerformanceIssues { get; set; } = new();
    public Dictionary<string, object> DetailedMetrics { get; set; } = new();
}

/// <summary>
/// System specifications
/// </summary>
public class SystemSpecifications
{
    public string CpuModel { get; set; } = string.Empty;
    public int CpuCores { get; set; }
    public double CpuFrequency { get; set; }
    public long TotalMemory { get; set; }
    public long AvailableMemory { get; set; }
    public string GpuModel { get; set; } = string.Empty;
    public long GpuMemory { get; set; }
    public string OperatingSystem { get; set; } = string.Empty;
    public Dictionary<string, object> AdditionalSpecs { get; set; } = new();
}

/// <summary>
/// Polish configuration recommendation
/// </summary>
public class PolishConfig
{
    public PerformanceLevel RecommendedLevel { get; set; }
    public EffectSettings Effects { get; set; } = new();
    public AnimationSettings Animations { get; set; } = new();
    public List<string> DisabledFeatures { get; set; } = new();
    public Dictionary<string, object> CustomSettings { get; set; } = new();
}

/// <summary>
/// Asset manifest for lazy loading
/// </summary>
public class AssetManifest
{
    public List<AssetDefinition> Assets { get; set; } = new();
    public LoadingStrategy Strategy { get; set; } = LoadingStrategy.Progressive;
    public int ConcurrentLoads { get; set; } = 3;
    public Dictionary<string, object> LoadingOptions { get; set; } = new();
}

/// <summary>
/// Asset definition
/// </summary>
public class AssetDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public long Size { get; set; }
    public int Priority { get; set; } = 0;
    public List<string> Dependencies { get; set; } = new();
    public bool Required { get; set; } = true;
}

/// <summary>
/// Loading strategy enumeration
/// </summary>
public enum LoadingStrategy
{
    Immediate,
    Progressive,
    OnDemand,
    Background
}

/// <summary>
/// Asset loading progress
/// </summary>
public class AssetLoadingProgress
{
    public int TotalAssets { get; set; }
    public int LoadedAssets { get; set; }
    public int FailedAssets { get; set; }
    public double PercentComplete => TotalAssets > 0 ? (double)LoadedAssets / TotalAssets * 100 : 0;
    public List<string> CurrentlyLoading { get; set; } = new();
    public List<string> LoadingErrors { get; set; } = new();
    public Dictionary<string, object> LoadingStats { get; set; } = new();
}

/// <summary>
/// Integration result for external libraries
/// </summary>
public class IntegrationResult
{
    public bool Success { get; set; }
    public string JavaScriptCode { get; set; } = string.Empty;
    public string CssCode { get; set; } = string.Empty;
    public List<string> RequiredLibraries { get; set; } = new();
    public List<string> Errors { get; set; } = new();
    public Dictionary<string, object> IntegrationData { get; set; } = new();
}

/// <summary>
/// Globe visualization configuration
/// </summary>
public class GlobeConfig
{
    public string ContainerId { get; set; } = string.Empty;
    public double GlobeRadius { get; set; } = 100.0;
    public string GlobeTexture { get; set; } = string.Empty;
    public bool ShowAtmosphere { get; set; } = true;
    public bool ShowGraticules { get; set; } = false;
    public bool EnableInteraction { get; set; } = true;
    public List<GlobePoint> Points { get; set; } = new();
    public List<GlobeArc> Arcs { get; set; } = new();
    public Dictionary<string, object> CustomOptions { get; set; } = new();
}

/// <summary>
/// Globe point definition
/// </summary>
public class GlobePoint
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Color { get; set; } = "#00d4ff";
    public double Size { get; set; } = 1.0;
    public Dictionary<string, object> Data { get; set; } = new();
}

/// <summary>
/// Globe arc definition
/// </summary>
public class GlobeArc
{
    public GlobePoint StartPoint { get; set; } = new();
    public GlobePoint EndPoint { get; set; } = new();
    public string Color { get; set; } = "#ff6b6b";
    public double Width { get; set; } = 1.0;
    public bool Animated { get; set; } = true;
}

/// <summary>
/// Terminal configuration for xterm.js integration
/// </summary>
public class TerminalConfig
{
    public string ContainerId { get; set; } = string.Empty;
    public int Rows { get; set; } = 24;
    public int Cols { get; set; } = 80;
    public string FontFamily { get; set; } = "Fira Code";
    public int FontSize { get; set; } = 14;
    public double LineHeight { get; set; } = 1.2;
    public string Theme { get; set; } = "cyberpunk";
    public bool EnableAddons { get; set; } = true;
    public List<string> Addons { get; set; } = new() { "fit", "webgl", "unicode11" };
    public Dictionary<string, object> CustomOptions { get; set; } = new();
}

/// <summary>
/// Particle configuration for particle.js
/// </summary>
public class ParticleConfig
{
    public string ContainerId { get; set; } = string.Empty;
    public int Number { get; set; } = 80;
    public string Shape { get; set; } = "circle";
    public double Size { get; set; } = 3.0;
    public double Speed { get; set; } = 6.0;
    public string Color { get; set; } = "#ffffff";
    public bool EnableConnections { get; set; } = true;
    public double ConnectionDistance { get; set; } = 150.0;
    public Dictionary<string, object> CustomConfig { get; set; } = new();
}

/// <summary>
/// Code validation result
/// </summary>
public class CodeValidationResult
{
    public bool IsValid { get; set; }
    public bool IsSafe { get; set; }
    public List<string> SecurityIssues { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public string SanitizedCode { get; set; } = string.Empty;
    public double RiskScore { get; set; } = 0.0;
}

/// <summary>
/// Library loading result
/// </summary>
public class LibraryLoadResult
{
    public bool Success { get; set; }
    public string LoadedVersion { get; set; } = string.Empty;
    public string LoadedFrom { get; set; } = string.Empty;
    public bool FallbackUsed { get; set; }
    public List<string> Errors { get; set; } = new();
    public Dictionary<string, object> LibraryInfo { get; set; } = new();
}

/// <summary>
/// Witty tip categories for contextual humor
/// </summary>
public static class WittyTips
{
    public static readonly Dictionary<string, List<string>> TipCategories = new()
    {
        ["General"] = new List<string>
        {
            "Hover for network nirvana!",
            "Click for cyber enlightenment!",
            "Interface magic at your fingertips!",
            "UI polish perfection activated!",
            "Sleekness supreme - hover engaged!",
            "Digital elegance in motion!",
            "Scandinavian simplicity meets cyber sass!"
        },
        
        ["Terminal"] = new List<string>
        {
            "Command line comedy - type away!",
            "Shell shocked by the beauty?",
            "Terminal velocity: Maximum sleekness!",
            "Bash into brilliance!",
            "SSH-ing into style!",
            "PowerShell powered by polish!",
            "Console couture at its finest!"
        },
        
        ["Network"] = new List<string>
        {
            "Network topology with attitude!",
            "Packets flying in formation!",
            "Bandwidth beauty unleashed!",
            "Connection perfection detected!",
            "Ping pong with panache!",
            "Router romance in progress!",
            "Switch to superior styling!"
        },
        
        ["Security"] = new List<string>
        {
            "Vulnerability scanning with swagger!",
            "Security meets sensational style!",
            "Firewall fashion statement!",
            "Encryption elegance enabled!",
            "Cyber defense with dazzle!",
            "Threat detection theatrics!",
            "Malware meets its match!"
        },
        
        ["Education"] = new List<string>
        {
            "Learning with luminous luxury!",
            "Education elevation activated!",
            "Knowledge nuggets with neon!",
            "Tutorial transcendence!",
            "Skill acquisition supreme!",
            "Mastery meets magnificence!",
            "Genius-grade guidance!"
        },
        
        ["Microsoft"] = new List<string>
        {
            "Azure administration with attitude!",
            "Office 365 optimization outstanding!",
            "Graph API glamour!",
            "PowerShell perfection!",
            "Microsoft mastery mode!",
            "Cloud control with class!",
            "Admin authority activated!"
        }
    };
}

/// <summary>
/// Built-in theme definitions
/// </summary>
public static class BuiltInThemes
{
    public static readonly Dictionary<string, ThemeDefinition> Themes = new()
    {
        ["scandinavian-cyber"] = new ThemeDefinition
        {
            Id = "scandinavian-cyber",
            Name = "Scandinavian Cyber",
            Description = "Clean Nordic aesthetics meet cyberpunk neon - the perfect fusion of minimalism and digital opulence",
            Author = "NetToolkit Team",
            Config = new ThemeConfig
            {
                Name = "Scandinavian Cyber",
                Description = "Premium theme combining Scandinavian design principles with cyberpunk aesthetics"
            }
        },
        
        ["midnight-hologram"] = new ThemeDefinition
        {
            Id = "midnight-hologram", 
            Name = "Midnight Hologram",
            Description = "Deep space darkness illuminated by holographic projections and ethereal glows",
            Author = "NetToolkit Team"
        },
        
        ["silver-chrome"] = new ThemeDefinition
        {
            Id = "silver-chrome",
            Name = "Silver Chrome",
            Description = "Metallic perfection with mirror-like surfaces and industrial elegance",
            Author = "NetToolkit Team"
        },
        
        ["neon-nights"] = new ThemeDefinition
        {
            Id = "neon-nights",
            Name = "Neon Nights", 
            Description = "Vibrant neon colors dancing through the digital darkness",
            Author = "NetToolkit Team"
        }
    };
}