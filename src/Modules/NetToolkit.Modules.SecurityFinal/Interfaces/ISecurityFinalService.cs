using NetToolkit.Modules.SecurityFinal.Models;

namespace NetToolkit.Modules.SecurityFinal.Interfaces;

/// <summary>
/// Ultimate security and finalization service - the capstone guardian of NetToolkit
/// Orchestrates comprehensive security, testing, and deployment readiness
/// </summary>
public interface ISecurityFinalService
{
    /// <summary>
    /// Apply comprehensive encryption to specified data types
    /// </summary>
    /// <param name="dataType">Type of data to encrypt</param>
    /// <returns>Async operation completing when encryption is applied</returns>
    Task ApplyEncryptionAsync(DataType dataType);
    
    /// <summary>
    /// Enable authentication with specified security level
    /// </summary>
    /// <param name="authLevel">Level of authentication to enforce</param>
    /// <returns>Async operation completing when authentication is enabled</returns>
    Task EnableAuthAsync(AuthLevel authLevel);
    
    /// <summary>
    /// Execute comprehensive test suite across entire NetToolkit
    /// </summary>
    /// <returns>Comprehensive test results</returns>
    Task<TestResult> RunGlobalTestsAsync();
    
    /// <summary>
    /// Prepare deployment package with specified options
    /// </summary>
    /// <param name="options">Deployment configuration options</param>
    /// <returns>Generated installer package information</returns>
    Task<InstallerPackage> PrepareDeploymentAsync(DeploymentOptions options);
    
    /// <summary>
    /// Get current security status across all modules
    /// </summary>
    /// <returns>Comprehensive security status report</returns>
    Task<SecurityStatus> GetSecurityStatusAsync();
    
    /// <summary>
    /// Apply final touches and optimizations to the entire system
    /// </summary>
    /// <returns>Async operation completing when final touches are applied</returns>
    Task ApplyFinalTouchesAsync();
    
    /// <summary>
    /// Validate system readiness for production deployment
    /// </summary>
    /// <returns>Production readiness assessment</returns>
    Task<ProductionReadiness> ValidateProductionReadinessAsync();
}

/// <summary>
/// Global error handling service with witty recovery mechanisms
/// </summary>
public interface IErrorHandler
{
    /// <summary>
    /// Handle exception with intelligent recovery and witty logging
    /// </summary>
    /// <param name="exception">Exception to handle</param>
    /// <param name="context">Context information for the error</param>
    /// <returns>Recovery action taken</returns>
    Task<RecoveryAction> HandleExceptionAsync(Exception exception, ErrorContext context);
    
    /// <summary>
    /// Register global exception handlers for the application
    /// </summary>
    /// <returns>Async operation completing when handlers are registered</returns>
    Task RegisterGlobalHandlersAsync();
    
    /// <summary>
    /// Get error statistics and recovery metrics
    /// </summary>
    /// <returns>Error handling statistics</returns>
    Task<ErrorStatistics> GetErrorStatisticsAsync();
    
    /// <summary>
    /// Test error handling with simulated exceptions
    /// </summary>
    /// <returns>Error handling test results</returns>
    Task<ErrorHandlingTestResult> TestErrorHandlingAsync();
}

/// <summary>
/// Encryption and data protection service
/// </summary>
public interface IEncryptionService
{
    /// <summary>
    /// Encrypt data using AES with DPAPI key protection
    /// </summary>
    /// <param name="data">Data to encrypt</param>
    /// <param name="context">Encryption context and metadata</param>
    /// <returns>Encrypted data package</returns>
    Task<EncryptedData> EncryptAsync(byte[] data, EncryptionContext context);
    
    /// <summary>
    /// Decrypt previously encrypted data
    /// </summary>
    /// <param name="encryptedData">Encrypted data package</param>
    /// <param name="context">Decryption context</param>
    /// <returns>Decrypted data</returns>
    Task<byte[]> DecryptAsync(EncryptedData encryptedData, EncryptionContext context);
    
    /// <summary>
    /// Encrypt configuration files and sensitive data
    /// </summary>
    /// <param name="configPath">Path to configuration file</param>
    /// <returns>Async operation completing when configuration is encrypted</returns>
    Task EncryptConfigurationAsync(string configPath);
    
