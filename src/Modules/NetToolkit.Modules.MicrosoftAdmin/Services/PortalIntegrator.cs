using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Web.WebView2.Core;
using NetToolkit.Modules.MicrosoftAdmin.Interfaces;
using NetToolkit.Modules.MicrosoftAdmin.Models;
using System.Text.Json;
using System.Web;
using System.Net.Http;
using System.IO; // INFERRED: For Path class

namespace NetToolkit.Modules.MicrosoftAdmin.Services;

/// <summary>
/// Portal integrator - the bridge to Microsoft's administrative realm
/// Seamlessly embeds Microsoft 365 Admin Center within NetToolkit's embrace
/// </summary>
public class PortalIntegrator : IPortalIntegrator
{
    private readonly ILogger<PortalIntegrator> _logger;
    private readonly IConfiguration _configuration;
    private readonly Dictionary<string, PortalSection> _portalSections;
    private readonly HttpClient _httpClient;

    public PortalIntegrator(
        ILogger<PortalIntegrator> logger,
        IConfiguration configuration,
        HttpClient httpClient)
    {
        _logger = logger;
        _configuration = configuration;
        _httpClient = httpClient;
        
        // Initialize portal sections mapping
        _portalSections = InitializePortalSections();
        
        _logger.LogInformation("Portal integrator initialized - administrative gateway to Microsoft realm ready");
    }

    public async Task<PortalConfig> GetPortalUrlAsync(string section = "home")
    {
        try
        {
            _logger.LogInformation("Preparing portal access for section '{Section}' - opening gateway to Microsoft realm", section);
            
            var portalSection = GetPortalSection(section);
            if (portalSection == null)
            {
                _logger.LogWarning("Unknown portal section '{Section}' requested - defaulting to home", section);
                portalSection = _portalSections["home"];
            }
            
            var baseUrl = _configuration["MicrosoftAdmin:PortalBaseUrl"] ?? "https://admin.microsoft.com";
            var tenantId = _configuration["MicrosoftAdmin:TenantId"] ?? string.Empty;
            
            var portalConfig = new PortalConfig
            {
                BaseUrl = baseUrl,
                Section = section,
                QueryParameters = new Dictionary<string, string>(),
                RequiredPermissions = portalSection.RequiredRoles
            };
            
            // Build section-specific URL
            var sectionUrl = BuildSectionUrl(baseUrl, portalSection, tenantId);
            portalConfig.FullUrl = sectionUrl;
            
            // Add authentication and tracking parameters
            AddAuthenticationParameters(portalConfig);
            AddTrackingParameters(portalConfig);
            
            // Check if portal is accessible
            var isAccessible = await CheckPortalAccessibility(portalConfig.FullUrl);
            portalConfig.IsAuthenticated = isAccessible;
            
            _logger.LogInformation("Portal configuration prepared for section '{Section}' - URL: {Url}", 
                section, portalConfig.FullUrl);
                
            return portalConfig;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to prepare portal configuration for section '{Section}'", section);
            
            return new PortalConfig
            {
                BaseUrl = "about:blank",
                Section = section,
                FullUrl = "about:blank",
                IsAuthenticated = false
            };
        }
    }

    public async Task<bool> HandleOAuthRedirectAsync(string redirectUrl)
    {
        try
        {
            _logger.LogInformation("Handling OAuth redirect - processing authentication callback");
            
            if (string.IsNullOrWhiteSpace(redirectUrl))
            {
                _logger.LogWarning("Empty redirect URL received - OAuth flow incomplete");
                return false;
            }
            
            var uri = new Uri(redirectUrl);
            var queryParameters = HttpUtility.ParseQueryString(uri.Query);
            
            // Extract authentication code
            var authCode = queryParameters["code"];
            var state = queryParameters["state"];
            var error = queryParameters["error"];
            var errorDescription = queryParameters["error_description"];
            
            if (!string.IsNullOrEmpty(error))
            {
                _logger.LogError("OAuth error received: {Error} - {Description}", error, errorDescription);
                return false;
            }
            
            if (string.IsNullOrEmpty(authCode))
            {
                _logger.LogWarning("No authorization code received in OAuth redirect");
                return false;
            }
            
            // Validate state parameter if provided
            if (!string.IsNullOrEmpty(state) && !ValidateOAuthState(state))
            {
                _logger.LogError("Invalid OAuth state parameter - possible security issue");
                return false;
            }
            
            _logger.LogInformation("OAuth redirect handled successfully - authentication code received");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to handle OAuth redirect: {Url}", redirectUrl);
            return false;
        }
    }

