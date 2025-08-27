using NetToolkit.Modules.MicrosoftAdmin.Models;

namespace NetToolkit.Modules.MicrosoftAdmin.Interfaces;

/// <summary>
/// Main Microsoft Admin service interface - the gateway to Office 365 administrative mastery
/// Provides secure, automated access to Microsoft Graph APIs for enterprise administration
/// </summary>
public interface IMicrosoftAdminService
{
    /// <summary>
    /// Authenticate with Microsoft Graph using OAuth 2.0 flow
    /// </summary>
    /// <param name="scopes">Required Graph API scopes for operations</param>
    /// <param name="useInteractiveAuth">True for interactive auth, false for cached tokens</param>
    /// <returns>Authentication result with access token and user information</returns>
    Task<AuthenticationResult> AuthenticateAsync(string[] scopes, bool useInteractiveAuth = true);

    /// <summary>
    /// Execute an administrative script template with provided parameters
    /// </summary>
    /// <param name="templateId">Unique identifier of the script template</param>
    /// <param name="parameters">Key-value pairs of template parameters</param>
    /// <param name="dryRun">If true, validate and preview without executing</param>
    /// <returns>Execution result with output, success status, and any errors</returns>
    Task<AdminTaskResult> ExecuteAdminScriptAsync(string templateId, Dictionary<string, object> parameters, bool dryRun = false);

    /// <summary>
    /// Get web portal content for embedding Microsoft 365 Admin Center
    /// </summary>
    /// <param name="portalSection">Specific admin center section to navigate to</param>
    /// <returns>Portal URL and configuration for WebView2 embedding</returns>
    Task<WebViewContent> GetPortalViewAsync(string portalSection = "home");

    /// <summary>
    /// Get current authentication status and user information
    /// </summary>
    /// <returns>Current authentication state and user details</returns>
    Task<AuthStatus> GetAuthStatusAsync();

    /// <summary>
    /// Refresh authentication tokens
    /// </summary>
    /// <returns>Success status of token refresh</returns>
    Task<bool> RefreshTokensAsync();

    /// <summary>
    /// Sign out and clear cached authentication tokens
    /// </summary>
    /// <returns>Success status of sign out operation</returns>
    Task<bool> SignOutAsync();

    /// <summary>
    /// Get available administrative templates
    /// </summary>
    /// <param name="category">Filter templates by category (optional)</param>
    /// <returns>List of available admin script templates</returns>
    Task<IEnumerable<AdminTemplate>> GetAvailableTemplatesAsync(string? category = null);

    /// <summary>
    /// Validate template parameters before execution
    /// </summary>
    /// <param name="templateId">Template to validate against</param>
    /// <param name="parameters">Parameters to validate</param>
    /// <returns>Validation result with any errors or warnings</returns>
    Task<ValidationResult> ValidateParametersAsync(string templateId, Dictionary<string, object> parameters);
}

/// <summary>
/// Script template engine for PowerShell admin automation
/// </summary>
public interface IScriptTemplateEngine
{
    /// <summary>
    /// Load all available admin script templates
    /// </summary>
    /// <returns>Collection of loaded templates</returns>
    Task<List<AdminTemplate>> LoadTemplatesAsync();

    /// <summary>
    /// Generate dynamic form controls for template parameters
    /// </summary>
    /// <param name="template">Template to generate form for</param>
    /// <returns>UI control specifications for dynamic form generation</returns>
    Task<List<UiControl>> GenerateFormControlsAsync(AdminTemplate template);

    /// <summary>
    /// Build PowerShell script from template and parameters
    /// </summary>
    /// <param name="template">Template to build from</param>
    /// <param name="parameters">Parameter values to substitute</param>
    /// <returns>Generated PowerShell script ready for execution</returns>
    Task<string> BuildScriptAsync(AdminTemplate template, Dictionary<string, object> parameters);

    /// <summary>
    /// Validate template syntax and structure
    /// </summary>
    /// <param name="template">Template to validate</param>
    /// <returns>Validation result</returns>
    Task<ValidationResult> ValidateTemplateAsync(AdminTemplate template);

    /// <summary>
    /// Save custom template created by user
    /// </summary>
    /// <param name="template">Custom template to save</param>
    /// <returns>Success status and assigned template ID</returns>
    Task<(bool Success, string TemplateId)> SaveCustomTemplateAsync(AdminTemplate template);

    /// <summary>
    /// Get template by ID
    /// </summary>
    /// <param name="templateId">Unique template identifier</param>
    /// <returns>Template if found, null otherwise</returns>
    Task<AdminTemplate?> GetTemplateByIdAsync(string templateId);
}

/// <summary>
/// Event publisher for admin operations
/// </summary>
public interface IAdminEventPublisher
{
    /// <summary>
    /// Publish authentication success event
    /// </summary>
    Task PublishAuthSuccessAsync(string userId, DateTime authenticatedAt, string[] scopes);

    /// <summary>
    /// Publish admin task execution event
    /// </summary>
    Task PublishTaskExecutedAsync(AdminTaskResult result);

    /// <summary>
    /// Publish error event for troubleshooting
    /// </summary>
    Task PublishErrorOccurredAsync(string operation, Exception error, Dictionary<string, object>? context = null);

    /// <summary>
    /// Publish portal access event
    /// </summary>
    Task PublishPortalAccessedAsync(string portalSection, string userId, DateTime accessedAt);

    /// <summary>
    /// Publish template usage event for analytics
    /// </summary>
    Task PublishTemplateUsedAsync(string templateId, string userId, bool successful);
}

/// <summary>
/// Portal integration for Microsoft 365 Admin Center
/// </summary>
public interface IPortalIntegrator
{
    /// <summary>
    /// Get authenticated portal URL for embedding
    /// </summary>
    /// <param name="section">Admin center section to navigate to</param>
    /// <returns>Portal configuration for WebView2</returns>
    Task<PortalConfig> GetPortalUrlAsync(string section = "home");

    /// <summary>
    /// Handle OAuth redirect from portal authentication
    /// </summary>
    /// <param name="redirectUrl">OAuth redirect URL with auth code</param>
    /// <returns>Authentication handling result</returns>
    Task<bool> HandleOAuthRedirectAsync(string redirectUrl);

    /// <summary>
    /// Check if portal is accessible with current authentication
    /// </summary>
    /// <returns>Portal accessibility status</returns>
    Task<bool> IsPortalAccessibleAsync();

    /// <summary>
    /// Get portal navigation options available to current user
    /// </summary>
    /// <returns>Available portal sections and their URLs</returns>
    Task<Dictionary<string, string>> GetAvailablePortalSectionsAsync();
}

/// <summary>
/// Form builder for dynamic UI generation
/// </summary>
public interface IFormBuilder
{
    /// <summary>
    /// Generate WPF form controls from template parameters
    /// </summary>
    /// <param name="template">Template containing parameter definitions</param>
    /// <returns>Form control specifications</returns>
    Task<FormDefinition> GenerateFormAsync(AdminTemplate template);

    /// <summary>
    /// Validate form input values
    /// </summary>
    /// <param name="formDefinition">Form structure</param>
    /// <param name="inputValues">User-provided values</param>
    /// <returns>Validation result with any errors</returns>
    Task<ValidationResult> ValidateFormInputAsync(FormDefinition formDefinition, Dictionary<string, object> inputValues);

    /// <summary>
    /// Generate form from custom parameter list
    /// </summary>
    /// <param name="parameters">Custom parameter definitions</param>
    /// <returns>Dynamic form definition</returns>
    Task<FormDefinition> GenerateCustomFormAsync(List<TemplateParameter> parameters);
}