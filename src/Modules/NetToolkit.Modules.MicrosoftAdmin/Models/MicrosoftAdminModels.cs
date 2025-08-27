using System.ComponentModel;
using System.Text.Json.Serialization;

namespace NetToolkit.Modules.MicrosoftAdmin.Models;

/// <summary>
/// Microsoft Graph API scope constants for enterprise integration
/// </summary>
public static class GraphScopes
{
    public const string GroupReadWriteAll = "Group.ReadWrite.All";
    public const string DirectoryReadWriteAll = "Directory.ReadWrite.All";
    public const string UserReadWriteAll = "User.ReadWrite.All";
    public const string MailReadWrite = "Mail.ReadWrite";
    public const string CalendarsReadWrite = "Calendars.ReadWrite";
    public const string FilesReadWriteAll = "Files.ReadWrite.All";
    public const string SitesReadWriteAll = "Sites.ReadWrite.All";
    public const string MailboxSettingsReadWrite = "MailboxSettings.ReadWrite";
    public const string RoleManagementRead = "RoleManagement.Read.Directory";
}

/// <summary>
/// Authentication result from Microsoft Graph OAuth flow
/// </summary>
public class AuthenticationResult
{
    public bool IsSuccess { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string[] GrantedScopes { get; set; } = Array.Empty<string>();
    public string TenantId { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

/// <summary>
/// Current authentication status
/// </summary>
public class AuthStatus
{
    public bool IsAuthenticated { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public DateTime? LastAuthenticated { get; set; }
    public DateTime? TokenExpiresAt { get; set; }
    public string[] ActiveScopes { get; set; } = Array.Empty<string>();
    public bool TokenNearExpiry => TokenExpiresAt?.AddMinutes(-10) <= DateTime.UtcNow;
}

/// <summary>
/// Administrative task execution result
/// </summary>
public class AdminTaskResult
{
    public string TaskId { get; set; } = Guid.NewGuid().ToString();
    public string TemplateId { get; set; } = string.Empty;
    public string TaskName { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
    public string Output { get; set; } = string.Empty;
    public string ErrorOutput { get; set; } = string.Empty;
    public TimeSpan ExecutionTime { get; set; }
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
    public string ExecutedBy { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public bool WasDryRun { get; set; }
    public string WittyFeedback { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// WebView2 portal content configuration
/// </summary>
public class WebViewContent
{
    public string Url { get; set; } = string.Empty;
    public Dictionary<string, string> Headers { get; set; } = new();
    public bool RequiresAuthentication { get; set; } = true;
    public string Title { get; set; } = string.Empty;
    public List<string> AllowedDomains { get; set; } = new();
    public Dictionary<string, object> Configuration { get; set; } = new();
}

/// <summary>
/// Portal configuration for Microsoft 365 Admin Center
/// </summary>
public class PortalConfig
{
    public string BaseUrl { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public string FullUrl { get; set; } = string.Empty;
    public Dictionary<string, string> QueryParameters { get; set; } = new();
    public bool IsAuthenticated { get; set; }
    public List<string> RequiredPermissions { get; set; } = new();
}

/// <summary>
/// Administrative script template definition
/// </summary>
public class AdminTemplate
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Author { get; set; } = "NetToolkit";
    public string Version { get; set; } = "1.0.0";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    [JsonPropertyName("script")]
    public string ScriptTemplate { get; set; } = string.Empty;
    
    public List<TemplateParameter> Parameters { get; set; } = new();
    public List<string> RequiredScopes { get; set; } = new();
    public List<string> Prerequisites { get; set; } = new();
    public string EstimatedDuration { get; set; } = string.Empty;
    public TemplateComplexity Complexity { get; set; } = TemplateComplexity.Basic;
    public List<string> Tags { get; set; } = new();
    public string IconName { get; set; } = "script";
    public bool IsCustomTemplate { get; set; }
    public string HelpUrl { get; set; } = string.Empty;
    public List<TemplateExample> Examples { get; set; } = new();
}

/// <summary>
/// Template parameter definition
/// </summary>
public class TemplateParameter
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ParameterType Type { get; set; } = ParameterType.String;
    public bool Required { get; set; } = true;
    public object? DefaultValue { get; set; }
    public List<string> ValidValues { get; set; } = new();
    public string ValidationPattern { get; set; } = string.Empty;
    public string ValidationMessage { get; set; } = string.Empty;
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }
    public double? MinValue { get; set; }
    public double? MaxValue { get; set; }
    public string Placeholder { get; set; } = string.Empty;
    public string HelpText { get; set; } = string.Empty;
    public bool IsSecret { get; set; }
    public List<string> DependsOn { get; set; } = new();
}

/// <summary>
/// Template usage example
/// </summary>
public class TemplateExample
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
    public string ExpectedOutcome { get; set; } = string.Empty;
}

/// <summary>
/// Parameter type enumeration
/// </summary>
public enum ParameterType
{
    String,
    Integer,
    Double,
    Boolean,
    Email,
    Url,
    Password,
    MultilineText,
    DropdownList,
    MultiSelect,
    FilePath,
    Date,
    DateTime,
    Guid,
    Json
}

/// <summary>
/// Template complexity levels
/// </summary>
public enum TemplateComplexity
{
    Basic,
    Intermediate,
    Advanced,
    Expert
}

/// <summary>
/// UI control specification for form generation
/// </summary>
public class UiControl
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public UiControlType Type { get; set; } = UiControlType.TextBox;
    public object? Value { get; set; }
    public bool Required { get; set; }
    public bool Enabled { get; set; } = true;
    public bool Visible { get; set; } = true;
    public string Tooltip { get; set; } = string.Empty;
    public List<SelectOption> Options { get; set; } = new();
    public Dictionary<string, object> Properties { get; set; } = new();
    public List<string> ValidationRules { get; set; } = new();
    public string ValidationMessage { get; set; } = string.Empty;
    public int TabOrder { get; set; }
    public string Group { get; set; } = string.Empty;
}

/// <summary>
/// UI control types for form generation
/// </summary>
public enum UiControlType
{
    TextBox,
    PasswordBox,
    ComboBox,
    CheckBox,
    RadioButton,
    NumericUpDown,
    DatePicker,
    TimePicker,
    MultilineTextBox,
    Button,
    Label,
    Slider,
    ProgressBar,
    FileSelector,
    ColorPicker
}

/// <summary>
/// Select option for dropdown controls
/// </summary>
public class SelectOption
{
    public string Value { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public bool IsSelected { get; set; }
    public string Description { get; set; } = string.Empty;
    public string IconName { get; set; } = string.Empty;
}

/// <summary>
/// Form definition for dynamic UI generation
/// </summary>
public class FormDefinition
{
    public string FormId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<UiControl> Controls { get; set; } = new();
    public List<FormSection> Sections { get; set; } = new();
    public Dictionary<string, object> FormProperties { get; set; } = new();
    public List<FormValidationRule> ValidationRules { get; set; } = new();
}

/// <summary>
/// Form section for organizing controls
/// </summary>
public class FormSection
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> ControlNames { get; set; } = new();
    public bool IsCollapsible { get; set; }
    public bool IsExpanded { get; set; } = true;
    public string IconName { get; set; } = string.Empty;
}

/// <summary>
/// Form validation rule
/// </summary>
public class FormValidationRule
{
    public string RuleId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> AffectedControls { get; set; } = new();
    public string ValidationExpression { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public ValidationSeverity Severity { get; set; } = ValidationSeverity.Error;
}

/// <summary>
/// Validation severity levels
/// </summary>
public enum ValidationSeverity
{
    Info,
    Warning,
    Error
}

/// <summary>
/// Validation result for operations
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; } = true;
    public List<ValidationMessage> Messages { get; set; } = new();
    public Dictionary<string, object> ValidationData { get; set; } = new();

    public bool HasErrors => Messages.Any(m => m.Severity == ValidationSeverity.Error);
    public bool HasWarnings => Messages.Any(m => m.Severity == ValidationSeverity.Warning);
}

/// <summary>
/// Individual validation message
/// </summary>
public class ValidationMessage
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public ValidationSeverity Severity { get; set; } = ValidationSeverity.Error;
    public string Code { get; set; } = string.Empty;
    public Dictionary<string, object> Context { get; set; } = new();
}

/// <summary>
/// Microsoft Graph API scope definitions
/// </summary>
public static class GraphScopes
{
    // User management scopes
    public const string UserRead = "User.Read";
    public const string UserReadAll = "User.Read.All";
    public const string UserReadWrite = "User.ReadWrite";
    public const string UserReadWriteAll = "User.ReadWrite.All";

    // Mail and mailbox scopes
    public const string MailRead = "Mail.Read";
    public const string MailReadWrite = "Mail.ReadWrite";
    public const string MailboxSettingsRead = "MailboxSettings.Read";
    public const string MailboxSettingsReadWrite = "MailboxSettings.ReadWrite";

    // Group management scopes
    public const string GroupRead = "Group.Read.All";
    public const string GroupReadWrite = "Group.ReadWrite.All";

    // Directory scopes
    public const string DirectoryRead = "Directory.Read.All";
    public const string DirectoryReadWrite = "Directory.ReadWrite.All";

    // Administrative scopes
    public const string RoleManagementRead = "RoleManagement.Read.All";
    public const string RoleManagementReadWrite = "RoleManagement.ReadWrite.All";

    // Common scope combinations
    public static readonly string[] BasicAdminScopes = {
        UserRead, MailboxSettingsRead, GroupRead, DirectoryRead
    };

    public static readonly string[] FullAdminScopes = {
        GraphScopes.UserReadWriteAll, GraphScopes.MailboxSettingsReadWrite, GraphScopes.GroupReadWriteAll, 
        GraphScopes.DirectoryReadWriteAll, GraphScopes.RoleManagementRead
    };
}

/// <summary>
/// Admin task categories
/// </summary>
public static class AdminTaskCategories
{
    public const string UserManagement = "User Management";
    public const string MailboxManagement = "Mailbox Management";
    public const string GroupManagement = "Group Management";
    public const string SecurityManagement = "Security Management";
    public const string LicenseManagement = "License Management";
    public const string ReportingAnalytics = "Reporting & Analytics";
    public const string ComplianceGovernance = "Compliance & Governance";
    public const string DeviceManagement = "Device Management";
    public const string ApplicationManagement = "Application Management";
    public const string TenantConfiguration = "Tenant Configuration";
}

/// <summary>
/// Predefined admin task templates metadata
/// </summary>
public static class BuiltInTemplates
{
    public static readonly Dictionary<string, (string Name, string Description, string Category)> Templates = new()
    {
        ["extend-mailbox"] = ("Extend Mailbox Quota", "Increase user mailbox storage quota", AdminTaskCategories.MailboxManagement),
        ["create-shared-mailbox"] = ("Create Shared Mailbox", "Create and configure shared mailbox", AdminTaskCategories.MailboxManagement),
        ["add-email-alias"] = ("Add Email Alias", "Add additional email alias to user", AdminTaskCategories.MailboxManagement),
        ["bulk-user-create"] = ("Bulk Create Users", "Create multiple users from CSV", AdminTaskCategories.UserManagement),
        ["reset-user-password"] = ("Reset User Password", "Reset and notify user password", AdminTaskCategories.UserManagement),
        ["assign-license"] = ("Assign License", "Assign Office 365 license to user", AdminTaskCategories.LicenseManagement),
        ["create-security-group"] = ("Create Security Group", "Create new security group with members", AdminTaskCategories.GroupManagement),
        ["enable-mfa"] = ("Enable MFA", "Enable multi-factor authentication for user", AdminTaskCategories.SecurityManagement),
        ["tenant-report"] = ("Generate Tenant Report", "Create comprehensive tenant usage report", AdminTaskCategories.ReportingAnalytics),
        ["mailbox-permissions"] = ("Configure Mailbox Permissions", "Set mailbox access permissions", AdminTaskCategories.MailboxManagement)
    };
}

/// <summary>
/// Admin operation audit log entry
/// </summary>
public class AdminAuditLog
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public string TemplateId { get; set; } = string.Empty;
    public string Resource { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string Result { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
    public string ClientIp { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
}

/// <summary>
/// Portal navigation section definition
/// </summary>
public class PortalSection
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string IconName { get; set; } = string.Empty;
    public List<string> RequiredRoles { get; set; } = new();
    public List<PortalSection> SubSections { get; set; } = new();
    public bool IsAvailable { get; set; } = true;
}