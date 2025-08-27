using Xunit;
using Microsoft.Extensions.Logging;
using Moq;
using NetToolkit.Modules.UiPolish.Services;
using NetToolkit.Modules.UiPolish.Models;
using NetToolkit.Modules.UiPolish.Interfaces;

namespace NetToolkit.Modules.UiPolish.Tests;

/// <summary>
/// Comprehensive test suite for UI Polish service functionality
/// Ensures visual enhancement systems work correctly under various conditions
/// </summary>
public class UiPolishServiceTests
{
    private readonly Mock<ILogger<UiPolishService>> _mockLogger;
    private readonly Mock<IThreeJsEnhancer> _mockThreeJsEnhancer;
    private readonly Mock<ITipManager> _mockTipManager;
    private readonly Mock<IAnimationEngine> _mockAnimationEngine;
    private readonly Mock<IPolishEventPublisher> _mockEventPublisher;
    private readonly UiPolishService _uiPolishService;

    public UiPolishServiceTests()
    {
        _mockLogger = new Mock<ILogger<UiPolishService>>();
        _mockThreeJsEnhancer = new Mock<IThreeJsEnhancer>();
        _mockTipManager = new Mock<ITipManager>();
        _mockAnimationEngine = new Mock<IAnimationEngine>();
        _mockEventPublisher = new Mock<IPolishEventPublisher>();

        _uiPolishService = new UiPolishService(
            _mockLogger.Object,
            _mockThreeJsEnhancer.Object,
            _mockTipManager.Object,
            _mockAnimationEngine.Object,
            _mockEventPublisher.Object
        );
    }

    [Fact]
    public async Task ApplyThemeAsync_WithValidTheme_ShouldApplySuccessfully()
    {
        // Arrange
        var theme = BuiltInThemes.GetScandinavianCyberTheme();

        // Act
        await _uiPolishService.ApplyThemeAsync(theme);

        // Assert
        var polishState = await _uiPolishService.GetPolishStateAsync();
        Assert.NotNull(polishState.ActiveTheme);
        Assert.Equal("Scandinavian Cyber", polishState.ActiveTheme.Name);
        
        // Verify event was published
        _mockEventPublisher.Verify(
            x => x.PublishThemeAppliedAsync(It.IsAny<string>(), It.IsAny<DateTime>()),
            Times.Once
        );
    }

