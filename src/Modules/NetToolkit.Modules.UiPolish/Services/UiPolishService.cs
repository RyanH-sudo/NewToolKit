using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using NetToolkit.Core.Interfaces;
using NetToolkit.Modules.UiPolish.Interfaces;
using NetToolkit.Modules.UiPolish.Models;
using NetToolkit.Modules.UiPolish.Events;
using System.Text.Json;
using System.Text;

namespace NetToolkit.Modules.UiPolish.Services;

/// <summary>
/// UI Polish service - the aesthetic alchemist transforming mundane interfaces into ethereal experiences
/// Orchestrates the symphony of visual excellence across all NetToolkit components
/// </summary>
public class UiPolishService : IUiPolishService
{
    private readonly ILogger<UiPolishService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IEventBus _eventBus;
    private readonly IThemeManager _themeManager;
    private readonly IAnimationEngine _animationEngine;
    private readonly IShaderUtility _shaderUtility;
    private readonly IPolishEventPublisher _eventPublisher;
    
    private readonly Dictionary<string, string> _activeTips = new();
    private readonly HashSet<string> _enhancedComponents = new();
    private PolishState _currentState = new();
    private readonly SemaphoreSlim _stateLock = new(1, 1);

    public UiPolishService(
        ILogger<UiPolishService> logger,
        IConfiguration configuration,
        IEventBus eventBus,
        IThemeManager themeManager,
        IAnimationEngine animationEngine,
        IShaderUtility shaderUtility,
        IPolishEventPublisher eventPublisher)
    {
        _logger = logger;
        _configuration = configuration;
        _eventBus = eventBus;
        _themeManager = themeManager;
        _animationEngine = animationEngine;
        _shaderUtility = shaderUtility;
        _eventPublisher = eventPublisher;
        
        _logger.LogInformation("UI Polish service initialized - aesthetic alchemy laboratory ready for visual transmutation");
    }

