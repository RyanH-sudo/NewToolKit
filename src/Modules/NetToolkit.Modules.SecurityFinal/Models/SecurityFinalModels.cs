using System.Text.Json.Serialization;

namespace NetToolkit.Modules.SecurityFinal.Models;

/// <summary>
/// Comprehensive security and final touch model definitions
/// The ultimate data structures for NetToolkit's security fortress
/// </summary>

#region Core Enumerations

/// <summary>
/// Types of data that can be encrypted
/// </summary>
public enum DataType
{
    Configuration,
    UserSettings,
    Credentials,
    ScriptContent,
    LogFiles,
    TemporaryFiles,
    DatabaseConnections,
    ApiKeys,
    CertificateData,
    All
}

/// <summary>
/// Authentication security levels
/// </summary>
public enum AuthLevel
{
    None,
    Basic,          // Windows authentication
    Enhanced,       // Windows auth + role checks
    SecurePin,      // Windows auth + PIN
    Maximum         // All security measures enabled
}

/// <summary>
/// Security levels for the overall system
/// </summary>
public enum SecurityLevel
{
    Minimal,
    Standard,
    Enhanced,
    Maximum,
    Military        // Ultimate security
}

/// <summary>
/// Types of encryption keys
/// </summary>
public enum KeyType
{
    UserData,
    Configuration,
    Temporary,
    System,
    Master
}

/// <summary>
/// Types of secure operations
/// </summary>
public enum SecureOperation
{
    ReadConfiguration,
    WriteConfiguration,
    ExecuteScript,
    AccessSensitiveData,
    ModifySettings,
    ViewAuditLogs,
    ManageUsers,
    DeployApplication,
    RunDiagnostics,
    AccessAdminFeatures
}

/// <summary>
/// User roles for authorization
/// </summary>
public enum UserRole
{
    Guest,
    User,
    PowerUser,
    Administrator,
    SystemAdministrator,
    SecurityOfficer
}

/// <summary>
/// Test execution status
/// </summary>
public enum TestStatus
{
    NotRun,
    Running,
    Passed,
    Failed,
    Skipped,
    Inconclusive
}

/// <summary>
/// Deployment target environments
/// </summary>
public enum DeploymentEnvironment
{
    Development,
    Testing,
    Staging,
    Production,
    Enterprise
}

/// <summary>
/// Recovery action types
/// </summary>
public enum RecoveryActionType
{
    Ignore,
    Retry,
    Fallback,
    Restart,
    Graceful,
    Emergency
}

#endregion

#region Security Models

/// <summary>
/// Comprehensive security status across all NetToolkit modules
/// </summary>
public class SecurityStatus
{
    public SecurityLevel CurrentLevel { get; set; }
    public Dictionary<DataType, bool> EncryptionStatus { get; set; } = new();
    public Dictionary<string, bool> ModuleSecurityStatus { get; set; } = new();
    public AuthenticationResult Authentication { get; set; } = new();
    public DateTime LastSecurityUpdate { get; set; }
    public List<SecurityVulnerability> IdentifiedVulnerabilities { get; set; } = new();
    public int AuditLogCount { get; set; }
    public bool IsProductionReady { get; set; }
    public Dictionary<string, object> SecurityMetrics { get; set; } = new();
}

/// <summary>
/// Security vulnerability information
/// </summary>
public class SecurityVulnerability
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public SeverityLevel Severity { get; set; }
    public required string Component { get; set; }
    public DateTime DiscoveredAt { get; set; }
    public string? MitigationSuggestion { get; set; }
    public bool IsResolved { get; set; }
}

/// <summary>
/// Severity levels for vulnerabilities
/// </summary>
public enum SeverityLevel
{
    Informational,
    Low,
    Medium,
    High,
    Critical
}

