using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.Graph;
using NetToolkit.Core.Interfaces;
using NetToolkit.Modules.MicrosoftAdmin.Interfaces;
using NetToolkit.Modules.MicrosoftAdmin.Models;
using NetToolkit.Modules.MicrosoftAdmin.Events;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http;
using Microsoft.Graph.Models;
using Polly;
using Polly.Retry;
using Polly.Timeout;
// Removed: using Microsoft.Graph.Auth; - deprecated in Microsoft Graph SDK v5
// Removed: using Polly.Extensions.DependencyInjection; - not available in Polly v8

namespace NetToolkit.Modules.MicrosoftAdmin.Services;

/// <summary>
/// Core Microsoft Admin service - the gateway to Office 365 administrative mastery
/// Bridges NetToolkit's genius with Microsoft's empire through Graph API excellence
/// </summary>
public class MicrosoftAdminService : IMicrosoftAdminService
{
    private readonly ILogger<MicrosoftAdminService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IEventBus _eventBus;
    private readonly IScriptTemplateEngine _templateEngine;
    private readonly IPortalIntegrator _portalIntegrator;
    private readonly IPublicClientApplication _msalClient;
    private readonly GraphServiceClient _graphClient;
    private readonly ResiliencePipeline _resilienceStrategy;
    
    private Models.AuthenticationResult? _currentAuth;
    private readonly SemaphoreSlim _authLock = new(1, 1);

