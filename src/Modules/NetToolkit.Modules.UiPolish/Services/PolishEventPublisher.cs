using NetToolkit.Modules.UiPolish.Models;
using NetToolkit.Modules.UiPolish.Interfaces;
using NetToolkit.Core.Interfaces;
using Microsoft.Extensions.Logging;
using MediatR;

namespace NetToolkit.Modules.UiPolish.Services;

/// <summary>
/// Polish event publisher for UI enhancement notifications
/// Integrates with NetToolkit's event system to coordinate polish activities across modules
/// </summary>
public class PolishEventPublisher : IPolishEventPublisher
{
    private readonly ILogger<PolishEventPublisher> _logger;
    private readonly IMediator? _mediator;
    private readonly IEventBus? _eventBus;

    public PolishEventPublisher(
        ILogger<PolishEventPublisher> logger, 
        IMediator? mediator = null, 
        IEventBus? eventBus = null)
    {
        _logger = logger;
        _mediator = mediator;
        _eventBus = eventBus;
    }

    public async Task PublishThemeAppliedAsync(string themeName, DateTime appliedAt)
    {
        try
        {
            _logger.LogDebug("Publishing theme applied event: {ThemeName}", themeName);

            var eventData = new ThemeAppliedEvent
            {
                ThemeName = themeName,
                AppliedAt = appliedAt,
                EventId = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow
            };

            await PublishEventAsync("UiPolish.ThemeApplied", eventData);
            
            _logger.LogInformation("🎭 Theme applied event published: {ThemeName} - Visual transformation broadcasted!", themeName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish theme applied event for {ThemeName}", themeName);
        }
    }

    public async Task PublishEnhancementLoadedAsync(ComponentType componentType, string enhancementType, bool success)
    {
        try
        {
            _logger.LogDebug("Publishing enhancement loaded event: {ComponentType} - {EnhancementType} (Success: {Success})", 
                componentType, enhancementType, success);

            var eventData = new EnhancementLoadedEvent
            {
                ComponentType = componentType,
                EnhancementType = enhancementType,
                Success = success,
                EventId = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow
            };

            await PublishEventAsync("UiPolish.EnhancementLoaded", eventData);
            
            var statusEmoji = success ? "✨" : "⚠️";
            var statusText = success ? "achieved" : "encountered issues";
            _logger.LogInformation("{Emoji} Enhancement event published: {ComponentType} {EnhancementType} - {Status}!", 
                statusEmoji, componentType, enhancementType, statusText);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish enhancement loaded event for {ComponentType}", componentType);
        }
    }

    public async Task PublishTipAddedAsync(string elementId, string tipText)
    {
        try
        {
            _logger.LogTrace("Publishing tip added event: {ElementId}", elementId);

            var eventData = new TipAddedEvent
            {
                ElementId = elementId,
                TipText = tipText,
                EventId = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow
            };

            await PublishEventAsync("UiPolish.TipAdded", eventData);
            
            _logger.LogDebug("💡 Tip added event published: {ElementId} - Wit dispersed successfully!", elementId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish tip added event for {ElementId}", elementId);
        }
    }

    public async Task PublishAnimationCompletedAsync(string elementId, AnimationType animationType, TimeSpan duration)
    {
        try
        {
            _logger.LogTrace("Publishing animation completed event: {ElementId} - {AnimationType}", elementId, animationType);

            var eventData = new AnimationCompletedEvent
            {
                ElementId = elementId,
                AnimationType = animationType,
                Duration = duration,
                EventId = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow
            };

            await PublishEventAsync("UiPolish.AnimationCompleted", eventData);
            
            _logger.LogTrace("🎭 Animation completed event published: {ElementId} {AnimationType} ({Duration}ms)", 
                elementId, animationType, duration.TotalMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to publish animation completed event for {ElementId}", elementId);
        }
    }

    public async Task PublishPerformanceMetricsAsync(PolishPerformanceMetrics metrics)
    {
        try
        {
            _logger.LogTrace("Publishing performance metrics event");

            var eventData = new PerformanceMetricsEvent
            {
                Metrics = metrics,
                EventId = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow
            };

            await PublishEventAsync("UiPolish.PerformanceMetrics", eventData);
            
            if (metrics.AverageFrameRate < 30)
            {
                _logger.LogWarning("⚡ Performance metrics published - Low framerate detected: {Fps:F1} fps", metrics.AverageFrameRate);
            }
            else
            {
                _logger.LogTrace("📊 Performance metrics published: {Fps:F1} fps, {Memory:F1} MB", 
                    metrics.AverageFrameRate, metrics.MemoryUsageMb);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish performance metrics event");
        }
    }

    private async Task PublishEventAsync(string eventName, object eventData)
    {
        try
        {
            // Try MediatR first if available
            if (_mediator != null && eventData is INotification notification)
            {
                await _mediator.Publish(notification);
                _logger.LogTrace("Event published via MediatR: {EventName}", eventName);
                return;
            }

            // Fall back to EventBus if available
            if (_eventBus != null)
            {
                await _eventBus.PublishAsync(eventName, eventData);
                _logger.LogTrace("Event published via EventBus: {EventName}", eventName);
                return;
            }

            // Log as a fallback if no event system is available
            _logger.LogInformation("📢 Event: {EventName} - {EventData}", eventName, 
                System.Text.Json.JsonSerializer.Serialize(eventData, new System.Text.Json.JsonSerializerOptions 
                { 
                    WriteIndented = false 
                }));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to publish event via primary systems: {EventName}", eventName);
            
            // Final fallback - just log the event
            _logger.LogInformation("📢 Event (fallback): {EventName}", eventName);
        }
    }
}

/// <summary>
/// Event data structures for UI Polish events
/// </summary>
public class ThemeAppliedEvent : INotification
{
    public required string ThemeName { get; set; }
    public DateTime AppliedAt { get; set; }
    public Guid EventId { get; set; }
    public DateTime Timestamp { get; set; }
}

public class EnhancementLoadedEvent : INotification
{
    public ComponentType ComponentType { get; set; }
    public required string EnhancementType { get; set; }
    public bool Success { get; set; }
    public Guid EventId { get; set; }
    public DateTime Timestamp { get; set; }
}

public class TipAddedEvent : INotification
{
    public required string ElementId { get; set; }
    public required string TipText { get; set; }
    public Guid EventId { get; set; }
    public DateTime Timestamp { get; set; }
}

public class AnimationCompletedEvent : INotification
{
    public required string ElementId { get; set; }
    public AnimationType AnimationType { get; set; }
    public TimeSpan Duration { get; set; }
    public Guid EventId { get; set; }
    public DateTime Timestamp { get; set; }
}

public class PerformanceMetricsEvent : INotification
{
    public required PolishPerformanceMetrics Metrics { get; set; }
    public Guid EventId { get; set; }
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Performance metrics for UI polish operations
/// </summary>
public class PolishPerformanceMetrics
{
    public float AverageFrameRate { get; set; }
    public float MemoryUsageMb { get; set; }
    public float CpuUsagePercent { get; set; }
    public int ActiveAnimations { get; set; }
    public int ActiveShaders { get; set; }
    public int ParticleCount { get; set; }
    public DateTime MeasuredAt { get; set; }
    public TimeSpan MeasurementDuration { get; set; }
    public QualityLevel CurrentQualityLevel { get; set; }
    
    public Dictionary<string, object> AdditionalMetrics { get; set; } = new();
    
    /// <summary>
    /// Calculate overall performance score (0-100)
    /// </summary>
    public float GetPerformanceScore()
    {
        var fpsScore = Math.Min(100, AverageFrameRate / 60.0f * 100);
        var memoryScore = Math.Max(0, 100 - (MemoryUsageMb / 500.0f * 100)); // 500MB as baseline
        var cpuScore = Math.Max(0, 100 - CpuUsagePercent);
        
        return (fpsScore + memoryScore + cpuScore) / 3.0f;
    }
    
    /// <summary>
    /// Determine if performance is acceptable
    /// </summary>
    public bool IsPerformanceAcceptable => GetPerformanceScore() > 70;
    
    /// <summary>
    /// Get performance status description
    /// </summary>
    public string GetPerformanceStatus()
    {
        var score = GetPerformanceScore();
        return score switch
        {
            >= 90 => "Excellent - Visual effects running at peak performance",
            >= 80 => "Good - Smooth operation with all enhancements active",
            >= 70 => "Acceptable - Minor optimizations may improve experience",
            >= 60 => "Fair - Consider reducing quality settings",
            >= 50 => "Poor - Significant optimizations recommended",
            _ => "Critical - Immediate performance attention required"
        };
    }
}