    /// <summary>
    /// Generate and manage encryption keys
    /// </summary>
    /// <param name="keyType">Type of key to generate</param>
    /// <returns>Generated key information</returns>
    Task<EncryptionKey> GenerateKeyAsync(KeyType keyType);
    
    /// <summary>
    /// Validate encryption integrity across all encrypted data
    /// </summary>
    /// <returns>Encryption validation results</returns>
    Task<EncryptionValidation> ValidateEncryptionIntegrityAsync();
}

/// <summary>
/// Windows authentication and authorization service
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Authenticate current Windows user
    /// </summary>
    /// <returns>Authentication result</returns>
    Task<AuthenticationResult> AuthenticateAsync();
    
    /// <summary>
    /// Check if user has required permissions for operation
    /// </summary>
    /// <param name="operation">Operation requiring authorization</param>
    /// <returns>Authorization result</returns>
    Task<AuthorizationResult> AuthorizeAsync(SecureOperation operation);
    
    /// <summary>
    /// Get current user's role and permissions
    /// </summary>
    /// <returns>User role information</returns>
    Task<UserRole> GetUserRoleAsync();
    
    /// <summary>
    /// Enable PIN-based secondary authentication
    /// </summary>
    /// <param name="pin">PIN for secondary authentication</param>
    /// <returns>Async operation completing when PIN is enabled</returns>
    Task EnablePinAuthenticationAsync(string pin);
    
    /// <summary>
    /// Audit authentication and authorization events
    /// </summary>
    /// <param name="auditEvent">Event to audit</param>
    /// <returns>Async operation completing when event is audited</returns>
    Task AuditSecurityEventAsync(SecurityAuditEvent auditEvent);
}

/// <summary>
/// Comprehensive testing service for NetToolkit
/// </summary>
public interface ITestingService
{
    /// <summary>
    /// Discover all test assemblies and methods
    /// </summary>
    /// <returns>Test discovery results</returns>
    Task<TestDiscovery> DiscoverTestsAsync();
    
    /// <summary>
    /// Execute all discovered tests
    /// </summary>
    /// <param name="testFilter">Optional filter for specific tests</param>
    /// <returns>Test execution results</returns>
    Task<TestExecution> ExecuteTestsAsync(TestFilter? testFilter = null);
    
    /// <summary>
    /// Generate code coverage reports
    /// </summary>
    /// <returns>Coverage analysis results</returns>
    Task<CoverageReport> GenerateCoverageReportAsync();
    
    /// <summary>
    /// Run integration tests across all modules
    /// </summary>
    /// <returns>Integration test results</returns>
    Task<IntegrationTestResult> RunIntegrationTestsAsync();
    
    /// <summary>
    /// Validate test quality and coverage metrics
    /// </summary>
    /// <returns>Test quality assessment</returns>
    Task<TestQualityAssessment> AssessTestQualityAsync();
}

/// <summary>
/// Deployment and installer generation service
/// </summary>
public interface IDeploymentService
{
    /// <summary>
    /// Generate MSI installer package
    /// </summary>
    /// <param name="options">Installer generation options</param>
    /// <returns>Generated installer information</returns>
    Task<InstallerPackage> GenerateMsiInstallerAsync(MsiOptions options);
    
    /// <summary>
    /// Create ClickOnce deployment package
    /// </summary>
    /// <param name="options">ClickOnce deployment options</param>
    /// <returns>ClickOnce deployment information</returns>
    Task<ClickOnceDeployment> CreateClickOnceDeploymentAsync(ClickOnceOptions options);
    
    /// <summary>
    /// Package application for distribution
    /// </summary>
    /// <param name="packageOptions">Packaging options</param>
    /// <returns>Application package information</returns>
    Task<ApplicationPackage> PackageApplicationAsync(PackageOptions packageOptions);
    
    /// <summary>
    /// Validate deployment package integrity
    /// </summary>
    /// <param name="packagePath">Path to deployment package</param>
    /// <returns>Package validation results</returns>
    Task<PackageValidation> ValidatePackageAsync(string packagePath);
    
    /// <summary>
    /// Create automatic update mechanism
    /// </summary>
    /// <param name="updateOptions">Update configuration options</param>
    /// <returns>Update mechanism information</returns>
    Task<UpdateMechanism> CreateUpdateMechanismAsync(UpdateOptions updateOptions);
}

