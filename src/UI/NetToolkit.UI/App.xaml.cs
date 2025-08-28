using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetToolkit.UI.Services;

namespace NetToolkit.UI;

/// <summary>
/// Application entry point with dependency injection configuration
/// SYSTEMATIC DI: Service container setup following best practices
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _serviceProvider;
    
    /// <summary>
    /// Configure dependency injection and launch MainWindow
    /// GENIUS ARCHITECTURE: Systematic service registration
    /// </summary>
    protected override void OnStartup(StartupEventArgs e)
    {
        // Configure services
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
        
        // Create and show main window with DI
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
        
        base.OnStartup(e);
    }
    
    /// <summary>
    /// Configure all services for dependency injection
    /// SYSTEMATIC REGISTRATION: All UI and business services
    /// </summary>
    private void ConfigureServices(IServiceCollection services)
    {
        // Logging configuration
        services.AddLogging(builder => {
            builder.AddConsole();
            builder.AddDebug();
            builder.SetMinimumLevel(LogLevel.Information);
        });
        
        // UI Services
        services.AddTransient<MainWindow>();
        services.AddSingleton<IThreeJsIntegrationService, ThreeJsIntegrationService>();
        services.AddSingleton<IModuleViewFactory, ModuleViewFactory>();
        services.AddTransient<IAnimationService, WpfAnimationService>();
        
        // TODO: Add backend services when fully integrated
        // services.AddTransient<INetworkScanner, NetworkScannerService>();
        // services.AddTransient<ISecurityScanner, SecurityScannerService>();
        // services.AddTransient<ISshTerminalHost, SshTerminalHostService>();
        // services.AddTransient<IPowerShellHost, PowerShellHostService>();
        // services.AddTransient<IAiOrbService, AiOrbService>();
        // services.AddTransient<IMicrosoftAdminService, MicrosoftAdminService>();
    }
    
    /// <summary>
    /// Cleanup resources on application exit
    /// SYSTEMATIC CLEANUP: Dispose all DI services
    /// </summary>
    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}