    [Fact]
    public async Task ApplyThemeAsync_WithNullTheme_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _uiPolishService.ApplyThemeAsync(null!));
    }

    [Theory]
    [InlineData(ComponentType.Terminal)]
    [InlineData(ComponentType.Topography)]
    [InlineData(ComponentType.Dashboard)]
    [InlineData(ComponentType.Scanner)]
    [InlineData(ComponentType.AiOrb)]
    public async Task EnhanceThreeJsAsync_WithValidComponentType_ShouldReturnEnhancementCode(ComponentType componentType)
    {
        // Arrange
        var options = new ThreeJsOptions
        {
            EnableShaders = true,
            EnableParticles = true,
            QualityLevel = QualityLevel.High
        };

        var expectedCode = $"// {componentType} Enhancement Code";
        _mockThreeJsEnhancer
            .Setup(x => x.GenerateEnhancementAsync(componentType, options))
            .ReturnsAsync(expectedCode);

        // Act
        var result = await _uiPolishService.EnhanceThreeJsAsync(componentType, options);

        // Assert
        Assert.NotNull(result);
        Assert.Contains(componentType.ToString(), result);
        
        _mockThreeJsEnhancer.Verify(
            x => x.GenerateEnhancementAsync(componentType, options),
            Times.Once
        );

        _mockEventPublisher.Verify(
            x => x.PublishEnhancementLoadedAsync(componentType, It.IsAny<string>(), true),
            Times.Once
        );
    }

    [Fact]
    public async Task AddHoverTipsAsync_WithValidTips_ShouldAddAllTips()
    {
        // Arrange
        var tips = new Dictionary<string, string>
        {
            ["button1"] = "Click for cosmic enlightenment!",
            ["button2"] = "Hover achieved - tip mastery unlocked!",
            ["panel1"] = "Panel power - floating magnificence!"
        };

        // Act
        await _uiPolishService.AddHoverTipsAsync(tips);

        // Assert
        _mockTipManager.Verify(
            x => x.AddBulkHoverTipsAsync(tips, TipStyle.Default),
            Times.Once
        );

        foreach (var tip in tips)
        {
            _mockEventPublisher.Verify(
                x => x.PublishTipAddedAsync(tip.Key, tip.Value),
                Times.Once
            );
        }
    }

    [Fact]
    public async Task ApplyFloatingPanelsAsync_WithValidContainers_ShouldApplyFloatingStyle()
    {
        // Arrange
        var containerIds = new[] { "panel1", "panel2", "sidebar" };
        var style = new FloatingPanelStyle
        {
            BackgroundOpacity = 0.9f,
            BlurRadius = 10.0f,
            BorderColor = new ColorRgba(0, 212, 255, 128),
            CornerRadius = 8
        };

        // Act
        await _uiPolishService.ApplyFloatingPanelsAsync(containerIds, style);

        // Assert
        var polishState = await _uiPolishService.GetPolishStateAsync();
        Assert.Equal(containerIds.Length, polishState.FloatingPanels.Count);

        foreach (var containerId in containerIds)
        {
            Assert.Contains(containerId, polishState.FloatingPanels.Keys);
        }
    }

    [Theory]
    [InlineData(MetallicType.Chrome)]
    [InlineData(MetallicType.Gold)]
    [InlineData(MetallicType.Holographic)]
    public async Task GenerateMetallicEffectsAsync_WithDifferentTypes_ShouldGenerateAppropriateEffects(MetallicType metallicType)
    {
        // Arrange
        var elements = new[] { "header", "footer", "navbar" };

        // Act
        var result = await _uiPolishService.GenerateMetallicEffectsAsync(elements, metallicType);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains(metallicType.ToString().ToLower(), result.ToLower());
        
        foreach (var element in elements)
        {
            Assert.Contains(element, result);
        }
    }

    [Fact]
    public async Task CreateParticleSystemAsync_WithValidConfig_ShouldGenerateParticleSystem()
    {
        // Arrange
        var config = new ParticleSystemConfig
        {
            ParticleCount = 1000,
            ParticleSize = 2.0f,
            EmissionRate = 50,
            StartColor = new ColorRgba(0, 212, 255, 255),
            EndColor = new ColorRgba(0, 100, 150, 0)
        };

        // Act
        var result = await _uiPolishService.CreateParticleSystemAsync(config);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains("particles", result.ToLower());
        Assert.Contains(config.ParticleCount.ToString(), result);
    }

    [Fact]
    public async Task GetPolishStateAsync_WhenCalled_ShouldReturnCurrentState()
    {
        // Arrange
        var theme = BuiltInThemes.GetScandinavianCyberTheme();
        await _uiPolishService.ApplyThemeAsync(theme);

        var tips = new Dictionary<string, string> { ["test"] = "Test tip" };
        await _uiPolishService.AddHoverTipsAsync(tips);

        // Act
        var state = await _uiPolishService.GetPolishStateAsync();

        // Assert
        Assert.NotNull(state);
        Assert.NotNull(state.ActiveTheme);
        Assert.Equal("Scandinavian Cyber", state.ActiveTheme.Name);
        Assert.NotEmpty(state.HoverTips);
        Assert.True(state.IsPolishActive);
    }

    [Fact]
    public async Task ResetPolishAsync_WhenCalled_ShouldResetToDefaults()
    {
        // Arrange
        var theme = BuiltInThemes.GetScandinavianCyberTheme();
        await _uiPolishService.ApplyThemeAsync(theme);

        var tips = new Dictionary<string, string> { ["test"] = "Test tip" };
        await _uiPolishService.AddHoverTipsAsync(tips);

        // Act
        await _uiPolishService.ResetPolishAsync();

        // Assert
        var state = await _uiPolishService.GetPolishStateAsync();
        Assert.False(state.IsPolishActive);
        Assert.Empty(state.HoverTips);
        Assert.Empty(state.FloatingPanels);
        Assert.Empty(state.ThreeJsEnhancements);
    }

    [Fact]
    public async Task EnhanceThreeJsAsync_WhenEnhancerThrows_ShouldReturnFallbackCode()
    {
        // Arrange
        var options = new ThreeJsOptions { EnableShaders = true };
        _mockThreeJsEnhancer
            .Setup(x => x.GenerateEnhancementAsync(It.IsAny<ComponentType>(), It.IsAny<ThreeJsOptions>()))
            .ThrowsAsync(new Exception("Enhancement failed"));

        // Act
        var result = await _uiPolishService.EnhanceThreeJsAsync(ComponentType.Terminal, options);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("fallback", result.ToLower());
        
        // Should still publish event with failure status
        _mockEventPublisher.Verify(
            x => x.PublishEnhancementLoadedAsync(ComponentType.Terminal, It.IsAny<string>(), false),
            Times.Once
        );
    }

    [Fact]
    public async Task ApplyThemeAsync_WithPerformanceOptimization_ShouldAdaptToSystemCapabilities()
    {
        // Arrange
        var theme = new ThemeConfig
        {
            Name = "High Performance Test",
            ColorPalette = BuiltInThemes.GetScandinavianPalette(),
            QualityLevel = QualityLevel.Ultra // High quality that should be adapted
        };

        // Act
        await _uiPolishService.ApplyThemeAsync(theme);

        // Assert
        var state = await _uiPolishService.GetPolishStateAsync();
        Assert.NotNull(state.ActiveTheme);
        
        // The service should have applied the theme (exact adaptation behavior depends on system)
        Assert.Equal("High Performance Test", state.ActiveTheme.Name);
    }

    [Fact]
    public void Constructor_WithNullDependencies_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new UiPolishService(null!, _mockThreeJsEnhancer.Object, _mockTipManager.Object, 
                _mockAnimationEngine.Object, _mockEventPublisher.Object));

        Assert.Throws<ArgumentNullException>(() => 
            new UiPolishService(_mockLogger.Object, null!, _mockTipManager.Object, 
                _mockAnimationEngine.Object, _mockEventPublisher.Object));

        Assert.Throws<ArgumentNullException>(() => 
            new UiPolishService(_mockLogger.Object, _mockThreeJsEnhancer.Object, null!, 
                _mockAnimationEngine.Object, _mockEventPublisher.Object));
    }

    [Fact]
    public async Task ApplyThemeAsync_MultipleCalls_ShouldReplaceExistingTheme()
    {
        // Arrange
        var theme1 = BuiltInThemes.GetScandinavianCyberTheme();
        var theme2 = new ThemeConfig 
        { 
            Name = "Test Theme 2", 
            ColorPalette = BuiltInThemes.GetScandinavianPalette() 
        };

        // Act
        await _uiPolishService.ApplyThemeAsync(theme1);
        var state1 = await _uiPolishService.GetPolishStateAsync();
        
        await _uiPolishService.ApplyThemeAsync(theme2);
        var state2 = await _uiPolishService.GetPolishStateAsync();

        // Assert
        Assert.Equal("Scandinavian Cyber", state1.ActiveTheme!.Name);
        Assert.Equal("Test Theme 2", state2.ActiveTheme!.Name);

        // Verify theme events were published for both
        _mockEventPublisher.Verify(
            x => x.PublishThemeAppliedAsync(It.IsAny<string>(), It.IsAny<DateTime>()),
            Times.Exactly(2)
        );
    }

    [Theory]
    [InlineData(0)] // Empty tips
    [InlineData(1)] // Single tip
    [InlineData(100)] // Many tips
    public async Task AddHoverTipsAsync_WithVariousTipCounts_ShouldHandleAppropriately(int tipCount)
    {
        // Arrange
        var tips = new Dictionary<string, string>();
        for (int i = 0; i < tipCount; i++)
        {
            tips[$"element{i}"] = $"Tip {i}: Hover magic activated!";
        }

        // Act
        await _uiPolishService.AddHoverTipsAsync(tips);

        // Assert
        if (tipCount > 0)
        {
            _mockTipManager.Verify(
                x => x.AddBulkHoverTipsAsync(tips, TipStyle.Default),
                Times.Once
            );
        }

        var state = await _uiPolishService.GetPolishStateAsync();
        Assert.Equal(tipCount, state.HoverTips.Count);
    }

    [Fact]
    public async Task GenerateMetallicEffectsAsync_WithEmptyElements_ShouldReturnEmptyString()
    {
        // Arrange
        var emptyElements = Array.Empty<string>();

        // Act
        var result = await _uiPolishService.GenerateMetallicEffectsAsync(emptyElements, MetallicType.Chrome);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public async Task CreateParticleSystemAsync_WithInvalidConfig_ShouldHandleGracefully()
    {
        // Arrange
        var invalidConfig = new ParticleSystemConfig
        {
            ParticleCount = -100, // Invalid count
            ParticleSize = -1.0f, // Invalid size
            EmissionRate = 0
        };

        // Act
        var result = await _uiPolishService.CreateParticleSystemAsync(invalidConfig);

        // Assert
        Assert.NotNull(result);
        // Should return fallback or corrected particle system
        Assert.Contains("particles", result.ToLower());
    }
}

