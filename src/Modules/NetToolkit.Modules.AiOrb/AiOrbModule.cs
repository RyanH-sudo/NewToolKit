using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetToolkit.Core.Interfaces;
using NetToolkit.Modules.AiOrb.Events;
using NetToolkit.Modules.AiOrb.Interfaces;
using NetToolkit.Modules.AiOrb.Services;

namespace NetToolkit.Modules.AiOrb;

/// <summary>
/// AI Orb Module - The floating intelligence nexus of NetToolkit
/// Provides comprehensive AI assistance through chat, OCR analysis, and CLI co-piloting
/// </summary>
public class AiOrbModule : IModule
{
    private readonly ILogger<AiOrbModule> _logger;
    private IServiceProvider? _serviceProvider;
    private IAiOrbService? _aiOrbService;
    private bool _isInitialized;

    public string Name => "AI Orb Module";
    public string Version => "1.0.0";

    public AiOrbModule()
    {
        // Create a temporary logger for initialization
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        _logger = loggerFactory.CreateLogger<AiOrbModule>();
    }

    /// <summary>
    /// Initialize the AI Orb module and register all services
    /// </summary>
    public async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        try
        {
            _serviceProvider = serviceProvider;
            _logger.LogInformation("Initializing AI Orb Module - Cosmic intelligence awakening! 🌟");

            // Register module services
            RegisterServices(serviceProvider);

            // Initialize core AI service
            _aiOrbService = serviceProvider.GetRequiredService<IAiOrbService>();
            
            // Subscribe to system events for proactive assistance
            await SubscribeToSystemEventsAsync(serviceProvider);

            // Initialize component subsystems
            await InitializeSubsystemsAsync();

            _isInitialized = true;
            
            _logger.LogInformation("AI Orb Module initialized successfully - Orb consciousness online! ✨");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI Orb Module initialization failed - Cosmic startup error! ⚠️");
            throw;
        }
    }

    /// <summary>
    /// Dispose of resources when module is unloaded
    /// </summary>
    public async Task DisposeAsync()
    {
        try
        {
            _logger.LogInformation("Disposing AI Orb Module - Orb consciousness archiving! 💤");

            // Dispose OCR processor
            var ocrProcessor = _serviceProvider?.GetService<IOcrProcessor>();
            if (ocrProcessor != null)
            {
                await ocrProcessor.DisposeAsync();
            }

            // Dispose AI client
            var aiClient = _serviceProvider?.GetService<AiClientService>();
            aiClient?.Dispose();

            _isInitialized = false;
            
            _logger.LogInformation("AI Orb Module disposed successfully - Cosmic systems powered down! 😴");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI Orb Module disposal failed - Cleanup cosmic interference! ⚠️");
        }
    }

    /// <summary>
    /// Register all AI Orb services with dependency injection
    /// </summary>
    private void RegisterServices(IServiceProvider serviceProvider)
    {
        var services = serviceProvider.GetRequiredService<IServiceCollection>();

        // Register core interfaces and implementations
        services.AddSingleton<IConfigManager, ConfigManager>();
        services.AddSingleton<AiClientService>();
        services.AddSingleton<IOcrProcessor, OcrProcessor>();
        services.AddSingleton<ICoPilotEngine, CoPilotEngine>();
        services.AddSingleton<IAiOrbService, AiOrbService>();
        services.AddSingleton<IOrbEventPublisher>(provider => 
            provider.GetRequiredService<IAiOrbService>() as IOrbEventPublisher 
            ?? throw new InvalidOperationException("AiOrbService must implement IOrbEventPublisher"));

        // Register event handlers for cross-module integration
        services.AddSingleton<OrbEventIntegrationHandler>();

        _logger.LogDebug("AI Orb services registered - Cosmic dependency injection complete! 🔧");
    }

    /// <summary>
    /// Subscribe to system-wide events for proactive AI assistance
    /// </summary>
    private async Task SubscribeToSystemEventsAsync(IServiceProvider serviceProvider)
    {
        try
        {
            var eventBus = serviceProvider.GetRequiredService<IEventBus>();
            var integrationHandler = serviceProvider.GetRequiredService<OrbEventIntegrationHandler>();

            // Subscribe to terminal events for co-pilot assistance
            eventBus.Subscribe<object>(async (eventData) => await integrationHandler.HandleTerminalEventAsync(eventData));
            
            // Subscribe to scanner events for proactive analysis
            eventBus.Subscribe<object>(async (eventData) => await integrationHandler.HandleScannerEventAsync(eventData));
            
            // Subscribe to security events for threat analysis
            eventBus.Subscribe<object>(async (eventData) => await integrationHandler.HandleSecurityEventAsync(eventData));
            
            // Subscribe to education events for contextual tips
            eventBus.Subscribe<object>(async (eventData) => await integrationHandler.HandleEducationEventAsync(eventData));

            _logger.LogInformation("Event subscriptions established - Orb omniscience activated! 👁️‍🗨️");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Event subscription failed - Orb will operate with limited omniscience! 👁️");
        }
    }

    /// <summary>
    /// Initialize all subsystems
    /// </summary>
    private async Task InitializeSubsystemsAsync()
    {
        try
        {
            // Initialize OCR processor
            var ocrProcessor = _serviceProvider?.GetService<IOcrProcessor>();
            if (ocrProcessor != null)
            {
                await ocrProcessor.InitializeAsync();
                _logger.LogDebug("OCR subsystem initialized - Orb vision enhanced! 👁️");
            }

            // Test API connectivity if configured
            var configManager = _serviceProvider?.GetService<IConfigManager>();
            if (configManager != null && await configManager.IsConfiguredAsync())
            {
                var apiConnected = await _aiOrbService!.TestApiConnectionAsync();
                if (apiConnected)
                {
                    _logger.LogInformation("API connectivity verified - Orb cosmic link established! 🌐");
                }
                else
                {
                    _logger.LogWarning("API connectivity test failed - Orb cosmic link unstable! 📡");
                }
            }

            _logger.LogInformation("All subsystems initialized - Orb fully operational! 🚀");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Subsystem initialization partial - Orb operating with reduced capabilities! ⚠️");
        }
    }
}

