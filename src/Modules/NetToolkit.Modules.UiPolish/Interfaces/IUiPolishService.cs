using NetToolkit.Modules.UiPolish.Models;

namespace NetToolkit.Modules.UiPolish.Interfaces;

/// <summary>
/// UI Polish service - the aesthetic alchemist of visual magnificence
/// Transforms mundane interfaces into ethereal holographic experiences
/// </summary>
public interface IUiPolishService
{
    /// <summary>
    /// Apply comprehensive theme configuration to all UI components
    /// </summary>
    /// <param name="config">Theme configuration with colors, materials, and aesthetic preferences</param>
    /// <returns>Async operation completing when theme transformation is achieved</returns>
    Task ApplyThemeAsync(ThemeConfig config);
    
    /// <summary>
    /// Enhance component with Three.js magical enhancements
    /// </summary>
    /// <param name="componentType">Type of component to enhance (Terminal, Topography, Dashboard)</param>
    /// <param name="options">Enhancement options including shaders, particles, and animations</param>
    /// <returns>Generated JavaScript snippet for Three.js integration</returns>
    Task<string> EnhanceThreeJsAsync(ComponentType componentType, ThreeJsOptions options);
    
    /// <summary>
    /// Add witty hover tips throughout the interface for enhanced user delight
    /// </summary>
    /// <param name="tips">Dictionary mapping UI element identifiers to witty tip messages</param>
    /// <returns>Async operation completing when tips are applied</returns>
    Task AddHoverTipsAsync(Dictionary<string, string> tips);
    
    /// <summary>
    /// Apply floating panel aesthetics to specified UI containers
    /// </summary>
    /// <param name="containerIds">Identifiers of containers to transform into floating panels</param>
    /// <param name="style">Floating panel style configuration</param>
    /// <returns>Async operation completing when panels achieve floating magnificence</returns>
    Task ApplyFloatingPanelsAsync(IEnumerable<string> containerIds, FloatingPanelStyle style);
    
    /// <summary>
    /// Generate metallic shader effects for UI elements
    /// </summary>
    /// <param name="elements">Elements to receive metallic enhancement</param>
    /// <param name="metallicType">Type of metallic effect (Chrome, Gold, Holographic)</param>
    /// <returns>CSS/XAML styling for metallic effects</returns>
    Task<string> GenerateMetallicEffectsAsync(IEnumerable<string> elements, MetallicType metallicType);
    
    /// <summary>
    /// Apply particle system enhancements to interactive elements
    /// </summary>
    /// <param name="particleConfig">Configuration for particle effects</param>
    /// <returns>JavaScript code for particle system integration</returns>
    Task<string> CreateParticleSystemAsync(ParticleSystemConfig particleConfig);
    
    /// <summary>
    /// Get current theme state and applied enhancements
    /// </summary>
    /// <returns>Current polish state information</returns>
    Task<PolishState> GetPolishStateAsync();
    
    /// <summary>
    /// Reset all UI enhancements to default state
    /// </summary>
    /// <returns>Async operation completing when polish is reset</returns>
    Task ResetPolishAsync();
}

/// <summary>
/// Animation engine for fluid UI transitions and dynamic effects
/// </summary>
public interface IAnimationEngine
{
    /// <summary>
    /// Animate UI element with specified transition type
    /// </summary>
    /// <param name="elementId">Identifier of element to animate</param>
    /// <param name="animationType">Type of animation to apply</param>
    /// <param name="duration">Animation duration in milliseconds</param>
    /// <param name="easing">Easing function for smooth transitions</param>
    /// <returns>Async operation completing when animation finishes</returns>
    Task AnimateTransitionAsync(string elementId, AnimationType animationType, int duration = 300, EasingType easing = EasingType.EaseOutQuart);
    
    /// <summary>
    /// Create complex animation sequence with multiple steps
    /// </summary>
    /// <param name="sequence">Animation sequence definition</param>
    /// <returns>Async operation completing when sequence finishes</returns>
    Task ExecuteAnimationSequenceAsync(AnimationSequence sequence);
    
    /// <summary>
    /// Apply hover animations to interactive elements
    /// </summary>
    /// <param name="elementId">Element to enhance with hover effects</param>
    /// <param name="hoverEffect">Type of hover effect</param>
    /// <returns>Async operation completing when hover effects are applied</returns>
    Task ApplyHoverAnimationAsync(string elementId, HoverEffectType hoverEffect);
    
