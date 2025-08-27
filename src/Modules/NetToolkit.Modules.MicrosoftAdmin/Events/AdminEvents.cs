using NetToolkit.Modules.MicrosoftAdmin.Models;

namespace NetToolkit.Modules.MicrosoftAdmin.Events;

/// <summary>
/// Authentication success event - admin powers activated
/// </summary>
public class AuthenticationSuccessEvent
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime AuthenticatedAt { get; set; }
    public string[] Scopes { get; set; } = Array.Empty<string>();
    public string TenantId { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Authentication failure event - admin gates remain sealed
/// </summary>
public class AuthenticationFailureEvent
{
    public string Error { get; set; } = string.Empty;
    public DateTime FailedAt { get; set; }
    public string AttemptedScopes { get; set; } = string.Empty;
    public Dictionary<string, object> Context { get; set; } = new();
}

/// <summary>
/// Admin task completion event - administrative victory achieved
/// </summary>
public class AdminTaskCompletedEvent{
    public AdminTaskResult TaskResult { get; set; } = new();
    public DateTime CompletedAt { get; set; }
    public bool RequiresFollowUp { get; set; }
    public List<string> NextSuggestedActions { get; set; } = new();
}

/// <summary>
/// Admin task failure event - administrative setback encountered
/// </summary>
public class AdminTaskFailureEvent{
    public string TemplateId { get; set; } = string.Empty;
    public string TaskName { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
    public DateTime FailedAt { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
    public bool RetryRecommended { get; set; }
}

/// <summary>
/// Portal access event - admin realm entered
/// </summary>
public class PortalAccessEvent{
    public string Section { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime AccessedAt { get; set; }
    public string PortalUrl { get; set; } = string.Empty;
    public Dictionary<string, object> AccessContext { get; set; } = new();
}

/// <summary>
/// Sign out event - admin departure from realm
/// </summary>
public class SignOutEvent{
    public DateTime SignedOutAt { get; set; }
    public string Reason { get; set; } = "User initiated";
    public TimeSpan SessionDuration { get; set; }
}

/// <summary>
/// Template usage analytics event - admin template utilization tracking
/// </summary>
public class TemplateUsageEvent{
    public string TemplateId { get; set; } = string.Empty;
    public string TemplateName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime UsedAt { get; set; }
    public bool Successful { get; set; }
    public TimeSpan ExecutionTime { get; set; }
    public Dictionary<string, object> UsageMetadata { get; set; } = new();
}

/// <summary>
/// Permission escalation event - admin privilege elevation detected
/// </summary>
public class PermissionEscalationEvent{
    public string UserId { get; set; } = string.Empty;
    public string[] PreviousScopes { get; set; } = Array.Empty<string>();
    public string[] NewScopes { get; set; } = Array.Empty<string>();
    public DateTime EscalatedAt { get; set; }
    public string Justification { get; set; } = string.Empty;
    public bool RequiresApproval { get; set; }
}

/// <summary>
/// Audit log generated event - administrative action audited
/// </summary>
public class AuditLogGeneratedEvent{
    public AdminAuditLog AuditEntry { get; set; } = new();
    public DateTime LoggedAt { get; set; }
    public bool RequiresEscalation { get; set; }
    public List<string> ComplianceFlags { get; set; } = new();
}

/// <summary>
/// Template validation event - admin script template verified
/// </summary>
public class TemplateValidationEvent{
    public string TemplateId { get; set; } = string.Empty;
    public ValidationResult ValidationResult { get; set; } = new();
    public DateTime ValidatedAt { get; set; }
    public bool IsCustomTemplate { get; set; }
    public string ValidatedBy { get; set; } = string.Empty;
}

/// <summary>
/// Bulk operation progress event - mass administrative action tracking
/// </summary>
public class BulkOperationProgressEvent{
    public string OperationId { get; set; } = string.Empty;
    public string OperationType { get; set; } = string.Empty;
    public int TotalItems { get; set; }
    public int ProcessedItems { get; set; }
    public int SuccessfulItems { get; set; }
    public int FailedItems { get; set; }
    public DateTime LastUpdated { get; set; }
    public List<string> RecentErrors { get; set; } = new();
}

/// <summary>
/// Security alert event - suspicious administrative activity detected
/// </summary>
public class SecurityAlertEvent{
    public string AlertType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime AlertedAt { get; set; }
    public string Severity { get; set; } = "Medium";
    public Dictionary<string, object> AlertData { get; set; } = new();
    public bool RequiresImmedateAction { get; set; }
}

/// <summary>
/// Integration sync event - cross-module communication established
/// </summary>
public class IntegrationSyncEvent{
    public string TargetModule { get; set; } = string.Empty;
    public string SyncType { get; set; } = string.Empty;
    public DateTime SyncedAt { get; set; }
    public bool SyncSuccessful { get; set; }
    public string SyncData { get; set; } = string.Empty;
    public Dictionary<string, object> SyncMetadata { get; set; } = new();
}