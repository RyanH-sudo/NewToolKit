using NetToolkit.Modules.UiPolish.Models;
using NetToolkit.Modules.UiPolish.Interfaces;
using Microsoft.Extensions.Logging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Collections.Concurrent;

namespace NetToolkit.Modules.UiPolish.Services;

/// <summary>
/// Witty tip management system providing delightful hover experiences
/// Orchestrates contextual, humorous, and informative tooltip interactions
/// </summary>
public class TipManager : ITipManager
{
    private readonly ILogger<TipManager> _logger;
    private readonly ConcurrentDictionary<string, ToolTip> _activeTips;
    private readonly Dictionary<ComponentType, List<string>> _contextualTips;
    private readonly Random _random;

    public TipManager(ILogger<TipManager> logger)
    {
        _logger = logger;
        _activeTips = new ConcurrentDictionary<string, ToolTip>();
        _contextualTips = InitializeContextualTips();
        _random = new Random();
    }

    public async Task AddHoverTipAsync(string elementId, string tipText, TipStyle style = TipStyle.Default)
    {
        try
        {
            _logger.LogDebug("Adding hover tip to element {ElementId}: {TipText}", elementId, tipText);

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                var element = FindElementById(elementId);
                if (element == null)
                {
                    _logger.LogWarning("Element {ElementId} not found for tip attachment", elementId);
                    return;
                }

                var tooltip = CreateStyledTooltip(tipText, style);
                element.ToolTip = tooltip;
                
                _activeTips[elementId] = tooltip;

                // Add event handlers for enhanced tip behavior
                element.MouseEnter += async (s, e) =>
                {
                    await OnTipShowAsync(elementId, tipText);
                };

                element.MouseLeave += async (s, e) =>
                {
                    await OnTipHideAsync(elementId);
                };

                _logger.LogDebug("✨ Wit woven: {ElementId} now sparkles with humor!", elementId);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add hover tip to element {ElementId}", elementId);
        }
    }

    public async Task AddBulkHoverTipsAsync(Dictionary<string, string> tips, TipStyle style = TipStyle.Default)
    {
        try
        {
            _logger.LogInformation("Adding {Count} bulk hover tips", tips.Count);

            var tasks = tips.Select(kvp => AddHoverTipAsync(kvp.Key, kvp.Value, style));
            await Task.WhenAll(tasks);

            _logger.LogInformation("🎭 Mass wit deployment complete: {Count} elements now brimming with charm!", tips.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add bulk hover tips");
        }
    }

    public async Task UpdateTipTextAsync(string elementId, string newTipText)
    {
        try
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                if (_activeTips.TryGetValue(elementId, out var tooltip))
                {
                    tooltip.Content = newTipText;
                    _logger.LogDebug("📝 Tip text updated for {ElementId}: {NewText}", elementId, newTipText);
                }
                else
                {
                    _logger.LogWarning("No active tip found for element {ElementId}", elementId);
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update tip text for element {ElementId}", elementId);
        }
    }

    public async Task RemoveTipAsync(string elementId)
    {
        try
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                var element = FindElementById(elementId);
                if (element != null)
                {
                    element.ToolTip = null;
                }

                _activeTips.TryRemove(elementId, out _);
                _logger.LogDebug("🗑️ Tip removed: {ElementId} returns to mundane existence", elementId);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove tip from element {ElementId}", elementId);
        }
    }

    public async Task<string> GetRandomTipForComponentAsync(ComponentType componentType)
    {
        try
        {
            if (_contextualTips.TryGetValue(componentType, out var tips) && tips.Any())
            {
                var randomTip = tips[_random.Next(tips.Count)];
                _logger.LogDebug("🎲 Random tip selected for {ComponentType}: {Tip}", componentType, randomTip);
                return randomTip;
            }

            // Fallback to generic tips
            var genericTips = WittyTips.GetGenericTips();
            var fallbackTip = genericTips[_random.Next(genericTips.Count)];
            _logger.LogDebug("🎯 Fallback tip for {ComponentType}: {Tip}", componentType, fallbackTip);
            return fallbackTip;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get random tip for component type {ComponentType}", componentType);
            return "Hover achieved - tip delivery pending...";
        }
    }

    public async Task ApplyContextualTipsAsync(ComponentType componentType, FrameworkElement containerElement)
    {
        try
        {
            _logger.LogInformation("Applying contextual tips for {ComponentType}", componentType);

            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                var interactiveElements = FindInteractiveElements(containerElement);
                var componentTips = _contextualTips.GetValueOrDefault(componentType, new List<string>());
                
                if (!componentTips.Any())
                {
                    componentTips = WittyTips.GetGenericTips();
                }

                foreach (var element in interactiveElements)
                {
                    var elementName = element.Name ?? element.GetType().Name;
                    var randomTip = componentTips[_random.Next(componentTips.Count)];
                    
                    await AddHoverTipAsync(elementName, randomTip, TipStyle.Contextual);
                }

                _logger.LogInformation("🌟 Contextual wit dispersed: {Count} elements enhanced for {ComponentType}", 
                    interactiveElements.Count, componentType);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply contextual tips for {ComponentType}", componentType);
        }
    }