/// <summary>
/// Auto-save and crash recovery service
/// </summary>
public interface IRecoveryService
{
    /// <summary>
    /// Enable automatic state saving
    /// </summary>
    /// <param name="saveInterval">Interval between automatic saves</param>
    /// <returns>Async operation completing when auto-save is enabled</returns>
    Task EnableAutoSaveAsync(TimeSpan saveInterval);
    
    /// <summary>
    /// Save current application state
    /// </summary>
    /// <param name="context">Context for the save operation</param>
    /// <returns>Save operation result</returns>
    Task<SaveResult> SaveStateAsync(SaveContext context);
    
    /// <summary>
    /// Recover application state from previous session
    /// </summary>
    /// <returns>Recovery operation result</returns>
    Task<RecoveryResult> RecoverStateAsync();
    
    /// <summary>
    /// Clean up old recovery files
    /// </summary>
    /// <param name="retentionPeriod">How long to retain recovery files</param>
    /// <returns>Cleanup operation result</returns>
    Task<CleanupResult> CleanupRecoveryFilesAsync(TimeSpan retentionPeriod);
    
    /// <summary>
    /// Test crash recovery mechanism
    /// </summary>
    /// <returns>Recovery test results</returns>
    Task<RecoveryTestResult> TestCrashRecoveryAsync();
}

/// <summary>
/// Audit logging service for security and operational events
/// </summary>
public interface IAuditService
{
    /// <summary>
    /// Log security-related event
    /// </summary>
    /// <param name="securityEvent">Security event to log</param>
    /// <returns>Async operation completing when event is logged</returns>
    Task LogSecurityEventAsync(SecurityAuditEvent securityEvent);
    
    /// <summary>
    /// Log operational event
    /// </summary>
    /// <param name="operationalEvent">Operational event to log</param>
    /// <returns>Async operation completing when event is logged</returns>
    Task LogOperationalEventAsync(OperationalAuditEvent operationalEvent);
    
    /// <summary>
    /// Generate audit reports
    /// </summary>
    /// <param name="reportPeriod">Time period for the report</param>
    /// <returns>Generated audit report</returns>
    Task<AuditReport> GenerateAuditReportAsync(TimeSpan reportPeriod);
    
    /// <summary>
    /// Search audit logs
    /// </summary>
    /// <param name="searchCriteria">Criteria for log search</param>
    /// <returns>Search results</returns>
    Task<AuditSearchResult> SearchAuditLogsAsync(AuditSearchCriteria searchCriteria);
    
    /// <summary>
    /// Archive old audit logs
    /// </summary>
    /// <param name="archiveOptions">Archive configuration options</param>
    /// <returns>Archive operation result</returns>
    Task<ArchiveResult> ArchiveAuditLogsAsync(ArchiveOptions archiveOptions);
}

/// <summary>
/// Final event publisher for security and deployment events
/// </summary>
public interface IFinalEventPublisher
{
    /// <summary>
    /// Publish security applied event
    /// </summary>
    /// <param name="securityLevel">Applied security level</param>
    /// <param name="appliedAt">When security was applied</param>
    /// <returns>Async operation completing when event is published</returns>
    Task PublishSecurityAppliedAsync(SecurityLevel securityLevel, DateTime appliedAt);
    
    /// <summary>
    /// Publish test completion event
    /// </summary>
    /// <param name="testResult">Test execution results</param>
    /// <returns>Async operation completing when event is published</returns>
    Task PublishTestCompletedAsync(TestResult testResult);
    
    /// <summary>
    /// Publish deployment readiness event
    /// </summary>
    /// <param name="readinessStatus">Deployment readiness status</param>
    /// <returns>Async operation completing when event is published</returns>
    Task PublishDeploymentReadyAsync(ProductionReadiness readinessStatus);
    
    /// <summary>
    /// Publish final touches completion event
    /// </summary>
    /// <param name="completionStatus">Completion status details</param>
    /// <returns>Async operation completing when event is published</returns>
    Task PublishFinalTouchesCompletedAsync(CompletionStatus completionStatus);
    
    /// <summary>
    /// Publish security audit event
    /// </summary>
    /// <param name="auditEvent">Security audit event details</param>
    /// <returns>Async operation completing when event is published</returns>
    Task PublishSecurityAuditAsync(SecurityAuditEvent auditEvent);
}