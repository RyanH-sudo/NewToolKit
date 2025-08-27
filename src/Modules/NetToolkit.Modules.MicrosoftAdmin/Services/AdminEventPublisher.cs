using Microsoft.Extensions.Logging;
using NetToolkit.Core.Interfaces;
using NetToolkit.Modules.MicrosoftAdmin.Interfaces;
using NetToolkit.Modules.MicrosoftAdmin.Models;
using NetToolkit.Modules.MicrosoftAdmin.Events;

namespace NetToolkit.Modules.MicrosoftAdmin.Services;

/// <summary>
/// Admin event publisher - the herald of administrative achievements
/// Broadcasts Microsoft Admin operations across the NetToolkit ecosystem
/// </summary>
public class AdminEventPublisher : IAdminEventPublisher
{
    private readonly ILogger<AdminEventPublisher> _logger;
    private readonly IEventBus _eventBus;
    private readonly Dictionary<string, int> _eventCounters;
    private readonly SemaphoreSlim _counterLock;

    public AdminEventPublisher(
        ILogger<AdminEventPublisher> logger,
        IEventBus eventBus)
    {
        _logger = logger;
        _eventBus = eventBus;
        _eventCounters = new Dictionary<string, int>();
        _counterLock = new SemaphoreSlim(1, 1);
        
        _logger.LogInformation("Admin event publisher initialized - ready to herald administrative victories and setbacks");
    }

    public async Task PublishAuthSuccessAsync(string userId, DateTime authenticatedAt, string[] scopes)
    {
        try
        {
            _logger.LogInformation("Publishing authentication success event for user {UserId} - admin powers activated!", userId);
            
            var authEvent = new AuthenticationSuccessEvent
            {
                UserId = userId,
                AuthenticatedAt = authenticatedAt,
                Scopes = scopes,
                Metadata = new Dictionary<string, object>
                {
                    ["eventId"] = Guid.NewGuid().ToString(),
                    ["source"] = "MicrosoftAdminModule",
                    ["scopeCount"] = scopes.Length,
                    ["authenticationMethod"] = "Interactive",
                    ["wittyMessage"] = "Another admin joins the digital realm!"
                }
            };
            
            await _eventBus.PublishAsync(authEvent);
            await IncrementEventCounter("AuthSuccess");
            
            // Also publish integration sync event for other modules
            await PublishIntegrationSyncAsync("Authentication", "AuthSuccess", new Dictionary<string, object>
            {
                ["userId"] = userId,
                ["scopes"] = scopes,
                ["authenticatedAt"] = authenticatedAt
            });
            
            _logger.LogDebug("Authentication success event published successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish authentication success event for user {UserId}", userId);
        }
    }

    public async Task PublishTaskExecutedAsync(AdminTaskResult result)
    {
        try
        {
            _logger.LogInformation("Publishing admin task execution event for {TemplateId} - {Status}", 
                result.TemplateId, result.IsSuccess ? "Success" : "Failure");
            
            var taskEvent = new AdminTaskCompletedEvent
            {
                TaskResult = result,
                CompletedAt = DateTime.UtcNow,
                RequiresFollowUp = DetermineFollowUpRequired(result),
                NextSuggestedActions = GetSuggestedActions(result)
            };
            
            await _eventBus.PublishAsync(taskEvent);
            await IncrementEventCounter("TaskExecuted");
            
            // Publish specific events based on task outcome
            if (!result.IsSuccess)
            {
                await PublishTaskFailureAsync(result);
            }
            
            // Trigger AI Orb suggestions for improvements
            if (ShouldTriggerAiSuggestions(result))
            {
                await PublishIntegrationSyncAsync("AiOrb", "TaskAnalysis", new Dictionary<string, object>
                {
                    ["taskResult"] = result,
                    ["suggestionsRequested"] = true,
                    ["analysisType"] = result.IsSuccess ? "optimization" : "troubleshooting"
                });
            }
            
            // Trigger security scan if needed
            if (ShouldTriggerSecurityScan(result))
            {
                await PublishIntegrationSyncAsync("Security", "PostTaskScan", new Dictionary<string, object>
                {
                    ["taskId"] = result.TaskId,
                    ["templateId"] = result.TemplateId,
                    ["executedBy"] = result.ExecutedBy,
                    ["scanType"] = "administrative-action"
                });
            }
            
            _logger.LogDebug("Admin task execution event published successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish task execution event for {TemplateId}", result.TemplateId);
        }
    }

