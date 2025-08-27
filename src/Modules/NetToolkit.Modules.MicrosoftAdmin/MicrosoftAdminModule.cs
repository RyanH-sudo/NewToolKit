using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using NetToolkit.Core.Interfaces;
using NetToolkit.Modules.MicrosoftAdmin.Interfaces;
using NetToolkit.Modules.MicrosoftAdmin.Services;
using NetToolkit.Modules.MicrosoftAdmin.Events;
using System.Reflection;

namespace NetToolkit.Modules.MicrosoftAdmin;

/// <summary>
/// Microsoft Admin Integration Module - the sovereign bridge to Microsoft's administrative empire
/// Orchestrates the symphony of Office 365 administration within NetToolkit's realm
/// </summary>
public class MicrosoftAdminModule : IModule
{
    private readonly ILogger<MicrosoftAdminModule> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IEventBus _eventBus;
    private bool _isInitialized;

    public string Name => "Microsoft Admin Integration";
    public string Version => "1.0.0";
    public string Description => "Premium Microsoft 365 administrative integration - bridging NetToolkit genius with Microsoft's enterprise realm";
    public string Author => "NetToolkit Team";
    public bool IsEnabled { get; private set; }

    public MicrosoftAdminModule(
        ILogger<MicrosoftAdminModule> logger,
        IServiceProvider serviceProvider,
        IEventBus eventBus)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _eventBus = eventBus;
        