/// <summary>
/// Event integration handler for cross-module AI assistance
/// Provides proactive intelligence based on system-wide events
/// </summary>
public class OrbEventIntegrationHandler
{
    private readonly ILogger<OrbEventIntegrationHandler> _logger;
    private readonly IAiOrbService _aiOrbService;
    private readonly IEventBus _eventBus;
    private readonly Dictionary<string, DateTime> _lastEventTimes;

    public OrbEventIntegrationHandler(
        ILogger<OrbEventIntegrationHandler> logger,
        IAiOrbService aiOrbService,
        IEventBus eventBus)
    {
        _logger = logger;
        _aiOrbService = aiOrbService;
        _eventBus = eventBus;
        _lastEventTimes = new Dictionary<string, DateTime>();
    }

    /// <summary>
    /// Handle terminal/PowerShell events for co-pilot assistance
    /// </summary>
    public async Task HandleTerminalEventAsync(object eventData)
    {
        try
        {
            // Throttle events to prevent spam
            if (!ShouldProcessEvent("terminal", TimeSpan.FromSeconds(5)))
                return;

            var eventType = eventData.GetType().Name;
            
            // Check if this is a terminal output event that could benefit from co-pilot analysis
            if (eventType.Contains("Output") || eventType.Contains("Error") || eventType.Contains("Command"))
            {
                // Extract relevant data and trigger proactive analysis
                var insight = new ProactiveInsightEvent
                {
                    Title = "Terminal Activity Detected",
                    Description = "Orb detected terminal activity that might benefit from AI assistance",
                    Category = "CLI Co-Pilot",
                    Priority = 2,
                    SourceEvent = eventType,
                    Suggestions = new List<Models.ActionSuggestion>
                    {
                        new()
                        {
                            Action = "Analyze Terminal Output",
                            Rationale = "AI co-pilot can provide suggestions for command optimization or error resolution",
                            Priority = 2,
                            CommandSuggestion = "Use Orb CLI co-pilot for intelligent terminal assistance"
                        }
                    }
                };

                await _eventBus.PublishAsync(insight);
                
                _logger.LogDebug("Terminal event processed - Orb co-pilot awareness updated! 💻");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Terminal event handling failed - Orb co-pilot sensors glitched! ⚡");
        }
    }

    /// <summary>
    /// Handle scanner/topology events for network analysis
    /// </summary>
    public async Task HandleScannerEventAsync(object eventData)
    {
        try
        {
            if (!ShouldProcessEvent("scanner", TimeSpan.FromSeconds(10)))
                return;

            var eventType = eventData.GetType().Name;
            
            if (eventType.Contains("Scan") || eventType.Contains("Network") || eventType.Contains("Device"))
            {
                var insight = new ProactiveInsightEvent
                {
                    Title = "Network Scan Activity",
                    Description = "Orb observed network scanning activity - AI analysis available for discovered devices",
                    Category = "Network Analysis",
                    Priority = 3,
                    SourceEvent = eventType,
                    Suggestions = new List<Models.ActionSuggestion>
                    {
                        new()
                        {
                            Action = "Analyze Scan Results",
                            Rationale = "AI can provide insights about discovered network devices and suggest optimizations",
                            Priority = 3,
                            CommandSuggestion = "Use Orb OCR analysis for network diagram interpretation"
                        }
                    }
                };

                await _eventBus.PublishAsync(insight);
                
                _logger.LogDebug("Scanner event processed - Orb network awareness enhanced! 🌐");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Scanner event handling failed - Orb network sensors confused! 📡");
        }
    }

    /// <summary>
    /// Handle security events for threat analysis
    /// </summary>
    public async Task HandleSecurityEventAsync(object eventData)
    {
        try
        {
            if (!ShouldProcessEvent("security", TimeSpan.FromSeconds(5)))
                return;

            var eventType = eventData.GetType().Name;
            
            if (eventType.Contains("Vulnerability") || eventType.Contains("Security") || eventType.Contains("Threat"))
            {
                var insight = new ProactiveInsightEvent
                {
                    Title = "Security Event Detected",
                    Description = "Orb detected security-related activity - AI assistance available for threat analysis",
                    Category = "Security Analysis",
                    Priority = 4, // High priority for security
                    SourceEvent = eventType,
                    Suggestions = new List<Models.ActionSuggestion>
                    {
                        new()
                        {
                            Action = "Security Analysis",
                            Rationale = "AI can help analyze security findings and suggest remediation steps",
                            Priority = 4,
                            CommandSuggestion = "Use Orb chat for security consultation and remediation guidance"
                        }
                    }
                };

                await _eventBus.PublishAsync(insight);
                
                _logger.LogDebug("Security event processed - Orb security awareness heightened! 🛡️");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Security event handling failed - Orb security sensors alarmed! 🚨");
        }
    }

    /// <summary>
    /// Handle education events for contextual learning tips
    /// </summary>
    public async Task HandleEducationEventAsync(object eventData)
    {
        try
        {
            if (!ShouldProcessEvent("education", TimeSpan.FromSeconds(15)))
                return;

            var eventType = eventData.GetType().Name;
            
            if (eventType.Contains("Lesson") || eventType.Contains("Quiz") || eventType.Contains("Learning"))
            {
                var tip = new EducationalTipEvent
                {
                    Title = "Learning Opportunity",
                    Content = "Orb can provide additional context and explanations for networking concepts you're exploring",
                    Category = "Educational Enhancement",
                    DifficultyLevel = 2,
                    SourceContext = eventType,
                    RelatedConcepts = new List<string> { "AI-Assisted Learning", "Contextual Help", "Deep Dive Analysis" }
                };

                await _eventBus.PublishAsync(tip);
                
                _logger.LogDebug("Education event processed - Orb learning assistance offered! 🎓");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Education event handling failed - Orb teaching circuits confused! 📚");
        }
    }

    /// <summary>
    /// Throttle event processing to prevent spam
    /// </summary>
    private bool ShouldProcessEvent(string eventCategory, TimeSpan minimumInterval)
    {
        var now = DateTime.UtcNow;
        
        if (_lastEventTimes.TryGetValue(eventCategory, out var lastTime))
        {
            if (now - lastTime < minimumInterval)
            {
                return false;
            }
        }

        _lastEventTimes[eventCategory] = now;
        return true;
    }
}