    public async Task<bool> IsPortalAccessibleAsync()
    {
        try
        {
            _logger.LogDebug("Checking portal accessibility - testing administrative gateway");
            
            var baseUrl = _configuration["MicrosoftAdmin:PortalBaseUrl"] ?? "https://admin.microsoft.com";
            var healthCheckUrl = $"{baseUrl}/api/health";
            
            using var response = await _httpClient.GetAsync(healthCheckUrl);
            var isAccessible = response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.Redirect;
            
            _logger.LogDebug("Portal accessibility check completed - Status: {IsAccessible}", isAccessible);
            return isAccessible;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Portal accessibility check failed due to network issue");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Portal accessibility check failed");
            return false;
        }
    }

    public async Task<Dictionary<string, string>> GetAvailablePortalSectionsAsync()
    {
        try
        {
            _logger.LogDebug("Retrieving available portal sections - cataloging administrative domains");
            
            var availableSections = new Dictionary<string, string>();
            var baseUrl = _configuration["MicrosoftAdmin:PortalBaseUrl"] ?? "https://admin.microsoft.com";
            
            foreach (var section in _portalSections.Values.Where(s => s.IsAvailable))
            {
                var sectionUrl = BuildSectionUrl(baseUrl, section, string.Empty);
                availableSections[section.Id] = sectionUrl;
            }
            
            _logger.LogDebug("Retrieved {Count} available portal sections", availableSections.Count);
            return await Task.FromResult(availableSections);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve available portal sections");
            return new Dictionary<string, string>();
        }
    }

    private Dictionary<string, PortalSection> InitializePortalSections()
    {
        return new Dictionary<string, PortalSection>
        {
            ["home"] = new PortalSection
            {
                Id = "home",
                Name = "Admin Home",
                Description = "Microsoft 365 Admin Center home dashboard",
                Url = "/",
                IconName = "home",
                IsAvailable = true
            },
            ["users"] = new PortalSection
            {
                Id = "users",
                Name = "Users",
                Description = "User management and administration",
                Url = "#/users",
                IconName = "users",
                RequiredRoles = new List<string> { "User Administrator", "Global Administrator" },
                IsAvailable = true
            },
            ["groups"] = new PortalSection
            {
                Id = "groups",
                Name = "Groups",
                Description = "Group management and collaboration settings",
                Url = "#/groups",
                IconName = "group",
                RequiredRoles = new List<string> { "Groups Administrator", "Global Administrator" },
                IsAvailable = true
            },
            ["licenses"] = new PortalSection
            {
                Id = "licenses",
                Name = "Licenses",
                Description = "License management and assignment",
                Url = "#/licenses",
                IconName = "certificate",
                RequiredRoles = new List<string> { "License Administrator", "Global Administrator" },
                IsAvailable = true
            },
            ["billing"] = new PortalSection
            {
                Id = "billing",
                Name = "Billing",
                Description = "Billing and subscription management",
                Url = "#/billing",
                IconName = "credit-card",
                RequiredRoles = new List<string> { "Billing Administrator", "Global Administrator" },
                IsAvailable = true
            },
            ["security"] = new PortalSection
            {
                Id = "security",
                Name = "Security",
                Description = "Security settings and compliance",
                Url = "#/security",
                IconName = "shield",
                RequiredRoles = new List<string> { "Security Administrator", "Global Administrator" },
                IsAvailable = true
            },
            ["exchange"] = new PortalSection
            {
                Id = "exchange",
                Name = "Exchange",
                Description = "Exchange Online administration",
                Url = "#/exchange",
                IconName = "mail",
                RequiredRoles = new List<string> { "Exchange Administrator", "Global Administrator" },
                IsAvailable = true,
                SubSections = new List<PortalSection>
                {
                    new()
                    {
                        Id = "mailboxes",
                        Name = "Mailboxes",
                        Description = "Mailbox management and configuration",
                        Url = "#/exchange/mailboxes",
                        IconName = "mailbox"
                    },
                    new()
                    {
                        Id = "mail-flow",
                        Name = "Mail Flow",
                        Description = "Mail flow rules and transport settings",
                        Url = "#/exchange/mail-flow",
                        IconName = "flow"
                    }
                }
            },
            ["sharepoint"] = new PortalSection
            {
                Id = "sharepoint",
                Name = "SharePoint",
                Description = "SharePoint Online administration",
                Url = "#/sharepoint",
                IconName = "sharepoint",
                RequiredRoles = new List<string> { "SharePoint Administrator", "Global Administrator" },
                IsAvailable = true
            },
            ["teams"] = new PortalSection
            {
                Id = "teams",
                Name = "Teams",
                Description = "Microsoft Teams administration",
                Url = "#/teams",
                IconName = "teams",
                RequiredRoles = new List<string> { "Teams Administrator", "Global Administrator" },
                IsAvailable = true
            },
            ["devices"] = new PortalSection
            {
                Id = "devices",
                Name = "Devices",
                Description = "Device management and mobile device administration",
                Url = "#/devices",
                IconName = "device",
                RequiredRoles = new List<string> { "Intune Administrator", "Global Administrator" },
                IsAvailable = true
            },
            ["reports"] = new PortalSection
            {
                Id = "reports",
                Name = "Reports",
                Description = "Usage reports and analytics",
                Url = "#/reports",
                IconName = "chart",
                RequiredRoles = new List<string> { "Reports Reader", "Global Administrator" },
                IsAvailable = true
            },
            ["settings"] = new PortalSection
            {
                Id = "settings",
                Name = "Settings",
                Description = "Organization settings and configuration",
                Url = "#/settings",
                IconName = "settings",
                RequiredRoles = new List<string> { "Global Administrator" },
                IsAvailable = true
            },
            ["health"] = new PortalSection
            {
                Id = "health",
                Name = "Service Health",
                Description = "Service health and status monitoring",
                Url = "#/servicehealth",
                IconName = "health",
                RequiredRoles = new List<string> { "Service Support Administrator", "Global Administrator" },
                IsAvailable = true
            },
            ["message-center"] = new PortalSection
            {
                Id = "message-center",
                Name = "Message Center",
                Description = "Microsoft 365 message center and announcements",
                Url = "#/messagecenter",
                IconName = "message",
                RequiredRoles = new List<string> { "Message Center Reader", "Global Administrator" },
                IsAvailable = true
            }
        };
    }

    private PortalSection? GetPortalSection(string sectionId)
    {
        if (_portalSections.TryGetValue(sectionId.ToLowerInvariant(), out var section))
        {
            return section;
        }
        
        // Check subsections
        foreach (var parentSection in _portalSections.Values)
        {
            var subSection = parentSection.SubSections.FirstOrDefault(s => 
                string.Equals(s.Id, sectionId, StringComparison.OrdinalIgnoreCase));
            if (subSection != null)
            {
                return subSection;
            }
        }
        
        return null;
    }

    private string BuildSectionUrl(string baseUrl, PortalSection section, string tenantId)
    {
        var urlBuilder = new UriBuilder(baseUrl);
        
        // Handle different URL patterns for different sections
        if (section.Url.StartsWith("#/"))
        {
            // Modern admin center URLs
            urlBuilder.Fragment = section.Url.Substring(1); // Remove the # prefix
        }
        else if (section.Url.StartsWith("/"))
        {
            // Root-relative URLs
            urlBuilder.Path = section.Url;
        }
        else
        {
            // Handle absolute URLs or special cases
            if (Uri.TryCreate(section.Url, UriKind.Absolute, out var absoluteUri))
            {
                return absoluteUri.ToString();
            }
            else
            {
                urlBuilder.Path = $"/{section.Url}";
            }
        }
        
        // Add tenant context if available
        if (!string.IsNullOrEmpty(tenantId))
        {
            var query = HttpUtility.ParseQueryString(urlBuilder.Query);
            query["tenantId"] = tenantId;
            urlBuilder.Query = query.ToString();
        }
        
        return urlBuilder.ToString();
    }

    private void AddAuthenticationParameters(PortalConfig config)
    {
        // Add authentication-related query parameters
        config.QueryParameters["source"] = "NetToolkit";
        config.QueryParameters["integrated"] = "true";
        config.QueryParameters["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        
        // Add client information
        config.QueryParameters["client"] = "NetToolkit-AdminPortal";
        config.QueryParameters["version"] = "1.0.0";
    }

    private void AddTrackingParameters(PortalConfig config)
    {
        // Add tracking parameters for analytics
        config.QueryParameters["utm_source"] = "NetToolkit";
        config.QueryParameters["utm_medium"] = "admin-portal";
        config.QueryParameters["utm_campaign"] = "integrated-access";
        
        // Generate session tracking ID
        var sessionId = Guid.NewGuid().ToString("N")[..16];
        config.QueryParameters["session_id"] = sessionId;
    }

    private async Task<bool> CheckPortalAccessibility(string url)
    {
        try
        {
            _logger.LogDebug("Checking accessibility for portal URL: {Url}", url);
            
            // Create a simple HEAD request to check if the portal is accessible
            using var request = new HttpRequestMessage(HttpMethod.Head, url);
            request.Headers.Add("User-Agent", "NetToolkit-AdminPortal/1.0");
            request.Headers.Add("Accept", "text/html,application/xhtml+xml");
            
            using var response = await _httpClient.SendAsync(request);
            
            // Consider 2xx and 3xx responses as accessible
            var isAccessible = ((int)response.StatusCode >= 200 && (int)response.StatusCode < 400);
            
            _logger.LogDebug("Portal accessibility check result: {StatusCode} - {IsAccessible}", 
                response.StatusCode, isAccessible);
                
            return isAccessible;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogDebug(ex, "Portal accessibility check failed with HTTP error");
            return false;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogDebug(ex, "Portal accessibility check timed out");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unexpected error during portal accessibility check");
            return false;
        }
    }

    private bool ValidateOAuthState(string state)
    {
        try
        {
            // Decode and validate the state parameter
            // This would typically contain a timestamp and/or session identifier
            var stateBytes = Convert.FromBase64String(state);
            var stateJson = System.Text.Encoding.UTF8.GetString(stateBytes);
            var stateData = JsonSerializer.Deserialize<Dictionary<string, object>>(stateJson);
            
            if (stateData == null) return false;
            
            // Check timestamp to prevent replay attacks (5 minute window)
            if (stateData.TryGetValue("timestamp", out var timestampObj) && 
                long.TryParse(timestampObj.ToString(), out var timestamp))
            {
                var stateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp);
                var now = DateTimeOffset.UtcNow;
                
                if (now.Subtract(stateTime).TotalMinutes > 5)
                {
                    _logger.LogWarning("OAuth state timestamp too old: {StateTime}", stateTime);
                    return false;
                }
            }
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate OAuth state parameter");
            return false;
        }
    }

    /// <summary>
    /// Generate secure state parameter for OAuth flows
    /// </summary>
    public string GenerateOAuthState(Dictionary<string, object>? additionalData = null)
    {
        try
        {
            var stateData = new Dictionary<string, object>
            {
                ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                ["nonce"] = Guid.NewGuid().ToString("N"),
                ["source"] = "NetToolkit"
            };
            
            if (additionalData != null)
            {
                foreach (var kvp in additionalData)
                {
                    stateData[kvp.Key] = kvp.Value;
                }
            }
            
            var stateJson = JsonSerializer.Serialize(stateData);
            var stateBytes = System.Text.Encoding.UTF8.GetBytes(stateJson);
            
            return Convert.ToBase64String(stateBytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate OAuth state parameter");
            return Guid.NewGuid().ToString("N");
        }
    }

    /// <summary>
    /// Configure WebView2 for optimal portal integration
    /// </summary>
    public async Task<CoreWebView2Environment> CreateWebView2EnvironmentAsync()
    {
        try
        {
            _logger.LogInformation("Creating WebView2 environment for portal integration");
            
            var options = new CoreWebView2EnvironmentOptions();
            
            // Configure for admin portal compatibility
            options.AdditionalBrowserArguments = 
                "--disable-web-security " +
                "--allow-running-insecure-content " +
                "--ignore-certificate-errors " +
                "--disable-features=VizDisplayCompositor " +
                "--user-agent=\"NetToolkit-AdminPortal/1.0 (Windows NT 10.0; Win64; x64)\"";
            
            var environment = await CoreWebView2Environment.CreateAsync(
                browserExecutableFolder: null,
                userDataFolder: Path.Combine(Path.GetTempPath(), "NetToolkit", "AdminPortal"),
                options: options);
            
            _logger.LogInformation("WebView2 environment created successfully for admin portal");
            return environment;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create WebView2 environment");
            throw;
        }
    }

    /// <summary>
    /// Configure WebView2 settings for admin portal
    /// </summary>
    public void ConfigureWebViewSettings(CoreWebView2Settings settings)
    {
        try
        {
            _logger.LogDebug("Configuring WebView2 settings for admin portal");
            
            settings.IsScriptEnabled = true;
            settings.AreDefaultScriptDialogsEnabled = true;
            settings.IsWebMessageEnabled = true;
            settings.AreHostObjectsAllowed = false; // Security: disable host objects
            settings.AreDefaultContextMenusEnabled = true;
            settings.AreDevToolsEnabled = false; // Disable dev tools in production
            settings.IsPasswordAutosaveEnabled = true;
            settings.IsGeneralAutofillEnabled = true;
            settings.UserAgent = "NetToolkit-AdminPortal/1.0 (Windows NT 10.0; Win64; x64)";
            
            _logger.LogDebug("WebView2 settings configured for optimal admin portal experience");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to configure WebView2 settings");
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}