    public MicrosoftAdminService(
        ILogger<MicrosoftAdminService> logger,
        IConfiguration configuration,
        IEventBus eventBus,
        IScriptTemplateEngine templateEngine,
        IPortalIntegrator portalIntegrator)
    {
        _logger = logger;
        _configuration = configuration;
        _eventBus = eventBus;
        _templateEngine = templateEngine;
        _portalIntegrator = portalIntegrator;
        
        // Initialize MSAL client for OAuth authentication
        var clientId = _configuration["MicrosoftAdmin:ClientId"] ?? 
            throw new InvalidOperationException("Microsoft Admin ClientId not configured");
        var tenantId = _configuration["MicrosoftAdmin:TenantId"] ?? "common";
        var redirectUri = _configuration["MicrosoftAdmin:RedirectUri"] ?? "http://localhost:8080/auth";
        
        _msalClient = PublicClientApplicationBuilder
            .Create(clientId)
            .WithAuthority($"https://login.microsoftonline.com/{tenantId}")
            .WithRedirectUri(redirectUri)
            .WithLogging((level, message, containsPii) => 
            {
                if (!containsPii)
                    _logger.LogDebug("MSAL: {Message}", message);
            })
            .Build();

        // Initialize Graph client with HTTP client that handles authentication
        var httpClient = new HttpClient();
        _graphClient = new GraphServiceClient(httpClient);

        // Configure resilience strategy for Graph API calls
        _resilienceStrategy = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                BackoffType = DelayBackoffType.Exponential,
                Delay = TimeSpan.FromSeconds(1),
                MaxDelay = TimeSpan.FromSeconds(30)
            })
            .AddTimeout(TimeSpan.FromMinutes(2))
            .Build();
    }

    public async Task<Models.AuthenticationResult> AuthenticateAsync(string[] scopes, bool useInteractiveAuth = true)
    {
        await _authLock.WaitAsync();
        try
        {
            _logger.LogInformation("Initiating Microsoft Graph authentication - preparing for admin ascension");

            Models.AuthenticationResult authResult;

            try
            {
                if (!useInteractiveAuth)
                {
                    // Try silent authentication first
                    var accounts = await _msalClient.GetAccountsAsync();
                    var firstAccount = accounts.FirstOrDefault();
                    
                    if (firstAccount != null)
                    {
                        var silentResult = await _msalClient
                            .AcquireTokenSilent(scopes, firstAccount)
                            .ExecuteAsync();
                            
                        authResult = await ConvertMsalResult(silentResult);
                        _logger.LogInformation("Silent authentication successful - admin powers reactivated!");
                        
                        await _eventBus.PublishAsync(new AuthenticationSuccessEvent
                        {
                            UserId = authResult.UserId,
                            UserName = authResult.UserName,
                            AuthenticatedAt = DateTime.UtcNow,
                            Scopes = authResult.GrantedScopes
                        });
                        
                        return authResult;
                    }
                }

                // Interactive authentication
                var interactiveResult = await _msalClient
                    .AcquireTokenInteractive(scopes)
                    .WithPrompt(Microsoft.Identity.Client.Prompt.SelectAccount)
                    .WithExtraQueryParameters(new Dictionary<string, string>
                    {
                        ["prompt"] = "consent"
                    })
                    .ExecuteAsync();

                authResult = await ConvertMsalResult(interactiveResult);
                _currentAuth = authResult;
                
                _logger.LogInformation("Interactive authentication successful - {UserName} ascends to admin throne!", 
                    authResult.UserName);

                await _eventBus.PublishAsync(new AuthenticationSuccessEvent
                {
                    UserId = authResult.UserId,
                    UserName = authResult.UserName,
                    AuthenticatedAt = DateTime.UtcNow,
                    Scopes = authResult.GrantedScopes
                });

                return authResult;
            }
            catch (MsalException ex) when (ex.ErrorCode == MsalError.AuthenticationCanceledError)
            {
                _logger.LogWarning("Authentication canceled by user - admin quest abandoned");
                return new Models.AuthenticationResult
                {
                    IsSuccess = false,
                    Error = "Authentication canceled by user - perhaps another day for admin mastery?"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Authentication failed - admin gates remain sealed");
                
                await _eventBus.PublishAsync(new AuthenticationFailureEvent
                {
                    Error = ex.Message,
                    FailedAt = DateTime.UtcNow
                });
                
                return new Models.AuthenticationResult
                {
                    IsSuccess = false,
                    Error = $"Authentication failed: {ex.Message}"
                };
            }
        }
        finally
        {
            _authLock.Release();
        }
    }

    public async Task<AdminTaskResult> ExecuteAdminScriptAsync(string templateId, Dictionary<string, object> parameters, bool dryRun = false)
    {
        var taskResult = new AdminTaskResult
        {
            TemplateId = templateId,
            Parameters = parameters,
            WasDryRun = dryRun,
            ExecutedBy = _currentAuth?.UserName ?? "Unknown Admin"
        };

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        try
        {
            _logger.LogInformation("Executing admin task: {TemplateId} (DryRun: {DryRun}) - admin alchemy begins!", 
                templateId, dryRun);

            // Get and validate template
            var template = await _templateEngine.GetTemplateByIdAsync(templateId);
            if (template == null)
            {
                taskResult.ErrorOutput = $"Template '{templateId}' not found - like searching for a unicorn in a server rack!";
                taskResult.WittyFeedback = "Template vanished faster than documentation updates!";
                return taskResult;
            }

            taskResult.TaskName = template.Name;

            // Validate parameters
            var validation = await _templateEngine.ValidateTemplateAsync(template);
            if (!validation.IsValid)
            {
                taskResult.ErrorOutput = string.Join("; ", validation.Messages.Select(m => m.Message));
                taskResult.WittyFeedback = "Parameters failed validation - like trying to fit a server in a smartphone case!";
                return taskResult;
            }

            // Build PowerShell script
            var script = await _templateEngine.BuildScriptAsync(template, parameters);
            if (string.IsNullOrEmpty(script))
            {
                taskResult.ErrorOutput = "Failed to generate script from template - the admin magic fizzled!";
                return taskResult;
            }

            if (dryRun)
            {
                taskResult.IsSuccess = true;
                taskResult.Output = $"DRY RUN - Script generated successfully:\n\n{script}";
                taskResult.WittyFeedback = "Dry run complete - like a dress rehearsal for admin greatness!";
                _logger.LogInformation("Dry run completed for template {TemplateId}", templateId);
                return taskResult;
            }

            // Execute script via PowerShell with resilience
            var executionResult = await _resilienceStrategy.ExecuteAsync(async (cancellationToken) =>
            {
                return await ExecutePowerShellScript(script, parameters);
            });

            taskResult.IsSuccess = executionResult.Success;
            taskResult.Output = executionResult.Output;
            taskResult.ErrorOutput = executionResult.ErrorOutput;
            
            if (taskResult.IsSuccess)
            {
                taskResult.WittyFeedback = GetSuccessMessage(templateId);
                _logger.LogInformation("Admin task {TemplateId} executed successfully - admin mastery achieved!", templateId);
            }
            else
            {
                taskResult.WittyFeedback = GetErrorMessage(templateId);
                _logger.LogWarning("Admin task {TemplateId} failed: {Error}", templateId, taskResult.ErrorOutput);
            }

            return taskResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Admin task execution failed catastrophically for {TemplateId}", templateId);
            
            taskResult.ErrorOutput = ex.Message;
            taskResult.WittyFeedback = "Task crashed harder than a Windows ME nostalgia session!";
            
            await _eventBus.PublishAsync(new AdminTaskFailureEvent
            {
                TemplateId = templateId,
                Error = ex.Message,
                FailedAt = DateTime.UtcNow
            });
            
            return taskResult;
        }
        finally
        {
            stopwatch.Stop();
            taskResult.ExecutionTime = stopwatch.Elapsed;
            
            await _eventBus.PublishAsync(new AdminTaskCompletedEvent
            {
                TaskResult = taskResult,
                CompletedAt = DateTime.UtcNow
            });
        }
    }

    public async Task<WebViewContent> GetPortalViewAsync(string portalSection = "home")
    {
        try
        {
            _logger.LogInformation("Preparing portal view for section: {Section} - opening admin gateway", portalSection);
            
            if (_currentAuth == null || !_currentAuth.IsSuccess)
            {
                await AuthenticateAsync(GraphScopes.BasicAdminScopes);
            }

            var portalConfig = await _portalIntegrator.GetPortalUrlAsync(portalSection);
            
            var webViewContent = new WebViewContent
            {
                Url = portalConfig.FullUrl,
                Title = $"Microsoft 365 Admin Center - {portalSection}",
                RequiresAuthentication = true,
                Headers = new Dictionary<string, string>
                {
                    ["Authorization"] = $"Bearer {_currentAuth?.AccessToken}",
                    ["X-Requested-With"] = "NetToolkit-AdminPortal"
                },
                AllowedDomains = new List<string>
                {
                    "admin.microsoft.com",
                    "portal.office.com",
                    "login.microsoftonline.com"
                },
                Configuration = new Dictionary<string, object>
                {
                    ["enableJavaScript"] = true,
                    ["enableWebMessaging"] = true,
                    ["userAgent"] = "NetToolkit-AdminPortal/1.0"
                }
            };

            await _eventBus.PublishAsync(new PortalAccessEvent
            {
                Section = portalSection,
                UserId = _currentAuth?.UserId ?? string.Empty,
                AccessedAt = DateTime.UtcNow
            });

            _logger.LogInformation("Portal view prepared successfully - admin realm awaits!");
            return webViewContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to prepare portal view for section {Section}", portalSection);
            
            return new WebViewContent
            {
                Url = "about:blank",
                Title = "Portal Error",
                Configuration = new Dictionary<string, object>
                {
                    ["error"] = $"Portal preparation failed: {ex.Message}"
                }
            };
        }
    }

    public async Task<AuthStatus> GetAuthStatusAsync()
    {
        try
        {
            if (_currentAuth == null || !_currentAuth.IsSuccess)
            {
                return new AuthStatus { IsAuthenticated = false };
            }

            var tokenExpiry = _currentAuth.ExpiresAt;
            var isExpired = tokenExpiry <= DateTime.UtcNow;
            
            if (isExpired)
            {
                _logger.LogInformation("Token expired - attempting refresh");
                var refreshSuccess = await RefreshTokensAsync();
                
                if (!refreshSuccess)
                {
                    return new AuthStatus { IsAuthenticated = false };
                }
            }

            return new AuthStatus
            {
                IsAuthenticated = true,
                UserName = _currentAuth.UserName,
                UserEmail = _currentAuth.UserEmail,
                TenantId = _currentAuth.TenantId,
                LastAuthenticated = DateTime.UtcNow,
                TokenExpiresAt = _currentAuth.ExpiresAt,
                ActiveScopes = _currentAuth.GrantedScopes
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get authentication status");
            return new AuthStatus { IsAuthenticated = false };
        }
    }

    public async Task<bool> RefreshTokensAsync()
    {
        await _authLock.WaitAsync();
        try
        {
            _logger.LogInformation("Refreshing authentication tokens - renewing admin powers");

            var accounts = await _msalClient.GetAccountsAsync();
            var firstAccount = accounts.FirstOrDefault();
            
            if (firstAccount == null)
            {
                _logger.LogWarning("No cached account found for token refresh");
                return false;
            }

            var refreshResult = await _msalClient
                .AcquireTokenSilent(_currentAuth?.GrantedScopes ?? GraphScopes.BasicAdminScopes, firstAccount)
                .ExecuteAsync();

            _currentAuth = await ConvertMsalResult(refreshResult);
            
            _logger.LogInformation("Token refresh successful - admin session extended!");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token refresh failed - admin powers waning");
            return false;
        }
        finally
        {
            _authLock.Release();
        }
    }

    public async Task<bool> SignOutAsync()
    {
        await _authLock.WaitAsync();
        try
        {
            _logger.LogInformation("Signing out - admin departing the realm");

            var accounts = await _msalClient.GetAccountsAsync();
            foreach (var account in accounts)
            {
                await _msalClient.RemoveAsync(account);
            }

            _currentAuth = null;
            
            await _eventBus.PublishAsync(new SignOutEvent
            {
                SignedOutAt = DateTime.UtcNow
            });
            
            _logger.LogInformation("Sign out successful - admin session concluded");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sign out failed - admin stuck in the system");
            return false;
        }
        finally
        {
            _authLock.Release();
        }
    }

    public async Task<IEnumerable<AdminTemplate>> GetAvailableTemplatesAsync(string? category = null)
    {
        try
        {
            var templates = await _templateEngine.LoadTemplatesAsync();
            
            if (!string.IsNullOrEmpty(category))
            {
                templates = templates.Where(t => 
                    string.Equals(t.Category, category, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            _logger.LogInformation("Retrieved {Count} admin templates (Category: {Category}) - arsenal ready!", 
                templates.Count, category ?? "All");
                
            return templates;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve admin templates");
            return Array.Empty<AdminTemplate>();
        }
    }

    public async Task<ValidationResult> ValidateParametersAsync(string templateId, Dictionary<string, object> parameters)
    {
        try
        {
            var template = await _templateEngine.GetTemplateByIdAsync(templateId);
            if (template == null)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Messages = new List<ValidationMessage>
                    {
                        new()
                        {
                            Field = "templateId",
                            Message = $"Template '{templateId}' not found - like searching for documentation that exists!",
                            Severity = ValidationSeverity.Error
                        }
                    }
                };
            }

            var validationResult = new ValidationResult();
            var messages = new List<ValidationMessage>();

            // Validate each template parameter
            foreach (var templateParam in template.Parameters)
            {
                if (!parameters.ContainsKey(templateParam.Name))
                {
                    if (templateParam.Required)
                    {
                        messages.Add(new ValidationMessage
                        {
                            Field = templateParam.Name,
                            Message = $"Required parameter '{templateParam.DisplayName}' is missing - like forgetting the coffee in a Monday morning!",
                            Severity = ValidationSeverity.Error
                        });
                    }
                    continue;
                }

                var value = parameters[templateParam.Name];
                var validationMessage = ValidateParameter(templateParam, value);
                if (validationMessage != null)
                {
                    messages.Add(validationMessage);
                }
            }

            validationResult.Messages = messages;
            validationResult.IsValid = !validationResult.HasErrors;

            return validationResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Parameter validation failed for template {TemplateId}", templateId);
            
            return new ValidationResult
            {
                IsValid = false,
                Messages = new List<ValidationMessage>
                {
                    new()
                    {
                        Field = "validation",
                        Message = $"Validation process failed: {ex.Message}",
                        Severity = ValidationSeverity.Error
                    }
                }
            };
        }
    }

    private async Task<Models.AuthenticationResult> ConvertMsalResult(Microsoft.Identity.Client.AuthenticationResult msalResult)
    {
        var userInfo = await GetUserInfoFromGraph(msalResult.AccessToken);
        
        return new Models.AuthenticationResult
        {
            IsSuccess = true,
            AccessToken = msalResult.AccessToken,
            ExpiresAt = msalResult.ExpiresOn.DateTime,
            UserId = msalResult.Account?.HomeAccountId?.Identifier ?? string.Empty,
            UserName = userInfo?.DisplayName ?? msalResult.Account?.Username ?? "Unknown User",
            UserEmail = userInfo?.Mail ?? msalResult.Account?.Username ?? string.Empty,
            GrantedScopes = msalResult.Scopes.ToArray(),
            TenantId = msalResult.TenantId ?? string.Empty,
            AdditionalData = new Dictionary<string, object>
            {
                ["AuthTime"] = DateTime.UtcNow,
                ["TokenType"] = "Bearer",
                ["AuthMethod"] = "Interactive"
            }
        };
    }

    private async Task<User?> GetUserInfoFromGraph(string accessToken)
    {
        try
        {
            var tempAuth = new Models.AuthenticationResult { AccessToken = accessToken, IsSuccess = true };
            var previousAuth = _currentAuth;
            _currentAuth = tempAuth;
            
            var user = await _graphClient.Me.GetAsync();
            
            _currentAuth = previousAuth;
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to retrieve user info from Graph API");
            return null;
        }
    }

    private async Task<(bool Success, string Output, string ErrorOutput)> ExecutePowerShellScript(
        string script, Dictionary<string, object> parameters)
    {
        // This would integrate with the PowerShell Terminal Module
        // For now, return a simulated result
        await Task.Delay(100); // Simulate script execution time
        
        return (true, $"Script executed successfully with parameters: {string.Join(", ", parameters.Keys)}", string.Empty);
    }

    private string GetSuccessMessage(string templateId) => templateId switch
    {
        "extend-mailbox" => "Mailbox expanded like admin ambitions - storage overflow averted!",
        "create-shared-mailbox" => "Shared mailbox materialized - collaboration portal activated!",
        "add-email-alias" => "Email alias conjured - identity multiplied like admin superpowers!",
        "bulk-user-create" => "Users spawned en masse - digital population boom achieved!",
        "reset-user-password" => "Password reset ritual completed - access gates refreshed!",
        "assign-license" => "License bestowed upon user - productivity powers unlocked!",
        "create-security-group" => "Security group forged - digital fortress reinforced!",
        "enable-mfa" => "Multi-factor authentication activated - security walls fortified!",
        "tenant-report" => "Tenant report compiled - administrative insights illuminated!",
        "mailbox-permissions" => "Permissions configured - access control matrix updated!",
        _ => "Admin task completed with masterful precision - another victory for NetToolkit!"
    };

    private string GetErrorMessage(string templateId) => templateId switch
    {
        "extend-mailbox" => "Mailbox extension failed - storage expansion hit a cosmic wall!",
        "create-shared-mailbox" => "Shared mailbox creation stumbled - collaboration dreams deferred!",
        "add-email-alias" => "Alias addition fumbled - identity multiplication malfunction!",
        "bulk-user-create" => "User creation spree crashed - digital population control activated!",
        "reset-user-password" => "Password reset ritual interrupted - access gates remain sealed!",
        "assign-license" => "License assignment failed - productivity powers withheld!",
        "create-security-group" => "Security group creation failed - fortress construction halted!",
        "enable-mfa" => "MFA activation blocked - security enhancement postponed!",
        "tenant-report" => "Report generation failed - insights remain in digital shadows!",
        "mailbox-permissions" => "Permission configuration failed - access matrix glitched!",
        _ => "Admin task failed spectacularly - even NetToolkit has off days!"
    };

    private ValidationMessage? ValidateParameter(TemplateParameter param, object value)
    {
        try
        {
            switch (param.Type)
            {
                case ParameterType.Email:
                    if (!IsValidEmail(value?.ToString()))
                    {
                        return new ValidationMessage
                        {
                            Field = param.Name,
                            Message = $"Invalid email format for '{param.DisplayName}' - like a letter without stamp!",
                            Severity = ValidationSeverity.Error
                        };
                    }
                    break;

                case ParameterType.Integer:
                    if (!int.TryParse(value?.ToString(), out var intValue))
                    {
                        return new ValidationMessage
                        {
                            Field = param.Name,
                            Message = $"'{param.DisplayName}' must be a valid integer - no decimal shenanigans allowed!",
                            Severity = ValidationSeverity.Error
                        };
                    }
                    
                    if (param.MinValue.HasValue && intValue < param.MinValue.Value)
                    {
                        return new ValidationMessage
                        {
                            Field = param.Name,
                            Message = $"'{param.DisplayName}' must be at least {param.MinValue.Value} - aim higher!",
                            Severity = ValidationSeverity.Error
                        };
                    }
                    break;

                case ParameterType.String:
                    var stringValue = value?.ToString() ?? string.Empty;
                    
                    if (param.MinLength.HasValue && stringValue.Length < param.MinLength.Value)
                    {
                        return new ValidationMessage
                        {
                            Field = param.Name,
                            Message = $"'{param.DisplayName}' must be at least {param.MinLength.Value} characters - brevity isn't always virtue!",
                            Severity = ValidationSeverity.Error
                        };
                    }
                    
                    if (!string.IsNullOrEmpty(param.ValidationPattern))
                    {
                        if (!System.Text.RegularExpressions.Regex.IsMatch(stringValue, param.ValidationPattern))
                        {
                            return new ValidationMessage
                            {
                                Field = param.Name,
                                Message = param.ValidationMessage.IsNotEmpty() ? param.ValidationMessage : 
                                    $"'{param.DisplayName}' format is invalid - pattern matching failed!",
                                Severity = ValidationSeverity.Error
                            };
                        }
                    }
                    break;
            }

            return null;
        }
        catch (Exception ex)
        {
            return new ValidationMessage
            {
                Field = param.Name,
                Message = $"Validation error for '{param.DisplayName}': {ex.Message}",
                Severity = ValidationSeverity.Error
            };
        }
    }

    private static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public void Dispose()
    {
        _authLock?.Dispose();
    }
}

// Extension method for string validation
public static class StringExtensions
{
    public static bool IsNotEmpty(this string? value) => !string.IsNullOrWhiteSpace(value);
}