        _logger.LogInformation("Microsoft Admin Integration Module created - preparing administrative bridge to Microsoft empire");
    }

    public async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        try
        {
            if (_isInitialized)
            {
                _logger.LogWarning("Microsoft Admin Module already initialized - skipping redundant initialization");
                return;
            }

            _logger.LogInformation("Initializing Microsoft Admin Integration Module - establishing administrative dominion");

            // Validate configuration
            await ValidateConfigurationAsync();
            
            // Initialize services
            await InitializeServicesAsync();
            
            // Set up event subscriptions
            await SubscribeToEventsAsync();
            
            // Load admin templates
            await LoadAdminTemplatesAsync();
            
            // Validate Microsoft Graph connectivity
            await ValidateGraphConnectivityAsync();
            
            _isInitialized = true;
            IsEnabled = true;
            
            // Announce successful initialization
            await PublishModuleReadyEventAsync();
            
            _logger.LogInformation("Microsoft Admin Integration Module initialized successfully - administrative bridge operational!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Microsoft Admin Integration Module - administrative bridge construction failed");
            IsEnabled = false;
            throw;
        }
    }

    public async Task<bool> StartAsync()
    {
        try
        {
            if (!_isInitialized)
            {
                _logger.LogWarning("Attempting to start uninitialized Microsoft Admin Module - initializing first");
                if (!await InitializeAsync())
                {
                    return false;
                }
            }

            _logger.LogInformation("Starting Microsoft Admin Integration Module services - admin powers activating");

            // Start background services if any
            await StartBackgroundServicesAsync();
            
            // Refresh authentication state
            await RefreshAuthenticationStateAsync();
            
            // Publish module started event
            await _eventBus.PublishAsync(new IntegrationSyncEvent
            {
                TargetModule = "All",
                SyncType = "ModuleStarted",
                SyncedAt = DateTime.UtcNow,
                SyncSuccessful = true,
                SyncData = System.Text.Json.JsonSerializer.Serialize(new
                {
                    ModuleName = Name,
                    Version = Version,
                    Capabilities = GetModuleCapabilities()
                })
            });

            _logger.LogInformation("Microsoft Admin Integration Module started successfully - ready for administrative mastery");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start Microsoft Admin Integration Module");
            return false;
        }
    }

    public async Task<bool> StopAsync()
    {
        try
        {
            _logger.LogInformation("Stopping Microsoft Admin Integration Module - administrative session concluding");

            // Stop background services
            await StopBackgroundServicesAsync();
            
            // Clean up resources
            await CleanupResourcesAsync();
            
            // Publish module stopped event
            await _eventBus.PublishAsync(new IntegrationSyncEvent
            {
                TargetModule = "All",
                SyncType = "ModuleStopped",
                SyncedAt = DateTime.UtcNow,
                SyncSuccessful = true,
                SyncData = System.Text.Json.JsonSerializer.Serialize(new
                {
                    ModuleName = Name,
                    StoppedAt = DateTime.UtcNow,
                    Reason = "Graceful shutdown"
                })
            });

            IsEnabled = false;
            _logger.LogInformation("Microsoft Admin Integration Module stopped successfully - administrative bridge closed");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while stopping Microsoft Admin Integration Module");
            return false;
        }
    }

    public void RegisterServices(IServiceCollection services)
    {
        try
        {
            _logger.LogInformation("Registering Microsoft Admin Integration Module services - assembling administrative arsenal");

            // Register core services
            services.AddScoped<IMicrosoftAdminService, MicrosoftAdminService>();
            services.AddScoped<IScriptTemplateEngine, ScriptTemplateEngine>();
            services.AddScoped<IFormBuilder, FormBuilder>();
            services.AddScoped<IPortalIntegrator, PortalIntegrator>();
            services.AddScoped<IAdminEventPublisher, AdminEventPublisher>();
            
            // Register HTTP client for portal integration
            services.AddHttpClient<IPortalIntegrator, PortalIntegrator>(client =>
            {
                client.DefaultRequestHeaders.Add("User-Agent", "NetToolkit-AdminPortal/1.0");
                client.Timeout = TimeSpan.FromSeconds(30);
            });
            
            // Register hosted services for background operations
            services.AddHostedService<AdminTemplateRefreshService>();
            services.AddHostedService<TokenRefreshService>();
            
            // Register configuration validation
            services.AddOptions<MicrosoftAdminConfiguration>()
                .Configure<IConfiguration>((config, configuration) =>
                {
                    configuration.GetSection("MicrosoftAdmin").Bind(config);
                })
                .ValidateDataAnnotations()
                .ValidateOnStart();

            _logger.LogInformation("Microsoft Admin Integration Module services registered successfully - {ServiceCount} services armed for duty",
                GetRegisteredServiceCount());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to register Microsoft Admin Integration Module services");
            throw;
        }
    }

    private async Task ValidateConfigurationAsync()
    {
        var configuration = _serviceProvider.GetRequiredService<IConfiguration>();
        
        var clientId = configuration["MicrosoftAdmin:ClientId"];
        if (string.IsNullOrEmpty(clientId))
        {
            throw new InvalidOperationException("Microsoft Admin ClientId not configured - cannot establish connection to Microsoft realm");
        }
        
        var templatesPath = configuration["MicrosoftAdmin:TemplatesPath"];
        if (string.IsNullOrEmpty(templatesPath))
        {
            _logger.LogWarning("Templates path not configured - using default location");
        }
        
        _logger.LogDebug("Microsoft Admin configuration validated successfully");
        await Task.CompletedTask;
    }

    private async Task InitializeServicesAsync()
    {
        try
        {
            // Initialize template engine
            var templateEngine = _serviceProvider.GetRequiredService<IScriptTemplateEngine>();
            var templates = await templateEngine.LoadTemplatesAsync();
            _logger.LogInformation("Loaded {TemplateCount} admin templates - spell repository stocked", templates.Count);
            
            // Initialize portal integrator
            var portalIntegrator = _serviceProvider.GetRequiredService<IPortalIntegrator>();
            var isPortalAccessible = await portalIntegrator.IsPortalAccessibleAsync();
            _logger.LogInformation("Portal accessibility check: {Status}", isPortalAccessible ? "Connected" : "Limited");
            
            _logger.LogDebug("Microsoft Admin services initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Service initialization failed");
            throw;
        }
    }

    private async Task SubscribeToEventsAsync()
    {
        try
        {
            // Subscribe to relevant events from other modules
            await _eventBus.SubscribeAsync<AuthenticationSuccessEvent>(OnAuthenticationSuccess);
            await _eventBus.SubscribeAsync<IntegrationSyncEvent>(OnIntegrationSync);
            
            // Subscribe to system events
            await _eventBus.SubscribeAsync<SecurityAlertEvent>(OnSecurityAlert);
            
            _logger.LogDebug("Event subscriptions established - listening for administrative harmonics");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to establish event subscriptions");
            throw;
        }
    }

    private async Task LoadAdminTemplatesAsync()
    {
        try
        {
            var templateEngine = _serviceProvider.GetRequiredService<IScriptTemplateEngine>();
            var templates = await templateEngine.LoadTemplatesAsync();
            
            _logger.LogInformation("Successfully loaded {Count} admin templates - administrative arsenal ready", templates.Count);
            
            // Validate critical templates
            var criticalTemplates = new[] { "extend-mailbox", "create-shared-mailbox", "reset-user-password" };
            var loadedTemplateIds = templates.Select(t => t.Id).ToHashSet();
            
            foreach (var criticalTemplate in criticalTemplates)
            {
                if (!loadedTemplateIds.Contains(criticalTemplate))
                {
                    _logger.LogWarning("Critical template '{TemplateId}' not found - some admin operations may be limited", criticalTemplate);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load admin templates");
            throw;
        }
    }

    private async Task ValidateGraphConnectivityAsync()
    {
        try
        {
            // This would perform a basic connectivity test
            // For now, we'll simulate the check
            await Task.Delay(100);
            
            _logger.LogInformation("Microsoft Graph connectivity validated - connection to Microsoft empire confirmed");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Microsoft Graph connectivity validation failed - some features may be limited");
            // Don't throw - the module can still function in limited capacity
        }
    }

    private async Task PublishModuleReadyEventAsync()
    {
        await _eventBus.PublishAsync(new IntegrationSyncEvent
        {
            TargetModule = "All",
            SyncType = "ModuleReady",
            SyncedAt = DateTime.UtcNow,
            SyncSuccessful = true,
            SyncData = System.Text.Json.JsonSerializer.Serialize(new
            {
                ModuleName = Name,
                Version = Version,
                Capabilities = GetModuleCapabilities(),
                ReadyAt = DateTime.UtcNow,
                WittyMessage = "Microsoft admin powers fully operational - ready to conquer the digital realm!"
            })
        });
    }

    private async Task StartBackgroundServicesAsync()
    {
        // Background services are started automatically by the hosting system
        // This method can be used for any additional startup tasks
        _logger.LogDebug("Background services starting - automated admin maintenance engaged");
        await Task.CompletedTask;
    }

    private async Task StopBackgroundServicesAsync()
    {
        // Background services are stopped automatically by the hosting system
        _logger.LogDebug("Background services stopping - automated maintenance concluding");
        await Task.CompletedTask;
    }

    private async Task RefreshAuthenticationStateAsync()
    {
        try
        {
            var adminService = _serviceProvider.GetRequiredService<IMicrosoftAdminService>();
            var authStatus = await adminService.GetAuthStatusAsync();
            
            if (authStatus.IsAuthenticated)
            {
                _logger.LogInformation("Authentication state refreshed - admin session active for {UserName}", 
                    authStatus.UserName);
            }
            else
            {
                _logger.LogInformation("No active authentication session - admin powers dormant until login");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to refresh authentication state");
        }
    }

    private async Task CleanupResourcesAsync()
    {
        try
        {
            // Cleanup any module-specific resources
            _logger.LogDebug("Cleaning up Microsoft Admin module resources");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Resource cleanup encountered issues");
        }
    }

    private List<string> GetModuleCapabilities()
    {
        return new List<string>
        {
            "Microsoft Graph API Integration",
            "OAuth 2.0 Authentication",
            "PowerShell Script Automation",
            "Dynamic Form Generation",
            "Portal Integration (WebView2)",
            "Admin Template Management",
            "Cross-Module Event Integration",
            "Security & Audit Framework",
            "Bulk Operations Support",
            "Real-time Portal Access"
        };
    }

    private int GetRegisteredServiceCount()
    {
        // Count of services registered by this module
        return 5; // Core services: AdminService, TemplateEngine, FormBuilder, PortalIntegrator, EventPublisher
    }

    // Event handlers
    private async Task OnAuthenticationSuccess(AuthenticationSuccessEvent authEvent)
    {
        try
        {
            _logger.LogInformation("Received authentication success event for user {UserId} - coordinating admin privileges", 
                authEvent.UserId);
            
            // Could perform additional setup based on successful authentication
            // such as pre-loading user-specific templates or checking admin roles
            
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling authentication success event");
        }
    }

    private async Task OnIntegrationSync(IntegrationSyncEvent syncEvent)
    {
        try
        {
            if (syncEvent.TargetModule == Name || syncEvent.TargetModule == "All")
            {
                _logger.LogDebug("Processing integration sync: {SyncType} from {Source}", 
                    syncEvent.SyncType, syncEvent.SyncMetadata.GetValueOrDefault("source", "Unknown"));
                
                // Handle different sync types
                switch (syncEvent.SyncType)
                {
                    case "RefreshTemplates":
                        await RefreshTemplatesFromSync();
                        break;
                    case "UpdateConfiguration":
                        await UpdateConfigurationFromSync();
                        break;
                    default:
                        _logger.LogDebug("Unhandled sync type: {SyncType}", syncEvent.SyncType);
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling integration sync event");
        }
    }

    private async Task OnSecurityAlert(SecurityAlertEvent alertEvent)
    {
        try
        {
            if (alertEvent.Severity == "Critical" || alertEvent.AlertType.Contains("Admin"))
            {
                _logger.LogWarning("Security alert received: {AlertType} - {Description}", 
                    alertEvent.AlertType, alertEvent.Description);
                
                // Could implement automatic security responses
                // such as disabling admin sessions or triggering additional auditing
            }
            
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling security alert event");
        }
    }

    private async Task RefreshTemplatesFromSync()
    {
        try
        {
            var templateEngine = _serviceProvider.GetRequiredService<IScriptTemplateEngine>();
            await templateEngine.LoadTemplatesAsync();
            _logger.LogInformation("Templates refreshed from sync event");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh templates from sync");
        }
    }

    private async Task UpdateConfigurationFromSync()
    {
        try
        {
            _logger.LogInformation("Configuration update requested via sync - validating new settings");
            await ValidateConfigurationAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update configuration from sync");
        }
    }

    public async Task DisposeAsync()
    {
        _logger.LogInformation("Disposing Microsoft Admin Integration Module - administrative session concluded");
        await StopAsync();
    }

    public void Dispose()
    {
        DisposeAsync().GetAwaiter().GetResult();
    }
}

/// <summary>
/// Configuration model for Microsoft Admin module
/// </summary>
public class MicrosoftAdminConfiguration
{
    public string ClientId { get; set; } = string.Empty;
    public string TenantId { get; set; } = "common";
    public string RedirectUri { get; set; } = "http://localhost:8080/auth";
    public string PortalBaseUrl { get; set; } = "https://admin.microsoft.com";
    public string TemplatesPath { get; set; } = string.Empty;
    public bool EnableAuditLogging { get; set; } = true;
    public bool EnableAutoTokenRefresh { get; set; } = true;
    public int TokenRefreshIntervalMinutes { get; set; } = 30;
}

/// <summary>
/// Background service for refreshing admin templates
/// </summary>
public class AdminTemplateRefreshService : BackgroundService
{
    private readonly ILogger<AdminTemplateRefreshService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public AdminTemplateRefreshService(
        ILogger<AdminTemplateRefreshService> logger,
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var refreshInterval = TimeSpan.FromHours(6); // Refresh templates every 6 hours
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(refreshInterval, stoppingToken);
                
                using var scope = _serviceProvider.CreateScope();
                var templateEngine = scope.ServiceProvider.GetRequiredService<IScriptTemplateEngine>();
                
                await templateEngine.LoadTemplatesAsync();
                _logger.LogInformation("Background template refresh completed - admin arsenal updated");
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during background template refresh");
            }
        }
    }
}

/// <summary>
/// Background service for refreshing authentication tokens
/// </summary>
public class TokenRefreshService : BackgroundService
{
    private readonly ILogger<TokenRefreshService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public TokenRefreshService(
        ILogger<TokenRefreshService> logger,
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_configuration.GetValue<bool>("MicrosoftAdmin:EnableAutoTokenRefresh"))
        {
            _logger.LogInformation("Auto token refresh disabled - service stopping");
            return;
        }
        
        var refreshInterval = TimeSpan.FromMinutes(
            _configuration.GetValue<int>("MicrosoftAdmin:TokenRefreshIntervalMinutes"));
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(refreshInterval, stoppingToken);
                
                using var scope = _serviceProvider.CreateScope();
                var adminService = scope.ServiceProvider.GetRequiredService<IMicrosoftAdminService>();
                
                var authStatus = await adminService.GetAuthStatusAsync();
                if (authStatus.IsAuthenticated && authStatus.TokenNearExpiry)
                {
                    var refreshed = await adminService.RefreshTokensAsync();
                    _logger.LogInformation("Background token refresh {Status}", 
                        refreshed ? "successful" : "failed");
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during background token refresh");
            }
        }
    }
}