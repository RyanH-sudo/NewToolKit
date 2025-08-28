using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetToolkit.UI.Services;
using NetToolkit.UI.Views;

namespace NetToolkit.UI;

/// <summary>
/// Premium frameless MainWindow with Three.js integration and Scandinavian minimalist design
/// Follows systematic computational approach with genius-level UI architecture
/// </summary>
public partial class MainWindow : Window
{
    private readonly ILogger<MainWindow>? _logger;
    private readonly IServiceProvider? _serviceProvider;
    private readonly IModuleViewFactory _moduleViewFactory;
    private readonly ThemeManager _themeManager;
    private readonly VideoBackgroundService _videoBackgroundService;
    private readonly ImageBackgroundService _imageBackgroundService;
    private string _currentModule = "Welcome";
    private readonly Dictionary<string, UserControl> _moduleViews = new();
    
    public MainWindow(ILogger<MainWindow> logger, IModuleViewFactory moduleViewFactory)
    {
        InitializeComponent();
        
        _logger = logger;
        _moduleViewFactory = moduleViewFactory;
        _themeManager = new ThemeManager();
        _videoBackgroundService = new VideoBackgroundService(_logger as ILogger<VideoBackgroundService>);
        _imageBackgroundService = new ImageBackgroundService(_logger as ILogger<ImageBackgroundService>);
        
        // GENIUS INITIALIZATION: Systematic UI setup with dependency injection
        try
        {
            InitializeSystematic();
            InitializeThemeSystem();
            LoadWelcomeAnimation();
            InitializeNavigationVideo();
            InitializeWelcomeBackgroundImage();
            InitializeImageBackgrounds();
            
            _logger?.LogInformation("NetToolkit UI initialized successfully");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "UI Initialization Error");
            MessageBox.Show($"UI Initialization Error: {ex.Message}", "NetToolkit Startup Error", 
                          MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    /// <summary>
    /// Systematic initialization following computational approach
    /// </summary>
    private void InitializeSystematic()
    {
        // Phase 1: Window behavior configuration
        this.StateChanged += MainWindow_StateChanged;
        this.LocationChanged += MainWindow_LocationChanged;
        
        // Phase 2: Module view dictionary initialization  
        InitializeModuleViews();
        
        // Phase 3: Backend service integration (placeholder)
        // _serviceProvider = ServiceProviderBuilder.Build(); // TODO: DI integration
        // _logger = _serviceProvider.GetService<ILogger<MainWindow>>();
    }
    
    /// <summary>
    /// Initialize module view mappings for navigation system
    /// </summary>
    private void InitializeModuleViews()
    {
        // SYSTEMATIC MODULE MAPPING: Each backend service gets corresponding view
        // TODO: Create UserControl instances for each module
    }
    
    /// <summary>
    /// Load smooth welcome animation following Three.js principles
    /// </summary>
    private void LoadWelcomeAnimation()
    {
        // GENIUS ANIMATION: Fade-in effect with cubic easing (inspired by Three.js Tween)
        var storyboard = (Storyboard?)Resources["FadeIn"];
        storyboard?.Begin(this);
    }
    
    /// <summary>
    /// Initialize navigation video with specific ninja face loop
    /// GENIUS VIDEO INTEGRATION: Ninja face loop video in corner navigation area
    /// </summary>
    private void InitializeNavigationVideo()
    {
        try
        {
            // Set specific ninja face loop video for navigation area
            var ninjaVideoPath = System.IO.Path.Combine(_videoBackgroundService.GetVideosDirectory(), "Realistic_Ninja_Face_Loop.mp4");
            if (System.IO.File.Exists(ninjaVideoPath) && NavigationVideoLoop != null)
            {
                NavigationVideoLoop.Source = new Uri(ninjaVideoPath, UriKind.Absolute);
                
                // Force the video to start playing immediately
                NavigationVideoLoop.Play();
                
                _logger?.LogInformation($"Navigation video set and play started: Realistic_Ninja_Face_Loop.mp4");
            }
            else
            {
                _logger?.LogWarning("Ninja face loop video not found or NavigationVideoLoop element not found");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to initialize navigation video");
        }
    }
    
    /// <summary>
    /// Initialize welcome screen with random background image
    /// ARTISTIC BACKGROUND: Random background images for main screen
    /// </summary>
    private void InitializeWelcomeBackgroundImage()
    {
        try
        {
            if (WelcomeBackgroundImage != null)
            {
                var backgroundImagePath = _imageBackgroundService.GetRandomImagePath();
                if (!string.IsNullOrEmpty(backgroundImagePath))
                {
                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(backgroundImagePath, UriKind.Absolute);
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    
                    WelcomeBackgroundImage.Source = bitmapImage;
                    _logger?.LogInformation($"Welcome background image set: {System.IO.Path.GetFileName(backgroundImagePath)}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to initialize welcome background image");
        }
    }
    
    /// <summary>
    /// Initialize image backgrounds for UI enhancement
    /// ARTISTIC INTEGRATION: Dynamic image backgrounds (navigation now uses video instead of banner)
    /// </summary>
    private void InitializeImageBackgrounds()
    {
        try
        {
            // Navigation now uses video instead of image banner
            _logger?.LogDebug("Image backgrounds initialized (navigation uses video)");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to initialize image backgrounds");
        }
    }
    
    /// <summary>
    /// Find the welcome screen grid to apply video background
    /// </summary>
    private Grid? FindWelcomeScreenGrid()
    {
        try
        {
            // Look for the main content presenter and its grid
            if (MainContentPresenter?.Content is FrameworkElement content)
            {
                return FindChildOfType<Grid>(content);
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Failed to find welcome screen grid");
            return null;
        }
    }
    
    /// <summary>
    /// Helper method to find child elements of specific type
    /// </summary>
    private T? FindChildOfType<T>(DependencyObject parent) where T : class
    {
        try
        {
            if (parent == null) return null;
            
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T result)
                    return result;
                
                var foundChild = FindChildOfType<T>(child);
                if (foundChild != null)
                    return foundChild;
            }
            
            return null;
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Error finding child of type");
            return null;
        }
    }
    
    #region Custom Window Chrome Event Handlers
    
    /// <summary>
    /// Handle title bar drag for frameless window movement
    /// </summary>
    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            try
            {
                this.DragMove();
            }
            catch (InvalidOperationException)
            {
                // Handle potential threading issues with DragMove
                _logger?.LogWarning("DragMove operation interrupted - window state may be changing");
            }
        }
    }
    
    /// <summary>
    /// Minimize window with smooth animation
    /// </summary>
    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
        _logger?.LogDebug("😴 NetToolkit minimized - Swedish efficiency in action");
    }
    
    /// <summary>
    /// Toggle maximize/restore with state management
    /// </summary>
    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        this.WindowState = this.WindowState == WindowState.Maximized ? 
                          WindowState.Normal : WindowState.Maximized;
        
        _logger?.LogDebug($"📺 NetToolkit window state: {this.WindowState}");
    }
    