    public async Task PublishErrorOccurredAsync(string operation, Exception error, Dictionary<string, object>? context = null)
    {
        try
        {
            _logger.LogWarning("Publishing error event for operation {Operation} - {ErrorMessage}", operation, error.Message);
            
            var errorEvent = new AdminTaskFailureEvent
            {
                TaskName = operation,
                Error = error.Message,
                FailedAt = DateTime.UtcNow,
                Parameters = context ?? new Dictionary<string, object>(),
                RetryRecommended = DetermineRetryRecommendation(error)
            };
            
            await _eventBus.PublishAsync(errorEvent);
            await IncrementEventCounter("ErrorOccurred");
            
            // Check if this is a critical error that needs immediate attention
            if (IsCriticalError(error))
            {
                await PublishSecurityAlertAsync("CriticalError", $"Critical error in {operation}: {error.Message}", 
                    context?.GetValueOrDefault("userId")?.ToString() ?? "System");
            }
            
            // Suggest education content based on error type
            var educationTopic = MapErrorToEducationTopic(operation, error);
            if (!string.IsNullOrEmpty(educationTopic))
            {
                await PublishIntegrationSyncAsync("Education", "SuggestContent", new Dictionary<string, object>
                {
                    ["topic"] = educationTopic,
                    ["triggerReason"] = "error-recovery",
                    ["errorType"] = error.GetType().Name,
                    ["operation"] = operation
                });
            }
            
            _logger.LogDebug("Error event published successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish error event for operation {Operation}", operation);
        }
    }

    public async Task PublishPortalAccessedAsync(string portalSection, string userId, DateTime accessedAt)
    {
        try
        {
            _logger.LogInformation("Publishing portal access event for section {Section} by user {UserId}", 
                portalSection, userId);
            
            var portalEvent = new PortalAccessEvent
            {
                Section = portalSection,
                UserId = userId,
                AccessedAt = accessedAt,
                AccessContext = new Dictionary<string, object>
                {
                    ["eventId"] = Guid.NewGuid().ToString(),
                    ["source"] = "PortalIntegrator",
                    ["sessionStart"] = accessedAt,
                    ["expectedDuration"] = "15-30 minutes",
                    ["wittyComment"] = GetPortalAccessComment(portalSection)
                }
            };
            
            await _eventBus.PublishAsync(portalEvent);
            await IncrementEventCounter("PortalAccessed");
            
            // Track usage analytics
            await PublishTemplateUsageAnalyticsAsync($"portal-{portalSection}", userId, true, TimeSpan.Zero);
            
            _logger.LogDebug("Portal access event published successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish portal access event for section {Section}", portalSection);
        }
    }

    public async Task PublishTemplateUsedAsync(string templateId, string userId, bool successful)
    {
        try
        {
            _logger.LogInformation("Publishing template usage event for {TemplateId} by user {UserId} - {Status}", 
                templateId, userId, successful ? "Success" : "Failure");
            
            var usageEvent = new TemplateUsageEvent
            {
                TemplateId = templateId,
                UserId = userId,
                UsedAt = DateTime.UtcNow,
                Successful = successful,
                UsageMetadata = new Dictionary<string, object>
                {
                    ["eventId"] = Guid.NewGuid().ToString(),
                    ["source"] = "TemplateEngine",
                    ["sessionType"] = "interactive",
                    ["feedback"] = successful ? "Template mastery achieved!" : "Template spell needs refinement!"
                }
            };
            
            await _eventBus.PublishAsync(usageEvent);
            await IncrementEventCounter("TemplateUsed");
            
            _logger.LogDebug("Template usage event published successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish template usage event for {TemplateId}", templateId);
        }
    }

