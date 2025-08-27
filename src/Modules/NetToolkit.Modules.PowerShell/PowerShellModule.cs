using Microsoft.Extensions.DependencyInjection;
using NetToolkit.Core.Modules;
using NetToolkit.Core.Interfaces;
using NetToolkit.Modules.PowerShell.Interfaces;
using NetToolkit.Modules.PowerShell.Services;

namespace NetToolkit.Modules.PowerShell;

/// <summary>
/// PowerShell Module - The beating heart of administrative wizardry!
/// Where terminal commands become reality and scripts transform into digital alchemy.
/// This is not just a module; it's the command center of network domination! 🎭
/// </summary>
public class PowerShellModule : BaseModule
{
    public override string Name => "PowerShell Terminal Module";
    public override string Version => "1.0.0";
    
    private IPowerShellHost? _powerShellHost;
    private IScriptTemplateService? _templateService;
    private IMicrosoftIntegrationService? _microsoftService;
    private ITerminalEventPublisher? _eventPublisher;
    
    /// <summary>
    /// Initialize the module - awakening the PowerShell spirits
    /// </summary>
    protected override async Task OnInitializeAsync()
    {
        try
        {
            Logger.LogInfo("🚀 Initializing PowerShell Terminal Module - Preparing the digital forge...");
            
            // Register services in the dependency injection container
            RegisterServices();
            
            // Get service instances
            _powerShellHost = ServiceProvider.GetRequiredService<IPowerShellHost>();
            _templateService = ServiceProvider.GetRequiredService<IScriptTemplateService>();
            _microsoftService = ServiceProvider.GetRequiredService<IMicrosoftIntegrationService>();
            _eventPublisher = ServiceProvider.GetRequiredService<ITerminalEventPublisher>();
            
            // Subscribe to core events for cross-module integration
            await SubscribeToCoreEventsAsync();
            
            // Initialize built-in script templates
            await InitializeScriptTemplatesAsync();
            
            // Test PowerShell environment
            await TestPowerShellEnvironmentAsync();
            
            Logger.LogInfo("✅ PowerShell Terminal Module initialized successfully - The digital forge burns bright! 🔥");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "❌ Failed to initialize PowerShell Terminal Module - The forge remains cold!");
            throw;
        }
    }
    
    /// <summary>
    /// Dispose the module - cooling the digital forge
    /// </summary>
    protected override async Task OnDisposeAsync()
    {
        try
        {
            Logger.LogInfo("🌙 Disposing PowerShell Terminal Module - The forge grows cold...");
            
            // Dispose PowerShell host
            if (_powerShellHost is IDisposable disposableHost)
            {
                disposableHost.Dispose();
            }
            
            Logger.LogInfo("✅ PowerShell Terminal Module disposed gracefully - The digital spirits rest in peace");
            
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "⚠️ Error during PowerShell Terminal Module disposal - Chaos in the digital realm!");
        }
    }
    
    /// <summary>
    /// Register services in dependency injection container - assembling the digital toolbox
    /// </summary>
    private void RegisterServices()
    {
        var services = new ServiceCollection();
        
        // Register PowerShell services
        services.AddSingleton<IPowerShellHost, SimplifiedPowerShellHostService>();
        services.AddSingleton<IScriptTemplateService, ScriptTemplateService>();
        services.AddSingleton<IMicrosoftIntegrationService, SimplifiedMicrosoftIntegrationService>();
        services.AddSingleton<ITerminalEventPublisher, TerminalEventPublisher>();
        
        // Register existing core services
        var coreLogger = ServiceProvider.GetRequiredService<ILoggerWrapper>();
        var coreEventBus = ServiceProvider.GetRequiredService<IEventBus>();
        
        services.AddSingleton(coreLogger);
        services.AddSingleton(coreEventBus);
        
        // Build service provider and register with core
        var moduleServiceProvider = services.BuildServiceProvider();
        
        // Replace the ServiceProvider with our module-specific one
        // Note: In a real implementation, you'd want a more sophisticated approach
        var serviceProviderField = typeof(BaseModule).GetField("ServiceProvider", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        serviceProviderField?.SetValue(this, moduleServiceProvider);
        
        Logger.LogDebug("🔧 PowerShell module services registered - The toolbox is assembled!");
    }
    
    /// <summary>
    /// Subscribe to core events for cross-module integration - digital telepathy
    /// </summary>
    private async Task SubscribeToCoreEventsAsync()
    {
        // Subscribe to network scan completed events
        EventBus.Subscribe<NetToolkit.Core.Events.NetworkScanCompletedEvent>(async (scanEvent) =>
        {
            Logger.LogInfo("🔍 Network scan completed - {DeviceCount} devices found. Suggesting configuration scripts...", 
                          scanEvent.Devices.Count);
            
            // Could suggest relevant scripts based on discovered devices
            if (scanEvent.Devices.Any(d => d.OpenPorts.Contains(22)))
            {
                Logger.LogInfo("🔧 SSH-enabled devices detected - SSH configuration scripts are available!");
            }
            
            if (scanEvent.Devices.Any(d => d.OpenPorts.Contains(80) || d.OpenPorts.Contains(443)))
            {
                Logger.LogInfo("🌐 Web servers detected - Web configuration scripts are available!");
            }
        });
        
        // Subscribe to script execution events for logging and monitoring
        EventBus.Subscribe<ScriptExecutionCompletedEvent>(async (scriptEvent) =>
        {
            Logger.LogInfo("📊 Script execution statistics logged - {ScriptName}: {Success} in {Duration}ms", 
                          scriptEvent.ScriptName, 
                          scriptEvent.Success ? "SUCCESS" : "FAILED",
                          scriptEvent.Duration.TotalMilliseconds);
            
            // Could trigger follow-up actions based on script results
            if (!scriptEvent.Success && scriptEvent.Errors.Any())
            {
                Logger.LogWarn("🚨 Script failure detected - Consider running diagnostic templates");
            }
        });
        
        // Subscribe to routing lesson completion events - enable routing automation scripts
        EventBus.Subscribe<NetToolkit.Modules.Education.Events.RoutingLessonCompletedEvent>(async (routingEvent) =>
        {
            Logger.LogInfo("🛤️ Routing lesson {LessonNumber} completed by user {UserId} - Unlocking routing automation scripts!", 
                          routingEvent.LessonNumber, routingEvent.UserId);
            
            // Enable routing-specific PowerShell scripts based on lesson progress
            if (routingEvent.RoutingScripts.Any())
            {
                Logger.LogInfo("⚡ Routing scripts unlocked: {Scripts}", string.Join(", ", routingEvent.RoutingScripts));
                
                // Could dynamically add routing script templates based on lesson completion
                if (_templateService != null)
                {
                    await AddRoutingScriptTemplatesAsync(routingEvent.RoutingScripts, routingEvent.LessonNumber);
                }
            }
            
            // Log troubleshooting tools availability
            if (routingEvent.TroubleshootingTools.Any())
            {
                Logger.LogInfo("🔧 Routing troubleshooting tools available: {Tools}", 
                              string.Join(", ", routingEvent.TroubleshootingTools));
            }
        });
        
        // Subscribe to routing mastery events - unlock advanced routing automation
        EventBus.Subscribe<NetToolkit.Modules.Education.Events.RoutingRiddleMasteredEvent>(async (masteryEvent) =>
        {
            Logger.LogInfo("🏆 User {UserId} achieved routing mastery level: {MasteryLevel}! Unlocking advanced automation...", 
                          masteryEvent.UserId, masteryEvent.MasteryLevel);
            
            // Enable advanced PowerShell routing automation scripts
            if (masteryEvent.AutomationScripts.Any())
            {
                Logger.LogInfo("🤖 Advanced routing automation scripts unlocked: {Scripts}", 
                              string.Join(", ", masteryEvent.AutomationScripts));
                
                if (_templateService != null)
                {
                    await AddAdvancedRoutingTemplatesAsync(masteryEvent.AutomationScripts, masteryEvent.MasteryLevel);
                }
            }
            
            // Log diagnostic tools availability
            if (masteryEvent.DiagnosticTools.Any())
            {
                Logger.LogInfo("🧰 Advanced routing diagnostic tools available: {Tools}", 
                              string.Join(", ", masteryEvent.DiagnosticTools));
            }
        });
        
        Logger.LogDebug("📡 Event subscriptions established - Digital telepathy activated!");
        await Task.CompletedTask;
    }
    
    /// <summary>
    /// Initialize built-in script templates - inscribing the sacred scrolls
    /// </summary>
    private async Task InitializeScriptTemplatesAsync()
    {
        try
        {
            if (_templateService == null) return;
            
            var templates = await _templateService.LoadTemplatesAsync();
            
            Logger.LogInfo("📜 Script templates loaded - {Count} spells available in the grimoire", templates.Count);
            
            // Log template categories for visibility
            var categories = await _templateService.GetCategoriesAsync();
            if (categories.Any())
            {
                Logger.LogInfo("📚 Template categories available: {Categories}", string.Join(", ", categories));
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to initialize script templates - The grimoire remains sealed!");
        }
    }
    
    /// <summary>
    /// Test PowerShell environment - ensuring the digital realm is functional
    /// </summary>
    private async Task TestPowerShellEnvironmentAsync()
    {
        try
        {
            if (_powerShellHost == null) return;
            
            // Test basic PowerShell functionality
            var testResult = await _powerShellHost.ExecuteCommandAsync("Get-Date");
            
            if (testResult.Success)
            {
                Logger.LogInfo("✅ PowerShell environment test passed - The digital spirits respond to our call!");
            }
            else
            {
                Logger.LogWarn("⚠️ PowerShell environment test failed - The spirits are restless! Error: {Error}", 
                              testResult.ErrorOutput);
            }
            
            // Test available cmdlets
            var cmdlets = await _powerShellHost.GetAvailableCommandsAsync();
            Logger.LogInfo("🎯 PowerShell cmdlets available - {Count} commands in the digital arsenal", cmdlets.Count);
            
            // Get session state
            var sessionState = _powerShellHost.GetSessionState();
            Logger.LogInfo("🏠 PowerShell session state - Host: {Host}, User: {User}, Location: {Location}", 
                          sessionState.HostName, sessionState.UserName, sessionState.CurrentLocation);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "PowerShell environment test failed - The digital realm is corrupted!");
        }
    }
    
    /// <summary>
    /// Get PowerShell host service - external access to the terminal engine
    /// </summary>
    public IPowerShellHost? GetPowerShellHost() => _powerShellHost;
    
    /// <summary>
    /// Get script template service - external access to the spell grimoire
    /// </summary>
    public IScriptTemplateService? GetTemplateService() => _templateService;
    
    /// <summary>
    /// Get Microsoft integration service - external access to the Office 365 realm
    /// </summary>
    public IMicrosoftIntegrationService? GetMicrosoftService() => _microsoftService;
    
    /// <summary>
    /// Get terminal event publisher - external access to the digital broadcaster
    /// </summary>
    public ITerminalEventPublisher? GetEventPublisher() => _eventPublisher;
    
    /// <summary>
    /// Execute a quick test command - diagnostic magic
    /// </summary>
    public async Task<bool> ExecuteTestCommandAsync()
    {
        try
        {
            if (_powerShellHost == null) return false;
            
            var result = await _powerShellHost.ExecuteCommandAsync("Write-Output 'NetToolkit PowerShell Module Test - Digital magic is alive!'");
            
            Logger.LogInfo("🧪 Test command executed - Result: {Success}", result.Success ? "SUCCESS" : "FAILED");
            
            return result.Success;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Test command failed - The diagnostic spell backfired!");
            return false;
        }
    }
    
    /// <summary>
    /// Get module statistics - digital introspection
    /// </summary>
    public async Task<PowerShellModuleStats> GetModuleStatsAsync()
    {
        try
        {
            var stats = new PowerShellModuleStats
            {
                ModuleName = Name,
                ModuleVersion = Version,
                IsInitialized = _powerShellHost != null,
                TemplateCount = _templateService != null ? (await _templateService.LoadTemplatesAsync()).Count : 0,
                CommandHistoryCount = _powerShellHost?.GetCommandHistory().Count ?? 0,
                SessionState = _powerShellHost?.GetSessionState() ?? new PowerShellSessionState()
            };
            
            Logger.LogDebug("📊 Module statistics retrieved - {TemplateCount} templates, {HistoryCount} commands in history", 
                           stats.TemplateCount, stats.CommandHistoryCount);
            
            return stats;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to retrieve module statistics - The digital accountant is on strike!");
            
            return new PowerShellModuleStats
            {
                ModuleName = Name,
                ModuleVersion = Version,
                IsInitialized = false
            };
        }
    }
    
    /// <summary>
    /// Add routing script templates based on lesson completion
    /// </summary>
    private async Task AddRoutingScriptTemplatesAsync(List<string> routingScripts, int lessonNumber)
    {
        try
        {
            foreach (var scriptName in routingScripts)
            {
                var template = CreateRoutingScriptTemplate(scriptName, lessonNumber);
                if (template != null && _templateService != null)
                {
                    await _templateService.SaveTemplateAsync(template);
                    Logger.LogInfo("📜 Routing script template '{TemplateName}' added to grimoire", template.Name);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to add routing script templates - The routing scrolls resist inscription!");
        }
    }
    
    /// <summary>
    /// Add advanced routing templates for mastery achievements
    /// </summary>
    private async Task AddAdvancedRoutingTemplatesAsync(List<string> automationScripts, string masteryLevel)
    {
        try
        {
            foreach (var scriptName in automationScripts)
            {
                var template = CreateAdvancedRoutingTemplate(scriptName, masteryLevel);
                if (template != null && _templateService != null)
                {
                    await _templateService.SaveTemplateAsync(template);
                    Logger.LogInfo("🏆 Advanced routing template '{TemplateName}' unlocked for {MasteryLevel}", 
                                  template.Name, masteryLevel);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to add advanced routing templates - The master scrolls remain sealed!");
        }
    }
    
    /// <summary>
    /// Create routing script template based on lesson content
    /// </summary>
    private ScriptTemplate? CreateRoutingScriptTemplate(string scriptName, int lessonNumber)
    {
        return scriptName.ToLowerInvariant() switch
        {
            "show-routing-table" => new ScriptTemplate
            {
                Id = $"routing-table-lesson-{lessonNumber}",
                Name = "Show Routing Table",
                Description = "Display the system's routing table - peek into the path-finding wisdom!",
                Category = "Routing Basics",
                RiskLevel = TemplateRisk.Low,
                Template = @"
# Display Routing Table - Unlocked from Lesson {LessonNumber}
# The ancient art of path discovery revealed! 🛤️

try {
    Write-Host ""🗺️ Displaying routing table - Unveiling the network's secret paths..."" -ForegroundColor Cyan
    
    if ($IsLinux -or $IsMacOS) {
        # Unix-like systems
        Write-Host ""📍 Using 'route' command for Unix systems..."" -ForegroundColor Yellow
        route -n
        Write-Host """"
        Write-Host ""📍 Using 'ip route' for detailed information..."" -ForegroundColor Yellow
        ip route show
    } else {
        # Windows systems
        Write-Host ""📍 Using 'route print' for Windows systems..."" -ForegroundColor Yellow
        route print
        Write-Host """"
        Write-Host ""📍 Using PowerShell Get-NetRoute for detailed info..."" -ForegroundColor Yellow
        Get-NetRoute | Format-Table -AutoSize
    }
    
    Write-Host ""✅ Routing table displayed successfully - The paths are revealed!"" -ForegroundColor Green
}
catch {
    Write-Error ""❌ Failed to display routing table: $($_.Exception.Message)""
    Write-Host ""The routing spirits are shy today! Check permissions and network configuration."" -ForegroundColor Red
}",
                Parameters = new List<TemplateParameter>(),
                Tags = { "routing", "table", "network", "paths" },
                Author = "NetToolkit Education Module"
            },
            
            "test-network-connectivity" => new ScriptTemplate
            {
                Id = $"connectivity-test-lesson-{lessonNumber}",
                Name = "Test Network Connectivity", 
                Description = "Test network connectivity to discover routing paths",
                Category = "Routing Diagnostics",
                RiskLevel = TemplateRisk.Low,
                Template = @"
# Network Connectivity Test - Unlocked from Lesson {LessonNumber}
# Testing the digital highways! 🚗

param(
    [string]$TargetHost = ""{TargetHost}"",
    [int]$Count = {Count}
)

try {
    Write-Host ""🎯 Testing connectivity to $TargetHost - Launching digital ping wizardry..."" -ForegroundColor Cyan
    
    # Ping test
    Write-Host ""📡 Running ping test..."" -ForegroundColor Yellow
    if ($IsLinux -or $IsMacOS) {
        ping -c $Count $TargetHost
    } else {
        ping -n $Count $TargetHost
    }
    
    Write-Host """"
    Write-Host ""🛤️ Tracing route to discover the path..."" -ForegroundColor Yellow
    if ($IsLinux -or $IsMacOS) {
        traceroute $TargetHost
    } else {
        tracert $TargetHost
    }
    
    Write-Host ""✅ Connectivity test complete - The digital paths are mapped!"" -ForegroundColor Green
}
catch {
    Write-Error ""❌ Connectivity test failed: $($_.Exception.Message)""
    Write-Host ""The network spirits are elusive! Check target host and network configuration."" -ForegroundColor Red
}",
                Parameters = new List<TemplateParameter>
                {
                    new()
                    {
                        Name = "TargetHost",
                        DisplayName = "Target Host",
                        Description = "Host to test connectivity to",
                        Type = ParameterType.String,
                        IsRequired = true,
                        DefaultValue = "8.8.8.8",
                        PlaceholderText = "google.com or 192.168.1.1"
                    },
                    new()
                    {
                        Name = "Count",
                        DisplayName = "Ping Count",
                        Description = "Number of ping attempts",
                        Type = ParameterType.Integer,
                        IsRequired = false,
                        DefaultValue = 4
                    }
                },
                Tags = { "routing", "ping", "traceroute", "connectivity" },
                Author = "NetToolkit Education Module"
            },
            
            _ => null
        };
    }
    
    /// <summary>
    /// Create advanced routing template for mastery achievements
    /// </summary>
    private ScriptTemplate? CreateAdvancedRoutingTemplate(string scriptName, string masteryLevel)
    {
        return scriptName.ToLowerInvariant() switch
        {
            "configure-static-routes" => new ScriptTemplate
            {
                Id = $"static-routes-{masteryLevel.ToLower()}",
                Name = "Configure Static Routes",
                Description = $"Advanced static route configuration - Mastery level: {masteryLevel}",
                Category = "Advanced Routing",
                RiskLevel = TemplateRisk.High,
                Template = @"
# Advanced Static Route Configuration - {MasteryLevel} Achievement
# Wielding the power of manual path manipulation! ⚡

param(
    [string]$Network = ""{Network}"",
    [string]$SubnetMask = ""{SubnetMask}"",
    [string]$Gateway = ""{Gateway}"",
    [string]$Interface = ""{Interface}""
)

try {
    Write-Host ""🏗️ Configuring static route - Network: $Network via $Gateway"" -ForegroundColor Cyan
    Write-Host ""⚠️ WARNING: This is advanced routing configuration - Use with caution!"" -ForegroundColor Yellow
    
    if ($IsLinux -or $IsMacOS) {
        # Unix-like systems
        Write-Host ""📍 Adding static route on Unix system..."" -ForegroundColor Yellow
        sudo ip route add $Network/$SubnetMask via $Gateway dev $Interface
        Write-Host ""✅ Static route added successfully!"" -ForegroundColor Green
        
        # Verify the route
        Write-Host ""🔍 Verifying the new route..."" -ForegroundColor Cyan
        ip route show | grep $Network
    } else {
        # Windows systems  
        Write-Host ""📍 Adding static route on Windows system..."" -ForegroundColor Yellow
        route add $Network mask $SubnetMask $Gateway
        Write-Host ""✅ Static route added successfully!"" -ForegroundColor Green
        
        # Verify the route
        Write-Host ""🔍 Verifying the new route..."" -ForegroundColor Cyan
        route print | Select-String $Network
    }
    
    Write-Host ""🏆 Route configuration complete - You have achieved routing mastery!"" -ForegroundColor Green
}
catch {
    Write-Error ""❌ Static route configuration failed: $($_.Exception.Message)""
    Write-Host ""The routing gods demand more preparation! Check parameters and permissions."" -ForegroundColor Red
}",
                Parameters = new List<TemplateParameter>
                {
                    new()
                    {
                        Name = "Network",
                        DisplayName = "Network Address",
                        Description = "Target network address",
                        Type = ParameterType.IPAddress,
                        IsRequired = true,
                        PlaceholderText = "192.168.100.0"
                    },
                    new()
                    {
                        Name = "SubnetMask",
                        DisplayName = "Subnet Mask", 
                        Description = "Network subnet mask",
                        Type = ParameterType.String,
                        IsRequired = true,
                        DefaultValue = "255.255.255.0",
                        PlaceholderText = "255.255.255.0 or /24"
                    },
                    new()
                    {
                        Name = "Gateway",
                        DisplayName = "Gateway Address",
                        Description = "Gateway IP address",
                        Type = ParameterType.IPAddress,
                        IsRequired = true,
                        PlaceholderText = "192.168.1.1"
                    },
                    new()
                    {
                        Name = "Interface",
                        DisplayName = "Network Interface",
                        Description = "Network interface name (Unix/Linux only)",
                        Type = ParameterType.String,
                        IsRequired = false,
                        DefaultValue = "eth0",
                        PlaceholderText = "eth0, wlan0, etc."
                    }
                },
                Tags = { "advanced-routing", "static-routes", "configuration", masteryLevel.ToLower() },
                Author = "NetToolkit Education Module"
            },
            
            _ => null
        };
    }
}

/// <summary>
/// PowerShell module statistics - the vital signs of digital productivity
/// </summary>
public class PowerShellModuleStats
{
    public string ModuleName { get; set; } = string.Empty;
    public string ModuleVersion { get; set; } = string.Empty;
    public bool IsInitialized { get; set; }
    public int TemplateCount { get; set; }
    public int CommandHistoryCount { get; set; }
    public PowerShellSessionState SessionState { get; set; } = new();
    public DateTime RetrievedAt { get; set; } = DateTime.UtcNow;
}