    public Task<Dictionary<string, string>> GetActiveTooltipsAsync()
    {
        try
        {
            var activeTooltips = new Dictionary<string, string>();
            
            foreach (var kvp in _activeTips)
            {
                if (kvp.Value.Content is string content)
                {
                    activeTooltips[kvp.Key] = content;
                }
            }

            _logger.LogDebug("📊 Retrieved {Count} active tooltips", activeTooltips.Count);
            return Task.FromResult(activeTooltips);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get active tooltips");
            return Task.FromResult(new Dictionary<string, string>());
        }
    }

    public async Task ShowTooltipAsync(string elementId, string text, TimeSpan duration)
    {
        try
        {
            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                var element = FindElementById(elementId);
                if (element == null)
                {
                    _logger.LogWarning("Element {ElementId} not found for tooltip display", elementId);
                    return;
                }

                var tooltip = CreateStyledTooltip(text, TipStyle.Temporary);
                element.ToolTip = tooltip;
                
                // Force show tooltip
                if (tooltip is ToolTip toolTip)
                {
                    toolTip.IsOpen = true;
                }

                // Auto-hide after duration
                await Task.Delay(duration);
                
                if (tooltip is ToolTip autoHideToolTip)
                {
                    autoHideToolTip.IsOpen = false;
                }
                
                element.ToolTip = null;

                _logger.LogDebug("⏰ Temporary tooltip shown and hidden for {ElementId}", elementId);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to show temporary tooltip for element {ElementId}", elementId);
        }
    }

    private ToolTip CreateStyledTooltip(string text, TipStyle style)
    {
        var tooltip = new ToolTip
        {
            Content = text,
            HasDropShadow = true,
            Placement = PlacementMode.Top,
            ShowDuration = 5000
        };

        // Apply styling based on tip style
        switch (style)
        {
            case TipStyle.Default:
                ApplyDefaultStyling(tooltip);
                break;
            case TipStyle.Witty:
                ApplyWittyStyling(tooltip);
                break;
            case TipStyle.Technical:
                ApplyTechnicalStyling(tooltip);
                break;
            case TipStyle.Warning:
                ApplyWarningStyling(tooltip);
                break;
            case TipStyle.Success:
                ApplySuccessStyling(tooltip);
                break;
            case TipStyle.Contextual:
                ApplyContextualStyling(tooltip);
                break;
            case TipStyle.Temporary:
                ApplyTemporaryStyling(tooltip);
                break;
        }

        return tooltip;
    }

