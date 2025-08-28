using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetToolkit.Core.Events;
using NetToolkit.Core.Interfaces;
using NetToolkit.Modules.Education.Data;
using NetToolkit.Modules.Education.Events;
using NetToolkit.Modules.Education.Interfaces;
using NetToolkit.Modules.Education.Models;
using NetToolkit.Modules.Education.Services;
using System.Reflection;

namespace NetToolkit.Modules.Education;

/// <summary>
/// Education Platform Module - the cosmic academy of network enlightenment
/// Where learning journeys unfold with gamified excellence and knowledge transforms minds
/// </summary>
public class EducationModule : IModule, IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEventBus _eventBus;
    private readonly ILogger<EducationModule> _logger;
    private IServiceScope? _serviceScope;

    public string Name => "Education Platform";
    public string Description => "Gamified learning platform for network education with interactive lessons, quizzes, badges, and cosmic wisdom!";
    public string Version => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";
    public bool IsEnabled { get; set; } = true;

    public EducationModule(IServiceProvider serviceProvider, IEventBus eventBus, ILogger<EducationModule> logger)
    {
        _serviceProvider = serviceProvider;
        _eventBus = eventBus;
        _logger = logger;
    }

    /// <summary>
    /// Initialize the Education Module (IModule implementation)
    /// </summary>
    public async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        await InitializeAsync(CancellationToken.None);
    }

    /// <summary>
    /// Initialize the Education Module
    /// </summary>
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Initializing Education Platform Module...");

            // Create service scope for initialization
            _serviceScope = _serviceProvider.CreateScope();
            var services = _serviceScope.ServiceProvider;

            // Initialize database and seed content
            var contentService = services.GetRequiredService<EducationContentService>();
            await contentService.InitializeContentAsync(cancellationToken);

            // Subscribe to cross-module events for cosmic integration
            await SubscribeToCrossModuleEventsAsync();

            // Register education-specific event handlers
            RegisterEducationEventHandlers();

            _logger.LogInformation("Education Platform Module initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Education Platform Module");
            throw;
        }
    }

    /// <summary>
    /// Start the Education Module as a hosted service
    /// </summary>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting Education Platform Module hosted service...");
        
        try
        {
            await InitializeAsync(cancellationToken);
            
            // Perform any startup-specific tasks
            await PerformStartupTasksAsync(cancellationToken);
            
            _logger.LogInformation("Education Platform Module started successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start Education Platform Module");
            throw;
        }
    }

    /// <summary>
    /// Stop the Education Module
    /// </summary>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Stopping Education Platform Module...");
        
        try
        {
            // Unsubscribe from events
            await UnsubscribeFromEventsAsync();
            
            // Dispose service scope
            _serviceScope?.Dispose();
            
            _logger.LogInformation("Education Platform Module stopped gracefully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Education Platform Module shutdown");
        }
    }

    /// <summary>
    /// Configure services for dependency injection
    /// </summary>
    public void ConfigureServices(IServiceCollection services)
    {

        // Register database context with SQLite
        services.AddDbContext<EducationDbContext>(options =>
        {
            var connectionString = "Data Source=NetToolkitEducation.db";
            options.UseSqlite(connectionString, sqliteOptions =>
            {
                sqliteOptions.MigrationsAssembly("NetToolkit.Modules.Education");
            });
            
            // Enable sensitive data logging in development
            #if DEBUG
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
            #endif
        });

        // Register core education services
        services.AddScoped<IEducationService, EducationContentService>();
        services.AddScoped<ISlideGenerator, SkiaSharpImageGenerator>();
        services.AddScoped<IGamificationEngine, GamificationEngineService>();

        // Register the content service explicitly
        services.AddScoped<EducationContentService>();

        // Register the module itself as a hosted service
        services.AddSingleton<IModule>(this);
        services.AddHostedService<EducationModule>(provider => this);

    }

    /// <summary>
    /// Get module metadata information
    /// </summary>
    public Dictionary<string, object> GetModuleInfo()
    {
        return new Dictionary<string, object>
        {
            ["Name"] = Name,
            ["Description"] = Description,
            ["Version"] = Version.ToString(),
            ["IsEnabled"] = IsEnabled,
            ["ModuleType"] = "Education Platform",
            ["Features"] = new[]
            {
                "Interactive Learning Modules",
                "Gamified Progress Tracking", 
                "Dynamic Slide Generation",
                "SkiaSharp Visual Content",
                "Three.js 3D Visualizations",
                "Hover Tips & Explanations",
                "Badge Achievement System",
                "Quiz Assessment Engine",
                "Cross-Module Integration",
                "SQLite Content Storage"
            },
            ["SupportedContentTypes"] = new[]
            {
                "Text Slides",
                "Image Slides", 
                "Quiz Slides",
                "Interactive 3D",
                "Network Diagrams",
                "Animation Sequences"
            },
            ["Dependencies"] = new[]
            {
                "NetToolkit.Core",
                "SkiaSharp",
                "EntityFrameworkCore.Sqlite",
                "Microsoft.Extensions.Hosting"
            },
            ["DatabaseInfo"] = new
            {
                Provider = "SQLite",
                ConnectionString = "Data Source=NetToolkitEducation.db",
                MigrationsAssembly = "NetToolkit.Modules.Education"
            },
            ["CrossModuleIntegrations"] = new[]
            {
                "PowerShell Terminal (Script Demos)",
                "Port Scanner (Visual Examples)",
                "Security Scanner (Threat Education)", 
                "SSH Terminal (Security Lessons)",
                "AI Orb (Smart Assistance)"
            },
            ["InitialContent"] = new
            {
                Module1Title = "Network Basics - From Cables to Cosmos",
                LessonCount = 20,
                EstimatedHours = 7,
                BadgeCount = 7,
                DifficultyProgression = "Beginner → Certification Level"
            }
        };
    }

    /// <summary>
    /// Cleanup resources
    /// </summary>
    public void Dispose()
    {
        
        try
        {
            _serviceScope?.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Education Platform Module disposal");
        }
    }

    #region Private Helper Methods

    /// <summary>
    /// Subscribe to events from other NetToolkit modules for cosmic integration
    /// </summary>
    private async Task SubscribeToCrossModuleEventsAsync()
    {
        try
        {

            // Subscribe to PowerShell Terminal events
            await _eventBus.SubscribeAsync<object>("PowerShell.ScriptExecuted", OnPowerShellScriptExecuted);
            await _eventBus.SubscribeAsync<object>("PowerShell.CommandCompleted", OnPowerShellCommandCompleted);

            // Subscribe to Port Scanner events  
            await _eventBus.SubscribeAsync<object>("PortScanner.ScanCompleted", OnPortScanCompleted);
            await _eventBus.SubscribeAsync<object>("NetworkTopology.Generated", OnNetworkTopologyGenerated);

            // Subscribe to Security Scanner events
            await _eventBus.SubscribeAsync<object>("SecurityScanner.VulnerabilityFound", OnSecurityVulnerabilityFound);
            await _eventBus.SubscribeAsync<object>("SecurityScanner.ScanCompleted", OnSecurityScanCompleted);

            // Subscribe to SSH Terminal events
            await _eventBus.SubscribeAsync<object>("SSH.ConnectionEstablished", OnSSHConnectionEstablished);
            await _eventBus.SubscribeAsync<object>("SSH.CommandExecuted", OnSSHCommandExecuted);

            // Subscribe to AI Orb events
            await _eventBus.SubscribeAsync<object>("AI.QueryReceived", OnAIQueryReceived);
            await _eventBus.SubscribeAsync<object>("AI.ResponseGenerated", OnAIResponseGenerated);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to subscribe to cross-module events");
        }
    }

    /// <summary>
    /// Register education-specific event handlers
    /// </summary>
    private void RegisterEducationEventHandlers()
    {
        try
        {

            // Register lesson completion handler
            _eventBus.SubscribeAsync<LessonCompletedEvent>("Education.LessonCompleted", OnLessonCompleted);

            // Register quiz passed handler
            _eventBus.SubscribeAsync<QuizPassedEvent>("Education.QuizPassed", OnQuizPassed);

            // Register badge unlocked handler
            _eventBus.SubscribeAsync<BadgeUnlockedEvent>("Education.BadgeUnlocked", OnBadgeUnlocked);

            // Register module started handler
            _eventBus.SubscribeAsync<ModuleStartedEvent>("Education.ModuleStarted", OnModuleStarted);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to register education event handlers");
        }
    }

    /// <summary>
    /// Perform startup tasks specific to the education module
    /// </summary>
    private async Task PerformStartupTasksAsync(CancellationToken cancellationToken)
    {
        try
        {

            if (_serviceScope == null)
            {
                _logger.LogWarning("Service scope not available for startup tasks");
                return;
            }

            var services = _serviceScope.ServiceProvider;

            // Optimize database for better performance
            var dbContext = services.GetRequiredService<EducationDbContext>();
            await dbContext.OptimizeDatabaseAsync();

            // Validate content integrity for both modules
            var contentService = services.GetRequiredService<EducationContentService>();
            var modules = await contentService.GetAllModulesAsync(cancellationToken);
            
            var availableModules = 0;
            foreach (var module in modules)
            {
                availableModules++;
                _logger.LogDebug("Module {ModuleId} validated: '{Title}' with {LessonCount} lessons", 
                    module.Id, module.Title, module.Lessons.Count);
            }
            
            if (availableModules == 0)
            {
                _logger.LogWarning("No modules found - content may need re-seeding");
            }

            // Publish module ready event
            await _eventBus.PublishAsync(new EducationModuleReadyEvent
            {
                ModuleName = Name,
                Version = Version.ToString(),
                AvailableModules = availableModules,
                ReadyAt = DateTime.UtcNow
            }, cancellationToken);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during education module startup tasks");
        }
    }

    /// <summary>
    /// Unsubscribe from all events during shutdown
    /// </summary>
    private async Task UnsubscribeFromEventsAsync()
    {
        try
        {

            // Unsubscribe from cross-module events (INFERRED: Generic type explicit for cross-module compatibility)
            await _eventBus.UnsubscribeAsync<object>("PowerShell.ScriptExecuted", OnPowerShellScriptExecuted);
            await _eventBus.UnsubscribeAsync<object>("PowerShell.CommandCompleted", OnPowerShellCommandCompleted);
            await _eventBus.UnsubscribeAsync<object>("PortScanner.ScanCompleted", OnPortScanCompleted);
            await _eventBus.UnsubscribeAsync<object>("NetworkTopology.Generated", OnNetworkTopologyGenerated);
            await _eventBus.UnsubscribeAsync<object>("SecurityScanner.VulnerabilityFound", OnSecurityVulnerabilityFound);
            await _eventBus.UnsubscribeAsync<object>("SecurityScanner.ScanCompleted", OnSecurityScanCompleted);
            await _eventBus.UnsubscribeAsync<object>("SSH.ConnectionEstablished", OnSSHConnectionEstablished);
            await _eventBus.UnsubscribeAsync<object>("SSH.CommandExecuted", OnSSHCommandExecuted);
            await _eventBus.UnsubscribeAsync<object>("AI.QueryReceived", OnAIQueryReceived);
            await _eventBus.UnsubscribeAsync<object>("AI.ResponseGenerated", OnAIResponseGenerated);

            // Unsubscribe from education events (INFERRED: Specific event types for proper handler matching)
            await _eventBus.UnsubscribeAsync<LessonCompletedEvent>("Education.LessonCompleted", OnLessonCompleted);
            await _eventBus.UnsubscribeAsync<QuizPassedEvent>("Education.QuizPassed", OnQuizPassed);
            await _eventBus.UnsubscribeAsync<BadgeUnlockedEvent>("Education.BadgeUnlocked", OnBadgeUnlocked);
            await _eventBus.UnsubscribeAsync<ModuleStartedEvent>("Education.ModuleStarted", OnModuleStarted);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during event unsubscription");
        }
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handle PowerShell script execution events
    /// </summary>
    private async Task OnPowerShellScriptExecuted(object eventData)
    {
        try
        {

            // If user executed networking commands, suggest related lessons
            var scriptData = eventData.ToString()?.ToLowerInvariant() ?? "";
            
            if (scriptData.Contains("ping") || scriptData.Contains("ipconfig") || scriptData.Contains("netstat"))
            {
                await _eventBus.PublishAsync(new LearningOpportunityEvent
                {
                    TriggeredBy = "PowerShell Command",
                    SuggestedLessons = new[] { "Basic Troubleshooting: Fix the Fun", "Troubleshooting Hardware: Hero Fixes" },
                    Context = "Network diagnostic commands detected",
                    Priority = "Medium"
                });
                
                _logger.LogDebug("Suggested networking lessons based on PowerShell activity");
            }
            else if (scriptData.Contains("get-netadapter") || scriptData.Contains("get-netipinterface"))
            {
                await _eventBus.PublishAsync(new LearningOpportunityEvent
                {
                    TriggeredBy = "PowerShell Hardware Command",
                    SuggestedLessons = new[] { "NICs: The Chatty Cards", "Scripting Hardware: Command Control" },
                    Context = "Hardware diagnostic commands detected",
                    Priority = "High"
                });
                
                _logger.LogDebug("Suggested Hardware Heroes lessons based on NIC command activity");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling PowerShell script execution event");
        }
    }

    /// <summary>
    /// Handle PowerShell command completion events
    /// </summary>
    private async Task OnPowerShellCommandCompleted(object eventData)
    {
        
        // Track command usage for learning recommendations
        // Implementation details would depend on the event data structure
    }

    /// <summary>
    /// Handle port scan completion events
    /// </summary>
    private async Task OnPortScanCompleted(object eventData)
    {
        try
        {

            await _eventBus.PublishAsync(new LearningOpportunityEvent
            {
                TriggeredBy = "Port Scanner",
                SuggestedLessons = new[] 
                { 
                    "Introduction to Security: Lock the Doors",
                    "Hardware Heroes: NICs and More",
                    "Firewalls: The Guardian Gadgets",
                    "Servers: The Data Keepers"
                },
                Context = "Port scanning activity detected",
                Priority = "High"
            });
            
            _logger.LogInformation("🛡️ Suggested security lessons based on port scanning activity!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error handling port scan completion event");
        }
    }

    /// <summary>
    /// Handle network topology generation events
    /// </summary>
    private async Task OnNetworkTopologyGenerated(object eventData)
    {
        try
        {

            await _eventBus.PublishAsync(new LearningOpportunityEvent
            {
                TriggeredBy = "Network Topology",
                SuggestedLessons = new[] 
                { 
                    "Topology Types: Star, Mesh, Oh My!",
                    "Switches: The Party Connectors",
                    "Routers: The Traffic Directors",
                    "Advanced Topologies: Hero Formations"
                },
                Context = "Network topology visualization generated",
                Priority = "Medium"
            });
            
            _logger.LogInformation("📊 Suggested topology lessons based on network visualization!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error handling network topology generation event");
        }
    }

    /// <summary>
    /// Handle security vulnerability discovery events
    /// </summary>
    private async Task OnSecurityVulnerabilityFound(object eventData)
    {
        try
        {

            await _eventBus.PublishAsync(new LearningOpportunityEvent
            {
                TriggeredBy = "Security Scanner",
                SuggestedLessons = new[] { "Introduction to Security: Lock the Doors" },
                Context = "Security vulnerabilities detected",
                Priority = "Critical"
            });
            
            _logger.LogInformation("🔒 Suggested security lessons based on vulnerability findings!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error handling security vulnerability event");
        }
    }

    /// <summary>
    /// Handle security scan completion events
    /// </summary>
    private async Task OnSecurityScanCompleted(object eventData)
    {
        // Implementation for security scan completion
    }

    /// <summary>
    /// Handle SSH connection establishment events
    /// </summary>
    private async Task OnSSHConnectionEstablished(object eventData)
    {
        try
        {

            await _eventBus.PublishAsync(new LearningOpportunityEvent
            {
                TriggeredBy = "SSH Terminal",
                SuggestedLessons = new[] { "Introduction to Security: Lock the Doors" },
                Context = "SSH connection activity",
                Priority = "Medium"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error handling SSH connection event");
        }
    }

    /// <summary>
    /// Handle SSH command execution events
    /// </summary>
    private async Task OnSSHCommandExecuted(object eventData)
    {
        // Track SSH command patterns for advanced lesson recommendations
    }

    /// <summary>
    /// Handle AI query events
    /// </summary>
    private async Task OnAIQueryReceived(object eventData)
    {
        // Analyze AI queries to identify knowledge gaps and suggest lessons
    }

    /// <summary>
    /// Handle AI response generation events
    /// </summary>
    private async Task OnAIResponseGenerated(object eventData)
    {
        // Check if AI responses can be enhanced with educational content
    }

    /// <summary>
    /// Handle education lesson completion events
    /// </summary>
    private async Task OnLessonCompleted(LessonCompletedEvent eventData)
    {
        try
        {
            _logger.LogInformation("🎉 Lesson completed: User {UserId} finished lesson {LessonId} with score {Score}%", 
                eventData.UserId, eventData.LessonId, eventData.QuizScore);

            // Publish celebration event
            await _eventBus.PublishAsync(new LearningCelebrationEvent
            {
                UserId = eventData.UserId,
                Achievement = $"Completed lesson with {eventData.QuizScore:F0}% score!",
                CelebrationMessage = GetCelebrationMessage(eventData.QuizScore),
                BadgesAwarded = eventData.BadgesAwarded
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error handling lesson completion event");
        }
    }

    /// <summary>
    /// Handle quiz passed events
    /// </summary>
    private async Task OnQuizPassed(QuizPassedEvent eventData)
    {
        _logger.LogInformation("🧠 Quiz mastery achieved: User {UserId} passed lesson {LessonId} with score {Score:F1}%", 
            eventData.UserId, eventData.LessonId, eventData.Score);
    }

    /// <summary>
    /// Handle badge unlocked events
    /// </summary>
    private async Task OnBadgeUnlocked(BadgeUnlockedEvent eventData)
    {
        _logger.LogInformation("🏆 Badge unlocked: User {UserId} earned '{BadgeName}' badge!", 
            eventData.UserId, eventData.BadgeName);
    }

    /// <summary>
    /// Handle module started events
    /// </summary>
    private async Task OnModuleStarted(ModuleStartedEvent eventData)
    {
        _logger.LogInformation("🚀 Learning journey begins: User {UserId} started module '{ModuleName}'", 
            eventData.UserId, eventData.ModuleName);
    }

    /// <summary>
    /// Get celebration message based on quiz score
    /// </summary>
    private string GetCelebrationMessage(double score) => score switch
    {
        >= 95 => "🌟 Absolutely stellar! You're a networking virtuoso!",
        >= 85 => "⭐ Excellent work! You're mastering the digital realm!",
        >= 75 => "🎯 Great job! Your network knowledge is expanding beautifully!",
        >= 65 => "📚 Good effort! Keep learning and you'll be a network ninja!",
        >= 50 => "💪 Not bad! Practice makes perfect in the networking cosmos!",
        _ => "🌱 Every expert was once a beginner - keep growing your network wisdom!"
    };

    /// <summary>
    /// Dispose the Education Module (IModule implementation)
    /// </summary>
    public async Task DisposeAsync()
    {
        try
        {
            _logger.LogInformation("🎓 Shutting down Cosmic Education Platform Module...");
            
            _serviceScope?.Dispose();
            
            _logger.LogInformation("✅ Education Platform Module shutdown complete");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error during Education Module shutdown");
        }
    }

    #endregion
}

/// <summary>
/// Event for learning opportunities triggered by other modules
/// </summary>
public class LearningOpportunityEvent
{
    public string TriggeredBy { get; set; } = string.Empty;
    public string[] SuggestedLessons { get; set; } = Array.Empty<string>();
    public string Context { get; set; } = string.Empty;
    public string Priority { get; set; } = "Medium";
    public DateTime TriggeredAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Event for celebrating learning achievements
/// </summary>
public class LearningCelebrationEvent
{
    public string UserId { get; set; } = string.Empty;
    public string Achievement { get; set; } = string.Empty;
    public string CelebrationMessage { get; set; } = string.Empty;
    public List<string> BadgesAwarded { get; set; } = new();
    public DateTime CelebratedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Event for when the education module is fully ready
/// </summary>
public class EducationModuleReadyEvent
{
    public string ModuleName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public int AvailableModules { get; set; }
    public DateTime ReadyAt { get; set; } = DateTime.UtcNow;
}