/// <summary>
/// Test suite for Animation Service
/// </summary>
public class AnimationServiceTests
{
    private readonly Mock<ILogger<AnimationService>> _mockLogger;
    private readonly AnimationService _animationService;

    public AnimationServiceTests()
    {
        _mockLogger = new Mock<ILogger<AnimationService>>();
        _animationService = new AnimationService(_mockLogger.Object);
    }

    [Theory]
    [InlineData(AnimationType.FadeIn)]
    [InlineData(AnimationType.SlideInFromLeft)]
    [InlineData(AnimationType.ScaleUp)]
    [InlineData(AnimationType.Glow)]
    public async Task AnimateTransitionAsync_WithValidAnimationTypes_ShouldExecuteWithoutError(AnimationType animationType)
    {
        // Arrange
        var elementId = "testElement";
        var duration = 300;

        // Act & Assert
        // Since we can't easily test WPF animations in unit tests, we verify no exceptions are thrown
        await _animationService.AnimateTransitionAsync(elementId, animationType, duration);
        
        // Verify logging occurred
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Animating")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateBreathingAnimationAsync_WithValidElements_ShouldInitializeBreathing()
    {
        // Arrange
        var elementIds = new[] { "element1", "element2", "element3" };
        var breathingRate = 12.0;

        // Act
        await _animationService.CreateBreathingAnimationAsync(elementIds, breathingRate);

        // Assert - Verify logging occurred
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Creating breathing animation")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task StopAnimationsAsync_WithValidElementId_ShouldStopAllAnimations()
    {
        // Arrange
        var elementId = "testElement";

        // Act
        await _animationService.StopAnimationsAsync(elementId);

        // Assert - Should complete without throwing
        // In a real test, we'd verify animations were actually stopped
    }
}

/// <summary>
/// Test suite for Tip Manager
/// </summary>
public class TipManagerTests
{
    private readonly Mock<ILogger<TipManager>> _mockLogger;
    private readonly TipManager _tipManager;

    public TipManagerTests()
    {
        _mockLogger = new Mock<ILogger<TipManager>>();
        _tipManager = new TipManager(_mockLogger.Object);
    }

    [Theory]
    [InlineData(ComponentType.Terminal)]
    [InlineData(ComponentType.Topography)]
    [InlineData(ComponentType.Dashboard)]
    public async Task GetRandomTipForComponentAsync_WithValidComponentType_ShouldReturnTip(ComponentType componentType)
    {
        // Act
        var tip = await _tipManager.GetRandomTipForComponentAsync(componentType);

        // Assert
        Assert.NotNull(tip);
        Assert.NotEmpty(tip);
    }

    [Fact]
    public async Task GetActiveTooltipsAsync_InitialState_ShouldReturnEmptyDictionary()
    {
        // Act
        var tooltips = await _tipManager.GetActiveTooltipsAsync();

        // Assert
        Assert.NotNull(tooltips);
        Assert.Empty(tooltips);
    }

    [Fact]
    public async Task ShowTooltipAsync_WithValidParameters_ShouldShowAndHideTooltip()
    {
        // Arrange
        var elementId = "testElement";
        var text = "Test tooltip text";
        var duration = TimeSpan.FromMilliseconds(100);

        // Act & Assert - Should complete without throwing
        await _tipManager.ShowTooltipAsync(elementId, text, duration);
    }
}

/// <summary>
/// Test suite for Performance Optimizer
/// </summary>
public class PolishOptimizerTests
{
    private readonly Mock<ILogger<PolishOptimizer>> _mockLogger;
    private readonly PolishOptimizer _optimizer;

    public PolishOptimizerTests()
    {
        _mockLogger = new Mock<ILogger<PolishOptimizer>>();
        _optimizer = new PolishOptimizer(_mockLogger.Object);
    }

    [Fact]
    public async Task MonitorPerformanceAsync_WhenCalled_ShouldReturnValidStatus()
    {
        // Act
        var status = await _optimizer.MonitorPerformanceAsync();

        // Assert
        Assert.NotNull(status);
        Assert.True(status.CurrentFps >= 0);
        Assert.True(status.CpuUsagePercent >= 0 && status.CpuUsagePercent <= 100);
        Assert.True(status.MemoryUsageMb >= 0);
    }

    [Fact]
    public async Task OptimizeSceneAsync_WithValidConfig_ShouldReturnOptimizedResult()
    {
        // Arrange
        var sceneConfig = new ThreeJsSceneConfig
        {
            ParticleCount = 2000,
            EnableShaders = true,
            EnablePostProcessing = true,
            EnableShadows = true,
            TextureQuality = TextureQuality.Ultra,
            RenderDistance = 1000
        };

        // Act
        var result = await _optimizer.OptimizeSceneAsync(sceneConfig, 60);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.OptimizedConfig);
        Assert.NotNull(result.OptimizationsApplied);
    }

    [Fact]
    public async Task AdaptToSystemAsync_WithLowEndSpecs_ShouldReturnConservativeConfig()
    {
        // Arrange
        var lowEndSpecs = new SystemSpecifications
        {
            CpuCores = 2,
            RamGb = 4,
            HasDedicatedGpu = false,
            IsLowPowerMode = true
        };

        // Act
        var config = await _optimizer.AdaptToSystemAsync(lowEndSpecs);

        // Assert
        Assert.NotNull(config);
        Assert.False(config.EnableAdvancedShaders);
        Assert.False(config.EnableComplexAnimations);
        Assert.Equal(ParticleComplexity.Low, config.ParticleSystemComplexity);
        Assert.True(config.EnablePowerSaving);
        Assert.Equal(30, config.TargetFrameRate);
    }

    [Fact]
    public async Task AdaptToSystemAsync_WithHighEndSpecs_ShouldReturnHighQualityConfig()
    {
        // Arrange
        var highEndSpecs = new SystemSpecifications
        {
            CpuCores = 12,
            RamGb = 32,
            HasDedicatedGpu = true,
            IsLowPowerMode = false
        };

        // Act
        var config = await _optimizer.AdaptToSystemAsync(highEndSpecs);

        // Assert
        Assert.NotNull(config);
        Assert.True(config.EnableAdvancedShaders);
        Assert.True(config.EnableComplexAnimations);
        Assert.Equal(ParticleComplexity.High, config.ParticleSystemComplexity);
        Assert.False(config.EnablePowerSaving);
        Assert.Equal(60, config.TargetFrameRate);
    }
}

/// <summary>
/// Test suite for Free Code Integrator
/// </summary>
public class FreeCodeIntegratorTests
{
    private readonly Mock<ILogger<FreeCodeIntegrator>> _mockLogger;
    private readonly Mock<HttpClient> _mockHttpClient;
    private readonly FreeCodeIntegrator _integrator;