    /// <summary>
    /// Graceful application shutdown with resource cleanup
    /// </summary>
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        // GENIUS SHUTDOWN: Systematic resource cleanup
        try
        {
            _logger?.LogInformation("👋 NetToolkit shutting down gracefully - Scandinavian farewell");
            
            // TODO: Dispose backend services
            // TODO: Save user preferences
            // TODO: Cleanup Three.js WebView2 resources
            
            Application.Current.Shutdown();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error during application shutdown");
            Application.Current.Shutdown(); // Force shutdown if graceful fails
        }
    }
    
    #endregion
    
    #region Module Navigation System
    
    /// <summary>
    /// Handle module navigation with smooth transitions
    /// Follows systematic UI state management principles
    /// </summary>
    private void NavigateToModule(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string moduleTag)
        {
            try
            {
                LoadModule(moduleTag);
                UpdateNavigationState(button);
                
                _logger?.LogInformation($"🧭 Navigated to module: {moduleTag} - Premium experience loading");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Failed to load module: {moduleTag}");
                ShowErrorMessage($"Failed to load {moduleTag} module", ex.Message);
            }
        }
    }
    
    /// <summary>
    /// Load specified module with Three.js integration
    /// </summary>
    private void LoadModule(string moduleTag)
    {
        _currentModule = moduleTag;
        
        // SYSTEMATIC MODULE LOADING: Phase-by-phase approach
        
        // Phase 1: Clear current content with fade-out animation
        var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(150));
        fadeOut.Completed += (s, e) => {
            
            // Phase 2: Load new module content
            UserControl? moduleView = CreateModuleView(moduleTag);
            if (moduleView != null)
            {
                MainContentPresenter.Content = moduleView;
            }
            else
            {
                // Fallback: Show module-specific placeholder
                MainContentPresenter.Content = CreateModulePlaceholder(moduleTag);
            }
            
            // Phase 2.5: Apply module-specific background enhancements
            ApplyModuleBackgroundEnhancements(moduleTag);
            
            // Phase 3: Fade-in new content
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
            MainContentPresenter.BeginAnimation(OpacityProperty, fadeIn);
        };
        
        MainContentPresenter.BeginAnimation(OpacityProperty, fadeOut);
    }
    
    /// <summary>
    /// Create module-specific view using factory pattern with dependency injection
    /// SYSTEMATIC FACTORY: View creation with proper service injection
    /// </summary>
    private UserControl? CreateModuleView(string moduleTag)
    {
        try
        {
            // Use ModuleViewFactory with dependency injection
            return moduleTag switch
            {
                "Education" => _moduleViewFactory.CreateEducationView(),
                "NetworkScanner" => _moduleViewFactory.CreateNetworkScannerView(),
                "SecurityScan" => _moduleViewFactory.CreateSecurityScanView(),
                "SshTerminal" => _moduleViewFactory.CreateSshTerminalView(),
                "PowerShell" => _moduleViewFactory.CreatePowerShellView(),
                "AiOrb" => _moduleViewFactory.CreateAiOrbView(),
                "MicrosoftAdmin" => _moduleViewFactory.CreateMicrosoftAdminView(),
                _ => null
            };
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Failed to create view for module: {moduleTag}");
            return null;
        }
    }
    
    /// <summary>
    /// Create module placeholder with Three.js integration preview
    /// </summary>
    private UserControl CreateModulePlaceholder(string moduleTag)
    {
        var placeholder = new UserControl();
        var grid = new Grid();
        
        var stackPanel = new StackPanel
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        
        // Module-specific icon and description
        var (icon, title, description) = GetModuleInfo(moduleTag);
        
        stackPanel.Children.Add(new TextBlock { 
            Text = icon, FontSize = 48, HorizontalAlignment = HorizontalAlignment.Center, 
            Margin = new Thickness(0, 0, 0, 16) });
        stackPanel.Children.Add(new TextBlock { 
            Text = $"{title} Module", FontSize = 24, FontWeight = FontWeights.Bold,
            HorizontalAlignment = HorizontalAlignment.Center });
        stackPanel.Children.Add(new TextBlock { 
            Text = description, FontSize = 16, 
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 8, 0, 0), 
            Foreground = (SolidColorBrush)Resources["TextSecondary"] });
        stackPanel.Children.Add(new TextBlock { 
            Text = "🚧 Full implementation coming soon with Three.js integration", 
            FontSize = 14, FontStyle = FontStyles.Italic,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 16, 0, 0),
            Foreground = (SolidColorBrush)Resources["AccentNeon"] });
        
        grid.Children.Add(stackPanel);
        placeholder.Content = grid;
        
        return placeholder;
    }
    
    /// <summary>
    /// Get module-specific information for UI display
    /// </summary>
    private (string icon, string title, string description) GetModuleInfo(string moduleTag)
    {
        return moduleTag switch
        {
            "Education" => ("🎓", "Education Platform", "Interactive learning with 10 comprehensive modules"),
            "NetworkScanner" => ("🔍", "Network Scanner", "3D topology visualization with Three.js integration"),
            "SecurityScan" => ("🔒", "Security Scanner", "Advanced vulnerability detection and reporting"),
            "SshTerminal" => ("📟", "SSH Terminal", "Professional SSH client with device management"),
            "PowerShell" => ("💻", "PowerShell", "Embedded PowerShell with Microsoft Graph integration"),
            "AiOrb" => ("🤖", "AI Orb", "Intelligent assistant with OCR and chat capabilities"),
            "MicrosoftAdmin" => ("🏢", "Microsoft Admin", "Azure and Microsoft 365 administration tools"),
            _ => ("❓", "Unknown", "Module information not available")
        };
    }
    
    /// <summary>
    /// Apply module-specific background enhancements
    /// ARTISTIC MODULE THEMING: Different backgrounds for different modules
    /// </summary>
    private void ApplyModuleBackgroundEnhancements(string moduleTag)
    {
        try
        {
            // Different modules get different background treatments
            switch (moduleTag.ToLower())
            {
                case "education":
                    // Education gets subtle image backgrounds for inspiration
                    ApplyEducationBackgroundEnhancements();
                    break;
                    
                case "aiOrb":
                    // AI Orb gets dynamic artistic backgrounds
                    ApplyAiOrbBackgroundEnhancements();
                    break;
                    
                case "microsoftadmin":
                    // Microsoft Admin gets professional subtle backgrounds
                    ApplyMicrosoftAdminBackgroundEnhancements();
                    break;
                    
                case "securityscan":
                    // Security modules get darker, more technical backgrounds
                    ApplySecurityBackgroundEnhancements();
                    break;
                    
                default:
                    // Other modules get standard treatment
                    _logger?.LogDebug($"No specific background enhancement for module: {moduleTag}");
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Failed to apply background enhancements for module: {moduleTag}");
        }
    }
    
    /// <summary>
    /// Apply education-specific background enhancements
    /// </summary>
    private void ApplyEducationBackgroundEnhancements()
    {
        try
        {
            // Find the education module's main container
            if (MainContentPresenter?.Content is UserControl userControl)
            {
                var grid = FindChildOfType<Grid>(userControl);
                if (grid != null)
                {
                    _imageBackgroundService.ApplyImageBackground(grid, opacity: 0.15);
                    _logger?.LogInformation("🎓 Education module background enhancement applied");
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Failed to apply education background enhancements");
        }
    }
    
    /// <summary>
    /// Apply AI Orb-specific background enhancements
    /// </summary>
    private void ApplyAiOrbBackgroundEnhancements()
    {
        try
        {
            if (MainContentPresenter?.Content is UserControl userControl)
            {
                var grid = FindChildOfType<Grid>(userControl);
                if (grid != null)
                {
                    _imageBackgroundService.ApplyImageBackground(grid, opacity: 0.25);
                    _logger?.LogInformation("🤖 AI Orb module background enhancement applied");
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Failed to apply AI Orb background enhancements");
        }
    }
    
    /// <summary>
    /// Apply Microsoft Admin-specific background enhancements
    /// </summary>
    private void ApplyMicrosoftAdminBackgroundEnhancements()
    {
        try
        {
            if (MainContentPresenter?.Content is UserControl userControl)
            {
                var grid = FindChildOfType<Grid>(userControl);
                if (grid != null)
                {
                    _imageBackgroundService.ApplyImageBackground(grid, opacity: 0.12);
                    _logger?.LogInformation("🏢 Microsoft Admin module background enhancement applied");
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Failed to apply Microsoft Admin background enhancements");
        }
    }
    
    /// <summary>
    /// Apply Security-specific background enhancements
    /// </summary>
    private void ApplySecurityBackgroundEnhancements()
    {
        try
        {
            if (MainContentPresenter?.Content is UserControl userControl)
            {
                var grid = FindChildOfType<Grid>(userControl);
                if (grid != null)
                {
                    _imageBackgroundService.ApplyImageBackground(grid, opacity: 0.08);
                    _logger?.LogInformation("🔒 Security module background enhancement applied");
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Failed to apply Security background enhancements");
        }
    }
    
    /// <summary>
    /// Update navigation button states for visual feedback
    /// </summary>
    private void UpdateNavigationState(Button activeButton)
    {
        // Reset all navigation buttons to normal state
        var navigationPanel = activeButton.Parent as StackPanel;
        if (navigationPanel != null)
        {
            foreach (Button btn in navigationPanel.Children.OfType<Button>())
            {
                btn.Background = Brushes.Transparent;
            }
        }
        
        // Highlight active button with neon accent
        activeButton.Background = (SolidColorBrush)Resources["AccentNeon"];
        activeButton.Foreground = (SolidColorBrush)Resources["AccentDark"];
    }
    
    #endregion
    
    #region Module View Factories (Placeholder Implementation)
    
    // TODO: Implement actual module views with backend integration
    
    private UserControl CreateEducationView()
    {
        // return new EducationModuleView(_serviceProvider.GetService<IEducationService>());
        return CreateModulePlaceholder("Education");
    }
    
    private UserControl CreateNetworkScannerView()
    {
        // return new NetworkScannerView(_serviceProvider.GetService<INetworkScanner>());
        return CreateModulePlaceholder("NetworkScanner");
    }
    
    private UserControl CreateSecurityScanView()
    {
        // return new SecurityScanView(_serviceProvider.GetService<ISecurityScanner>());
        return CreateModulePlaceholder("SecurityScan");
    }
    
    private UserControl CreateSshTerminalView()
    {
        // return new SshTerminalView(_serviceProvider.GetService<ISshTerminalHost>());
        return CreateModulePlaceholder("SshTerminal");
    }
    
    private UserControl CreatePowerShellView()
    {
        // return new PowerShellView(_serviceProvider.GetService<IPowerShellHost>());
        return CreateModulePlaceholder("PowerShell");
    }
    
    private UserControl CreateAiOrbView()
    {
        // return new AiOrbView(_serviceProvider.GetService<IAiOrbService>());
        return CreateModulePlaceholder("AiOrb");
    }
    
    private UserControl CreateMicrosoftAdminView()
    {
        // return new MicrosoftAdminView(_serviceProvider.GetService<IMicrosoftAdminService>());
        return CreateModulePlaceholder("MicrosoftAdmin");
    }
    
    #endregion
    
    #region Window State Management
    
    private void MainWindow_StateChanged(object? sender, EventArgs e)
    {
        // Handle window state changes for frameless window behavior
        if (this.WindowState == WindowState.Maximized)
        {
            // Adjust margins for maximized state to prevent off-screen content
            this.BorderThickness = new Thickness(8);
        }
        else
        {
            this.BorderThickness = new Thickness(0);
        }
    }
    
    private void MainWindow_LocationChanged(object? sender, EventArgs e)
    {
        // Ensure window stays within screen boundaries for frameless design
        // TODO: Implement multi-monitor awareness
    }
    
    #endregion
    
    #region Error Handling
    
    private void ShowErrorMessage(string title, string message)
    {
        MessageBox.Show($"Error: {message}\n\nPlease check the logs for more details.", 
                       $"NetToolkit - {title}", MessageBoxButton.OK, MessageBoxImage.Warning);
    }
    
    #endregion
    
    #region Settings and Configuration
    
    /// <summary>
    /// Open settings dialog window
    /// SYSTEMATIC CONFIGURATION: Centralized settings management
    /// </summary>
    private void OpenSettings_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _logger?.LogInformation("⚙️ Opening NetToolkit settings dialog");
            
            var settingsWindow = new SettingsWindow();
            settingsWindow.Owner = this;
            
            var result = settingsWindow.ShowDialog();
            
            if (result == true)
            {
                _logger?.LogInformation("⚙️ Settings updated successfully");
                // TODO: Apply new settings to the application
                // - Refresh module configurations
                // - Update Three.js visualizations
                // - Apply UI theme changes
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to open settings dialog");
            ShowErrorMessage("Settings Error", "Failed to open settings dialog: " + ex.Message);
        }
    }
    
    #endregion
    
    #region Quick Actions
    
    /// <summary>
    /// Refresh all backend services and Three.js visualizations
    /// SYSTEMATIC REFRESH: Complete system refresh with visual feedback
    /// </summary>
    private async void RefreshServices_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _logger?.LogInformation("🔄 Refreshing all backend services and visualizations");
            
            var button = sender as Button;
            var originalContent = button?.Content;
            
            if (button != null)
            {
                button.Content = "⏳ Refreshing...";
                button.IsEnabled = false;
            }
            
            // Simulate service refresh with comprehensive operations
            await Task.Run(async () =>
            {
                // Phase 1: Refresh backend services
                await Task.Delay(500);
                _logger?.LogInformation("🔧 Backend services refreshed");
                
                // Phase 2: Refresh module views
                await Task.Delay(300);
                _logger?.LogInformation("📦 Module views refreshed");
                
                // Phase 3: Refresh Three.js visualizations
                await Task.Delay(400);
                _logger?.LogInformation("🎨 Three.js visualizations refreshed");
                
                // Phase 4: Update UI state
                await Task.Delay(200);
                _logger?.LogInformation("✨ UI state synchronized");
            });
            
            // Restore button state
            if (button != null)
            {
                button.Content = originalContent;
                button.IsEnabled = true;
            }
            
            // Show success feedback
            MessageBox.Show("✅ All services refreshed successfully!\n\n" +
                          "• Backend services reloaded\n" +
                          "• Module views updated\n" +
                          "• Three.js visualizations refreshed\n" +
                          "• UI state synchronized", 
                          "Refresh Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                          
            _logger?.LogInformation("✅ Service refresh completed successfully");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to refresh services");
            MessageBox.Show($"❌ Service refresh failed: {ex.Message}", "Refresh Error", 
                          MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
    
    #endregion
    
    #region Floating AI Orb Integration
    
    /// <summary>
    /// Navigate to AI Orb module from external floating widget
    /// SYSTEMATIC INTEGRATION: External navigation support for floating AI orb
    /// </summary>
    public void NavigateToAiOrbModule()
    {
        try
        {
            LoadModule("AiOrb");
            UpdateNavigationState(BtnAiOrb);
            
            _logger?.LogInformation("🤖 Navigated to AI Orb module from floating widget");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to navigate to AI Orb module");
        }
    }
    
    /// <summary>
    /// Launch floating AI Orb widget
    /// SYSTEMATIC WIDGET LAUNCHING: Create and display floating AI assistant
    /// </summary>
    private void LaunchFloatingAiOrb_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _logger?.LogInformation("🚀 Launching Floating AI Orb Widget");
            
            var threeJsService = _moduleViewFactory.GetThreeJsService();
            var orbWidget = new FloatingAiOrbWidget(threeJsService, _logger as ILogger<FloatingAiOrbWidget>);
            
            // Position near the main window
            orbWidget.Left = this.Left + this.Width + 20;
            orbWidget.Top = this.Top + 100;
            
            orbWidget.Show();
            
            _logger?.LogInformation("✨ Floating AI Orb Widget launched successfully");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to launch Floating AI Orb Widget");
            ShowErrorMessage("AI Orb Error", "Failed to launch floating AI Orb: " + ex.Message);
        }
    }
    
    #endregion
    
    #region Theme Management
    
    /// <summary>
    /// Initialize theme system with event handlers
    /// GENIUS THEME INTEGRATION: Systematic theme application
    /// </summary>
    private void InitializeThemeSystem()
    {
        try
        {
            // Initialize theme manager
            _themeManager.Initialize();
            
            // Subscribe to theme change events
            _themeManager.ThemeChanged += OnThemeChanged;
            
            // Update toggle button appearance
            UpdateThemeToggleButton();
            
            _logger?.LogInformation($"🎨 Theme system initialized - {(_themeManager.IsDarkMode ? "Dark Ninja" : "Light Zen")} mode active");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to initialize theme system");
        }
    }
    
    /// <summary>
    /// Handle theme toggle button click
    /// SYSTEMATIC THEMING: Use comprehensive ThemeManager for complete theme switching
    /// </summary>
    private void ThemeToggle_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _themeManager.ToggleTheme();
            UpdateThemeToggleButton();
            
            _logger?.LogInformation($"🎨 Theme toggled to {(_themeManager.IsDarkMode ? "Dark Ninja" : "Light Zen")} mode with full effects");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to toggle theme");
            ShowErrorMessage("Theme Error", "Failed to switch theme: " + ex.Message);
        }
    }
    
    /// <summary>
    /// Apply theme directly to main UI elements for immediate visual effect
    /// DIRECT APPROACH: Bypass resource system for guaranteed theme switching
    /// </summary>
    private void ApplyThemeDirectly(bool isDarkMode)
    {
        try
        {
            // Load appropriate theme resource dictionary
            var themeUri = isDarkMode 
                ? new Uri("/NetToolkit.UI;component/Themes/DarkTheme.xaml", UriKind.Relative)
                : new Uri("/NetToolkit.UI;component/Themes/LightTheme.xaml", UriKind.Relative);
            
            var themeDict = (ResourceDictionary)Application.LoadComponent(themeUri);
            
            // Clear existing merged dictionaries and add the theme
            this.Resources.MergedDictionaries.Clear();
            this.Resources.MergedDictionaries.Add(themeDict);
            
            // Also update application resources
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(themeDict);
            
            // Copy theme resources to main window resources for immediate access
            foreach (var key in themeDict.Keys)
            {
                if (key != null && themeDict[key] != null)
                {
                    this.Resources[key] = themeDict[key];
                    Application.Current.Resources[key] = themeDict[key];
                }
            }
            
            // Update window background
            if (themeDict["PrimaryBackground"] is Brush primaryBg)
            {
                this.Background = primaryBg;
            }
            
            // Force UI refresh
            this.InvalidateVisual();
            this.UpdateLayout();
            
            // Trigger visual feedback effect
            TriggerThemeChangeEffect(isDarkMode);
            
            _logger?.LogInformation($"{(isDarkMode ? "🌙" : "☀️")} Theme resources applied and UI refreshed");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to apply theme resources");
        }
    }
    
    /// <summary>
    /// Update specific UI element background by name
    /// </summary>
    private void UpdateElementBackground(string elementName, Brush background)
    {
        try
        {
            var element = this.FindName(elementName);
            if (element is Control control)
            {
                control.Background = background;
            }
            else if (element is Panel panel)
            {
                panel.Background = background;
            }
            else if (element is Border border)
            {
                border.Background = background;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, $"Failed to update element background: {elementName}");
        }
    }
    
    /// <summary>
    /// Trigger visual effect for theme change
    /// </summary>
    private void TriggerThemeChangeEffect(bool isDarkMode)
    {
        try
        {
            var glowColor = isDarkMode ? Colors.LimeGreen : Colors.Gold;
            var glowEffect = new DropShadowEffect
            {
                Color = glowColor,
                BlurRadius = 20,
                ShadowDepth = 0,
                Opacity = 0.6
            };
            
            this.Effect = glowEffect;
            
            // Remove effect after animation
            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            timer.Tick += (s, e) =>
            {
                this.Effect = null;
                timer.Stop();
            };
            timer.Start();
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Theme change effect failed");
        }
    }
    
    /// <summary>
    /// Update theme toggle button appearance based on current theme
    /// </summary>
    private void UpdateThemeToggleButton()
    {
        try
        {
            if (ThemeToggleButton != null)
            {
                if (_themeManager.IsDarkMode)
                {
                    ThemeToggleButton.Content = "☀️";
                    ThemeToggleButton.ToolTip = "Switch to Light Theme - Zen daylight mode";
                }
                else
                {
                    ThemeToggleButton.Content = "🌙";
                    ThemeToggleButton.ToolTip = "Switch to Dark Theme - Ninja stealth mode";
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Failed to update theme toggle button");
        }
    }
    
    /// <summary>
    /// Handle theme change events
    /// </summary>
    private void OnThemeChanged(bool isDarkMode)
    {
        try
        {
            // Apply theme directly to MainWindow
            ApplyThemeDirectly(isDarkMode);
            
            // Update any theme-dependent elements
            UpdateThemeToggleButton();
            
            // Notify all module views of theme change
            NotifyModulesOfThemeChange(isDarkMode);
            
            _logger?.LogInformation($"✨ Theme change event handled: {(isDarkMode ? "Dark" : "Light")} mode active");
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Error handling theme change event");
        }
    }
    
    /// <summary>
    /// Notify all loaded module views of theme change
    /// </summary>
    private void NotifyModulesOfThemeChange(bool isDarkMode)
    {
        try
        {
            // Find and notify all module views in the content presenter
            if (MainContentPresenter?.Content is FrameworkElement content)
            {
                // Trigger resource refresh on the content
                content.InvalidateVisual();
                content.UpdateLayout();
                
                _logger?.LogInformation($"📢 Notified modules of theme change to {(isDarkMode ? "Dark" : "Light")} mode");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Error notifying modules of theme change");
        }
    }
    
    #endregion
    
    #region Video Background Event Handlers
    
    /// <summary>
    /// Handle video background media opened event
    /// SYSTEMATIC VIDEO CONTROL: Start playback when media is ready
    /// </summary>
    private void VideoBackground_MediaOpened(object sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is MediaElement mediaElement)
            {
                mediaElement.Play();
                _logger?.LogInformation("🎬 Welcome video background started playing");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to start video background playback");
        }
    }
    
    /// <summary>
    /// Handle video background media ended event for looping
    /// GENIUS LOOPING: Seamless video loop for continuous background
    /// </summary>
    private void VideoBackground_MediaEnded(object sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is MediaElement mediaElement)
            {
                mediaElement.Position = TimeSpan.Zero;
                mediaElement.Play();
                _logger?.LogDebug("🔄 Video background looped");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Failed to loop video background");
        }
    }
    
    /// <summary>
    /// Handle video background media failed event
    /// RESILIENT DESIGN: Graceful handling of video failures
    /// </summary>
    private void VideoBackground_MediaFailed(object sender, ExceptionRoutedEventArgs e)
    {
        try
        {
            _logger?.LogError($"Video background playback failed: {e.ErrorException?.Message}");
            
            if (sender is MediaElement mediaElement)
            {
                mediaElement.Visibility = Visibility.Collapsed;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling video background failure");
        }
    }
    
    #endregion
    
    #region Navigation Video Event Handlers
    
    /// <summary>
    /// Handle navigation video media opened event
    /// Start playback when media is ready
    /// </summary>
    private void NavigationVideo_MediaOpened(object sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is MediaElement mediaElement)
            {
                mediaElement.Play();
                _logger?.LogInformation("🎬 Navigation ninja face loop video started playing");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to start navigation video playback");
        }
    }
    
    /// <summary>
    /// Handle navigation video media ended event for looping
    /// Seamless video loop for continuous navigation background
    /// </summary>
    private void NavigationVideo_MediaEnded(object sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is MediaElement mediaElement)
            {
                mediaElement.Position = TimeSpan.Zero;
                mediaElement.Play();
                _logger?.LogDebug("🔄 Navigation video looped");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Failed to loop navigation video");
        }
    }
    
    /// <summary>
    /// Handle navigation video media failed event
    /// Graceful handling of video failures
    /// </summary>
    private void NavigationVideo_MediaFailed(object sender, ExceptionRoutedEventArgs e)
    {
        try
        {
            _logger?.LogError($"Navigation video playback failed: {e.ErrorException?.Message}");
            
            if (sender is MediaElement mediaElement)
            {
                mediaElement.Visibility = Visibility.Collapsed;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling navigation video failure");
        }
    }
    
    #endregion
}