    /// <summary>
    /// Create breathing animation for ambient UI elements
    /// </summary>
    /// <param name="elementIds">Elements to apply breathing effect</param>
    /// <param name="breathingRate">Breathing cycles per minute</param>
    /// <returns>Async operation setting up breathing animation</returns>
    Task CreateBreathingAnimationAsync(IEnumerable<string> elementIds, double breathingRate = 12.0);
    
    /// <summary>
    /// Stop all animations for specified element
    /// </summary>
    /// <param name="elementId">Element to stop animations for</param>
    /// <returns>Async operation completing when animations are stopped</returns>
    Task StopAnimationsAsync(string elementId);
}

/// <summary>
/// Shader utility for advanced visual effects
/// </summary>
public interface IShaderUtility
{
    /// <summary>
    /// Generate GLSL shader code for Three.js materials
    /// </summary>
    /// <param name="shaderType">Type of shader to generate</param>
    /// <param name="parameters">Shader parameters and uniforms</param>
    /// <returns>Generated GLSL shader source code</returns>
    Task<ShaderCode> GenerateGlslShaderAsync(ShaderType shaderType, Dictionary<string, object> parameters);
    
    /// <summary>
    /// Create holographic material effect
    /// </summary>
    /// <param name="hologramConfig">Holographic effect configuration</param>
    /// <returns>Three.js material definition with holographic properties</returns>
    Task<string> CreateHolographicMaterialAsync(HolographicConfig hologramConfig);
    
    /// <summary>
    /// Generate cyberpunk-style glow effects
    /// </summary>
    /// <param name="glowConfig">Glow effect configuration</param>
    /// <returns>CSS and JavaScript for glow implementation</returns>
    Task<GlowEffect> CreateCyberpunkGlowAsync(GlowConfig glowConfig);
    
    /// <summary>
    /// Compile and validate shader code
    /// </summary>
    /// <param name="shaderSource">GLSL shader source to validate</param>
    /// <returns>Validation result with any errors or warnings</returns>
    Task<ShaderValidationResult> ValidateShaderAsync(string shaderSource);
}

/// <summary>
/// Polish event publisher for UI enhancement notifications
/// </summary>
public interface IPolishEventPublisher
{
    /// <summary>
    /// Publish theme application event
    /// </summary>
    /// <param name="themeName">Name of applied theme</param>
    /// <param name="appliedAt">Time when theme was applied</param>
    /// <returns>Async operation completing when event is published</returns>
    Task PublishThemeAppliedAsync(string themeName, DateTime appliedAt);
    
    /// <summary>
    /// Publish enhancement loading event
    /// </summary>
    /// <param name="componentType">Type of component enhanced</param>
    /// <param name="enhancementType">Type of enhancement applied</param>
    /// <param name="success">Whether enhancement was successful</param>
    /// <returns>Async operation completing when event is published</returns>
    Task PublishEnhancementLoadedAsync(ComponentType componentType, string enhancementType, bool success);
    
    /// <summary>
    /// Publish hover tip addition event
    /// </summary>
    /// <param name="elementId">Element that received tip</param>
    /// <param name="tipText">Text of the added tip</param>
    /// <returns>Async operation completing when event is published</returns>
    Task PublishTipAddedAsync(string elementId, string tipText);
    
    /// <summary>
    /// Publish animation completion event
    /// </summary>
    /// <param name="elementId">Element that was animated</param>
    /// <param name="animationType">Type of animation completed</param>
    /// <param name="duration">Duration of completed animation</param>
    /// <returns>Async operation completing when event is published</returns>
    Task PublishAnimationCompletedAsync(string elementId, AnimationType animationType, TimeSpan duration);
    
    /// <summary>
    /// Publish performance metrics for UI polish operations
    /// </summary>
    /// <param name="metrics">Performance metrics data</param>
    /// <returns>Async operation completing when metrics are published</returns>
    Task PublishPerformanceMetricsAsync(PolishPerformanceMetrics metrics);
}

/// <summary>
/// Theme manager for global aesthetic coordination
/// </summary>
public interface IThemeManager
{
    /// <summary>
    /// Get available theme configurations
    /// </summary>
    /// <returns>Collection of available themes</returns>
    Task<IEnumerable<ThemeDefinition>> GetAvailableThemesAsync();
    
