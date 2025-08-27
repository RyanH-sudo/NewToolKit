using NetToolkit.Modules.UiPolish.Models;
using NetToolkit.Modules.UiPolish.Interfaces;
using Microsoft.Extensions.Logging;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Concurrent;

namespace NetToolkit.Modules.UiPolish.Services;

/// <summary>
/// Advanced animation engine providing fluid UI transitions and dynamic effects
/// Orchestrates sophisticated easing, storyboards, and performance-optimized animations
/// </summary>
public class AnimationService : IAnimationEngine
{
    private readonly ILogger<AnimationService> _logger;
    private readonly ConcurrentDictionary<string, Storyboard> _activeAnimations;
    private readonly Dictionary<EasingType, IEasingFunction> _easingFunctions;
    private readonly Dictionary<AnimationType, TimeSpan> _defaultDurations;

    public AnimationService(ILogger<AnimationService> logger)
    {
        _logger = logger;
        _activeAnimations = new ConcurrentDictionary<string, Storyboard>();
        _easingFunctions = InitializeEasingFunctions();
        _defaultDurations = InitializeDefaultDurations();
    }

    public async Task AnimateTransitionAsync(string elementId, AnimationType animationType, int duration = 300, EasingType easing = EasingType.EaseOutQuart)
    {
        try
        {
            _logger.LogDebug("Animating {AnimationType} transition for element {ElementId} over {Duration}ms", animationType, elementId, duration);

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                var element = FindElementById(elementId);
                if (element == null)
                {
                    _logger.LogWarning("Element {ElementId} not found for animation", elementId);
                    return;
                }

                var storyboard = CreateTransitionStoryboard(element, animationType, TimeSpan.FromMilliseconds(duration), easing);
                
                // Stop any existing animation for this element
                await StopAnimationsAsync(elementId);
                
                // Start new animation
                _activeAnimations[elementId] = storyboard;
                
                storyboard.Completed += (s, e) =>
                {
                    _activeAnimations.TryRemove(elementId, out _);
                    _logger.LogDebug("Animation completed for element {ElementId}", elementId);
                };

                storyboard.Begin();
                _logger.LogInformation("✨ Animation sparkle activated: {AnimationType} flourish applied!", animationType);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to animate element {ElementId}", elementId);
        }
    }

    public async Task ExecuteAnimationSequenceAsync(AnimationSequence sequence)
    {
        try
        {
            _logger.LogInformation("Executing animation sequence: {SequenceName} with {StepCount} steps", sequence.Name, sequence.Steps.Count);

            var masterStoryboard = new Storyboard();
            var currentOffset = TimeSpan.Zero;

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                foreach (var step in sequence.Steps)
                {
                    var element = FindElementById(step.ElementId);
                    if (element == null)
                    {
                        _logger.LogWarning("Element {ElementId} not found in sequence step", step.ElementId);
                        continue;
                    }

                    var stepStoryboard = CreateTransitionStoryboard(element, step.AnimationType, step.Duration, step.Easing);
                    
                    // Set begin time for sequence coordination
                    stepStoryboard.BeginTime = currentOffset;
                    
                    // Add to master storyboard
                    foreach (Timeline timeline in stepStoryboard.Children)
                    {
                        masterStoryboard.Children.Add(timeline);
                    }

                    if (sequence.IsParallel)
                    {
                        // Parallel execution - don't advance offset
                    }
                    else
                    {
                        // Sequential execution - advance offset
                        currentOffset = currentOffset.Add(step.Duration);
                    }
                }

                masterStoryboard.Completed += (s, e) =>
                {
                    _logger.LogInformation("🎭 Animation sequence finale: {SequenceName} spectacle complete!", sequence.Name);
                };

                masterStoryboard.Begin();
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute animation sequence: {SequenceName}", sequence.Name);
        }
    }