    public async Task ApplyThemeAsync(ThemeConfig config)
    {
        await _stateLock.WaitAsync();
        try
        {
            _logger.LogInformation("Applying theme '{ThemeName}' - visual transformation commencing", config.Name);
            
            // Apply theme globally through theme manager
            var themeDefinition = await _themeManager.CreateCustomThemeAsync(config);
            await _themeManager.ApplyGlobalThemeAsync(themeDefinition.Id);
            
            // Update current state
            _currentState.ActiveTheme = config.Name;
            _currentState.LastUpdated = DateTime.UtcNow;
            
            // Generate theme-specific CSS variables
            var cssVariables = GenerateThemeCssVariables(config);
            await InjectGlobalStylesAsync(cssVariables);
            
            // Apply floating panel aesthetics if enabled
            if (config.Effects.EnableGlow)
            {
                await ApplyGlobalGlowEffectsAsync(config);
            }
            
            // Setup metallic effects if enabled
            if (config.Materials.EnableMetallicEffects)
            {
                await ApplyGlobalMetallicEffectsAsync(config);
            }
            
            // Publish theme applied event
            await _eventPublisher.PublishThemeAppliedAsync(config.Name, DateTime.UtcNow);
            
            _logger.LogInformation("Theme '{ThemeName}' applied successfully - visual transformation complete!", config.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply theme '{ThemeName}' - aesthetic alchemy interrupted", config.Name);
            throw;
        }
        finally
        {
            _stateLock.Release();
        }
    }

    public async Task<string> EnhanceThreeJsAsync(ComponentType componentType, ThreeJsOptions options)
    {
        try
        {
            _logger.LogInformation("Enhancing {ComponentType} with Three.js magic - visual sorcery begins", componentType);
            
            var enhancementScript = new StringBuilder();
            
            // Add component-specific Three.js enhancements
            switch (componentType)
            {
                case ComponentType.Topography:
                    enhancementScript.Append(await GenerateTopographyEnhancementAsync(options));
                    break;
                    
                case ComponentType.Terminal:
                    enhancementScript.Append(await GenerateTerminalEnhancementAsync(options));
                    break;
                    
                case ComponentType.Dashboard:
                    enhancementScript.Append(await GenerateDashboardEnhancementAsync(options));
                    break;
                    
                case ComponentType.Scanner:
                    enhancementScript.Append(await GenerateScannerEnhancementAsync(options));
                    break;
                    
                case ComponentType.AiOrb:
                    enhancementScript.Append(await GenerateOrbEnhancementAsync(options));
                    break;
                    
                default:
                    enhancementScript.Append(await GenerateGenericEnhancementAsync(options));
                    break;
            }
            
            // Add performance optimizations
            if (options.PerformanceLevel != PerformanceLevel.Ultra)
            {
                enhancementScript.AppendLine(GeneratePerformanceOptimizations(options.PerformanceLevel));
            }
            
            // Track enhanced component
            _enhancedComponents.Add(componentType.ToString());
            _currentState.AppliedEnhancements.Add($"{componentType}-ThreeJs");
            
            // Publish enhancement event
            await _eventPublisher.PublishEnhancementLoadedAsync(componentType, "Three.js", true);
            
            var script = enhancementScript.ToString();
            _logger.LogInformation("Three.js enhancement for {ComponentType} generated - {ScriptLength} characters of visual magic", 
                componentType, script.Length);
                
            return script;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enhance {ComponentType} with Three.js", componentType);
            await _eventPublisher.PublishEnhancementLoadedAsync(componentType, "Three.js", false);
            return GenerateFallbackScript();
        }
    }

    public async Task AddHoverTipsAsync(Dictionary<string, string> tips)
    {
        try
        {
            _logger.LogInformation("Adding {TipCount} hover tips - wit and wisdom distribution initiated", tips.Count);
            
            foreach (var (elementId, tipText) in tips)
            {
                _activeTips[elementId] = tipText;
                
                // Generate hover tip JavaScript
                var tipScript = GenerateHoverTipScript(elementId, tipText);
                await InjectScriptAsync(tipScript);
                
                // Publish tip added event
                await _eventPublisher.PublishTipAddedAsync(elementId, tipText);
            }
            
            // Update current state
            _currentState.ActiveTips = new Dictionary<string, string>(_activeTips);
            _currentState.LastUpdated = DateTime.UtcNow;
            
            _logger.LogInformation("Hover tips applied successfully - interface wisdom distributed with wit!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply hover tips - wit distribution interrupted");
            throw;
        }
    }

    public async Task ApplyFloatingPanelsAsync(IEnumerable<string> containerIds, FloatingPanelStyle style)
    {
        try
        {
            _logger.LogInformation("Applying floating panel aesthetics - ethereal UI transformation commencing");
            
            var floatingPanelCss = GenerateFloatingPanelCss(style);
            var floatingPanelScript = GenerateFloatingPanelScript(containerIds, style);
            
            // Inject CSS for floating panels
            await InjectGlobalStylesAsync(floatingPanelCss);
            
            // Apply JavaScript animations
            await InjectScriptAsync(floatingPanelScript);
            
            // Add floating animations if enabled
            if (style.FloatingEffect != FloatingAnimation.None)
            {
                foreach (var containerId in containerIds)
                {
                    await _animationEngine.CreateBreathingAnimationAsync(new[] { containerId }, GetBreathingRate(style.FloatingEffect));
                }
            }
            
            _logger.LogInformation("Floating panels applied - UI transcends mundane boundaries!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply floating panels - ethereal transformation interrupted");
            throw;
        }
    }

    public async Task<string> GenerateMetallicEffectsAsync(IEnumerable<string> elements, MetallicType metallicType)
    {
        try
        {
            _logger.LogInformation("Generating {MetallicType} effects for {ElementCount} elements", metallicType, elements.Count());
            
            var metallicShader = await _shaderUtility.GenerateGlslShaderAsync(ShaderType.Metallic, new Dictionary<string, object>
            {
                ["metallicType"] = metallicType.ToString(),
                ["roughness"] = GetMetallicRoughness(metallicType),
                ["metallic"] = GetMetallicIntensity(metallicType),
                ["baseColor"] = GetMetallicColor(metallicType)
            });
            
            var cssEffects = GenerateMetallicCss(elements, metallicType, metallicShader);
            
            _logger.LogInformation("{MetallicType} effects generated - metallic magnificence materialized", metallicType);
            return cssEffects;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate metallic effects for type {MetallicType}", metallicType);
            return GenerateFallbackMetallicCss(elements);
        }
    }

    public async Task<string> CreateParticleSystemAsync(ParticleSystemConfig particleConfig)
    {
        try
        {
            _logger.LogInformation("Creating particle system for container {ContainerId} - ambient magic summoning", particleConfig.ContainerId);
            
            var particleScript = new StringBuilder();
            
            // Initialize particle system
            particleScript.AppendLine($"// NetToolkit Particle System - {particleConfig.Type}");
            particleScript.AppendLine($"const particleSystem_{particleConfig.ContainerId} = new ParticleSystem({{");
            particleScript.AppendLine($"    container: '{particleConfig.ContainerId}',");
            particleScript.AppendLine($"    particleCount: {particleConfig.ParticleCount},");
            particleScript.AppendLine($"    type: '{particleConfig.Type}',");
            
            // Add color configuration
            particleScript.AppendLine("    color: {");
            particleScript.AppendLine($"        start: '{particleConfig.ColorRange.StartColor}',");
            particleScript.AppendLine($"        end: '{particleConfig.ColorRange.EndColor}'");
            particleScript.AppendLine("    },");
            
            // Add size configuration
            particleScript.AppendLine("    size: {");
            particleScript.AppendLine($"        min: {particleConfig.SizeRange.MinSize},");
            particleScript.AppendLine($"        max: {particleConfig.SizeRange.MaxSize}");
            particleScript.AppendLine("    },");
            
            // Add speed configuration
            particleScript.AppendLine("    speed: {");
            particleScript.AppendLine($"        min: {particleConfig.SpeedRange.MinSpeed},");
            particleScript.AppendLine($"        max: {particleConfig.SpeedRange.MaxSpeed}");
            particleScript.AppendLine("    },");
            
            // Add interactivity if enabled
            if (particleConfig.EnableInteractivity)
            {
                particleScript.AppendLine("    interactivity: {");
                particleScript.AppendLine("        detectsOn: 'canvas',");
                particleScript.AppendLine("        events: {");
                particleScript.AppendLine("            onHover: { enable: true, mode: 'repulse' },");
                particleScript.AppendLine("            onClick: { enable: true, mode: 'push' }");
                particleScript.AppendLine("        }");
                particleScript.AppendLine("    },");
            }
            
            // Add connections if enabled
            if (particleConfig.EnableConnections)
            {
                particleScript.AppendLine("    lineLinked: {");
                particleScript.AppendLine("        enable: true,");
                particleScript.AppendLine($"        distance: {particleConfig.ConnectionDistance},");
                particleScript.AppendLine("        color: '#ffffff',");
                particleScript.AppendLine("        opacity: 0.4,");
                particleScript.AppendLine("        width: 1");
                particleScript.AppendLine("    }");
            }
            
            particleScript.AppendLine("});");
            particleScript.AppendLine();
            particleScript.AppendLine("// Start particle system");
            particleScript.AppendLine($"particleSystem_{particleConfig.ContainerId}.start();");
            
            var script = particleScript.ToString();
            _logger.LogInformation("Particle system created - ambient magic flows through {ContainerId}", particleConfig.ContainerId);
            
            return script;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create particle system for {ContainerId}", particleConfig.ContainerId);
            return GenerateFallbackParticleScript(particleConfig.ContainerId);
        }
    }

    public async Task<PolishState> GetPolishStateAsync()
    {
        await _stateLock.WaitAsync();
        try
        {
            // Update performance metrics
            _currentState.Performance = await CollectPerformanceMetricsAsync();
            _currentState.LastUpdated = DateTime.UtcNow;
            
            return _currentState;
        }
        finally
        {
            _stateLock.Release();
        }
    }

    public async Task ResetPolishAsync()
    {
        await _stateLock.WaitAsync();
        try
        {
            _logger.LogInformation("Resetting UI polish - returning to pristine state");
            
            // Clear active tips
            _activeTips.Clear();
            
            // Clear enhanced components
            _enhancedComponents.Clear();
            
            // Reset state
            _currentState = new PolishState();
            
            // Apply default theme
            var defaultTheme = await _themeManager.GetAvailableThemesAsync();
            var scandinavianTheme = defaultTheme.FirstOrDefault(t => t.Id == "scandinavian-cyber");
            if (scandinavianTheme != null)
            {
                await _themeManager.ApplyGlobalThemeAsync(scandinavianTheme.Id);
            }
            
            _logger.LogInformation("UI polish reset complete - pristine state restored");
        }
        finally
        {
            _stateLock.Release();
        }
    }

    // Private helper methods for enhancement generation
    
    private async Task<string> GenerateTopographyEnhancementAsync(ThreeJsOptions options)
    {
        var script = new StringBuilder();
        
        script.AppendLine("// Three.js Topography Enhancement - Network visualization magic");
        script.AppendLine("function enhanceTopography(containerId) {");
        script.AppendLine("    const container = document.getElementById(containerId);");
        script.AppendLine("    const scene = new THREE.Scene();");
        script.AppendLine("    const camera = new THREE.PerspectiveCamera(75, container.offsetWidth / container.offsetHeight, 0.1, 1000);");
        script.AppendLine("    const renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true });");
        script.AppendLine("    ");
        script.AppendLine("    renderer.setSize(container.offsetWidth, container.offsetHeight);");
        script.AppendLine("    renderer.setClearColor(0x000000, 0);");
        script.AppendLine("    container.appendChild(renderer.domElement);");
        script.AppendLine("    ");
        
        if (options.EnableShaders)
        {
            script.AppendLine("    // Add holographic network nodes with custom shaders");
            script.AppendLine("    const nodeGeometry = new THREE.SphereGeometry(0.1, 16, 16);");
            script.AppendLine("    const holographicMaterial = new THREE.ShaderMaterial({");
            script.AppendLine("        vertexShader: `");
            script.AppendLine("            varying vec3 vPosition;");
            script.AppendLine("            void main() {");
            script.AppendLine("                vPosition = position;");
            script.AppendLine("                gl_Position = projectionMatrix * modelViewMatrix * vec4(position, 1.0);");
            script.AppendLine("            }");
            script.AppendLine("        `,");
            script.AppendLine("        fragmentShader: `");
            script.AppendLine("            uniform float time;");
            script.AppendLine("            varying vec3 vPosition;");
            script.AppendLine("            void main() {");
            script.AppendLine("                float glow = sin(time + vPosition.x * 10.0) * 0.5 + 0.5;");
            script.AppendLine("                gl_FragColor = vec4(0.0, 0.8 + glow * 0.2, 1.0, 0.8);");
            script.AppendLine("            }");
            script.AppendLine("        `,");
            script.AppendLine("        uniforms: { time: { value: 0.0 } },");
            script.AppendLine("        transparent: true");
            script.AppendLine("    });");
        }
        
        script.AppendLine("    ");
        script.AppendLine("    // Animation loop");
        script.AppendLine("    function animate() {");
        script.AppendLine("        requestAnimationFrame(animate);");
        script.AppendLine("        if (holographicMaterial) holographicMaterial.uniforms.time.value += 0.01;");
        script.AppendLine("        renderer.render(scene, camera);");
        script.AppendLine("    }");
        script.AppendLine("    animate();");
        script.AppendLine("}");
        
        return await Task.FromResult(script.ToString());
    }

    private async Task<string> GenerateTerminalEnhancementAsync(ThreeJsOptions options)
    {
        var script = new StringBuilder();
        
        script.AppendLine("// Three.js Terminal Enhancement - Holographic console magic");
        script.AppendLine("function enhanceTerminal(containerId) {");
        script.AppendLine("    const terminal = document.getElementById(containerId);");
        script.AppendLine("    ");
        script.AppendLine("    // Add holographic scanlines effect");
        script.AppendLine("    const scanlinesOverlay = document.createElement('div');");
        script.AppendLine("    scanlinesOverlay.className = 'terminal-scanlines';");
        script.AppendLine("    scanlinesOverlay.style.cssText = `");
        script.AppendLine("        position: absolute;");
        script.AppendLine("        top: 0; left: 0; right: 0; bottom: 0;");
        script.AppendLine("        pointer-events: none;");
        script.AppendLine("        background: repeating-linear-gradient(");
        script.AppendLine("            0deg,");
        script.AppendLine("            transparent,");
        script.AppendLine("            transparent 2px,");
        script.AppendLine("            rgba(0, 212, 255, 0.1) 2px,");
        script.AppendLine("            rgba(0, 212, 255, 0.1) 4px");
        script.AppendLine("        );");
        script.AppendLine("        animation: scanlines 0.1s linear infinite;");
        script.AppendLine("    `;");
        script.AppendLine("    terminal.appendChild(scanlinesOverlay);");
        script.AppendLine("    ");
        script.AppendLine("    // Add typing glow effect");
        script.AppendLine("    const cursor = terminal.querySelector('.cursor, .terminal-cursor');");
        script.AppendLine("    if (cursor) {");
        script.AppendLine("        cursor.style.boxShadow = '0 0 10px #00d4ff, 0 0 20px #00d4ff';");
        script.AppendLine("        cursor.style.backgroundColor = '#00d4ff';");
        script.AppendLine("    }");
        script.AppendLine("}");
        
        script.AppendLine("// CSS keyframes for scanlines");
        script.AppendLine("const scanlinesCSS = `");
        script.AppendLine("@keyframes scanlines {");
        script.AppendLine("    0% { transform: translateY(0px); }");
        script.AppendLine("    100% { transform: translateY(4px); }");
        script.AppendLine("}`;");
        script.AppendLine("const style = document.createElement('style');");
        script.AppendLine("style.textContent = scanlinesCSS;");
        script.AppendLine("document.head.appendChild(style);");
        
        return await Task.FromResult(script.ToString());
    }

    private async Task<string> GenerateDashboardEnhancementAsync(ThreeJsOptions options)
    {
        var script = new StringBuilder();
        
        script.AppendLine("// Three.js Dashboard Enhancement - Holographic data visualization");
        script.AppendLine("function enhanceDashboard(containerId) {");
        script.AppendLine("    const dashboard = document.getElementById(containerId);");
        script.AppendLine("    ");
        script.AppendLine("    // Add floating data particles");
        script.AppendLine("    const particleContainer = document.createElement('div');");
        script.AppendLine("    particleContainer.className = 'dashboard-particles';");
        script.AppendLine("    particleContainer.style.cssText = `");
        script.AppendLine("        position: absolute;");
        script.AppendLine("        top: 0; left: 0; right: 0; bottom: 0;");
        script.AppendLine("        pointer-events: none;");
        script.AppendLine("        overflow: hidden;");
        script.AppendLine("    `;");
        script.AppendLine("    dashboard.appendChild(particleContainer);");
        script.AppendLine("    ");
        script.AppendLine("    // Create floating particles");
        script.AppendLine("    for (let i = 0; i < 20; i++) {");
        script.AppendLine("        const particle = document.createElement('div');");
        script.AppendLine("        particle.className = 'data-particle';");
        script.AppendLine("        particle.style.cssText = `");
        script.AppendLine("            position: absolute;");
        script.AppendLine("            width: 4px; height: 4px;");
        script.AppendLine("            background: #00d4ff;");
        script.AppendLine("            border-radius: 50%;");
        script.AppendLine("            box-shadow: 0 0 6px #00d4ff;");
        script.AppendLine("            left: ${Math.random() * 100}%;");
        script.AppendLine("            top: ${Math.random() * 100}%;");
        script.AppendLine("            animation: floatParticle ${5 + Math.random() * 10}s linear infinite;");
        script.AppendLine("            animation-delay: ${Math.random() * 5}s;");
        script.AppendLine("        `;");
        script.AppendLine("        particleContainer.appendChild(particle);");
        script.AppendLine("    }");
        script.AppendLine("}");
        
        return await Task.FromResult(script.ToString());
    }

    private async Task<string> GenerateScannerEnhancementAsync(ThreeJsOptions options)
    {
        var script = new StringBuilder();
        
        script.AppendLine("// Three.js Scanner Enhancement - Network scanning visualization");
        script.AppendLine("function enhanceScanner(containerId) {");
        script.AppendLine("    const scanner = document.getElementById(containerId);");
        script.AppendLine("    ");
        script.AppendLine("    // Add scanning pulse effect");
        script.AppendLine("    const pulseOverlay = document.createElement('div');");
        script.AppendLine("    pulseOverlay.className = 'scanner-pulse';");
        script.AppendLine("    pulseOverlay.style.cssText = `");
        script.AppendLine("        position: absolute;");
        script.AppendLine("        top: 50%; left: 50%;");
        script.AppendLine("        width: 100px; height: 100px;");
        script.AppendLine("        margin: -50px 0 0 -50px;");
        script.AppendLine("        border: 2px solid #00d4ff;");
        script.AppendLine("        border-radius: 50%;");
        script.AppendLine("        pointer-events: none;");
        script.AppendLine("        animation: scannerPulse 2s ease-out infinite;");
        script.AppendLine("    `;");
        script.AppendLine("    scanner.appendChild(pulseOverlay);");
        script.AppendLine("}");
        
        return await Task.FromResult(script.ToString());
    }

    private async Task<string> GenerateOrbEnhancementAsync(ThreeJsOptions options)
    {
        var script = new StringBuilder();
        
        script.AppendLine("// Three.js AI Orb Enhancement - Holographic intelligence visualization");
        script.AppendLine("function enhanceAiOrb(containerId) {");
        script.AppendLine("    const orb = document.getElementById(containerId);");
        script.AppendLine("    ");
        script.AppendLine("    // Add orbital rings");
        script.AppendLine("    const rings = document.createElement('div');");
        script.AppendLine("    rings.className = 'orb-rings';");
        script.AppendLine("    rings.style.cssText = `");
        script.AppendLine("        position: absolute;");
        script.AppendLine("        top: 50%; left: 50%;");
        script.AppendLine("        width: 200px; height: 200px;");
        script.AppendLine("        margin: -100px 0 0 -100px;");
        script.AppendLine("        pointer-events: none;");
        script.AppendLine("    `;");
        script.AppendLine("    ");
        script.AppendLine("    // Create multiple rings");
        script.AppendLine("    for (let i = 0; i < 3; i++) {");
        script.AppendLine("        const ring = document.createElement('div');");
        script.AppendLine("        ring.style.cssText = `");
        script.AppendLine("            position: absolute;");
        script.AppendLine("            top: 50%; left: 50%;");
        script.AppendLine("            width: ${80 + i * 40}px;");
        script.AppendLine("            height: ${80 + i * 40}px;");
        script.AppendLine("            margin: ${-40 - i * 20}px 0 0 ${-40 - i * 20}px;");
        script.AppendLine("            border: 1px solid rgba(0, 212, 255, ${0.3 - i * 0.1});");
        script.AppendLine("            border-radius: 50%;");
        script.AppendLine("            animation: orbitalRotate ${10 + i * 5}s linear infinite;");
        script.AppendLine("        `;");
        script.AppendLine("        rings.appendChild(ring);");
        script.AppendLine("    }");
        script.AppendLine("    orb.appendChild(rings);");
        script.AppendLine("}");
        
        return await Task.FromResult(script.ToString());
    }

    private async Task<string> GenerateGenericEnhancementAsync(ThreeJsOptions options)
    {
        var script = new StringBuilder();
        
        script.AppendLine("// Generic Three.js Enhancement");
        script.AppendLine("function enhanceGenericComponent(containerId) {");
        script.AppendLine("    const component = document.getElementById(containerId);");
        script.AppendLine("    if (!component) return;");
        script.AppendLine("    ");
        script.AppendLine("    // Add subtle glow effect");
        script.AppendLine("    component.style.boxShadow = 'inset 0 0 20px rgba(0, 212, 255, 0.1)';");
        script.AppendLine("    component.style.border = '1px solid rgba(0, 212, 255, 0.2)';");
        script.AppendLine("}");
        
        return await Task.FromResult(script.ToString());
    }

    private string GeneratePerformanceOptimizations(PerformanceLevel level)
    {
        var optimizations = new StringBuilder();
        
        optimizations.AppendLine("// Performance optimizations");
        
        switch (level)
        {
            case PerformanceLevel.Low:
                optimizations.AppendLine("// Low performance mode - minimal effects");
                optimizations.AppendLine("const maxParticles = 10;");
                optimizations.AppendLine("const renderScale = 0.5;");
                optimizations.AppendLine("const targetFPS = 30;");
                break;
                
            case PerformanceLevel.Medium:
                optimizations.AppendLine("// Medium performance mode - balanced effects");
                optimizations.AppendLine("const maxParticles = 50;");
                optimizations.AppendLine("const renderScale = 0.75;");
                optimizations.AppendLine("const targetFPS = 45;");
                break;
                
            case PerformanceLevel.High:
                optimizations.AppendLine("// High performance mode - full effects");
                optimizations.AppendLine("const maxParticles = 100;");
                optimizations.AppendLine("const renderScale = 1.0;");
                optimizations.AppendLine("const targetFPS = 60;");
                break;
        }
        
        return optimizations.ToString();
    }

    private string GenerateFallbackScript()
    {
        return @"
// Fallback enhancement script
console.log('Three.js enhancement fallback - basic styling applied');
function fallbackEnhancement(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.style.transition = 'all 0.3s ease';
        element.style.filter = 'drop-shadow(0 0 5px rgba(0, 212, 255, 0.3))';
    }
}";
    }