    /// <summary>
    /// Apply theme globally across all components
    /// </summary>
    /// <param name="themeId">Identifier of theme to apply</param>
    /// <returns>Async operation completing when theme is fully applied</returns>
    Task ApplyGlobalThemeAsync(string themeId);
    
    /// <summary>
    /// Create custom theme from configuration
    /// </summary>
    /// <param name="themeConfig">Custom theme configuration</param>
    /// <returns>Created theme definition</returns>
    Task<ThemeDefinition> CreateCustomThemeAsync(ThemeConfig themeConfig);
    
    /// <summary>
    /// Get current active theme
    /// </summary>
    /// <returns>Currently active theme definition</returns>
    Task<ThemeDefinition> GetActiveThemeAsync();
    
    /// <summary>
    /// Export theme configuration for sharing
    /// </summary>
    /// <param name="themeId">Theme to export</param>
    /// <returns>Serialized theme configuration</returns>
    Task<string> ExportThemeAsync(string themeId);
    
    /// <summary>
    /// Import theme from configuration
    /// </summary>
    /// <param name="themeData">Serialized theme data</param>
    /// <returns>Imported theme definition</returns>
    Task<ThemeDefinition> ImportThemeAsync(string themeData);
}

/// <summary>
/// Resource optimizer for performance-conscious polish
/// </summary>
public interface IPolishOptimizer
{
    /// <summary>
    /// Optimize Three.js scene for target framerate
    /// </summary>
    /// <param name="sceneConfig">Scene configuration to optimize</param>
    /// <param name="targetFps">Target frames per second</param>
    /// <returns>Optimized scene configuration</returns>
    Task<SceneOptimizationResult> OptimizeSceneAsync(ThreeJsSceneConfig sceneConfig, int targetFps = 60);
    
    /// <summary>
    /// Monitor performance metrics and adjust polish accordingly
    /// </summary>
    /// <returns>Current performance status</returns>
    Task<PerformanceStatus> MonitorPerformanceAsync();
    
    /// <summary>
    /// Enable/disable resource-intensive effects based on system capabilities
    /// </summary>
    /// <param name="systemSpecs">Current system specifications</param>
    /// <returns>Recommended polish configuration</returns>
    Task<PolishConfig> AdaptToSystemAsync(SystemSpecifications systemSpecs);
    
    /// <summary>
    /// Lazy load visual assets to minimize initial load time
    /// </summary>
    /// <param name="assetManifest">Manifest of assets to load</param>
    /// <returns>Asset loading progress</returns>
    Task<AssetLoadingProgress> LazyLoadAssetsAsync(AssetManifest assetManifest);
}

/// <summary>
/// Free code integrator for external libraries
/// </summary>
public interface IFreeCodeIntegrator
{
    /// <summary>
    /// Integrate three-globe library for 3D network visualization
    /// </summary>
    /// <param name="globeConfig">Globe visualization configuration</param>
    /// <returns>Integration result with JavaScript setup code</returns>
    Task<IntegrationResult> IntegrateThreeGlobeAsync(GlobeConfig globeConfig);
    
    /// <summary>
    /// Integrate xterm.js for enhanced terminal experiences
    /// </summary>
    /// <param name="terminalConfig">Terminal enhancement configuration</param>
    /// <returns>Integration result with terminal setup code</returns>
    Task<IntegrationResult> IntegrateXtermJsAsync(TerminalConfig terminalConfig);
    
    /// <summary>
    /// Integrate particle.js for ambient effects
    /// </summary>
    /// <param name="particleConfig">Particle system configuration</param>
    /// <returns>Integration result with particle system code</returns>
    Task<IntegrationResult> IntegrateParticleJsAsync(ParticleConfig particleConfig);
    
    /// <summary>
    /// Validate and sanitize external JavaScript code
    /// </summary>
    /// <param name="jsCode">JavaScript code to validate</param>
    /// <returns>Validation result with safety assessment</returns>
    Task<CodeValidationResult> ValidateExternalCodeAsync(string jsCode);
    
    /// <summary>
    /// Load external library from CDN or local cache
    /// </summary>
    /// <param name="libraryName">Name of library to load</param>
    /// <param name="version">Version to load</param>
    /// <param name="fallbackPath">Local fallback path</param>
    /// <returns>Library loading result</returns>
    Task<LibraryLoadResult> LoadExternalLibraryAsync(string libraryName, string version, string? fallbackPath = null);
}