    public async Task ApplyHoverAnimationAsync(string elementId, HoverEffectType hoverEffect)
    {
        try
        {
            _logger.LogDebug("Applying hover animation {HoverEffect} to element {ElementId}", hoverEffect, elementId);

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                var element = FindElementById(elementId);
                if (element == null)
                {
                    _logger.LogWarning("Element {ElementId} not found for hover animation", elementId);
                    return;
                }

                // Create hover enter and leave storyboards
                var hoverEnterStoryboard = CreateHoverEnterStoryboard(element, hoverEffect);
                var hoverLeaveStoryboard = CreateHoverLeaveStoryboard(element, hoverEffect);

                // Attach event handlers
                element.MouseEnter += (s, e) =>
                {
                    hoverLeaveStoryboard.Stop();
                    hoverEnterStoryboard.Begin();
                };

                element.MouseLeave += (s, e) =>
                {
                    hoverEnterStoryboard.Stop();
                    hoverLeaveStoryboard.Begin();
                };

                _logger.LogDebug("🎨 Hover magic woven: Element primed for {HoverEffect} enchantment!", hoverEffect);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply hover animation to element {ElementId}", elementId);
        }
    }

    public async Task CreateBreathingAnimationAsync(IEnumerable<string> elementIds, double breathingRate = 12.0)
    {
        try
        {
            var cycleDuration = TimeSpan.FromMinutes(1.0 / breathingRate);
            _logger.LogInformation("Creating breathing animation at {Rate} breaths/min ({Duration}ms cycle)", breathingRate, cycleDuration.TotalMilliseconds);

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                foreach (var elementId in elementIds)
                {
                    var element = FindElementById(elementId);
                    if (element == null)
                    {
                        _logger.LogWarning("Element {ElementId} not found for breathing animation", elementId);
                        continue;
                    }

                    var breathingStoryboard = CreateBreathingStoryboard(element, cycleDuration);
                    breathingStoryboard.RepeatBehavior = RepeatBehavior.Forever;
                    
                    _activeAnimations[$"{elementId}_breathing"] = breathingStoryboard;
                    breathingStoryboard.Begin();
                }

                _logger.LogInformation("🫁 Breathing life breathed: Elements now pulse with organic rhythm!");
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create breathing animation");
        }
    }

    public async Task StopAnimationsAsync(string elementId)
    {
        try
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                var keysToRemove = _activeAnimations.Keys
                    .Where(key => key.StartsWith(elementId))
                    .ToList();

                foreach (var key in keysToRemove)
                {
                    if (_activeAnimations.TryRemove(key, out var storyboard))
                    {
                        storyboard.Stop();
                        _logger.LogDebug("Stopped animation for {Key}", key);
                    }
                }

                if (keysToRemove.Any())
                {
                    _logger.LogDebug("⏹️ Animation pause: All effects for {ElementId} gracefully halted!", elementId);
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to stop animations for element {ElementId}", elementId);
        }
    }

    private Storyboard CreateTransitionStoryboard(FrameworkElement element, AnimationType animationType, TimeSpan duration, EasingType easingType)
    {
        var storyboard = new Storyboard();
        var easing = _easingFunctions[easingType];

        switch (animationType)
        {
            case AnimationType.FadeIn:
                var fadeInAnimation = new DoubleAnimation(0, 1, duration)
                {
                    EasingFunction = easing
                };
                Storyboard.SetTarget(fadeInAnimation, element);
                Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath("Opacity"));
                storyboard.Children.Add(fadeInAnimation);
                element.Opacity = 0;
                break;

            case AnimationType.FadeOut:
                var fadeOutAnimation = new DoubleAnimation(element.Opacity, 0, duration)
                {
                    EasingFunction = easing
                };
                Storyboard.SetTarget(fadeOutAnimation, element);
                Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath("Opacity"));
                storyboard.Children.Add(fadeOutAnimation);
                break;