    private void ApplyDefaultStyling(ToolTip tooltip)
    {
        tooltip.Background = new SolidColorBrush(Color.FromRgb(26, 26, 46)); // Midnight blue
        tooltip.Foreground = new SolidColorBrush(Color.FromRgb(196, 196, 196)); // Silver
        tooltip.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 212, 255)); // Neon cyan
        tooltip.BorderThickness = new Thickness(1);
        tooltip.Padding = new Thickness(8, 4, 8, 4);
        tooltip.FontFamily = new FontFamily("Segoe UI");
        tooltip.FontSize = 12;
    }

    private void ApplyWittyStyling(ToolTip tooltip)
    {
        ApplyDefaultStyling(tooltip);
        tooltip.FontWeight = FontWeights.Medium;
        tooltip.FontStyle = FontStyles.Italic;
        var gradient = new LinearGradientBrush();
        gradient.GradientStops.Add(new GradientStop(Color.FromRgb(0, 212, 255), 0));
        gradient.GradientStops.Add(new GradientStop(Color.FromRgb(0, 150, 200), 1));
        tooltip.Background = gradient;
    }

    private void ApplyTechnicalStyling(ToolTip tooltip)
    {
        ApplyDefaultStyling(tooltip);
        tooltip.FontFamily = new FontFamily("Consolas, 'Courier New'");
        tooltip.FontSize = 11;
        tooltip.Background = new SolidColorBrush(Color.FromRgb(20, 20, 20));
        tooltip.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 255, 127));
    }

    private void ApplyWarningStyling(ToolTip tooltip)
    {
        ApplyDefaultStyling(tooltip);
        tooltip.Background = new SolidColorBrush(Color.FromRgb(255, 193, 7));
        tooltip.Foreground = new SolidColorBrush(Color.FromRgb(33, 37, 41));
        tooltip.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 143, 0));
        tooltip.FontWeight = FontWeights.SemiBold;
    }

    private void ApplySuccessStyling(ToolTip tooltip)
    {
        ApplyDefaultStyling(tooltip);
        tooltip.Background = new SolidColorBrush(Color.FromRgb(40, 167, 69));
        tooltip.Foreground = new SolidColorBrush(Colors.White);
        tooltip.BorderBrush = new SolidColorBrush(Color.FromRgb(34, 139, 58));
        tooltip.FontWeight = FontWeights.Medium;
    }

    private void ApplyContextualStyling(ToolTip tooltip)
    {
        ApplyDefaultStyling(tooltip);
        tooltip.Opacity = 0.95;
        tooltip.Background = new SolidColorBrush(Color.FromRgb(18, 18, 35));
        var glowEffect = new System.Windows.Media.Effects.DropShadowEffect
        {
            Color = Color.FromRgb(0, 212, 255),
            BlurRadius = 10,
            ShadowDepth = 0,
            Opacity = 0.6
        };
        tooltip.Effect = glowEffect;
    }

    private void ApplyTemporaryStyling(ToolTip tooltip)
    {
        ApplyDefaultStyling(tooltip);
        tooltip.Background = new SolidColorBrush(Color.FromRgb(50, 50, 80));
        tooltip.FontWeight = FontWeights.Bold;
        tooltip.FontSize = 14;
    }

    private async Task OnTipShowAsync(string elementId, string tipText)
    {
        _logger.LogTrace("Tip shown for {ElementId}: {TipText}", elementId, tipText);
        // Could emit events here for analytics or other integrations
    }

    private async Task OnTipHideAsync(string elementId)
    {
        _logger.LogTrace("Tip hidden for {ElementId}", elementId);
        // Could emit events here for analytics or other integrations
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

    private List<FrameworkElement> FindInteractiveElements(FrameworkElement container)
    {
        var interactiveElements = new List<FrameworkElement>();
        
        if (container is Panel panel)
        {
            foreach (UIElement child in panel.Children)
            {
                if (child is FrameworkElement frameworkElement)
                {
                    if (IsInteractiveElement(frameworkElement))
                    {
                        interactiveElements.Add(frameworkElement);
                    }
                    
                    // Recursively search children
                    interactiveElements.AddRange(FindInteractiveElements(frameworkElement));
                }
            }
        }
        else if (container is ContentControl contentControl && contentControl.Content is FrameworkElement contentElement)
        {
            if (IsInteractiveElement(contentElement))
            {
                interactiveElements.Add(contentElement);
            }
            
            interactiveElements.AddRange(FindInteractiveElements(contentElement));
        }

        return interactiveElements;
    }

    private bool IsInteractiveElement(FrameworkElement element)
    {
        return element is ButtonBase ||
               element is TextBox ||
               element is ComboBox ||
               element is Slider ||
               element is CheckBox ||
               element is RadioButton ||
               element is ListBox ||
               element is TreeView ||
               element is MenuItem ||
               element.IsMouseOver;
    }

    private Dictionary<ComponentType, List<string>> InitializeContextualTips()
    {
        return new Dictionary<ComponentType, List<string>>
        {
            [ComponentType.Terminal] = WittyTips.GetTerminalTips(),
            [ComponentType.Topography] = WittyTips.GetTopographyTips(),
            [ComponentType.Dashboard] = WittyTips.GetDashboardTips(),
            [ComponentType.Scanner] = WittyTips.GetScannerTips(),
            [ComponentType.AiOrb] = WittyTips.GetAiOrbTips(),
            [ComponentType.Education] = WittyTips.GetEducationTips(),
            [ComponentType.Admin] = WittyTips.GetAdminTips(),
            [ComponentType.Generic] = WittyTips.GetGenericTips()
        };
    }
}

/// <summary>
/// Tip manager interface for hover tooltip functionality
/// </summary>
public interface ITipManager
{
    /// <summary>
    /// Add a hover tip to a specific UI element
    /// </summary>
    Task AddHoverTipAsync(string elementId, string tipText, TipStyle style = TipStyle.Default);
    
    /// <summary>
    /// Add multiple hover tips in bulk
    /// </summary>
    Task AddBulkHoverTipsAsync(Dictionary<string, string> tips, TipStyle style = TipStyle.Default);
    
    /// <summary>
    /// Update the text of an existing tip
    /// </summary>
    Task UpdateTipTextAsync(string elementId, string newTipText);
    
    /// <summary>
    /// Remove a hover tip from an element
    /// </summary>
    Task RemoveTipAsync(string elementId);
    
    /// <summary>
    /// Get a random contextual tip for a component type
    /// </summary>
    Task<string> GetRandomTipForComponentAsync(ComponentType componentType);
    
    /// <summary>
    /// Apply contextual tips to all interactive elements in a container
    /// </summary>
    Task ApplyContextualTipsAsync(ComponentType componentType, FrameworkElement containerElement);
    
    /// <summary>
    /// Get all currently active tooltips
    /// </summary>
    Task<Dictionary<string, string>> GetActiveTooltipsAsync();
    
    /// <summary>
    /// Show a temporary tooltip for a specific duration
    /// </summary>
    Task ShowTooltipAsync(string elementId, string text, TimeSpan duration);
}

/// <summary>
/// Styling options for tooltips
/// </summary>
public enum TipStyle
{
    Default,
    Witty,
    Technical,
    Warning,
    Success,
    Contextual,
    Temporary
}