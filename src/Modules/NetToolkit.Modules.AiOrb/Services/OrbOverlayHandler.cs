using Microsoft.Extensions.Logging;
using NetToolkit.Modules.AiOrb.Interfaces;
using NetToolkit.Modules.AiOrb.Models;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WpfMouseEventArgs = System.Windows.Input.MouseEventArgs;
using WinFormsMouseEventArgs = System.Windows.Forms.MouseEventArgs;
using WpfToolTip = System.Windows.Controls.ToolTip;
using WinFormsToolTip = System.Windows.Forms.ToolTip;

namespace NetToolkit.Modules.AiOrb.Services;

/// <summary>
/// WPF overlay handler for the floating AI orb widget
/// Creates and manages the cosmic orb presence on the desktop
/// </summary>
public class OrbOverlayHandler : IOrbOverlayHandler, IDisposable
{
    private readonly ILogger<OrbOverlayHandler> _logger;
    private readonly IConfigManager _configManager;
    private Window? _orbWindow;
    private Ellipse? _orbShape;
    private DispatcherTimer? _animationTimer;
    private bool _isDragging;
    private System.Windows.Point _lastMousePosition;
    private OrbVisualState _currentState;
    private bool _isDisposed;

    // Win32 API for window positioning
    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, 
        int X, int Y, int cx, int cy, uint uFlags);

    private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_SHOWWINDOW = 0x0040;

    public event EventHandler<OrbClickEventArgs>? OrbClicked;
    public event EventHandler<OrbPositionChangedEventArgs>? PositionChanged;
    public event EventHandler<OrbStateChangedEventArgs>? StateChanged;

    public OrbOverlayHandler(ILogger<OrbOverlayHandler> logger, IConfigManager configManager)
    {
        _logger = logger;
        _configManager = configManager;
        _currentState = new OrbVisualState();
        
        _logger.LogInformation("Orb overlay handler initialized - Cosmic presence manager ready! 👁️‍🗨️");
    }

    /// <summary>
    /// Show the floating orb on the desktop
    /// </summary>
    public async Task<bool> ShowOrbAsync(System.Drawing.Point? position = null)
    {
        try
        {
            if (_orbWindow != null && _orbWindow.IsVisible)
            {
                _logger.LogDebug("Orb already visible - cosmic presence maintained! ✨");
                return true;
            }

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                CreateOrbWindow();
                
                if (position.HasValue)
                {
                    _orbWindow!.Left = position.Value.X;
                    _orbWindow!.Top = position.Value.Y;
                }
                else
                {
                    // Load saved position or use default
                    LoadSavedPosition();
                }

                _orbWindow!.Show();
                
                // Make sure window stays on top
                MakeWindowTopmost();
                
                // Start animation
                StartOrbAnimation();
            });

            _logger.LogInformation("Orb displayed successfully - Cosmic presence manifested! 🌟");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to show orb - Cosmic manifestation disrupted! ⚡");
            return false;
        }
    }

    /// <summary>
    /// Hide the floating orb from desktop
    /// </summary>
    public async Task<bool> HideOrbAsync()
    {
        try
        {
            if (_orbWindow == null || !_orbWindow.IsVisible)
            {
                return true;
            }

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                // Save current position before hiding
                SaveCurrentPosition();
                
                // Stop animation
                StopOrbAnimation();
                
                // Hide window
                _orbWindow.Hide();
            });

            _logger.LogInformation("Orb hidden successfully - Cosmic presence archived! 💤");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to hide orb - Cosmic concealment failed! 🌫️");
            return false;
        }
    }

    /// <summary>
    /// Toggle orb visibility state
    /// </summary>
    public async Task<bool> ToggleVisibilityAsync()
    {
        try
        {
            var isCurrentlyVisible = _orbWindow?.IsVisible ?? false;
            
            if (isCurrentlyVisible)
            {
                return await HideOrbAsync();
            }
            else
            {
                return await ShowOrbAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to toggle orb visibility - Cosmic toggle malfunction! 🔄");
            return false;
        }
    }

    /// <summary>
    /// Update orb visual state (glow, pulse, color)
    /// </summary>
    public async Task<bool> UpdateVisualStateAsync(OrbVisualState state)
    {
        try
        {
            var oldState = _currentState;
            _currentState = state;

            if (_orbWindow != null && _orbShape != null)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    ApplyVisualState(state);
                });
            }

            // Fire state changed event
            StateChanged?.Invoke(this, new OrbStateChangedEventArgs 
            { 
                OldState = oldState, 
                NewState = state,
                ChangeReason = "Manual Update"
            });

            _logger.LogDebug("Orb visual state updated - Cosmic appearance transformed! 🎨");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update orb visual state - Cosmic styling error! 💫");
            return false;
        }
    }

    /// <summary>
    /// Set orb position on screen
    /// </summary>
    public async Task<bool> SetPositionAsync(System.Drawing.Point position, bool savePosition = true)
    {
        try
        {
            if (_orbWindow == null)
            {
                return false;
            }

            var oldPosition = new System.Drawing.Point((int)_orbWindow.Left, (int)_orbWindow.Top);

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                _orbWindow.Left = position.X;
                _orbWindow.Top = position.Y;
            });

            if (savePosition)
            {
                SaveCurrentPosition();
            }

            // Fire position changed event
            PositionChanged?.Invoke(this, new OrbPositionChangedEventArgs
            {
                OldPosition = new System.Windows.Point(oldPosition.X, oldPosition.Y),
                NewPosition = new System.Windows.Point(position.X, position.Y)
            });

            _logger.LogDebug("Orb position updated - Cosmic coordinates adjusted! 📍");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set orb position - Cosmic navigation error! 🧭");
            return false;
        }
    }

    /// <summary>
    /// Get current orb position and state
    /// </summary>
    public async Task<OrbDisplayInfo> GetDisplayInfoAsync()
    {
        try
        {
            var displayInfo = new OrbDisplayInfo();

            if (_orbWindow != null)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    displayInfo.Position = new System.Drawing.Point((int)_orbWindow.Left, (int)_orbWindow.Top);
                    displayInfo.IsVisible = _orbWindow.IsVisible;
                    displayInfo.IsDraggable = true; // Always draggable by default
                    displayInfo.VisualState = _currentState;
                    displayInfo.LastInteraction = DateTime.UtcNow;
                });
            }

            return displayInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get orb display info - Cosmic status query failed! ❓");
            return new OrbDisplayInfo();
        }
    }

    /// <summary>
    /// Enable or disable orb dragging functionality
    /// </summary>
    public async Task<bool> SetDraggableAsync(bool enabled)
    {
        try
        {
            if (_orbWindow != null)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    if (enabled)
                    {
                        _orbWindow.MouseLeftButtonDown += OnOrbMouseDown;
                        _orbWindow.MouseMove += OnOrbMouseMove;
                        _orbWindow.MouseLeftButtonUp += OnOrbMouseUp;
                    }
                    else
                    {
                        _orbWindow.MouseLeftButtonDown -= OnOrbMouseDown;
                        _orbWindow.MouseMove -= OnOrbMouseMove;
                        _orbWindow.MouseLeftButtonUp -= OnOrbMouseUp;
                    }
                });
            }

            _logger.LogDebug("Orb dragging {Status} - Cosmic mobility adjusted! 🖱️", enabled ? "enabled" : "disabled");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set orb dragging - Cosmic mobility error! 🚫");
            return false;
        }
    }

    /// <summary>
    /// Open chat interface from orb
    /// </summary>
    public async Task<bool> OpenChatInterfaceAsync()
    {
        try
        {
            // This would open a chat window or interface
            // For now, we'll simulate the action
            _logger.LogInformation("Chat interface requested - Orb conversation mode activated! 💬");
            
            // Fire click event to notify other components
            OrbClicked?.Invoke(this, new OrbClickEventArgs
            {
                ClickPosition = new System.Drawing.Point((int)_orbWindow!.Left, (int)_orbWindow.Top),
                Button = MouseButton.Left,
                ClickCount = 1
            });

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to open chat interface - Cosmic communication disrupted! 📞");
            return false;
        }
    }

    /// <summary>
    /// Open OCR analysis interface
    /// </summary>
    public async Task<bool> OpenOcrInterfaceAsync()
    {
        try
        {
            _logger.LogInformation("OCR interface requested - Orb vision mode activated! 👁️");
            
            // This would trigger screenshot capture and OCR analysis
            OrbClicked?.Invoke(this, new OrbClickEventArgs
            {
                ClickPosition = new System.Drawing.Point((int)_orbWindow!.Left, (int)_orbWindow.Top),
                Button = MouseButton.Right,
                ClickCount = 1
            });

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to open OCR interface - Orb vision circuits confused! 👁️‍🗨️");
            return false;
        }
    }

    /// <summary>
    /// Open CLI co-pilot interface
    /// </summary>
    public async Task<bool> OpenCoPilotInterfaceAsync()
    {
        try
        {
            _logger.LogInformation("Co-pilot interface requested - Orb coding assistance activated! 💻");
            
            OrbClicked?.Invoke(this, new OrbClickEventArgs
            {
                ClickPosition = new System.Drawing.Point((int)_orbWindow!.Left, (int)_orbWindow.Top),
                Button = MouseButton.Middle,
                ClickCount = 1
            });

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to open co-pilot interface - Orb coding circuits jammed! 🤖");
            return false;
        }
    }

    // Private implementation methods

    private void CreateOrbWindow()
    {
        _orbWindow = new Window
        {
            Width = _currentState.Size,
            Height = _currentState.Size,
            WindowStyle = WindowStyle.None,
            AllowsTransparency = true,
            Background = System.Windows.Media.Brushes.Transparent,
            Topmost = true,
            ShowInTaskbar = false,
            ResizeMode = ResizeMode.NoResize
        };

        // Create the orb shape
        _orbShape = new Ellipse
        {
            Width = _currentState.Size,
            Height = _currentState.Size,
            Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(
                (byte)(_currentState.Opacity * 255),
                _currentState.PrimaryColor.R,
                _currentState.PrimaryColor.G,
                _currentState.PrimaryColor.B)),
            Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(
                _currentState.AccentColor.R,
                _currentState.AccentColor.G,
                _currentState.AccentColor.B)),
            StrokeThickness = 2
        };

        // Add glow effect
        if (_currentState.IsGlowing)
        {
            var glowEffect = new System.Windows.Media.Effects.DropShadowEffect
            {
                Color = System.Windows.Media.Color.FromRgb(
                    _currentState.PrimaryColor.R,
                    _currentState.PrimaryColor.G,
                    _currentState.PrimaryColor.B),
                BlurRadius = 20,
                ShadowDepth = 0,
                Opacity = 0.8
            };
            _orbShape.Effect = glowEffect;
        }

        _orbWindow.Content = _orbShape;

        // Set up event handlers
        _orbWindow.MouseLeftButtonDown += OnOrbMouseDown;
        _orbWindow.MouseMove += OnOrbMouseMove;
        _orbWindow.MouseLeftButtonUp += OnOrbMouseUp;
        _orbWindow.MouseRightButtonDown += OnOrbRightClick;
        _orbWindow.MouseWheel += OnOrbMouseWheel;
        
        // Enable dragging by default
        _ = SetDraggableAsync(true);
    }

    private void ApplyVisualState(OrbVisualState state)
    {
        if (_orbShape == null) return;

        // Update size
        _orbShape.Width = state.Size;
        _orbShape.Height = state.Size;
        
        if (_orbWindow != null)
        {
            _orbWindow.Width = state.Size;
            _orbWindow.Height = state.Size;
        }

        // Update colors
        _orbShape.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(
            (byte)(state.Opacity * 255),
            state.PrimaryColor.R,
            state.PrimaryColor.G,
            state.PrimaryColor.B));

        _orbShape.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(
            state.AccentColor.R,
            state.AccentColor.G,
            state.AccentColor.B));

        // Update glow effect
        if (state.IsGlowing)
        {
            var glowEffect = new System.Windows.Media.Effects.DropShadowEffect
            {
                Color = System.Windows.Media.Color.FromRgb(
                    state.PrimaryColor.R,
                    state.PrimaryColor.G,
                    state.PrimaryColor.B),
                BlurRadius = 20,
                ShadowDepth = 0,
                Opacity = 0.8
            };
            _orbShape.Effect = glowEffect;
        }
        else
        {
            _orbShape.Effect = null;
        }

        // Handle pulsing animation
        if (state.IsPulsing)
        {
            StartPulseAnimation();
        }
    }

    private void StartOrbAnimation()
    {
        if (_animationTimer == null)
        {
            _animationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50) // 20 FPS
            };
            _animationTimer.Tick += AnimationTick;
        }

        if (_currentState.IsPulsing || _currentState.IsGlowing)
        {
            _animationTimer.Start();
        }
    }

    private void StopOrbAnimation()
    {
        _animationTimer?.Stop();
    }

    private void AnimationTick(object? sender, EventArgs e)
    {
        if (_orbShape == null) return;

        // Simple breathing animation for idle state
        var time = DateTime.Now.TimeOfDay.TotalSeconds;
        var pulseValue = (Math.Sin(time * 2) + 1) / 2; // 0 to 1

        if (_currentState.IsPulsing)
        {
            var targetOpacity = _currentState.Opacity * (0.5 + pulseValue * 0.5);
            _orbShape.Opacity = targetOpacity;
        }

        if (_currentState.IsGlowing && _orbShape.Effect is System.Windows.Media.Effects.DropShadowEffect glow)
        {
            glow.Opacity = 0.5 + pulseValue * 0.3;
        }
    }

    private void StartPulseAnimation()
    {
        if (_orbShape == null) return;

        var pulseAnimation = new DoubleAnimation
        {
            From = _currentState.Opacity,
            To = _currentState.Opacity * 0.5,
            Duration = TimeSpan.FromSeconds(1),
            AutoReverse = true,
            RepeatBehavior = RepeatBehavior.Forever
        };

        _orbShape.BeginAnimation(UIElement.OpacityProperty, pulseAnimation);
    }

    private void MakeWindowTopmost()
    {
        if (_orbWindow == null) return;

        var windowHandle = new System.Windows.Interop.WindowInteropHelper(_orbWindow).Handle;
        SetWindowPos(windowHandle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
    }

    private void LoadSavedPosition()
    {
        try
        {
            // In a real implementation, this would load from configuration
            // For now, use center of primary screen
            var screen = System.Windows.Forms.Screen.PrimaryScreen;
            if (screen != null && _orbWindow != null)
            {
                _orbWindow.Left = screen.WorkingArea.Width - _currentState.Size - 20;
                _orbWindow.Top = screen.WorkingArea.Height - _currentState.Size - 20;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not load saved position - using default cosmic coordinates! 🌍");
        }
    }

    private void SaveCurrentPosition()
    {
        try
        {
            if (_orbWindow != null)
            {
                // In a real implementation, this would save to configuration
                var position = new System.Drawing.Point((int)_orbWindow.Left, (int)_orbWindow.Top);
                _logger.LogDebug("Saving orb position: ({X}, {Y}) - Cosmic coordinates archived! 📍", 
                    position.X, position.Y);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not save orb position - cosmic memory glitch! 💾");
        }
    }

    // Event handlers

    private void OnOrbMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            _isDragging = true;
            _lastMousePosition = e.GetPosition(_orbWindow);
            _orbWindow?.CaptureMouse();

            // Fire click event
            OrbClicked?.Invoke(this, new OrbClickEventArgs
            {
                ClickPosition = new System.Drawing.Point((int)e.GetPosition(null).X, (int)e.GetPosition(null).Y),
                Button = MouseButton.Left,
                ClickCount = e.ClickCount
            });
        }
    }

    private void OnOrbMouseMove(object sender, WpfMouseEventArgs e)
    {
        if (_isDragging && _orbWindow != null)
        {
            var currentPosition = e.GetPosition(null);
            var deltaX = currentPosition.X - _lastMousePosition.X;
            var deltaY = currentPosition.Y - _lastMousePosition.Y;

            var newLeft = _orbWindow.Left + deltaX;
            var newTop = _orbWindow.Top + deltaY;

            // Keep orb within screen bounds
            var screen = System.Windows.Forms.Screen.FromPoint(new System.Drawing.Point((int)newLeft, (int)newTop));
            newLeft = Math.Max(0, Math.Min(newLeft, screen.WorkingArea.Width - _orbWindow.Width));
            newTop = Math.Max(0, Math.Min(newTop, screen.WorkingArea.Height - _orbWindow.Height));

            _ = SetPositionAsync(new System.Drawing.Point((int)newLeft, (int)newTop), false);
        }
    }

    private void OnOrbMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (_isDragging)
        {
            _isDragging = false;
            _orbWindow?.ReleaseMouseCapture();
            
            // Save position after drag
            SaveCurrentPosition();
        }
    }

    private void OnOrbRightClick(object sender, MouseButtonEventArgs e)
    {
        // Right click for OCR interface
        _ = OpenOcrInterfaceAsync();
    }

    private void OnOrbMouseWheel(object sender, MouseWheelEventArgs e)
    {
        // Mouse wheel for co-pilot interface
        if (e.Delta != 0)
        {
            _ = OpenCoPilotInterfaceAsync();
        }
    }

    public void Dispose()
    {
        if (_isDisposed) return;

        try
        {
            _animationTimer?.Stop();
            _animationTimer = null;

            if (_orbWindow != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _orbWindow.Close();
                    _orbWindow = null;
                });
            }

            _isDisposed = true;
            _logger.LogInformation("Orb overlay handler disposed - Cosmic presence dissolved! 💫");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disposing orb overlay handler - Cosmic cleanup glitch! ⚠️");
        }
    }
}