            case AnimationType.SlideInFromLeft:
                var slideTransform = GetOrCreateTransform<TranslateTransform>(element);
                var slideAnimation = new DoubleAnimation(-element.ActualWidth, 0, duration)
                {
                    EasingFunction = easing
                };
                Storyboard.SetTarget(slideAnimation, slideTransform);
                Storyboard.SetTargetProperty(slideAnimation, new PropertyPath("X"));
                storyboard.Children.Add(slideAnimation);
                slideTransform.X = -element.ActualWidth;
                break;

            case AnimationType.ScaleUp:
                var scaleTransform = GetOrCreateTransform<ScaleTransform>(element);
                var scaleXAnimation = new DoubleAnimation(0.8, 1.0, duration)
                {
                    EasingFunction = easing
                };
                var scaleYAnimation = new DoubleAnimation(0.8, 1.0, duration)
                {
                    EasingFunction = easing
                };
                Storyboard.SetTarget(scaleXAnimation, scaleTransform);
                Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath("ScaleX"));
                Storyboard.SetTarget(scaleYAnimation, scaleTransform);
                Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath("ScaleY"));
                storyboard.Children.Add(scaleXAnimation);
                storyboard.Children.Add(scaleYAnimation);
                scaleTransform.ScaleX = 0.8;
                scaleTransform.ScaleY = 0.8;
                break;

            case AnimationType.Rotate:
                var rotateTransform = GetOrCreateTransform<RotateTransform>(element);
                var rotateAnimation = new DoubleAnimation(0, 360, duration)
                {
                    EasingFunction = easing
                };
                Storyboard.SetTarget(rotateAnimation, rotateTransform);
                Storyboard.SetTargetProperty(rotateAnimation, new PropertyPath("Angle"));
                storyboard.Children.Add(rotateAnimation);
                break;

            case AnimationType.Glow:
                CreateGlowEffect(element, storyboard, duration, easing);
                break;

            case AnimationType.Pulse:
                var pulseAnimation = new DoubleAnimation(1.0, 1.1, duration)
                {
                    EasingFunction = easing,
                    AutoReverse = true
                };
                var pulseTransform = GetOrCreateTransform<ScaleTransform>(element);
                Storyboard.SetTarget(pulseAnimation, pulseTransform);
                Storyboard.SetTargetProperty(pulseAnimation, new PropertyPath("ScaleX"));
                storyboard.Children.Add(pulseAnimation);
                
                var pulseYAnimation = new DoubleAnimation(1.0, 1.1, duration)
                {
                    EasingFunction = easing,
                    AutoReverse = true
                };
                Storyboard.SetTarget(pulseYAnimation, pulseTransform);
                Storyboard.SetTargetProperty(pulseYAnimation, new PropertyPath("ScaleY"));
                storyboard.Children.Add(pulseYAnimation);
                break;

            case AnimationType.Bounce:
                var bounceTransform = GetOrCreateTransform<TranslateTransform>(element);
                var bounceAnimation = new DoubleAnimation(0, -20, TimeSpan.FromMilliseconds(duration.TotalMilliseconds / 2))
                {
                    EasingFunction = new BounceEase { Bounces = 3, Bounciness = 2 },
                    AutoReverse = true
                };
                Storyboard.SetTarget(bounceAnimation, bounceTransform);
                Storyboard.SetTargetProperty(bounceAnimation, new PropertyPath("Y"));
                storyboard.Children.Add(bounceAnimation);
                break;
        }

        return storyboard;
    }

    private Storyboard CreateHoverEnterStoryboard(FrameworkElement element, HoverEffectType hoverEffect)
    {
        var storyboard = new Storyboard();
        var quickEasing = _easingFunctions[EasingType.EaseOutQuart];
        var duration = TimeSpan.FromMilliseconds(200);

        switch (hoverEffect)
        {
            case HoverEffectType.Glow:
                CreateGlowEffect(element, storyboard, duration, quickEasing);
                break;

            case HoverEffectType.Scale:
                var scaleTransform = GetOrCreateTransform<ScaleTransform>(element);
                var scaleAnimation = new DoubleAnimation(scaleTransform.ScaleX, 1.05, duration)
                {
                    EasingFunction = quickEasing
                };
                Storyboard.SetTarget(scaleAnimation, scaleTransform);
                Storyboard.SetTargetProperty(scaleAnimation, new PropertyPath("ScaleX"));
                storyboard.Children.Add(scaleAnimation);

                var scaleYAnimation = new DoubleAnimation(scaleTransform.ScaleY, 1.05, duration)
                {
                    EasingFunction = quickEasing
                };
                Storyboard.SetTarget(scaleYAnimation, scaleTransform);
                Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath("ScaleY"));
                storyboard.Children.Add(scaleYAnimation);
                break;

            case HoverEffectType.Lift:
                var liftTransform = GetOrCreateTransform<TranslateTransform>(element);
                var liftAnimation = new DoubleAnimation(liftTransform.Y, -5, duration)
                {
                    EasingFunction = quickEasing
                };
                Storyboard.SetTarget(liftAnimation, liftTransform);
                Storyboard.SetTargetProperty(liftAnimation, new PropertyPath("Y"));
                storyboard.Children.Add(liftAnimation);
                break;

            case HoverEffectType.Brighten:
                var brightenAnimation = new DoubleAnimation(element.Opacity, Math.Min(1.0, element.Opacity + 0.2), duration)
                {
                    EasingFunction = quickEasing
                };
                Storyboard.SetTarget(brightenAnimation, element);
                Storyboard.SetTargetProperty(brightenAnimation, new PropertyPath("Opacity"));
                storyboard.Children.Add(brightenAnimation);
                break;
        }

        return storyboard;
    }

    private Storyboard CreateHoverLeaveStoryboard(FrameworkElement element, HoverEffectType hoverEffect)
    {
        var storyboard = new Storyboard();
        var quickEasing = _easingFunctions[EasingType.EaseOutQuart];
        var duration = TimeSpan.FromMilliseconds(150);

        switch (hoverEffect)
        {
            case HoverEffectType.Scale:
                var scaleTransform = GetOrCreateTransform<ScaleTransform>(element);
                var scaleAnimation = new DoubleAnimation(scaleTransform.ScaleX, 1.0, duration)
                {
                    EasingFunction = quickEasing
                };
                Storyboard.SetTarget(scaleAnimation, scaleTransform);
                Storyboard.SetTargetProperty(scaleAnimation, new PropertyPath("ScaleX"));
                storyboard.Children.Add(scaleAnimation);

                var scaleYAnimation = new DoubleAnimation(scaleTransform.ScaleY, 1.0, duration)
                {
                    EasingFunction = quickEasing
                };
                Storyboard.SetTarget(scaleYAnimation, scaleTransform);
                Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath("ScaleY"));
                storyboard.Children.Add(scaleYAnimation);
                break;

            case HoverEffectType.Lift:
                var liftTransform = GetOrCreateTransform<TranslateTransform>(element);
                var liftAnimation = new DoubleAnimation(liftTransform.Y, 0, duration)
                {
                    EasingFunction = quickEasing
                };
                Storyboard.SetTarget(liftAnimation, liftTransform);
                Storyboard.SetTargetProperty(liftAnimation, new PropertyPath("Y"));
                storyboard.Children.Add(liftAnimation);
                break;

            case HoverEffectType.Brighten:
                var brightenAnimation = new DoubleAnimation(element.Opacity, Math.Max(0.8, element.Opacity - 0.2), duration)
                {
                    EasingFunction = quickEasing
                };
                Storyboard.SetTarget(brightenAnimation, element);
                Storyboard.SetTargetProperty(brightenAnimation, new PropertyPath("Opacity"));
                storyboard.Children.Add(brightenAnimation);
                break;
        }

        return storyboard;
    }

    private Storyboard CreateBreathingStoryboard(FrameworkElement element, TimeSpan cycleDuration)
    {
        var storyboard = new Storyboard();
        var scaleTransform = GetOrCreateTransform<ScaleTransform>(element);
        
        var breatheAnimation = new DoubleAnimation(1.0, 1.05, cycleDuration)
        {
            EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut },
            AutoReverse = true
        };
        
        Storyboard.SetTarget(breatheAnimation, scaleTransform);
        Storyboard.SetTargetProperty(breatheAnimation, new PropertyPath("ScaleX"));
        storyboard.Children.Add(breatheAnimation);
        
        var breatheYAnimation = new DoubleAnimation(1.0, 1.05, cycleDuration)
        {
            EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut },
            AutoReverse = true
        };
        
        Storyboard.SetTarget(breatheYAnimation, scaleTransform);
        Storyboard.SetTargetProperty(breatheYAnimation, new PropertyPath("ScaleY"));
        storyboard.Children.Add(breatheYAnimation);

        return storyboard;
    }

    private void CreateGlowEffect(FrameworkElement element, Storyboard storyboard, TimeSpan duration, IEasingFunction easing)
    {
        // Create drop shadow effect for glow simulation
        var dropShadow = new System.Windows.Media.Effects.DropShadowEffect
        {
            Color = Colors.Cyan,
            BlurRadius = 0,
            Opacity = 0,
            ShadowDepth = 0
        };
        
        element.Effect = dropShadow;

        var glowAnimation = new DoubleAnimation(0, 20, duration)
        {
            EasingFunction = easing
        };
        Storyboard.SetTarget(glowAnimation, dropShadow);
        Storyboard.SetTargetProperty(glowAnimation, new PropertyPath("BlurRadius"));
        storyboard.Children.Add(glowAnimation);

        var opacityAnimation = new DoubleAnimation(0, 0.8, duration)
        {
            EasingFunction = easing
        };
        Storyboard.SetTarget(opacityAnimation, dropShadow);
        Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));
        storyboard.Children.Add(opacityAnimation);
    }

    private T GetOrCreateTransform<T>(FrameworkElement element) where T : Transform, new()
    {
        var transformGroup = element.RenderTransform as TransformGroup;
        
        if (transformGroup == null)
        {
            transformGroup = new TransformGroup();
            element.RenderTransform = transformGroup;
            element.RenderTransformOrigin = new Point(0.5, 0.5);
        }

        var transform = transformGroup.Children.OfType<T>().FirstOrDefault();
        if (transform == null)
        {
            transform = new T();
            transformGroup.Children.Add(transform);
        }

        return transform;
    }

    private FrameworkElement? FindElementById(string elementId)
    {
        // In a real implementation, this would search the visual tree
        // For now, we'll simulate finding elements by name
        if (Application.Current?.MainWindow != null)
        {
            return Application.Current.MainWindow.FindName(elementId) as FrameworkElement;
        }
        return null;
    }

    private Dictionary<EasingType, IEasingFunction> InitializeEasingFunctions()
    {
        return new Dictionary<EasingType, IEasingFunction>
        {
            [EasingType.Linear] = new PowerEase { Power = 1, EasingMode = EasingMode.EaseInOut },
            [EasingType.EaseInQuad] = new QuadraticEase { EasingMode = EasingMode.EaseIn },
            [EasingType.EaseOutQuad] = new QuadraticEase { EasingMode = EasingMode.EaseOut },
            [EasingType.EaseInOutQuad] = new QuadraticEase { EasingMode = EasingMode.EaseInOut },
            [EasingType.EaseInCubic] = new CubicEase { EasingMode = EasingMode.EaseIn },
            [EasingType.EaseOutCubic] = new CubicEase { EasingMode = EasingMode.EaseOut },
            [EasingType.EaseInOutCubic] = new CubicEase { EasingMode = EasingMode.EaseInOut },
            [EasingType.EaseInQuart] = new QuarticEase { EasingMode = EasingMode.EaseIn },
            [EasingType.EaseOutQuart] = new QuarticEase { EasingMode = EasingMode.EaseOut },
            [EasingType.EaseInOutQuart] = new QuarticEase { EasingMode = EasingMode.EaseInOut },
            [EasingType.EaseInQuint] = new QuinticEase { EasingMode = EasingMode.EaseIn },
            [EasingType.EaseOutQuint] = new QuinticEase { EasingMode = EasingMode.EaseOut },
            [EasingType.EaseInOutQuint] = new QuinticEase { EasingMode = EasingMode.EaseInOut },
            [EasingType.EaseInSine] = new SineEase { EasingMode = EasingMode.EaseIn },
            [EasingType.EaseOutSine] = new SineEase { EasingMode = EasingMode.EaseOut },
            [EasingType.EaseInOutSine] = new SineEase { EasingMode = EasingMode.EaseInOut },
            [EasingType.EaseInBack] = new BackEase { EasingMode = EasingMode.EaseIn, Amplitude = 1.7 },
            [EasingType.EaseOutBack] = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 1.7 },
            [EasingType.EaseInOutBack] = new BackEase { EasingMode = EasingMode.EaseInOut, Amplitude = 1.7 },
            [EasingType.EaseInElastic] = new ElasticEase { EasingMode = EasingMode.EaseIn, Oscillations = 3, Springiness = 3 },
            [EasingType.EaseOutElastic] = new ElasticEase { EasingMode = EasingMode.EaseOut, Oscillations = 3, Springiness = 3 },
            [EasingType.EaseInOutElastic] = new ElasticEase { EasingMode = EasingMode.EaseInOut, Oscillations = 3, Springiness = 3 },
            [EasingType.EaseInBounce] = new BounceEase { EasingMode = EasingMode.EaseIn, Bounces = 3, Bounciness = 2 },
            [EasingType.EaseOutBounce] = new BounceEase { EasingMode = EasingMode.EaseOut, Bounces = 3, Bounciness = 2 },
            [EasingType.EaseInOutBounce] = new BounceEase { EasingMode = EasingMode.EaseInOut, Bounces = 3, Bounciness = 2 }
        };
    }

    private Dictionary<AnimationType, TimeSpan> InitializeDefaultDurations()
    {
        return new Dictionary<AnimationType, TimeSpan>
        {
            [AnimationType.FadeIn] = TimeSpan.FromMilliseconds(300),
            [AnimationType.FadeOut] = TimeSpan.FromMilliseconds(300),
            [AnimationType.SlideInFromLeft] = TimeSpan.FromMilliseconds(400),
            [AnimationType.SlideInFromRight] = TimeSpan.FromMilliseconds(400),
            [AnimationType.SlideInFromTop] = TimeSpan.FromMilliseconds(400),
            [AnimationType.SlideInFromBottom] = TimeSpan.FromMilliseconds(400),
            [AnimationType.SlideOutToLeft] = TimeSpan.FromMilliseconds(300),
            [AnimationType.SlideOutToRight] = TimeSpan.FromMilliseconds(300),
            [AnimationType.SlideOutToTop] = TimeSpan.FromMilliseconds(300),
            [AnimationType.SlideOutToBottom] = TimeSpan.FromMilliseconds(300),
            [AnimationType.ScaleUp] = TimeSpan.FromMilliseconds(250),
            [AnimationType.ScaleDown] = TimeSpan.FromMilliseconds(200),
            [AnimationType.Rotate] = TimeSpan.FromMilliseconds(600),
            [AnimationType.Glow] = TimeSpan.FromMilliseconds(400),
            [AnimationType.Pulse] = TimeSpan.FromMilliseconds(800),
            [AnimationType.Bounce] = TimeSpan.FromMilliseconds(600)
        };
    }
}