/// <summary>
/// Authentication result information
/// </summary>
public class AuthenticationResult
{
    public bool IsAuthenticated { get; set; }
    public required string Username { get; set; }
    public UserRole Role { get; set; }
    public DateTime AuthenticatedAt { get; set; }
    public bool IsPinEnabled { get; set; }
    public List<string> Permissions { get; set; } = new();
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Authorization result for specific operations
/// </summary>
public class AuthorizationResult
{
    public bool IsAuthorized { get; set; }
    public SecureOperation Operation { get; set; }
    public UserRole RequiredRole { get; set; }
    public UserRole UserRole { get; set; }
    public string? DenialReason { get; set; }
    public DateTime CheckedAt { get; set; }
}

#endregion

#region Encryption Models

/// <summary>
/// Encrypted data package with metadata
/// </summary>
public class EncryptedData
{
    public required byte[] Data { get; set; }
    public required byte[] Salt { get; set; }
    public required byte[] IV { get; set; }
    public required string Algorithm { get; set; }
    public int KeySize { get; set; }
    public DateTime EncryptedAt { get; set; }
    public required string KeyId { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
}

/// <summary>
/// Encryption context and configuration
/// </summary>
public class EncryptionContext
{
    public required string Purpose { get; set; }
    public KeyType KeyType { get; set; }
    public required string UserId { get; set; }
    public Dictionary<string, string> Parameters { get; set; } = new();
    public bool UseHardwareEncryption { get; set; }
    public int KeySize { get; set; } = 256;
}

/// <summary>
/// Encryption key information
/// </summary>
public class EncryptionKey
{
    public required string Id { get; set; }
    public KeyType Type { get; set; }
    public required string Algorithm { get; set; }
    public int KeySize { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public required string CreatedBy { get; set; }
    public bool IsActive { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
}

/// <summary>
/// Encryption validation results
/// </summary>
public class EncryptionValidation
{
    public bool IsValid { get; set; }
    public Dictionary<string, bool> FileValidation { get; set; } = new();
    public List<string> CorruptedFiles { get; set; } = new();
    public List<string> ValidationErrors { get; set; } = new();
    public DateTime ValidatedAt { get; set; }
    public int TotalFilesChecked { get; set; }
    public int ValidFiles { get; set; }
    public int InvalidFiles { get; set; }
}

#endregion

#region Error Handling Models

/// <summary>
/// Error context information for exception handling
/// </summary>
public class ErrorContext
{
    public required string ModuleName { get; set; }
    public required string OperationName { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
    public required string UserId { get; set; }
    public DateTime OccurredAt { get; set; }
    public string? AdditionalInfo { get; set; }
    public int AttemptNumber { get; set; } = 1;
}

/// <summary>
/// Recovery action taken for an exception
/// </summary>
public class RecoveryAction
{
    public RecoveryActionType ActionType { get; set; }
    public required string Description { get; set; }
    public required string WittyMessage { get; set; }
    public bool WasSuccessful { get; set; }
    public TimeSpan TimeTaken { get; set; }
    public Dictionary<string, object> RecoveryDetails { get; set; } = new();
    public string? NextSteps { get; set; }
}

/// <summary>
/// Error handling statistics
/// </summary>
public class ErrorStatistics
{
    public int TotalErrors { get; set; }
    public int RecoveredErrors { get; set; }
    public int UnrecoveredErrors { get; set; }
    public Dictionary<string, int> ErrorsByType { get; set; } = new();
    public Dictionary<string, int> ErrorsByModule { get; set; } = new();
    public TimeSpan TotalDowntime { get; set; }
    public DateTime StatisticsPeriodStart { get; set; }
    public DateTime StatisticsPeriodEnd { get; set; }
    public float RecoverySuccessRate => TotalErrors > 0 ? (float)RecoveredErrors / TotalErrors * 100 : 100;
}

/// <summary>
/// Error handling test results
/// </summary>
public class ErrorHandlingTestResult
{
    public bool AllTestsPassed { get; set; }
    public Dictionary<string, bool> TestResults { get; set; } = new();
    public List<string> FailedTests { get; set; } = new();
    public TimeSpan TotalTestTime { get; set; }
    public DateTime TestedAt { get; set; }
    public string? AdditionalNotes { get; set; }
}

#endregion

#region Testing Models

/// <summary>
/// Comprehensive test results for NetToolkit
/// </summary>
public class TestResult
{
    public bool AllTestsPassed { get; set; }
    public int TotalTests { get; set; }
    public int PassedTests { get; set; }
    public int FailedTests { get; set; }
    public int SkippedTests { get; set; }
    public TimeSpan TotalExecutionTime { get; set; }
    public float CoveragePercentage { get; set; }
    public Dictionary<string, ModuleTestResult> ModuleResults { get; set; } = new();
    public List<TestFailure> Failures { get; set; } = new();
    public DateTime ExecutedAt { get; set; }
    public TestExecutionEnvironment Environment { get; set; } = new();
}

/// <summary>
/// Test results for a specific module
/// </summary>
public class ModuleTestResult
{
    public required string ModuleName { get; set; }
    public int TotalTests { get; set; }
    public int PassedTests { get; set; }
    public int FailedTests { get; set; }
    public int SkippedTests { get; set; }
    public TimeSpan ExecutionTime { get; set; }
    public float CoveragePercentage { get; set; }
    public List<TestFailure> Failures { get; set; } = new();
}

/// <summary>
/// Test failure information
/// </summary>
public class TestFailure
{
    public required string TestName { get; set; }
    public required string ModuleName { get; set; }
    public required string ErrorMessage { get; set; }
    public string? StackTrace { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTime FailedAt { get; set; }
}

/// <summary>
/// Test execution environment information
/// </summary>
public class TestExecutionEnvironment
{
    public required string MachineName { get; set; }
    public required string OperatingSystem { get; set; }
    public required string Runtime { get; set; }
    public int ProcessorCount { get; set; }
    public long TotalMemoryMb { get; set; }
    public Dictionary<string, string> EnvironmentVariables { get; set; } = new();
}

/// <summary>
/// Test discovery results
/// </summary>
public class TestDiscovery
{
    public int TotalTestsFound { get; set; }
    public Dictionary<string, int> TestsByAssembly { get; set; } = new();
    public List<string> TestAssemblies { get; set; } = new();
    public DateTime DiscoveredAt { get; set; }
    public TimeSpan DiscoveryTime { get; set; }
}

/// <summary>
/// Test execution configuration
/// </summary>
public class TestExecution
{
    public TestFilter? Filter { get; set; }
    public TestResult Result { get; set; } = new();
    public bool ParallelExecution { get; set; } = true;
    public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;
    public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(30);
}

/// <summary>
/// Test filtering criteria
/// </summary>
public class TestFilter
{
    public List<string> IncludedCategories { get; set; } = new();
    public List<string> ExcludedCategories { get; set; } = new();
    public List<string> IncludedModules { get; set; } = new();
    public List<string> ExcludedModules { get; set; } = new();
    public string? NamePattern { get; set; }
}

/// <summary>
/// Code coverage report
/// </summary>
public class CoverageReport
{
    public float OverallCoverage { get; set; }
    public Dictionary<string, float> ModuleCoverage { get; set; } = new();
    public Dictionary<string, float> ClassCoverage { get; set; } = new();
    public List<string> UncoveredMethods { get; set; } = new();
    public DateTime GeneratedAt { get; set; }
    public string? ReportPath { get; set; }
}

/// <summary>
/// Integration test results
/// </summary>
public class IntegrationTestResult
{
    public bool AllIntegrationTestsPassed { get; set; }
    public Dictionary<string, bool> ModuleIntegrationTests { get; set; } = new();
    public List<string> FailedIntegrations { get; set; } = new();
    public TimeSpan TotalExecutionTime { get; set; }
    public DateTime ExecutedAt { get; set; }
}

/// <summary>
/// Test quality assessment
/// </summary>
public class TestQualityAssessment
{
    public float QualityScore { get; set; } // 0-100
    public float CoverageScore { get; set; }
    public float ReliabilityScore { get; set; }
    public float PerformanceScore { get; set; }
    public List<string> Recommendations { get; set; } = new();
    public DateTime AssessedAt { get; set; }
}

#endregion

#region Deployment Models

/// <summary>
/// Deployment configuration options
/// </summary>
public class DeploymentOptions
{
    public DeploymentEnvironment TargetEnvironment { get; set; }
    public required string Version { get; set; }
    public required string ApplicationName { get; set; }
    public bool IncludeDebugSymbols { get; set; }
    public bool CreateInstaller { get; set; } = true;
    public bool EnableUpdates { get; set; } = true;
    public Dictionary<string, string> Configuration { get; set; } = new();
    public List<string> AdditionalFiles { get; set; } = new();
}

/// <summary>
/// Generated installer package information
/// </summary>
public class InstallerPackage
{
    public required string PackagePath { get; set; }
    public required string Version { get; set; }
    public long SizeBytes { get; set; }
    public DateTime CreatedAt { get; set; }
    public required string Checksum { get; set; }
    public InstallerType Type { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
    public List<string> IncludedFiles { get; set; } = new();
    public bool IsSigned { get; set; }
}

/// <summary>
/// Types of installers
/// </summary>
public enum InstallerType
{
    Msi,
    ClickOnce,
    Portable,
    Zip,
    Setup
}

/// <summary>
/// MSI installer options
/// </summary>
public class MsiOptions
{
    public required string ProductName { get; set; }
    public required string Version { get; set; }
    public required string Manufacturer { get; set; }
    public string? ProductCode { get; set; }
    public string? UpgradeCode { get; set; }
    public string? InstallDirectory { get; set; }
    public bool RequireAdministrator { get; set; } = true;
    public List<string> Shortcuts { get; set; } = new();
    public Dictionary<string, string> RegistryEntries { get; set; } = new();
}

/// <summary>
/// ClickOnce deployment options
/// </summary>
public class ClickOnceOptions
{
    public required string PublishUrl { get; set; }
    public required string ApplicationName { get; set; }
    public required string Version { get; set; }
    public bool AutoUpdate { get; set; } = true;
    public TimeSpan UpdateCheckInterval { get; set; } = TimeSpan.FromHours(24);
    public SecurityLevel RequiredSecurityLevel { get; set; } = SecurityLevel.Standard;
}

/// <summary>
/// ClickOnce deployment information
/// </summary>
public class ClickOnceDeployment
{
    public required string ManifestPath { get; set; }
    public required string PublishUrl { get; set; }
    public required string Version { get; set; }
    public DateTime PublishedAt { get; set; }
    public bool IsPublished { get; set; }
    public List<string> DeployedFiles { get; set; } = new();
}

/// <summary>
/// Application packaging options
/// </summary>
public class PackageOptions
{
    public required string OutputPath { get; set; }
    public PackageFormat Format { get; set; } = PackageFormat.Zip;
    public CompressionLevel Compression { get; set; } = CompressionLevel.Optimal;
    public bool IncludeSource { get; set; } = false;
    public bool IncludeDocumentation { get; set; } = true;
    public List<string> ExcludePatterns { get; set; } = new();
}

/// <summary>
/// Package formats
/// </summary>
public enum PackageFormat
{
    Zip,
    SevenZip,
    Tar,
    TarGz,
    Installer
}

/// <summary>
/// Compression levels
/// </summary>
public enum CompressionLevel
{
    None,
    Fastest,
    Optimal,
    Maximum
}

/// <summary>
/// Application package information
/// </summary>
public class ApplicationPackage
{
    public required string PackagePath { get; set; }
    public PackageFormat Format { get; set; }
    public long SizeBytes { get; set; }
    public DateTime CreatedAt { get; set; }
    public required string Checksum { get; set; }
    public List<string> ContainedFiles { get; set; } = new();
    public Dictionary<string, string> Metadata { get; set; } = new();
}

/// <summary>
/// Package validation results
/// </summary>
public class PackageValidation
{
    public bool IsValid { get; set; }
    public List<string> ValidationErrors { get; set; } = new();
    public List<string> ValidationWarnings { get; set; } = new();
    public bool ChecksumValid { get; set; }
    public bool AllFilesPresent { get; set; }
    public DateTime ValidatedAt { get; set; }
}

/// <summary>
/// Update mechanism configuration
/// </summary>
public class UpdateOptions
{
    public required string UpdateUrl { get; set; }
    public TimeSpan CheckInterval { get; set; } = TimeSpan.FromHours(24);
    public bool AutoDownload { get; set; } = true;
    public bool AutoInstall { get; set; } = false;
    public SecurityLevel RequiredSecurityLevel { get; set; } = SecurityLevel.Enhanced;
}

/// <summary>
/// Update mechanism information
/// </summary>
public class UpdateMechanism
{
    public required string UpdateUrl { get; set; }
    public TimeSpan CheckInterval { get; set; }
    public bool IsEnabled { get; set; }
    public DateTime LastCheckAt { get; set; }
    public string? LatestVersion { get; set; }
    public bool UpdateAvailable { get; set; }
}

#endregion

#region Recovery Models

/// <summary>
/// Save operation context
/// </summary>
public class SaveContext
{
    public required string SessionId { get; set; }
    public required string UserId { get; set; }
    public Dictionary<string, object> ApplicationState { get; set; } = new();
    public DateTime SavedAt { get; set; }
    public SaveType Type { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// Types of save operations
/// </summary>
public enum SaveType
{
    Automatic,
    Manual,
    Emergency,
    Scheduled
}

/// <summary>
/// Save operation result
/// </summary>
public class SaveResult
{
    public bool Success { get; set; }
    public required string SavePath { get; set; }
    public long SizeBytes { get; set; }
    public TimeSpan TimeTaken { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime SavedAt { get; set; }
}

/// <summary>
/// Recovery operation result
/// </summary>
public class RecoveryResult
{
    public bool Success { get; set; }
    public Dictionary<string, object>? RecoveredState { get; set; }
    public DateTime RecoveredFrom { get; set; }
    public TimeSpan RecoveryTime { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string> RecoveredComponents { get; set; } = new();
}

/// <summary>
/// Cleanup operation result
/// </summary>
public class CleanupResult
{
    public bool Success { get; set; }
    public int FilesDeleted { get; set; }
    public long SpaceFreedBytes { get; set; }
    public TimeSpan TimeTaken { get; set; }
    public List<string> DeletedFiles { get; set; } = new();
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Recovery test result
/// </summary>
public class RecoveryTestResult
{
    public bool TestPassed { get; set; }
    public Dictionary<string, bool> ComponentTests { get; set; } = new();
    public TimeSpan RecoveryTime { get; set; }
    public List<string> FailedComponents { get; set; } = new();
    public DateTime TestedAt { get; set; }
}

#endregion

#region Audit Models

/// <summary>
/// Security audit event
/// </summary>
public class SecurityAuditEvent
{
    public required string EventId { get; set; }
    public SecurityEventType EventType { get; set; }
    public required string UserId { get; set; }
    public required string Description { get; set; }
    public DateTime OccurredAt { get; set; }
    public required string SourceModule { get; set; }
    public SeverityLevel Severity { get; set; }
    public Dictionary<string, object> EventData { get; set; } = new();
    public bool IsSuccessful { get; set; }
}

/// <summary>
/// Types of security events
/// </summary>
public enum SecurityEventType
{
    Login,
    Logout,
    AuthenticationFailure,
    AuthorizationFailure,
    DataAccess,
    DataModification,
    ConfigurationChange,
    SecurityPolicyChange,
    EncryptionOperation,
    KeyGeneration,
    SecurityViolation
}

/// <summary>
/// Operational audit event
/// </summary>
public class OperationalAuditEvent
{
    public required string EventId { get; set; }
    public OperationalEventType EventType { get; set; }
    public required string UserId { get; set; }
    public required string Description { get; set; }
    public DateTime OccurredAt { get; set; }
    public required string SourceModule { get; set; }
    public Dictionary<string, object> EventData { get; set; } = new();
    public bool IsSuccessful { get; set; }
}

/// <summary>
/// Types of operational events
/// </summary>
public enum OperationalEventType
{
    ApplicationStart,
    ApplicationShutdown,
    ModuleLoaded,
    ModuleUnloaded,
    TaskStarted,
    TaskCompleted,
    ErrorOccurred,
    PerformanceAlert,
    ConfigurationLoaded,
    UpdateCheck
}

/// <summary>
/// Audit report
/// </summary>
public class AuditReport
{
    public DateTime GeneratedAt { get; set; }
    public TimeSpan ReportPeriod { get; set; }
    public int TotalSecurityEvents { get; set; }
    public int TotalOperationalEvents { get; set; }
    public Dictionary<SecurityEventType, int> SecurityEventCounts { get; set; } = new();
    public Dictionary<OperationalEventType, int> OperationalEventCounts { get; set; } = new();
    public List<SecurityAuditEvent> CriticalEvents { get; set; } = new();
    public Dictionary<string, object> Summary { get; set; } = new();
}

/// <summary>
/// Audit search criteria
/// </summary>
public class AuditSearchCriteria
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<SecurityEventType> SecurityEventTypes { get; set; } = new();
    public List<OperationalEventType> OperationalEventTypes { get; set; } = new();
    public string? UserId { get; set; }
    public string? SourceModule { get; set; }
    public SeverityLevel? MinimumSeverity { get; set; }
    public string? SearchText { get; set; }
    public int MaxResults { get; set; } = 1000;
}

/// <summary>
/// Audit search results
/// </summary>
public class AuditSearchResult
{
    public List<SecurityAuditEvent> SecurityEvents { get; set; } = new();
    public List<OperationalAuditEvent> OperationalEvents { get; set; } = new();
    public int TotalResults { get; set; }
    public bool ResultsTruncated { get; set; }
    public DateTime SearchedAt { get; set; }
    public TimeSpan SearchTime { get; set; }
}

/// <summary>
/// Archive options for audit logs
/// </summary>
public class ArchiveOptions
{
    public TimeSpan RetentionPeriod { get; set; } = TimeSpan.FromDays(365);
    public required string ArchivePath { get; set; }
    public CompressionLevel Compression { get; set; } = CompressionLevel.Optimal;
    public bool EncryptArchive { get; set; } = true;
}

/// <summary>
/// Archive operation result
/// </summary>
public class ArchiveResult
{
    public bool Success { get; set; }
    public int ArchivedEvents { get; set; }
    public long ArchiveSizeBytes { get; set; }
    public required string ArchivePath { get; set; }
    public TimeSpan TimeTaken { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime ArchivedAt { get; set; }
}

#endregion

#region Final Status Models

/// <summary>
/// Production readiness assessment
/// </summary>
public class ProductionReadiness
{
    public bool IsReady { get; set; }
    public float ReadinessScore { get; set; } // 0-100
    public Dictionary<string, bool> ComponentReadiness { get; set; } = new();
    public List<string> BlockingIssues { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
    public SecurityAssessment Security { get; set; } = new();
    public QualityAssessment Quality { get; set; } = new();
    public PerformanceAssessment Performance { get; set; } = new();
    public DateTime AssessedAt { get; set; }
}

/// <summary>
/// Security assessment details
/// </summary>
public class SecurityAssessment
{
    public bool IsSecure { get; set; }
    public float SecurityScore { get; set; }
    public List<string> SecurityIssues { get; set; } = new();
    public bool EncryptionEnabled { get; set; }
    public bool AuthenticationEnabled { get; set; }
    public bool AuditingEnabled { get; set; }
}

/// <summary>
/// Quality assessment details
/// </summary>
public class QualityAssessment
{
    public bool MeetsQualityStandards { get; set; }
    public float QualityScore { get; set; }
    public float TestCoverage { get; set; }
    public int CodeIssues { get; set; }
    public List<string> QualityIssues { get; set; } = new();
}

/// <summary>
/// Performance assessment details
/// </summary>
public class PerformanceAssessment
{
    public bool MeetsPerformanceTargets { get; set; }
    public float PerformanceScore { get; set; }
    public Dictionary<string, float> Metrics { get; set; } = new();
    public List<string> PerformanceIssues { get; set; } = new();
}

/// <summary>
/// Final completion status
/// </summary>
public class CompletionStatus
{
    public bool IsComplete { get; set; }
    public float CompletionPercentage { get; set; }
    public Dictionary<string, bool> ComponentStatus { get; set; } = new();
    public List<string> CompletedTasks { get; set; } = new();
    public List<string> RemainingTasks { get; set; } = new();
    public DateTime CompletedAt { get; set; }
    public TimeSpan TotalDevelopmentTime { get; set; }
    public string? CelebrationMessage { get; set; }
}

#endregion

#region Witty Messages Collection

/// <summary>
/// Collection of witty messages for various scenarios
/// </summary>
public static class WittyMessages
{
    public static readonly Dictionary<RecoveryActionType, List<string>> RecoveryMessages = new()
    {
        [RecoveryActionType.Retry] = new List<string>
        {
            "Retry initiated - persistence is the programmer's virtue!",
            "Second attempt commencing - because first tries are overrated!",
            "Retrying with enhanced determination - failure is not an option!",
            "Round two: Fight! Deploying retry sequence with extra caffeine!",
            "Attempt #2: Now with 100% more hope and 50% less despair!"
        },
        [RecoveryActionType.Fallback] = new List<string>
        {
            "Fallback activated - like a safety net for digital acrobats!",
            "Plan B engaged - because even code needs backup plans!",
            "Graceful fallback initiated - stumbling elegantly toward success!",
            "Secondary systems online - the show must go on!",
            "Fallback protocol activated - retreat to fight another day!"
        },
        [RecoveryActionType.Graceful] = new List<string>
        {
            "Graceful recovery in progress - like a ballet dancer after a stumble!",
            "Elegant error handling activated - making mistakes look intentional!",
            "Smooth recovery sequence - because style matters, even in failure!",
            "Graceful degradation engaged - failing upward with panache!",
            "Sophisticated error recovery - turning bugs into features!"
        },
        [RecoveryActionType.Emergency] = new List<string>
        {
            "Emergency protocol activated - calling all digital first responders!",
            "Red alert! All hands on deck for emergency recovery procedures!",
            "Crisis mode engaged - deploying emergency countermeasures!",
            "Emergency recovery initiated - this is not a drill!",
            "Panic button pressed - but in a very calm and professional way!"
        }
    };

    public static readonly List<string> SecurityMessages = new()
    {
        "Security fortress erected - Fort Knox has nothing on us!",
        "Encryption shields activated - data now wrapped in digital armor!",
        "Authentication protocols engaged - no entry without proper credentials!",
        "Security perimeter established - unauthorized access will be met with witty error messages!",
        "Digital vault secured - your secrets are safe with our virtual bouncer!"
    };

    public static readonly List<string> TestingMessages = new()
    {
        "Test suite deployed - bugs beware, the hunters are here!",
        "Quality assurance activated - perfectionism mode: ON!",
        "Testing protocols initiated - time to separate the wheat from the chaff!",
        "Bug hunt commenced - armed with assertions and caffeinated determination!",
        "Test execution in progress - may the coverage be with you!"
    };

    public static readonly List<string> DeploymentMessages = new()
    {
        "Deployment sequence initiated - preparing for digital launch!",
        "Package assembly commenced - wrapping your code with a bow!",
        "Installer generation activated - creating the key to your digital kingdom!",
        "Deployment preparation complete - ready for prime time!",
        "Launch preparations finalized - T-minus zero to awesome!"
    };

    public static readonly List<string> CompletionMessages = new()
    {
        "Mission accomplished - NetToolkit stands complete and magnificent!",
        "Final seal applied - the masterpiece is ready for the world!",
        "Development journey concluded - from concept to reality!",
        "The opus is complete - behold the culmination of digital artistry!",
        "Project finalized - time to pop the virtual champagne! 🥂"
    };

    public static string GetRandomMessage(List<string> messages)
    {
        var random = new Random();
        return messages[random.Next(messages.Count)];
    }

    public static string GetRecoveryMessage(RecoveryActionType actionType)
    {
        if (RecoveryMessages.TryGetValue(actionType, out var messages))
        {
            return GetRandomMessage(messages);
        }
        return "Recovery action initiated - digital resilience at its finest!";
    }
}