    private string GenerateThemeCssVariables(ThemeConfig config)
    {
        var css = new StringBuilder();
        
        css.AppendLine(":root {");
        css.AppendLine($"  --nt-primary: {config.Colors.Primary};");
        css.AppendLine($"  --nt-primary-light: {config.Colors.PrimaryLight};");
        css.AppendLine($"  --nt-primary-dark: {config.Colors.PrimaryDark};");
        css.AppendLine($"  --nt-secondary: {config.Colors.Secondary};");
        css.AppendLine($"  --nt-accent: {config.Colors.Accent};");
        css.AppendLine($"  --nt-accent-warm: {config.Colors.AccentWarm};");
        css.AppendLine($"  --nt-accent-cool: {config.Colors.AccentCool};");
        css.AppendLine($"  --nt-surface: {config.Colors.Surface};");
        css.AppendLine($"  --nt-background: {config.Colors.Background};");
        css.AppendLine($"  --nt-glow-color: {config.Colors.GlowColor};");
        css.AppendLine($"  --nt-hologram-primary: {config.Colors.HologramPrimary};");
        css.AppendLine($"  --nt-font-primary: '{config.Typography.PrimaryFont}';");
        css.AppendLine($"  --nt-font-mono: '{config.Typography.MonospaceFont}';");
        css.AppendLine($"  --nt-border-radius: 12px;");
        css.AppendLine($"  --nt-shadow-glow: 0 0 20px var(--nt-glow-color);");
        css.AppendLine("}");
        
        return css.ToString();
    }