    private async Task PublishTaskFailureAsync(AdminTaskResult result)
    {
        try
        {
            var failureEvent = new AdminTaskFailureEvent
            {
                TemplateId = result.TemplateId,
                TaskName = result.TaskName,
                Error = result.ErrorOutput,
                FailedAt = DateTime.UtcNow,
                Parameters = result.Parameters,
                RetryRecommended = !result.ErrorOutput.Contains("permission", StringComparison.OrdinalIgnoreCase)
            };
            
            await _eventBus.PublishAsync(failureEvent);
            _logger.LogDebug("Task failure event published for {TemplateId}", result.TemplateId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish task failure event");
        }
    }

    private async Task PublishSecurityAlertAsync(string alertType, string description, string userId)
    {
        try
        {
            var alertEvent = new SecurityAlertEvent
            {
                AlertType = alertType,
                Description = description,
                UserId = userId,
                AlertedAt = DateTime.UtcNow,
                Severity = DetermineSeverity(alertType),
                AlertData = new Dictionary<string, object>
                {
                    ["source"] = "MicrosoftAdminModule",
                    ["autoGenerated"] = true,
                    ["requiresInvestigation"] = true
                },
                RequiresImmedateAction = alertType.Contains("Critical", StringComparison.OrdinalIgnoreCase)
            };
            
            await _eventBus.PublishAsync(alertEvent);
            _logger.LogWarning("Security alert published: {AlertType} - {Description}", alertType, description);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish security alert");
        }
    }

    private async Task PublishTemplateUsageAnalyticsAsync(string templateId, string userId, bool successful, TimeSpan executionTime)
    {
        try
        {
            var analyticsEvent = new TemplateUsageEvent
            {
                TemplateId = templateId,
                UserId = userId,
                UsedAt = DateTime.UtcNow,
                Successful = successful,
                ExecutionTime = executionTime,
                UsageMetadata = new Dictionary<string, object>
                {
                    ["analyticsType"] = "usage-tracking",
                    ["source"] = "AdminEventPublisher",
                    ["category"] = GetTemplateCategory(templateId)
                }
            };
            
            await _eventBus.PublishAsync(analyticsEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish template usage analytics");
        }
    }

    private async Task PublishIntegrationSyncAsync(string targetModule, string syncType, Dictionary<string, object> syncData)
    {
        try
        {
            var syncEvent = new IntegrationSyncEvent
            {
                TargetModule = targetModule,
                SyncType = syncType,
                SyncedAt = DateTime.UtcNow,
                SyncSuccessful = true,
                SyncData = System.Text.Json.JsonSerializer.Serialize(syncData),
                SyncMetadata = new Dictionary<string, object>
                {
                    ["source"] = "MicrosoftAdminModule",
                    ["priority"] = GetSyncPriority(targetModule, syncType),
                    ["expectedResponse"] = GetExpectedResponse(targetModule, syncType)
                }
            };
            
            await _eventBus.PublishAsync(syncEvent);
            _logger.LogDebug("Integration sync event published to {TargetModule} - {SyncType}", targetModule, syncType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish integration sync event to {TargetModule}", targetModule);
        }
    }

    private async Task IncrementEventCounter(string eventType)
    {
        await _counterLock.WaitAsync();
        try
        {
            _eventCounters[eventType] = _eventCounters.GetValueOrDefault(eventType, 0) + 1;
        }
        finally
        {
            _counterLock.Release();
        }
    }

    private bool DetermineFollowUpRequired(AdminTaskResult result)
    {
        // Determine if follow-up actions are needed based on the task result
        if (!result.IsSuccess)
            return true;
            
        // Check for specific templates that typically require follow-up
        return result.TemplateId switch
        {
            "bulk-user-create" => true, // Usually needs license assignment
            "create-shared-mailbox" => true, // Usually needs permission configuration
            "enable-mfa" => true, // May need user communication
            _ => result.Warnings.Count > 0
        };
    }

    private List<string> GetSuggestedActions(AdminTaskResult result)
    {
        var suggestions = new List<string>();
        
        if (!result.IsSuccess)
        {
            suggestions.Add("Review error details and retry with corrected parameters");
            suggestions.Add("Check user permissions and required scopes");
            suggestions.Add("Consult NetToolkit education module for troubleshooting guidance");
        }
        else
        {
            suggestions.AddRange(result.TemplateId switch
            {
                "bulk-user-create" => new[] { "Assign licenses to new users", "Set up user groups", "Send welcome emails" },
                "create-shared-mailbox" => new[] { "Configure mailbox permissions", "Add mailbox to address lists", "Set up mail forwarding if needed" },
                "extend-mailbox" => new[] { "Notify user of increased quota", "Monitor mailbox usage", "Consider archiving policies" },
                "enable-mfa" => new[] { "Notify user about MFA setup", "Provide MFA setup instructions", "Monitor MFA adoption" },
                _ => new[] { "Verify operation completed successfully", "Check for any additional configuration needs" }
            });
        }
        
        return suggestions;
    }

    private bool ShouldTriggerAiSuggestions(AdminTaskResult result)
    {
        // Trigger AI suggestions for failures or complex operations
        return !result.IsSuccess || 
               result.ExecutionTime > TimeSpan.FromMinutes(2) ||
               result.Warnings.Count > 0 ||
               result.TemplateId.Contains("bulk", StringComparison.OrdinalIgnoreCase);
    }

    private bool ShouldTriggerSecurityScan(AdminTaskResult result)
    {
        // Trigger security scans for sensitive operations
        var securitySensitiveTemplates = new[]
        {
            "reset-user-password", "enable-mfa", "create-security-group", 
            "assign-license", "mailbox-permissions"
        };
        
        return result.IsSuccess && securitySensitiveTemplates.Contains(result.TemplateId);
    }

    private bool DetermineRetryRecommendation(Exception error)
    {
        // Don't recommend retry for permission or authentication errors
        var errorMessage = error.Message.ToLowerInvariant();
        
        if (errorMessage.Contains("permission") || 
            errorMessage.Contains("unauthorized") || 
            errorMessage.Contains("forbidden") ||
            errorMessage.Contains("invalid credentials"))
        {
            return false;
        }
        
        // Recommend retry for network or temporary service errors
        return errorMessage.Contains("timeout") || 
               errorMessage.Contains("network") ||
               errorMessage.Contains("service unavailable") ||
               errorMessage.Contains("rate limit");
    }

    private bool IsCriticalError(Exception error)
    {
        // Define critical errors that need immediate attention
        return error is UnauthorizedAccessException ||
               error is System.Security.SecurityException ||
               error.Message.Contains("critical", StringComparison.OrdinalIgnoreCase) ||
               error.Message.Contains("security", StringComparison.OrdinalIgnoreCase);
    }

    private string MapErrorToEducationTopic(string operation, Exception error)
    {
        // Map errors to relevant education topics
        var errorMessage = error.Message.ToLowerInvariant();
        
        if (errorMessage.Contains("permission") || errorMessage.Contains("unauthorized"))
            return "admin-permissions";
            
        if (errorMessage.Contains("quota") || errorMessage.Contains("limit"))
            return "resource-management";
            
        if (errorMessage.Contains("authentication") || errorMessage.Contains("token"))
            return "authentication-troubleshooting";
            
        return operation switch
        {
            var op when op.Contains("mailbox", StringComparison.OrdinalIgnoreCase) => "mailbox-management",
            var op when op.Contains("user", StringComparison.OrdinalIgnoreCase) => "user-administration",
            var op when op.Contains("security", StringComparison.OrdinalIgnoreCase) => "security-configuration",
            _ => "general-troubleshooting"
        };
    }

    private string GetPortalAccessComment(string portalSection)
    {
        return portalSection switch
        {
            "home" => "Welcome to the admin command center!",
            "users" => "Managing digital identities - like a digital HR department!",
            "groups" => "Organizing collaboration - teamwork makes the dream work!",
            "licenses" => "Distributing digital keys to the productivity kingdom!",
            "security" => "Fortress management mode - shields up!",
            "exchange" => "Mail mastery realm - where messages find their destiny!",
            "sharepoint" => "Document collaboration nexus - sharing is caring!",
            "teams" => "Communication hub central - connecting minds and ideas!",
            _ => "Exploring administrative territories - adventure awaits!"
        };
    }

    private string DetermineSeverity(string alertType)
    {
        return alertType switch
        {
            var type when type.Contains("Critical", StringComparison.OrdinalIgnoreCase) => "Critical",
            var type when type.Contains("Security", StringComparison.OrdinalIgnoreCase) => "High",
            var type when type.Contains("Permission", StringComparison.OrdinalIgnoreCase) => "High",
            var type when type.Contains("Authentication", StringComparison.OrdinalIgnoreCase) => "Medium",
            _ => "Low"
        };
    }

    private string GetTemplateCategory(string templateId)
    {
        if (templateId.StartsWith("portal-"))
            return "Portal Access";
            
        return templateId switch
        {
            var id when id.Contains("mailbox", StringComparison.OrdinalIgnoreCase) => AdminTaskCategories.MailboxManagement,
            var id when id.Contains("user", StringComparison.OrdinalIgnoreCase) => AdminTaskCategories.UserManagement,
            var id when id.Contains("group", StringComparison.OrdinalIgnoreCase) => AdminTaskCategories.GroupManagement,
            var id when id.Contains("security", StringComparison.OrdinalIgnoreCase) => AdminTaskCategories.SecurityManagement,
            var id when id.Contains("license", StringComparison.OrdinalIgnoreCase) => AdminTaskCategories.LicenseManagement,
            _ => "General Administration"
        };
    }

    private string GetSyncPriority(string targetModule, string syncType)
    {
        return (targetModule, syncType) switch
        {
            ("Security", "PostTaskScan") => "High",
            ("AiOrb", "TaskAnalysis") => "Medium",
            ("Education", "SuggestContent") => "Low",
            _ => "Normal"
        };
    }

    private string GetExpectedResponse(string targetModule, string syncType)
    {
        return (targetModule, syncType) switch
        {
            ("AiOrb", "TaskAnalysis") => "AI suggestions and recommendations",
            ("Security", "PostTaskScan") => "Security scan results and alerts",
            ("Education", "SuggestContent") => "Relevant educational content recommendations",
            ("Authentication", "AuthSuccess") => "Cross-module authentication state update",
            _ => "Acknowledgment"
        };
    }

    /// <summary>
    /// Get event statistics for monitoring and analytics
    /// </summary>
    public async Task<Dictionary<string, int>> GetEventStatisticsAsync()
    {
        await _counterLock.WaitAsync();
        try
        {
            return new Dictionary<string, int>(_eventCounters);
        }
        finally
        {
            _counterLock.Release();
        }
    }

    public void Dispose()
    {
        _counterLock?.Dispose();
    }
}