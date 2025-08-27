using NetToolkit.Modules.AiOrb.Models;
using System.Drawing;

namespace NetToolkit.Modules.AiOrb.Interfaces;

/// <summary>
/// WPF overlay handler for the floating AI orb widget
/// Manages the visual presence and interactions of the orb on desktop
/// </summary>
public interface IOrbOverlayHandler
{
    /// <summary>
    /// Show the floating orb on the desktop
    /// </summary>
    /// <param name="position">Initial position for the orb, null for saved position</param>
    /// <returns>Success status</returns>
    Task<bool> ShowOrbAsync(Point? position = null);

    /// <summary>
    /// Hide the floating orb from desktop
    /// </summary>
    /// <returns>Success status</returns>
    Task<bool> HideOrbAsync();

    /// <summary>
    /// Toggle orb visibility state
    /// </summary>
    /// <returns>New visibility state</returns>
    Task<bool> ToggleVisibilityAsync();

    /// <summary>
    /// Update orb visual state (glow, pulse, color)
    /// </summary>
    /// <param name="state">New visual state for the orb</param>
    /// <returns>Success status</returns>
    Task<bool> UpdateVisualStateAsync(OrbVisualState state);

    /// <summary>
    /// Set orb position on screen
    /// </summary>
    /// <param name="position">New position coordinates</param>
    /// <param name="savePosition">Whether to save this position as default</param>
    /// <returns>Success status</returns>
    Task<bool> SetPositionAsync(Point position, bool savePosition = true);

    /// <summary>
    /// Get current orb position and state
    /// </summary>
    /// <returns>Current orb status information</returns>
    Task<OrbDisplayInfo> GetDisplayInfoAsync();

    /// <summary>
    /// Enable or disable orb dragging functionality
    /// </summary>
    /// <param name="enabled">True to enable dragging, false to disable</param>
    /// <returns>Success status</returns>
    Task<bool> SetDraggableAsync(bool enabled);

    /// <summary>
    /// Open chat interface from orb
    /// </summary>
    /// <returns>Success status</returns>
    Task<bool> OpenChatInterfaceAsync();

    /// <summary>
    /// Open OCR analysis interface
    /// </summary>
    /// <returns>Success status</returns>
    Task<bool> OpenOcrInterfaceAsync();

    /// <summary>
    /// Open CLI co-pilot interface
    /// </summary>
    /// <returns>Success status</returns>
    Task<bool> OpenCoPilotInterfaceAsync();

    /// <summary>
    /// Event fired when orb is clicked
    /// </summary>
    event EventHandler<OrbClickEventArgs>? OrbClicked;

    /// <summary>
    /// Event fired when orb position changes
    /// </summary>
    event EventHandler<OrbPositionChangedEventArgs>? PositionChanged;

    /// <summary>
    /// Event fired when orb visual state changes
    /// </summary>
    event EventHandler<OrbStateChangedEventArgs>? StateChanged;
}