    private async Task ApplyGlobalGlowEffectsAsync(ThemeConfig config)
    {
        var glowCss = @"
.nt-glow {
    box-shadow: 0 0 10px var(--nt-glow-color), 
                inset 0 0 10px rgba(0, 212, 255, 0.1);
}

.nt-glow-text {
    text-shadow: 0 0 8px var(--nt-glow-color);
}

.nt-glow-border {
    border: 1px solid var(--nt-glow-color);
    box-shadow: inset 0 0 5px rgba(0, 212, 255, 0.2);
}";
        
        await InjectGlobalStylesAsync(glowCss);
    }

    private async Task ApplyGlobalMetallicEffectsAsync(ThemeConfig config)
    {
        var metallicCss = @"
.nt-metallic {
    background: linear-gradient(135deg, 
                var(--nt-secondary) 0%, 
                var(--nt-primary-light) 50%, 
                var(--nt-secondary) 100%);
    background-size: 200% 200%;
    animation: metallicShimmer 3s ease-in-out infinite;
}

@keyframes metallicShimmer {
    0%, 100% { background-position: 0% 0%; }
    50% { background-position: 100% 100%; }
}

.nt-chrome {
    background: linear-gradient(45deg, #c4c4c4, #ffffff, #c4c4c4);
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
}";
        
        await InjectGlobalStylesAsync(metallicCss);
    }

    private string GenerateHoverTipScript(string elementId, string tipText)
    {
        return $@"
// Hover tip for {elementId}
(function() {{
    const element = document.getElementById('{elementId}');
    if (!element) return;
    
    const tip = document.createElement('div');
    tip.className = 'nt-hover-tip';
    tip.textContent = '{tipText}';
    tip.style.cssText = `
        position: absolute;
        background: var(--nt-surface);
        color: var(--nt-accent);
        padding: 8px 12px;
        border-radius: 6px;
        font-size: 12px;
        font-family: var(--nt-font-primary);
        backdrop-filter: blur(10px);
        border: 1px solid var(--nt-accent);
        box-shadow: var(--nt-shadow-glow);
        z-index: 10000;
        opacity: 0;
        pointer-events: none;
        transition: opacity 0.3s ease;
        white-space: nowrap;
    `;
    
    element.addEventListener('mouseenter', (e) => {{
        document.body.appendChild(tip);
        const rect = element.getBoundingClientRect();
        tip.style.left = rect.left + 'px';
        tip.style.top = (rect.top - tip.offsetHeight - 8) + 'px';
        tip.style.opacity = '1';
    }});
    
    element.addEventListener('mouseleave', () => {{
        tip.style.opacity = '0';
        setTimeout(() => tip.remove(), 300);
    }});
}})();";
    }

    private string GenerateFloatingPanelCss(FloatingPanelStyle style)
    {
        return $@"
.nt-floating-panel {{
    background: {style.BackgroundColor};
    backdrop-filter: blur({style.BlurRadius}px);
    border: {style.BorderThickness}px solid {style.BorderColor};
    border-radius: {style.BorderRadius}px;
    opacity: {style.Opacity};
    box-shadow: {style.Shadow.OffsetX}px {style.Shadow.OffsetY}px {style.Shadow.BlurRadius}px {style.Shadow.Color};
    transition: all 0.3s ease;
}}

.nt-floating-panel:hover {{
    transform: translateY(-2px);
    box-shadow: {style.Shadow.OffsetX}px {style.Shadow.OffsetY + 4}px {style.Shadow.BlurRadius + 8}px {style.Shadow.Color};
}}

@keyframes gentleFloat {{
    0%, 100% {{ transform: translateY(0px); }}
    50% {{ transform: translateY(-3px); }}
}}

@keyframes moderateFloat {{
    0%, 100% {{ transform: translateY(0px); }}
    50% {{ transform: translateY(-6px); }}
}}

@keyframes pronouncedFloat {{
    0%, 100% {{ transform: translateY(0px); }}
    50% {{ transform: translateY(-10px); }}
}}";
    }

    private string GenerateFloatingPanelScript(IEnumerable<string> containerIds, FloatingPanelStyle style)
    {
        var script = new StringBuilder();
        
        foreach (var containerId in containerIds)
        {
            script.AppendLine($"// Apply floating panel style to {containerId}");
            script.AppendLine($"const panel_{containerId} = document.getElementById('{containerId}');");
            script.AppendLine($"if (panel_{containerId}) {{");
            script.AppendLine($"    panel_{containerId}.classList.add('nt-floating-panel');");
            
            if (style.FloatingEffect != FloatingAnimation.None)
            {
                var animationName = style.FloatingEffect.ToString().ToLower() + "Float";
                script.AppendLine($"    panel_{containerId}.style.animation = '{animationName} 4s ease-in-out infinite';");
            }
            
            script.AppendLine("}");
        }
        
        return script.ToString();
    }

    private double GetBreathingRate(FloatingAnimation animation)
    {
        return animation switch
        {
            FloatingAnimation.Gentle => 8.0,
            FloatingAnimation.Moderate => 12.0,
            FloatingAnimation.Pronounced => 16.0,
            _ => 10.0
        };
    }

    private double GetMetallicRoughness(MetallicType type)
    {
        return type switch
        {
            MetallicType.Chrome => 0.05,
            MetallicType.Gold => 0.1,
            MetallicType.Silver => 0.08,
            MetallicType.Copper => 0.15,
            MetallicType.Titanium => 0.2,
            MetallicType.Holographic => 0.0,
            MetallicType.Iridescent => 0.0,
            _ => 0.1
        };
    }

    private double GetMetallicIntensity(MetallicType type)
    {
        return type switch
        {
            MetallicType.Chrome => 0.9,
            MetallicType.Gold => 0.8,
            MetallicType.Silver => 0.85,
            MetallicType.Copper => 0.7,
            MetallicType.Titanium => 0.6,
            MetallicType.Holographic => 0.3,
            MetallicType.Iridescent => 0.4,
            _ => 0.7
        };
    }

    private string GetMetallicColor(MetallicType type)
    {
        return type switch
        {
            MetallicType.Chrome => "#e8e8e8",
            MetallicType.Gold => "#ffd700",
            MetallicType.Silver => "#c4c4c4",
            MetallicType.Copper => "#b87333",
            MetallicType.Titanium => "#878681",
            MetallicType.Holographic => "#00ffff",
            MetallicType.Iridescent => "#ff00ff",
            _ => "#c4c4c4"
        };
    }

    private string GenerateMetallicCss(IEnumerable<string> elements, MetallicType type, ShaderCode shader)
    {
        var css = new StringBuilder();
        var color = GetMetallicColor(type);
        
        foreach (var element in elements)
        {
            css.AppendLine($"#{element}.nt-metallic-{type.ToString().ToLower()} {{");
            css.AppendLine($"    background: linear-gradient(135deg, {color}88, {color}, {color}88);");
            css.AppendLine("    background-size: 200% 200%;");
            css.AppendLine("    animation: metallicShimmer 3s ease-in-out infinite;");
            css.AppendLine("}");
        }
        
        return css.ToString();
    }

    private string GenerateFallbackMetallicCss(IEnumerable<string> elements)
    {
        var css = new StringBuilder();
        
        foreach (var element in elements)
        {
            css.AppendLine($"#{element} {{");
            css.AppendLine("    background: linear-gradient(135deg, #c4c4c4, #ffffff, #c4c4c4);");
            css.AppendLine("    filter: brightness(1.1);");
            css.AppendLine("}");
        }
        
        return css.ToString();
    }

    private string GenerateFallbackParticleScript(string containerId)
    {
        return $@"
// Fallback particle system for {containerId}
console.log('Particle system fallback for {containerId}');
const container = document.getElementById('{containerId}');
if (container) {{
    container.style.position = 'relative';
    container.style.overflow = 'hidden';
    
    // Simple CSS animation fallback
    const particle = document.createElement('div');
    particle.style.cssText = `
        position: absolute;
        top: 50%; left: 50%;
        width: 4px; height: 4px;
        background: #00d4ff;
        border-radius: 50%;
        animation: simplePulse 2s ease-in-out infinite;
    `;
    container.appendChild(particle);
}}";
    }

    private async Task<PerformanceMetrics> CollectPerformanceMetricsAsync()
    {
        return await Task.FromResult(new PerformanceMetrics
        {
            FrameRate = 60.0, // Would collect from actual performance API
            CpuUsage = 5.0,   // Would collect from system
            MemoryUsage = 150.0, // MB
            ActiveAnimations = _currentState.RunningAnimations.Count,
            LoadedTextures = _enhancedComponents.Count * 2,
            RenderTime = 16.67 // ms for 60fps
        });
    }

    private async Task InjectGlobalStylesAsync(string css)
    {
        // This would inject CSS into the global style system
        // Implementation depends on the specific UI framework being used
        _logger.LogDebug("Injecting global styles - {CssLength} characters", css.Length);
        await Task.CompletedTask;
    }

    private async Task InjectScriptAsync(string script)
    {
        // This would inject JavaScript into the page
        // Implementation depends on the specific UI framework being used
        _logger.LogDebug("Injecting script - {ScriptLength} characters", script.Length);
        await Task.CompletedTask;
    }
}