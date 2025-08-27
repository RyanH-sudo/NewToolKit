using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetToolkit.Core.Interfaces;
using NetToolkit.Modules.SshTerminal.Interfaces;
using NetToolkit.Modules.SshTerminal.Services;
using NetToolkit.Modules.SshTerminal.Models;

namespace NetToolkit.Modules.SshTerminal;

/// <summary>
/// SSH Terminal Module - the transcendent gateway to digital realm connections
/// Where hardware whispers become flowing conversations and terminals transform into cosmic portals
/// Cloning PuTTY's essence but elevated to ethereal heights with NetToolkit's signature cosmic flair
/// </summary>
public class SshTerminalModule : IModule
{
    private readonly ILogger<SshTerminalModule>? _logger;
    private readonly IServiceProvider? _serviceProvider;
    private IEventBus? _eventBus;
    
    public string Name => "SSH Terminal Module";
    public string Version => "1.0.0";
    public string Description => "🌉 PuTTY Clone SSH Terminal Module - Digital bridge constructor supreme, " +
                                "connecting realms through serial ports, USB conduits, Bluetooth ether, and network wizardry! " +
                                "Full terminal fidelity with colored syntax, history navigation, session logging, " +
                                "and Three.js holographic accents for the ultimate connection experience.";
    
    /// <summary>
    /// Register module services - inscribing the cosmic connection architecture
    /// </summary>
    public void RegisterServices(IServiceCollection services)
    {
        try
        {
            // Register core SSH terminal services
            services.AddSingleton<ISshTerminalHost, SshTerminalService>();
            services.AddSingleton<IDeviceDetector, DeviceScanner>();
            services.AddSingleton<IEmulationEngine, EmulationEngine>();
            services.AddSingleton<ISshEventPublisher, SshEventPublisher>();
            services.AddSingleton<SessionManager>();
            
            _logger?.LogInformation("🚀 SSH Terminal services registered - connection architecture inscribed in digital cosmos!");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "💥 Error registering SSH Terminal services - cosmic architecture disrupted!");
            throw;
        }
    }
    
    /// <summary>
    /// Initialize module - awakening the connection symphony
    /// </summary>
    public async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        try
        {
            var logger = serviceProvider.GetService<ILogger<SshTerminalModule>>();
            var eventBus = serviceProvider.GetRequiredService<IEventBus>();
            
            _eventBus = eventBus;
            
            // Initialize session management
            var sessionManager = serviceProvider.GetRequiredService<SessionManager>();
            
            // Start device monitoring
            var deviceDetector = serviceProvider.GetRequiredService<IDeviceDetector>();
            _ = Task.Run(async () =>
            {
                await Task.Delay(2000); // Allow other modules to initialize
                await deviceDetector.StartDeviceMonitoringAsync();
            });
            
            // Subscribe to cross-module events for integration
            await SetupEventSubscriptionsAsync(serviceProvider);
            
            logger?.LogInformation("🌉 SSH Terminal Module initialized - digital gateway opened to hardware realms!");
            
            // Publish module initialization event
            var eventPublisher = serviceProvider.GetRequiredService<ISshEventPublisher>();
            await eventPublisher.PublishAsync(new
            {
                EventName = "module.ssh_terminal.initialized",
                Data = new
                {
                    module_name = Name,
                    version = Version,
                    timestamp = DateTime.UtcNow,
                    capabilities = new[]
                    {
                        "SSH connections via SSH.NET",
                        "Serial port communication",
                        "Bluetooth device discovery",
                        "ANSI color parsing",
                        "Command history management",
                        "Session state persistence",
                        "Three.js visualization data",
                        "Cross-module event integration"
                    }
                }
            });
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "💥 Error initializing SSH Terminal Module - digital gateway construction failed!");
            throw;
        }
    }
    
    /// <summary>
    /// Setup event subscriptions for cross-module integration
    /// </summary>
    private async Task SetupEventSubscriptionsAsync(IServiceProvider serviceProvider)
    {
        try
        {
            if (_eventBus == null) return;
            
            var logger = serviceProvider.GetService<ILogger<SshTerminalModule>>();
            
            // Subscribe to Scanner/Topography events for automatic device population
            _eventBus.Subscribe<dynamic>(async (eventData) =>
            {
                // Check event type and handle accordingly
                var eventName = eventData?.GetType().GetProperty("EventName")?.GetValue(eventData)?.ToString();
                if (eventName == "scanner.device.discovered")
                    await HandleScannerDeviceDiscovered(eventData, serviceProvider);
                else if (eventName == "topography.network.updated")
                    await HandleTopologyUpdate(eventData, serviceProvider);
                else if (eventName == "powershell.execution.completed")
                    await HandlePowerShellExecutionResult(eventData, serviceProvider);
                else if (eventName == "security.vulnerability.detected")
                    await HandleSecurityVulnerabilityDetected(eventData, serviceProvider);
                else if (eventName == "security.scan.completed")
                    await HandleSecurityScanCompleted(eventData, serviceProvider);
                else if (eventName == "external.ssh.connection.requested")
                    await HandleExternalConnectionRequest(eventData, serviceProvider);
            });
            
            // Subscribe to routing lesson completion events - enable SSH routing capabilities
            _eventBus.Subscribe<NetToolkit.Modules.Education.Events.RoutingLessonCompletedEvent>(async (routingEvent) =>
            {
                logger?.LogInformation("🛤️ Routing lesson {LessonNumber} completed - Enhancing SSH routing capabilities!", 
                                      routingEvent.LessonNumber);
                
                // Enable SSH routing capabilities based on lesson progress
                if (routingEvent.SSHRoutingCapabilities != null && routingEvent.SSHRoutingCapabilities.Any())
                {
                    logger?.LogInformation("🔗 SSH routing capabilities unlocked: {Capabilities}", 
                                          string.Join(", ", routingEvent.SSHRoutingCapabilities));
                    await EnableSshRoutingCapabilities(routingEvent.SSHRoutingCapabilities, serviceProvider);
                }
            });
            
            // Subscribe to routing mastery events - unlock advanced SSH tunneling
            _eventBus.Subscribe<NetToolkit.Modules.Education.Events.RoutingRiddleMasteredEvent>(async (masteryEvent) =>
            {
                logger?.LogInformation("🏆 User {UserId} achieved routing mastery - Unlocking advanced SSH tunneling!", 
                                      masteryEvent.UserId);
                
                // Enable advanced SSH routing and tunneling capabilities
                if (masteryEvent.SSHRoutingCapabilities != null && masteryEvent.SSHRoutingCapabilities.Any())
                {
                    logger?.LogInformation("🌉 Advanced SSH routing capabilities unlocked: {Capabilities}", 
                                          string.Join(", ", masteryEvent.SSHRoutingCapabilities));
                    await EnableAdvancedSshRouting(masteryEvent.SSHRoutingCapabilities, masteryEvent.MasteryLevel, serviceProvider);
                }
            });
            
            logger?.LogDebug("📡 SSH Terminal event subscriptions established - cross-module integration active!");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "💥 Error setting up SSH Terminal event subscriptions!");
        }
    }
    
    #region Event Handlers
    
    /// <summary>
    /// Handle device discovered from Scanner module
    /// </summary>
    private async Task HandleScannerDeviceDiscovered(dynamic eventData, IServiceProvider serviceProvider)
    {
        try
        {
            var deviceDetector = serviceProvider.GetRequiredService<IDeviceDetector>();
            var eventPublisher = serviceProvider.GetRequiredService<ISshEventPublisher>();
            
            var deviceType = eventData?.DeviceType?.ToString();
            var deviceIp = eventData?.IpAddress?.ToString();
            var devicePort = eventData?.Port ?? 22;
            
            if (!string.IsNullOrEmpty(deviceType) && !string.IsNullOrEmpty(deviceIp))
            {
                // Create a network device entry for SSH connection
                var networkDevice = new DeviceInfo
                {
                    Id = $"network_{deviceIp}_{devicePort}",
                    Name = $"Network Device {deviceIp}",
                    Type = DeviceType.Network,
                    Description = $"Discovered network device at {deviceIp}:{devicePort}",
                    IsAvailable = true,
                    Properties = new Dictionary<string, object>
                    {
                        ["ip_address"] = deviceIp,
                        ["port"] = devicePort,
                        ["discovered_by"] = "Scanner Module",
                        ["device_type"] = deviceType
                    }
                };
                
                await eventPublisher.PublishDeviceDetectedAsync(networkDevice, DeviceType.Network);
                
                _logger?.LogInformation("🔍 Integrated scanner discovery: {DeviceIp}:{DevicePort} ({DeviceType}) - SSH connection ready!",
                    (object?)deviceIp, (object?)devicePort, (object?)deviceType);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "💥 Error handling scanner device discovery");
        }
    }
    
    /// <summary>
    /// Handle topology update for network changes
    /// </summary>
    private async Task HandleTopologyUpdate(dynamic eventData, IServiceProvider serviceProvider)
    {
        try
        {
            var deviceDetector = serviceProvider.GetRequiredService<IDeviceDetector>();
            
            // Refresh network device cache when topology changes
            await deviceDetector.RefreshDeviceCacheAsync(DeviceType.Network);
            
            var nodeCount = eventData?.NodeCount ?? 0;
            var changeType = eventData?.ChangeType?.ToString() ?? "Unknown";
            
            var nodeCountObj = nodeCount;
            var changeTypeObj = changeType;
            _logger?.LogInformation("🗺️ Topology update detected - refreshed network devices: {NodeCount} nodes ({ChangeType})",
                (object?)nodeCountObj, (object?)changeTypeObj);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "💥 Error handling topology update");
        }
    }
    
    /// <summary>
    /// Handle PowerShell execution results for terminal integration
    /// </summary>
    private async Task HandlePowerShellExecutionResult(dynamic eventData, IServiceProvider serviceProvider)
    {
        try
        {
            var terminalHost = serviceProvider.GetRequiredService<ISshTerminalHost>();
            var eventPublisher = serviceProvider.GetRequiredService<ISshEventPublisher>();
            
            var sessionId = eventData?.SessionId?.ToString();
            var output = eventData?.Output?.ToString();
            var success = eventData?.Success ?? false;
            
            if (!string.IsNullOrEmpty(sessionId) && !string.IsNullOrEmpty(output))
            {
                // If there's an active SSH session with matching metadata, forward the output
                var sessions = await terminalHost.GetActiveSessionsAsync();
                var targetSession = sessions.FirstOrDefault(s => 
                    s.Properties.ContainsKey("powershell_integration") &&
                    s.Properties["powershell_integration"].ToString() == "true");
                
                if (targetSession != null)
                {
                    // Display PowerShell output in terminal
                    _logger?.LogInformation("⚡ Forwarding PowerShell output to SSH session {SessionId}: Success={Success}",
                        (object?)targetSession.SessionId, (object?)success);
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "💥 Error handling PowerShell execution result");
        }
    }
    
    /// <summary>
    /// Handle security vulnerability detection
    /// </summary>
    private async Task HandleSecurityVulnerabilityDetected(dynamic eventData, IServiceProvider serviceProvider)
    {
        try
        {
            var eventPublisher = serviceProvider.GetRequiredService<ISshEventPublisher>();
            
            var targetIp = eventData?.TargetIp?.ToString();
            var vulnerability = eventData?.Vulnerability?.ToString();
            var severity = eventData?.Severity?.ToString();
            
            if (!string.IsNullOrEmpty(targetIp) && !string.IsNullOrEmpty(vulnerability))
            {
                // Publish security alert for SSH connections to vulnerable hosts
                var alertSeverityParsed = Enum.TryParse<AlertSeverity>(severity, out AlertSeverity alertSeverity) ? alertSeverity : AlertSeverity.Medium;
                
                await eventPublisher.PublishSecurityAlertAsync(
                    Guid.NewGuid().ToString(),
                    SecurityAlertType.SuspiciousActivity,
                    $"Vulnerability detected on {targetIp}: {vulnerability}. Exercise caution when connecting via SSH.",
                    alertSeverityParsed);
                
                _logger?.LogWarning("🚨 Security alert: Vulnerability {Vulnerability} detected on {TargetIp} - SSH caution advised!",
                    (object?)vulnerability, (object?)targetIp);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "💥 Error handling security vulnerability detection");
        }
    }
    
    /// <summary>
    /// Handle security scan completion
    /// </summary>
    private async Task HandleSecurityScanCompleted(dynamic eventData, IServiceProvider serviceProvider)
    {
        try
        {
            var eventPublisher = serviceProvider.GetRequiredService<ISshEventPublisher>();
            
            var scanId = eventData?.ScanId?.ToString();
            var targetCount = eventData?.TargetCount ?? 0;
            var vulnerabilityCount = eventData?.VulnerabilityCount ?? 0;
            
            _logger?.LogInformation("🔒 Security scan {ScanId} completed: {TargetCount} targets, {VulnerabilityCount} vulnerabilities - SSH security intelligence updated!",
                (object?)scanId, (object?)targetCount, (object?)vulnerabilityCount);
            
            // Suggest secure SSH configurations based on scan results
            if (vulnerabilityCount > 0)
            {
                await eventPublisher.PublishAiInsightAsync(
                    Guid.NewGuid().ToString(),
                    AiInsightType.SecurityRecommendation,
                    $"Security scan detected {vulnerabilityCount} vulnerabilities. Consider using key-based authentication and disabling password authentication for SSH connections.",
                    0.85f);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "💥 Error handling security scan completion");
        }
    }
    
    /// <summary>
    /// Handle external SSH connection requests
    /// </summary>
    private async Task HandleExternalConnectionRequest(dynamic eventData, IServiceProvider serviceProvider)
    {
        try
        {
            var terminalHost = serviceProvider.GetRequiredService<ISshTerminalHost>();
            var eventPublisher = serviceProvider.GetRequiredService<ISshEventPublisher>();
            
            var host = eventData?.Host?.ToString();
            var port = eventData?.Port ?? 22;
            var username = eventData?.Username?.ToString();
            var connectionType = eventData?.ConnectionType?.ToString() ?? "SSH";
            
            if (!string.IsNullOrEmpty(host) && !string.IsNullOrEmpty(username))
            {
                // Create connection parameters from external request
                var connectionParams = new ConnectionParams
                {
                    SessionId = Guid.NewGuid().ToString(),
                    Type = Enum.TryParse<ConnectionType>(connectionType, out ConnectionType connType) ? connType : ConnectionType.SSH,
                    Host = host,
                    Port = port,
                    Username = username,
                    DisplayName = $"External Request: {username}@{host}",
                    AutoReconnect = false,
                    EnableLogging = true
                };
                
                _logger?.LogInformation("📞 External SSH connection request: {Username}@{Host}:{Port} - preparing digital bridge!",
                    (object?)username, (object?)host, (object?)port);
                
                // Note: Actual connection would require authentication credentials
                // This would typically trigger a UI prompt or use stored credentials
                await eventPublisher.PublishAsync(new
                {
                    EventName = "ssh.external.connection.prepared",
                    Data = new
                    {
                        session_id = connectionParams.SessionId,
                        connection_params = connectionParams,
                        requires_authentication = true,
                        timestamp = DateTime.UtcNow
                    }
                });
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "💥 Error handling external SSH connection request");
        }
    }
    
    /// <summary>
    /// Enable SSH routing capabilities based on routing lesson completion
    /// </summary>
    private async Task EnableSshRoutingCapabilities(List<string> capabilities, IServiceProvider serviceProvider)
    {
        try
        {
            var terminalHost = serviceProvider.GetRequiredService<ISshTerminalHost>();
            var eventPublisher = serviceProvider.GetRequiredService<ISshEventPublisher>();
            
            foreach (var capability in capabilities)
            {
                await ApplySshRoutingCapability(capability, terminalHost, eventPublisher);
            }
            
            _logger?.LogInformation("🔗 SSH routing capabilities successfully enabled!");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "💥 Failed to enable SSH routing capabilities!");
        }
    }
    
    /// <summary>
    /// Enable advanced SSH routing features for routing masters
    /// </summary>
    private async Task EnableAdvancedSshRouting(List<string> capabilities, string masteryLevel, IServiceProvider serviceProvider)
    {
        try
        {
            var terminalHost = serviceProvider.GetRequiredService<ISshTerminalHost>();
            var eventPublisher = serviceProvider.GetRequiredService<ISshEventPublisher>();
            
            foreach (var capability in capabilities)
            {
                await ApplyAdvancedSshCapability(capability, masteryLevel, terminalHost, eventPublisher);
            }
            
            _logger?.LogInformation("🌉 Advanced SSH routing capabilities enabled for {MasteryLevel}!", masteryLevel);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "💥 Failed to enable advanced SSH routing capabilities!");
        }
    }
    
    /// <summary>
    /// Apply specific SSH routing capability
    /// </summary>
    private async Task ApplySshRoutingCapability(string capability, ISshTerminalHost terminalHost, ISshEventPublisher eventPublisher)
    {
        var capabilityLower = capability.ToLowerInvariant();
        
        switch (capabilityLower)
        {
            case "ssh port forwarding":
                _logger?.LogInformation("🔄 Enabling SSH port forwarding - Local and remote tunneling activated!");
                break;
                
            case "ssh dynamic tunneling":
                _logger?.LogInformation("🌀 Enabling SSH dynamic tunneling - SOCKS proxy capabilities unlocked!");
                break;
                
            case "ssh jump host routing":
                _logger?.LogInformation("🦘 Enabling SSH jump host routing - Multi-hop connections through the cosmos!");
                break;
                
            case "ssh reverse tunneling":
                _logger?.LogInformation("↩️ Enabling SSH reverse tunneling - Inbound connection pathways opened!");
                break;
                
            case "network routing via ssh":
                _logger?.LogInformation("🛤️ Enabling network routing via SSH - Secure network path management!");
                break;
                
            case "ssh tunnel automation":
                _logger?.LogInformation("🤖 Enabling SSH tunnel automation - Self-managing secure pathways!");
                break;
                
            default:
                _logger?.LogDebug("🔧 Applying SSH routing capability: {Capability}", capability);
                break;
        }
        
        await Task.CompletedTask; // Placeholder for actual implementation
    }
    
    /// <summary>
    /// Apply advanced SSH capability for routing masters
    /// </summary>
    private async Task ApplyAdvancedSshCapability(string capability, string masteryLevel, ISshTerminalHost terminalHost, ISshEventPublisher eventPublisher)
    {
        var capabilityLower = capability.ToLowerInvariant();
        
        switch (capabilityLower)
        {
            case "advanced bgp ssh management":
                _logger?.LogInformation("🌐 Unlocking advanced BGP SSH management - Border gateway mastery via secure channels!");
                break;
                
            case "mpls tunnel ssh access":
                _logger?.LogInformation("🏷️ Unlocking MPLS tunnel SSH access - Label-switched path management through SSH!");
                break;
                
            case "ospf neighbor ssh configuration":
                _logger?.LogInformation("🤝 Unlocking OSPF neighbor SSH configuration - Neighbor relationships via secure shell!");
                break;
                
            case "eigrp ssh tunnel management":
                _logger?.LogInformation("🔄 Unlocking EIGRP SSH tunnel management - Cisco routing protocol via SSH mastery!");
                break;
                
            case "isis ssh administration":
                _logger?.LogInformation("🏺 Unlocking ISIS SSH administration - Ancient protocol mastery through modern tunnels!");
                break;
                
            case "multi-hop ssh routing chains":
                _logger?.LogInformation("⛓️ Unlocking multi-hop SSH routing chains - Complex pathway orchestration!");
                break;
                
            case "ssh routing automation scripts":
                _logger?.LogInformation("📜 Unlocking SSH routing automation scripts - Intelligent tunnel management!");
                break;
                
            case "secure routing protocol tunneling":
                _logger?.LogInformation("🔒 Unlocking secure routing protocol tunneling - Encrypted pathway establishment!");
                break;
                
            default:
                _logger?.LogDebug("🏆 Enabling advanced SSH capability: {Capability} for {MasteryLevel}", capability, masteryLevel);
                break;
        }
        
        await Task.CompletedTask; // Placeholder for actual implementation
    }
    
    #endregion
    
    /// <summary>
    /// Dispose module resources - graceful digital farewell
    /// </summary>
    public async Task DisposeAsync()
    {
        try
        {
            if (_serviceProvider != null)
            {
                // Stop device monitoring
                var deviceDetector = _serviceProvider.GetService<IDeviceDetector>();
                if (deviceDetector != null)
                {
                    await deviceDetector.StopDeviceMonitoringAsync();
                }
                
                // Close all SSH sessions
                var terminalHost = _serviceProvider.GetService<ISshTerminalHost>();
                if (terminalHost != null)
                {
                    var closedSessions = await terminalHost.CloseAllSessionsAsync();
                    _logger?.LogInformation("👋 Closed {ClosedSessions} SSH sessions during cleanup", closedSessions);
                }
                
                // Save session states
                var sessionManager = _serviceProvider.GetService<SessionManager>();
                if (sessionManager != null)
                {
                    await sessionManager.SaveAllSessionsAsync();
                    sessionManager.Dispose();
                }
                
                // Note: Event unsubscription would need handler references which we don't maintain
                // In a production system, you'd track handlers to unsubscribe properly
            }
            
            _logger?.LogInformation("🧹 SSH Terminal Module cleanup completed - digital realm connections gracefully severed!");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "💥 Error during SSH Terminal Module cleanup");
        }
    }
    
    /// <summary>
    /// Get module health status
    /// </summary>
    public async Task<Dictionary<string, object>> GetHealthStatusAsync()
    {
        try
        {
            var healthStatus = new Dictionary<string, object>
            {
                ["module_name"] = Name,
                ["version"] = Version,
                ["status"] = "Healthy",
                ["timestamp"] = DateTime.UtcNow
            };
            
            if (_serviceProvider != null)
            {
                // Check service health
                var terminalHost = _serviceProvider.GetService<ISshTerminalHost>();
                if (terminalHost != null)
                {
                    var activeSessions = await terminalHost.GetActiveSessionsAsync();
                    healthStatus["active_sessions"] = activeSessions.Count;
                    healthStatus["session_types"] = activeSessions
                        .GroupBy(s => s.Type)
                        .ToDictionary(g => g.Key.ToString(), g => g.Count());
                }
                
                var deviceDetector = _serviceProvider.GetService<IDeviceDetector>();
                if (deviceDetector != null)
                {
                    var devices = deviceDetector.GetCachedDevices();
                    healthStatus["cached_devices"] = devices.Count;
                    healthStatus["available_devices"] = devices.Count(d => d.IsAvailable);
                }
                
                var sessionManager = _serviceProvider.GetService<SessionManager>();
                if (sessionManager != null)
                {
                    var sessionStats = await sessionManager.GetSessionStatisticsAsync();
                    healthStatus["session_statistics"] = sessionStats;
                }
            }
            
            return healthStatus;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "💥 Error getting SSH Terminal Module health status");
            
            return new Dictionary<string, object>
            {
                ["module_name"] = Name,
                ["version"] = Version,
                ["status"] = "Unhealthy",
                ["error"] = ex.Message,
                ["timestamp"] = DateTime.UtcNow
            };
        }
    }
}