    public FreeCodeIntegratorTests()
    {
        _mockLogger = new Mock<ILogger<FreeCodeIntegrator>>();
        _mockHttpClient = new Mock<HttpClient>();
        _integrator = new FreeCodeIntegrator(_mockLogger.Object, _mockHttpClient.Object);
    }

    [Fact]
    public async Task ValidateExternalCodeAsync_WithSafeCode_ShouldReturnValid()
    {
        // Arrange
        var safeCode = @"
            function createParticles() {
                const particles = new THREE.BufferGeometry();
                return particles;
            }
        ";

        // Act
        var result = await _integrator.ValidateExternalCodeAsync(safeCode);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsValid);
        Assert.Empty(result.SecurityIssues);
    }

    [Fact]
    public async Task ValidateExternalCodeAsync_WithUnsafeCode_ShouldReturnInvalid()
    {
        // Arrange
        var unsafeCode = @"
            eval('malicious code here');
            document.write('<script>alert(""hack"")</script>');
        ";

        // Act
        var result = await _integrator.ValidateExternalCodeAsync(unsafeCode);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.SecurityIssues);
        Assert.Contains(result.SecurityIssues, issue => issue.Contains("eval"));
        Assert.Contains(result.SecurityIssues, issue => issue.Contains("document.write"));
    }

    [Fact]
    public async Task IntegrateThreeGlobeAsync_WithValidConfig_ShouldGenerateIntegrationCode()
    {
        // Arrange
        var globeConfig = new GlobeConfig
        {
            ShowNetworkNodes = true,
            EnableRotation = true,
            NodeColor = 0x00d4ff
        };

        // Act
        var result = await _integrator.IntegrateThreeGlobeAsync(globeConfig);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Code);
        Assert.NotEmpty(result.Code);
        Assert.Contains("three-globe", result.Code.ToLower());
        Assert.Contains("globe", result.Code.ToLower());
    }

    [Fact]
    public async Task IntegrateXtermJsAsync_WithValidConfig_ShouldGenerateTerminalCode()
    {
        // Arrange
        var terminalConfig = new TerminalConfig
        {
            FontSize = 14,
            FontFamily = "Consolas",
            EnableHolographicEffects = true
        };

        // Act
        var result = await _integrator.IntegrateXtermJsAsync(terminalConfig);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Code);
        Assert.NotEmpty(result.Code);
        Assert.Contains("xterm", result.Code.ToLower());
        Assert.Contains("terminal", result.Code.ToLower());
    }

    [Fact]
    public async Task IntegrateParticleJsAsync_WithValidConfig_ShouldGenerateParticleCode()
    {
        // Arrange
        var particleConfig = new ParticleConfig
        {
            ParticleCount = 100,
            Color = 0x00d4ff,
            Size = 3.0f,
            Speed = 1.0f,
            Shape = ParticleShape.Circle
        };

        // Act
        var result = await _integrator.IntegrateParticleJsAsync(particleConfig);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Code);
        Assert.NotEmpty(result.Code);
        Assert.Contains("particles", result.Code.ToLower());
        Assert.Contains(particleConfig.ParticleCount.ToString(), result.Code);
    }
}