using NetToolkit.Modules.UiPolish.Models;
using NetToolkit.Modules.UiPolish.Interfaces;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Management;

namespace NetToolkit.Modules.UiPolish.Services;

/// <summary>
/// Performance optimizer for resource-conscious UI polish
/// Dynamically adjusts visual effects based on system capabilities and performance metrics
/// </summary>
public class PolishOptimizer : IPolishOptimizer
{
    private readonly ILogger<PolishOptimizer> _logger;
    private readonly PerformanceCounter? _cpuCounter;
    private readonly PerformanceCounter? _memoryCounter;
    private readonly Stopwatch _frameTimer;
    private readonly Queue<double> _frameTimeHistory;
    private readonly object _metricsLock = new();
    
    private int _targetFps = 60;
    private QualityLevel _currentQuality = QualityLevel.High;
    private DateTime _lastOptimization = DateTime.MinValue;
    private long _memoryBaseline = 0;

    public PolishOptimizer(ILogger<PolishOptimizer> logger)
    {
        _logger = logger;
        _frameTimer = Stopwatch.StartNew();
        _frameTimeHistory = new Queue<double>();
        
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                _memoryCounter = new PerformanceCounter("Memory", "Available MBytes");
            }
            
            _memoryBaseline = GC.GetTotalMemory(false);
            _logger.LogDebug("Performance monitoring initialized - baseline memory: {Memory} MB", _memoryBaseline / 1024 / 1024);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to initialize performance counters - using fallback monitoring");
        }
    }

    public async Task<SceneOptimizationResult> OptimizeSceneAsync(ThreeJsSceneConfig sceneConfig, int targetFps = 60)
    {
        try
        {
            _targetFps = targetFps;
            _logger.LogInformation("🎯 Optimizing scene for {TargetFps} fps target", targetFps);

            var optimizedConfig = new ThreeJsSceneConfig
            {
                ParticleCount = sceneConfig.ParticleCount,
                EnableShaders = sceneConfig.EnableShaders,
                EnablePostProcessing = sceneConfig.EnablePostProcessing,
                EnableShadows = sceneConfig.EnableShadows,
                TextureQuality = sceneConfig.TextureQuality,
                RenderDistance = sceneConfig.RenderDistance,
                EnableLevelOfDetail = sceneConfig.EnableLevelOfDetail
            };

            var result = new SceneOptimizationResult
            {
                OriginalConfig = sceneConfig,
                OptimizedConfig = optimizedConfig,
                OptimizationsApplied = new List<string>()
            };

            // Get current performance metrics
            var perfStatus = await MonitorPerformanceAsync();
            
            // Apply optimizations based on performance
            if (perfStatus.CurrentFps < targetFps * 0.8) // Below 80% of target
            {
                _logger.LogWarning("⚡ Performance below target ({Fps:F1} fps) - applying optimizations", perfStatus.CurrentFps);

                // Reduce particle count
                if (optimizedConfig.ParticleCount > 500)
                {
                    optimizedConfig.ParticleCount = Math.Max(250, optimizedConfig.ParticleCount / 2);
                    result.OptimizationsApplied.Add($"Reduced particle count to {optimizedConfig.ParticleCount}");
                    _logger.LogDebug("✂️ Particle count reduced for performance");
                }

                // Disable expensive effects if performance is poor
                if (perfStatus.CurrentFps < targetFps * 0.6) // Below 60% of target
                {
                    optimizedConfig.EnableShaders = false;
                    optimizedConfig.EnablePostProcessing = false;
                    optimizedConfig.EnableShadows = false;
                    result.OptimizationsApplied.Add("Disabled shaders, post-processing, and shadows");
                    _logger.LogDebug("🚫 Advanced effects disabled for performance");
                }

                // Reduce texture quality
                if (optimizedConfig.TextureQuality > TextureQuality.Medium)
                {
                    optimizedConfig.TextureQuality = TextureQuality.Medium;
                    result.OptimizationsApplied.Add("Reduced texture quality to Medium");
                }

                // Reduce render distance
                if (optimizedConfig.RenderDistance > 500)
                {
                    optimizedConfig.RenderDistance = Math.Max(200, optimizedConfig.RenderDistance / 2);
                    result.OptimizationsApplied.Add($"Reduced render distance to {optimizedConfig.RenderDistance}");
                }

                // Enable Level of Detail if not already enabled
                if (!optimizedConfig.EnableLevelOfDetail)
                {
                    optimizedConfig.EnableLevelOfDetail = true;
                    result.OptimizationsApplied.Add("Enabled Level of Detail optimization");
                }

                _currentQuality = QualityLevel.Medium;
            }
            else if (perfStatus.CurrentFps < targetFps * 0.9) // Below 90% of target - minor optimizations
            {
                if (optimizedConfig.ParticleCount > 1000)
                {
                    optimizedConfig.ParticleCount = 1000;
                    result.OptimizationsApplied.Add("Capped particle count at 1000");
                }

                if (optimizedConfig.TextureQuality > TextureQuality.High)
                {
                    optimizedConfig.TextureQuality = TextureQuality.High;
                    result.OptimizationsApplied.Add("Reduced texture quality to High");
                }

                _currentQuality = QualityLevel.High;
            }
            else
            {
                // Performance is good - potentially increase quality if system can handle it
                if (perfStatus.CurrentFps > targetFps * 1.2 && _currentQuality < QualityLevel.Ultra)
                {
                    _logger.LogDebug("🚀 Performance excellent - considering quality improvements");
                    _currentQuality = QualityLevel.Ultra;
                }
            }

            result.EstimatedPerformanceGain = CalculatePerformanceGain(sceneConfig, optimizedConfig);
            result.QualityLevel = _currentQuality;

            _logger.LogInformation("🎨 Scene optimization complete - {OptimizationCount} optimizations applied", 
                result.OptimizationsApplied.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to optimize scene");
            return new SceneOptimizationResult
            {
                OriginalConfig = sceneConfig,
                OptimizedConfig = sceneConfig,
                OptimizationsApplied = new List<string> { "Optimization failed - using original config" },
                EstimatedPerformanceGain = 0
            };
        }
    }

    public async Task<PerformanceStatus> MonitorPerformanceAsync()
    {
        try
        {
            lock (_metricsLock)
            {
                // Calculate FPS from frame time history
                var currentTime = _frameTimer.Elapsed.TotalMilliseconds;
                _frameTimeHistory.Enqueue(currentTime);

                // Keep only the last 60 frame times (1 second at 60fps)
                while (_frameTimeHistory.Count > 60)
                {
                    _frameTimeHistory.Dequeue();
                }
            }

            var fps = CalculateCurrentFps();
            var cpuUsage = GetCpuUsage();
            var memoryUsage = GetMemoryUsage();

            var status = new PerformanceStatus
            {
                CurrentFps = fps,
                CpuUsagePercent = cpuUsage,
                MemoryUsageMb = memoryUsage,
                IsPerformanceAcceptable = fps >= _targetFps * 0.8,
                QualityLevel = _currentQuality,
                LastUpdated = DateTime.UtcNow
            };

            // Log performance warnings
            if (fps < _targetFps * 0.6)
            {
                _logger.LogWarning("⚠️ Performance critical: {Fps:F1} fps (target: {Target})", fps, _targetFps);
            }
            else if (fps < _targetFps * 0.8)
            {
                _logger.LogDebug("📉 Performance below target: {Fps:F1} fps", fps);
            }

            if (cpuUsage > 80)
            {
                _logger.LogWarning("🔥 High CPU usage detected: {Usage:F1}%", cpuUsage);
            }

            if (memoryUsage > 500)
            {
                _logger.LogWarning("💾 High memory usage detected: {Usage:F1} MB", memoryUsage);
            }

            return status;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to monitor performance");
            return new PerformanceStatus
            {
                CurrentFps = 30, // Assume degraded performance
                CpuUsagePercent = 50,
                MemoryUsageMb = 200,
                IsPerformanceAcceptable = false,
                QualityLevel = QualityLevel.Low,
                LastUpdated = DateTime.UtcNow
            };
        }
    }

    public async Task<PolishConfig> AdaptToSystemAsync(SystemSpecifications systemSpecs)
    {
        try
        {
            _logger.LogInformation("🔧 Adapting polish configuration to system specifications");

            var config = new PolishConfig();

            // CPU-based adaptations
            if (systemSpecs.CpuCores >= 8)
            {
                config.EnableAdvancedShaders = true;
                config.EnableComplexAnimations = true;
                config.ParticleSystemComplexity = ParticleComplexity.High;
                _logger.LogDebug("💪 High-performance CPU detected - enabling advanced features");
            }
            else if (systemSpecs.CpuCores >= 4)
            {
                config.EnableAdvancedShaders = true;
                config.EnableComplexAnimations = false;
                config.ParticleSystemComplexity = ParticleComplexity.Medium;
                _logger.LogDebug("⚖️ Mid-range CPU detected - balanced configuration");
            }
            else
            {
                config.EnableAdvancedShaders = false;
                config.EnableComplexAnimations = false;
                config.ParticleSystemComplexity = ParticleComplexity.Low;
                _logger.LogDebug("🐌 Limited CPU detected - conservative configuration");
            }

            // RAM-based adaptations
            if (systemSpecs.RamGb >= 16)
            {
                config.TextureCacheSize = TextureCacheSize.Large;
                config.EnableHighResTextures = true;
            }
            else if (systemSpecs.RamGb >= 8)
            {
                config.TextureCacheSize = TextureCacheSize.Medium;
                config.EnableHighResTextures = false;
            }
            else
            {
                config.TextureCacheSize = TextureCacheSize.Small;
                config.EnableHighResTextures = false;
                _logger.LogDebug("💾 Limited RAM detected - reducing texture quality");
            }

            // GPU-based adaptations
            if (systemSpecs.HasDedicatedGpu)
            {
                config.EnableGpuParticles = true;
                config.EnableAdvancedLighting = true;
                config.MaxShaderComplexity = ShaderComplexity.High;
                _logger.LogDebug("🎮 Dedicated GPU detected - enabling GPU-accelerated features");
            }
            else
            {
                config.EnableGpuParticles = false;
                config.EnableAdvancedLighting = false;
                config.MaxShaderComplexity = ShaderComplexity.Low;
                _logger.LogDebug("📱 Integrated graphics detected - using CPU-based rendering");
            }

            // Power mode adaptations
            if (systemSpecs.IsLowPowerMode)
            {
                config.TargetFrameRate = 30;
                config.EnablePowerSaving = true;
                config.ReduceBackgroundAnimations = true;
                _logger.LogInformation("🔋 Low power mode - reducing performance targets");
            }
            else
            {
                config.TargetFrameRate = 60;
                config.EnablePowerSaving = false;
                config.ReduceBackgroundAnimations = false;
            }

            // Performance-based quality level
            var perfStatus = await MonitorPerformanceAsync();
            if (!perfStatus.IsPerformanceAcceptable)
            {
                config.QualityLevel = QualityLevel.Low;
                config.EnableAdvancedShaders = false;
                config.ParticleSystemComplexity = ParticleComplexity.Low;
                _logger.LogWarning("⚡ Poor performance detected - forcing low quality mode");
            }

            _lastOptimization = DateTime.UtcNow;
            _logger.LogInformation("✅ System adaptation complete - Configuration optimized for hardware");

            return config;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to adapt to system specifications");
            
            // Return conservative fallback configuration
            return new PolishConfig
            {
                QualityLevel = QualityLevel.Low,
                EnableAdvancedShaders = false,
                EnableComplexAnimations = false,
                ParticleSystemComplexity = ParticleComplexity.Low,
                TargetFrameRate = 30,
                EnablePowerSaving = true
            };
        }
    }

    public async Task<AssetLoadingProgress> LazyLoadAssetsAsync(AssetManifest assetManifest)
    {
        try
        {
            _logger.LogInformation("📦 Starting lazy loading of {AssetCount} assets", assetManifest.Assets.Count);

            var progress = new AssetLoadingProgress
            {
                TotalAssets = assetManifest.Assets.Count,
                LoadedAssets = 0,
                FailedAssets = 0,
                CurrentAsset = null,
                LoadedAssetPaths = new List<string>(),
                FailedAssetPaths = new List<string>()
            };

            // Sort assets by priority
            var sortedAssets = assetManifest.Assets
                .OrderBy(a => GetAssetPriority(a.Type))
                .ThenBy(a => a.SizeBytes)
                .ToList();

            foreach (var asset in sortedAssets)
            {
                try
                {
                    progress.CurrentAsset = asset.Path;
                    _logger.LogDebug("Loading asset: {AssetPath}", asset.Path);

                    // Simulate loading with appropriate delay based on size
                    var loadTimeMs = Math.Min(100, asset.SizeBytes / 10000); // Max 100ms
                    await Task.Delay((int)loadTimeMs);

                    // Check if we need to throttle loading based on performance
                    var perfStatus = await MonitorPerformanceAsync();
                    if (perfStatus.CpuUsagePercent > 70)
                    {
                        _logger.LogDebug("🐌 Throttling asset loading due to high CPU usage");
                        await Task.Delay(50);
                    }

                    progress.LoadedAssets++;
                    progress.LoadedAssetPaths.Add(asset.Path);
                    progress.ProgressPercentage = (float)progress.LoadedAssets / progress.TotalAssets * 100;

                    // Yield control periodically
                    if (progress.LoadedAssets % 10 == 0)
                    {
                        await Task.Yield();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to load asset: {AssetPath}", asset.Path);
                    progress.FailedAssets++;
                    progress.FailedAssetPaths.Add(asset.Path);
                }
            }

            progress.IsComplete = true;
            progress.CurrentAsset = null;

            _logger.LogInformation("📁 Asset loading complete - {Loaded}/{Total} loaded successfully", 
                progress.LoadedAssets, progress.TotalAssets);

            return progress;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to lazy load assets");
            return new AssetLoadingProgress
            {
                TotalAssets = assetManifest.Assets.Count,
                LoadedAssets = 0,
                FailedAssets = assetManifest.Assets.Count,
                IsComplete = true,
                ProgressPercentage = 0
            };
        }
    }

    private double CalculateCurrentFps()
    {
        lock (_metricsLock)
        {
            if (_frameTimeHistory.Count < 2)
                return 60; // Default assumption

            var frameTimes = _frameTimeHistory.ToArray();
            var totalTime = frameTimes[^1] - frameTimes[0];
            var frameCount = frameTimes.Length - 1;

            if (totalTime <= 0)
                return 60;

            return 1000.0 * frameCount / totalTime; // Convert to FPS
        }
    }

    private float GetCpuUsage()
    {
        try
        {
            if (_cpuCounter != null)
            {
                return _cpuCounter.NextValue();
            }

            // Fallback: estimate based on process CPU time
            using var currentProcess = Process.GetCurrentProcess();
            var cpuTime = currentProcess.TotalProcessorTime.TotalMilliseconds;
            var realTime = (DateTime.UtcNow - currentProcess.StartTime).TotalMilliseconds;
            
            return Math.Min(100, (float)(cpuTime / realTime / Environment.ProcessorCount * 100));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get CPU usage");
            return 25; // Conservative estimate
        }
    }

    private float GetMemoryUsage()
    {
        try
        {
            var currentMemory = GC.GetTotalMemory(false);
            return (currentMemory - _memoryBaseline) / 1024.0f / 1024.0f; // MB
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get memory usage");
            return 100; // Conservative estimate
        }
    }

    private float CalculatePerformanceGain(ThreeJsSceneConfig original, ThreeJsSceneConfig optimized)
    {
        float gain = 0;

        // Calculate estimated gains from various optimizations
        if (optimized.ParticleCount < original.ParticleCount)
        {
            var reduction = 1.0f - (float)optimized.ParticleCount / original.ParticleCount;
            gain += reduction * 30; // Particle reduction can provide significant gains
        }

        if (!optimized.EnableShaders && original.EnableShaders)
            gain += 20; // Shader disabling provides major gains

        if (!optimized.EnablePostProcessing && original.EnablePostProcessing)
            gain += 15;

        if (!optimized.EnableShadows && original.EnableShadows)
            gain += 10;

        if (optimized.TextureQuality < original.TextureQuality)
            gain += 8;

        if (optimized.RenderDistance < original.RenderDistance)
        {
            var reduction = 1.0f - (float)optimized.RenderDistance / original.RenderDistance;
            gain += reduction * 12;
        }

        return Math.Min(100, gain); // Cap at 100% gain
    }

    private int GetAssetPriority(AssetType assetType)
    {
        return assetType switch
        {
            AssetType.Shader => 1, // Load shaders first
            AssetType.Texture => 2,
            AssetType.Model => 3,
            AssetType.Audio => 4,
            AssetType.Font => 5,
            AssetType.Configuration => 6,
            _ => 10
        };
    }

    public void Dispose()
    {
        try
        {
            _cpuCounter?.Dispose();
            _memoryCounter?.Dispose();
            _frameTimer.Stop();
            _logger.LogDebug("Polish optimizer disposed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during polish optimizer disposal");
        }
    }
}