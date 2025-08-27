using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using NetToolkit.Core.Interfaces;
using NetToolkit.Modules.MicrosoftAdmin.Models;
using NetToolkit.Modules.MicrosoftAdmin.Events;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text;

namespace NetToolkit.Modules.MicrosoftAdmin.Services;

/// <summary>
/// Admin audit service - the vigilant chronicler of administrative deeds
/// Records, monitors, and protects the integrity of all Microsoft admin operations
/// </summary>
public class AdminAuditService : IDisposable
{
    private readonly ILogger<AdminAuditService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IEventBus _eventBus;
    private readonly string _auditLogPath;
    private readonly SemaphoreSlim _writeLock;
    private readonly System.Threading.Timer _cleanupTimer;
    private readonly Dictionary<string, AdminAuditLog> _auditCache;
    private readonly RateLimiter _rateLimiter;

    public AdminAuditService(
        ILogger<AdminAuditService> logger,
        IConfiguration configuration,
        IEventBus eventBus)
    {
        _logger = logger;
        _configuration = configuration;
        _eventBus = eventBus;
        
        _auditLogPath = _configuration["MicrosoftAdmin:AuditLogPath"] ?? 
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "Admin");
        
        _writeLock = new SemaphoreSlim(1, 1);
        _auditCache = new Dictionary<string, AdminAuditLog>();
        _rateLimiter = new RateLimiter();
        
        // Ensure audit directory exists
        if (!Directory.Exists(_auditLogPath))
        {
            Directory.CreateDirectory(_auditLogPath);
            _logger.LogInformation("Admin audit directory created at {AuditPath}", _auditLogPath);
        }
        
        // Set up periodic cleanup timer (runs daily)
        _cleanupTimer = new System.Threading.Timer(PerformCleanup, null, TimeSpan.FromHours(24), TimeSpan.FromHours(24));
        
        _logger.LogInformation("Admin audit service initialized - vigilant chronicler ready to record administrative deeds");
    }

    /// <summary>
    /// Record an administrative operation in the audit log
    /// </summary>
    public async Task<string> RecordAdminOperationAsync(
        string userId,
        string userName,
        string operation,
        string templateId,
        string resource,
        bool success,
        string result,
        Dictionary<string, object> parameters,
        TimeSpan duration,
        string? clientIp = null,
        string? userAgent = null)
    {
        try
        {
            // Check rate limiting
            if (!_rateLimiter.IsAllowed(userId))
            {
                _logger.LogWarning("Rate limit exceeded for user {UserId} - audit recording limited", userId);
                return string.Empty;
            }

            var auditEntry = new AdminAuditLog
            {
                Id = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow,
                UserId = userId,
                UserName = userName,
                Operation = operation,
                TemplateId = templateId,
                Resource = resource,
                Success = success,
                Result = result,
                Parameters = parameters,
                ClientIp = clientIp ?? "Unknown",
                UserAgent = userAgent ?? "Unknown",
                Duration = duration
            };

            await RecordAuditEntryAsync(auditEntry);
            
            // Perform security analysis
            await AnalyzeSecurityImplicationsAsync(auditEntry);
            
            // Publish audit event
            await _eventBus.PublishAsync(new AuditLogGeneratedEvent
            {
                AuditEntry = auditEntry,
                LoggedAt = DateTime.UtcNow,
                RequiresEscalation = DetermineEscalationRequired(auditEntry),
                ComplianceFlags = GetComplianceFlags(auditEntry)
            });

            _logger.LogDebug("Admin operation audited: {Operation} by {UserName} - {Status}", 
                operation, userName, success ? "Success" : "Failure");
                
            return auditEntry.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to record admin operation audit for {Operation}", operation);
            return string.Empty;
        }
    }

    /// <summary>
    /// Retrieve audit logs with filtering and pagination
    /// </summary>
    public async Task<(List<AdminAuditLog> Logs, int TotalCount)> GetAuditLogsAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? userId = null,
        string? operation = null,
        bool? success = null,
        int page = 1,
        int pageSize = 100)
    {
        try
        {
            _logger.LogDebug("Retrieving audit logs - Date range: {StartDate} to {EndDate}", 
                startDate?.ToString("yyyy-MM-dd") ?? "None", 
                endDate?.ToString("yyyy-MM-dd") ?? "None");

            var logs = await LoadAuditLogsFromStorageAsync(startDate, endDate);
            
            // Apply filters
            var filteredLogs = logs.AsQueryable();
            
            if (!string.IsNullOrEmpty(userId))
                filteredLogs = filteredLogs.Where(l => l.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase));
                
            if (!string.IsNullOrEmpty(operation))
                filteredLogs = filteredLogs.Where(l => l.Operation.Contains(operation, StringComparison.OrdinalIgnoreCase));
                
            if (success.HasValue)
                filteredLogs = filteredLogs.Where(l => l.Success == success.Value);
            
            var totalCount = filteredLogs.Count();
            
            // Apply pagination
            var pagedLogs = filteredLogs
                .OrderByDescending(l => l.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            _logger.LogDebug("Retrieved {Count} audit logs out of {Total} total entries", pagedLogs.Count, totalCount);
            return (pagedLogs, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve audit logs");
            return (new List<AdminAuditLog>(), 0);
        }
    }

    /// <summary>
    /// Generate comprehensive audit report
    /// </summary>
    public async Task<AdminAuditReport> GenerateAuditReportAsync(
        DateTime startDate,
        DateTime endDate,
        string? userId = null)
    {
        try
        {
            _logger.LogInformation("Generating audit report for period {StartDate} to {EndDate}", 
                startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));

            var (logs, totalCount) = await GetAuditLogsAsync(startDate, endDate, userId);
            
            var report = new AdminAuditReport
            {
                ReportId = Guid.NewGuid().ToString(),
                GeneratedAt = DateTime.UtcNow,
                PeriodStart = startDate,
                PeriodEnd = endDate,
                GeneratedBy = "AdminAuditService",
                TotalOperations = totalCount,
                SuccessfulOperations = logs.Count(l => l.Success),
                FailedOperations = logs.Count(l => !l.Success),
                UniqueUsers = logs.Select(l => l.UserId).Distinct().Count(),
                MostActiveUser = GetMostActiveUser(logs),
                TopOperations = GetTopOperations(logs),
                SecurityAlerts = GetSecurityAlerts(logs),
                ComplianceViolations = GetComplianceViolations(logs),
                PerformanceMetrics = CalculatePerformanceMetrics(logs),
                Recommendations = GenerateRecommendations(logs)
            };

            // Store report
            await StoreAuditReportAsync(report);
            
            _logger.LogInformation("Audit report generated successfully - {TotalOps} operations analyzed", 
                report.TotalOperations);
                
            return report;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate audit report");
            return CreateEmptyReport(startDate, endDate);
        }
    }

    /// <summary>
    /// Detect suspicious administrative activities
    /// </summary>
    public async Task<List<SecurityAlert>> DetectSuspiciousActivityAsync(TimeSpan lookbackPeriod)
    {
        try
        {
            var startDate = DateTime.UtcNow.Subtract(lookbackPeriod);
            var (logs, _) = await GetAuditLogsAsync(startDate);
            
            var alerts = new List<SecurityAlert>();
            
            // Detect unusual patterns
            alerts.AddRange(DetectUnusualLoginPatterns(logs));
            alerts.AddRange(DetectMassOperations(logs));
            alerts.AddRange(DetectOffHoursActivity(logs));
            alerts.AddRange(DetectFailedOperationSpikes(logs));
            alerts.AddRange(DetectPrivilegeEscalation(logs));
            alerts.AddRange(DetectSuspiciousIPActivity(logs));
            
            // Publish security alerts
            foreach (var alert in alerts.Where(a => a.Severity == "High" || a.Severity == "Critical"))
            {
                await _eventBus.PublishAsync(new SecurityAlertEvent
                {
                    AlertType = alert.AlertType,
                    Description = alert.Description,
                    UserId = alert.UserId,
                    AlertedAt = DateTime.UtcNow,
                    Severity = alert.Severity,
                    AlertData = alert.Evidence,
                    RequiresImmedateAction = alert.Severity == "Critical"
                });
            }
            
            _logger.LogInformation("Security analysis completed - {AlertCount} potential threats detected", alerts.Count);
            return alerts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to detect suspicious activity");
            return new List<SecurityAlert>();
        }
    }

    /// <summary>
    /// Verify audit log integrity using cryptographic hashing
    /// </summary>
    public async Task<bool> VerifyAuditIntegrityAsync(string auditLogId)
    {
        try
        {
            var auditEntry = await GetAuditLogByIdAsync(auditLogId);
            if (auditEntry == null)
            {
                _logger.LogWarning("Audit log {LogId} not found for integrity verification", auditLogId);
                return false;
            }
            
            var expectedHash = ComputeAuditHash(auditEntry);
            var storedHash = await GetStoredHashAsync(auditLogId);
            
            var isValid = expectedHash.Equals(storedHash, StringComparison.OrdinalIgnoreCase);
            
            if (!isValid)
            {
                _logger.LogError("Audit log integrity violation detected for {LogId}", auditLogId);
                await PublishIntegrityViolationAsync(auditLogId, expectedHash, storedHash);
            }
            
            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to verify audit integrity for {LogId}", auditLogId);
            return false;
        }
    }

    private async Task RecordAuditEntryAsync(AdminAuditLog auditEntry)
    {
        await _writeLock.WaitAsync();
        try
        {
            // Store in cache for quick access
            _auditCache[auditEntry.Id] = auditEntry;
            
            // Compute and store integrity hash
            var hash = ComputeAuditHash(auditEntry);
            await StoreHashAsync(auditEntry.Id, hash);
            
            // Write to persistent storage
            await WriteAuditLogToDisk(auditEntry);
            
            // Cleanup cache if too large
            if (_auditCache.Count > 10000)
            {
                var oldestEntries = _auditCache.OrderBy(kvp => kvp.Value.Timestamp).Take(1000);
                foreach (var entry in oldestEntries.ToList())
                {
                    _auditCache.Remove(entry.Key);
                }
            }
        }
        finally
        {
            _writeLock.Release();
        }
    }

    private async Task WriteAuditLogToDisk(AdminAuditLog auditEntry)
    {
        var logDate = auditEntry.Timestamp.ToString("yyyy-MM-dd");
        var logFileName = $"admin-audit-{logDate}.jsonl";
        var logFilePath = Path.Combine(_auditLogPath, logFileName);
        
        var logJson = JsonSerializer.Serialize(auditEntry, new JsonSerializerOptions
        {
            WriteIndented = false
        });
        
        await File.AppendAllTextAsync(logFilePath, logJson + Environment.NewLine);
    }

    private async Task<List<AdminAuditLog>> LoadAuditLogsFromStorageAsync(DateTime? startDate, DateTime? endDate)
    {
        var logs = new List<AdminAuditLog>();
        
        var start = startDate ?? DateTime.UtcNow.AddDays(-30);
        var end = endDate ?? DateTime.UtcNow;
        
        for (var date = start.Date; date <= end.Date; date = date.AddDays(1))
        {
            var logFileName = $"admin-audit-{date:yyyy-MM-dd}.jsonl";
            var logFilePath = Path.Combine(_auditLogPath, logFileName);
            
            if (File.Exists(logFilePath))
            {
                var lines = await File.ReadAllLinesAsync(logFilePath);
                foreach (var line in lines)
                {
                    try
                    {
                        var logEntry = JsonSerializer.Deserialize<AdminAuditLog>(line);
                        if (logEntry != null && 
                            logEntry.Timestamp >= start && 
                            logEntry.Timestamp <= end)
                        {
                            logs.Add(logEntry);
                        }
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogWarning(ex, "Failed to parse audit log entry: {Line}", line);
                    }
                }
            }
        }
        
        return logs;
    }

    private async Task AnalyzeSecurityImplicationsAsync(AdminAuditLog auditEntry)
    {
        // Check for high-risk operations
        var highRiskOperations = new[] { "reset-user-password", "assign-license", "create-security-group" };
        if (highRiskOperations.Contains(auditEntry.TemplateId))
        {
            await PublishSecurityAnalysisAsync(auditEntry, "HighRiskOperation");
        }
        
        // Check for bulk operations
        if (auditEntry.Operation.Contains("bulk", StringComparison.OrdinalIgnoreCase))
        {
            await PublishSecurityAnalysisAsync(auditEntry, "BulkOperation");
        }
        
        // Check for failed operations
        if (!auditEntry.Success && auditEntry.Operation.Contains("security", StringComparison.OrdinalIgnoreCase))
        {
            await PublishSecurityAnalysisAsync(auditEntry, "SecurityOperationFailure");
        }
    }

    private async Task PublishSecurityAnalysisAsync(AdminAuditLog auditEntry, string analysisType)
    {
        await _eventBus.PublishAsync(new SecurityAlertEvent
        {
            AlertType = analysisType,
            Description = $"Security analysis triggered for operation {auditEntry.Operation}",
            UserId = auditEntry.UserId,
            UserName = auditEntry.UserName,
            AlertedAt = DateTime.UtcNow,
            Severity = GetAnalysisSeverity(analysisType),
            AlertData = new Dictionary<string, object>
            {
                ["auditLogId"] = auditEntry.Id,
                ["operation"] = auditEntry.Operation,
                ["templateId"] = auditEntry.TemplateId,
                ["resource"] = auditEntry.Resource,
                ["clientIp"] = auditEntry.ClientIp
            }
        });
    }

    private string GetAnalysisSeverity(string analysisType)
    {
        return analysisType switch
        {
            "HighRiskOperation" => "High",
            "BulkOperation" => "Medium",
            "SecurityOperationFailure" => "High",
            _ => "Low"
        };
    }

    private bool DetermineEscalationRequired(AdminAuditLog auditEntry)
    {
        // Escalation criteria
        return !auditEntry.Success && 
               (auditEntry.Operation.Contains("security", StringComparison.OrdinalIgnoreCase) ||
                auditEntry.Operation.Contains("admin", StringComparison.OrdinalIgnoreCase) ||
                auditEntry.Duration > TimeSpan.FromMinutes(5));
    }

    private List<string> GetComplianceFlags(AdminAuditLog auditEntry)
    {
        var flags = new List<string>();
        
        // GDPR compliance flags
        if (auditEntry.Operation.Contains("user", StringComparison.OrdinalIgnoreCase))
        {
            flags.Add("GDPR-Personal-Data");
        }
        
        // SOX compliance flags
        if (auditEntry.Operation.Contains("admin", StringComparison.OrdinalIgnoreCase))
        {
            flags.Add("SOX-Administrative-Access");
        }
        
        // HIPAA compliance flags (if healthcare context)
        if (auditEntry.Parameters.ContainsKey("healthcare") || 
            auditEntry.Resource.Contains("health", StringComparison.OrdinalIgnoreCase))
        {
            flags.Add("HIPAA-Health-Data");
        }
        
        return flags;
    }

    private string ComputeAuditHash(AdminAuditLog auditEntry)
    {
        var hashInput = $"{auditEntry.Id}|{auditEntry.Timestamp:yyyy-MM-ddTHH:mm:ss.fffZ}|" +
                       $"{auditEntry.UserId}|{auditEntry.Operation}|{auditEntry.Success}|" +
                       $"{JsonSerializer.Serialize(auditEntry.Parameters)}";
        
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(hashInput));
        return Convert.ToBase64String(hashBytes);
    }

    private async Task StoreHashAsync(string auditLogId, string hash)
    {
        var hashFilePath = Path.Combine(_auditLogPath, "hashes", $"{auditLogId}.hash");
        var hashDirectory = Path.GetDirectoryName(hashFilePath)!;
        
        if (!Directory.Exists(hashDirectory))
        {
            Directory.CreateDirectory(hashDirectory);
        }
        
        await File.WriteAllTextAsync(hashFilePath, hash);
    }

    private async Task<string?> GetStoredHashAsync(string auditLogId)
    {
        var hashFilePath = Path.Combine(_auditLogPath, "hashes", $"{auditLogId}.hash");
        
        if (File.Exists(hashFilePath))
        {
            return await File.ReadAllTextAsync(hashFilePath);
        }
        
        return null;
    }

    private async Task<AdminAuditLog?> GetAuditLogByIdAsync(string auditLogId)
    {
        // Check cache first
        if (_auditCache.TryGetValue(auditLogId, out var cachedEntry))
        {
            return cachedEntry;
        }
        
        // Search in storage (this is a simplified implementation)
        var logs = await LoadAuditLogsFromStorageAsync(DateTime.UtcNow.AddDays(-90), DateTime.UtcNow);
        return logs.FirstOrDefault(l => l.Id == auditLogId);
    }

    private async Task PublishIntegrityViolationAsync(string auditLogId, string expectedHash, string? storedHash)
    {
        await _eventBus.PublishAsync(new SecurityAlertEvent
        {
            AlertType = "AuditIntegrityViolation",
            Description = $"Audit log integrity violation detected for entry {auditLogId}",
            AlertedAt = DateTime.UtcNow,
            Severity = "Critical",
            AlertData = new Dictionary<string, object>
            {
                ["auditLogId"] = auditLogId,
                ["expectedHash"] = expectedHash,
                ["storedHash"] = storedHash ?? "Missing",
                ["detectedAt"] = DateTime.UtcNow
            },
            RequiresImmedateAction = true
        });
    }

    // Security pattern detection methods
    private List<SecurityAlert> DetectUnusualLoginPatterns(List<AdminAuditLog> logs)
    {
        var alerts = new List<SecurityAlert>();
        
        // Group by user and check for unusual login times/locations
        var userGroups = logs.GroupBy(l => l.UserId);
        
        foreach (var userGroup in userGroups)
        {
            var userLogs = userGroup.OrderBy(l => l.Timestamp).ToList();
            
            // Check for rapid successive logins from different IPs
            for (int i = 1; i < userLogs.Count; i++)
            {
                var timeDiff = userLogs[i].Timestamp - userLogs[i-1].Timestamp;
                var ipDiff = userLogs[i].ClientIp != userLogs[i-1].ClientIp;
                
                if (timeDiff < TimeSpan.FromMinutes(5) && ipDiff)
                {
                    alerts.Add(new SecurityAlert
                    {
                        AlertType = "RapidLocationChange",
                        UserId = userGroup.Key,
                        Description = "User logged in from different IP addresses within 5 minutes",
                        Severity = "High",
                        DetectedAt = DateTime.UtcNow,
                        Evidence = new Dictionary<string, object>
                        {
                            ["firstIp"] = userLogs[i-1].ClientIp,
                            ["secondIp"] = userLogs[i].ClientIp,
                            ["timeDifference"] = timeDiff.TotalMinutes
                        }
                    });
                }
            }
        }
        
        return alerts;
    }

    private List<SecurityAlert> DetectMassOperations(List<AdminAuditLog> logs)
    {
        var alerts = new List<SecurityAlert>();
        
        // Detect users performing unusually high number of operations
        var userOperationCounts = logs.GroupBy(l => l.UserId)
            .Select(g => new { UserId = g.Key, Count = g.Count() })
            .Where(x => x.Count > 100) // Threshold for suspicious activity
            .ToList();
        
        foreach (var userOps in userOperationCounts)
        {
            alerts.Add(new SecurityAlert
            {
                AlertType = "MassOperations",
                UserId = userOps.UserId,
                Description = $"User performed {userOps.Count} operations in the analyzed period",
                Severity = userOps.Count > 500 ? "Critical" : "High",
                DetectedAt = DateTime.UtcNow,
                Evidence = new Dictionary<string, object>
                {
                    ["operationCount"] = userOps.Count,
                    ["threshold"] = 100
                }
            });
        }
        
        return alerts;
    }

    private List<SecurityAlert> DetectOffHoursActivity(List<AdminAuditLog> logs)
    {
        var alerts = new List<SecurityAlert>();
        
        // Define business hours (9 AM to 6 PM)
        var businessStart = new TimeSpan(9, 0, 0);
        var businessEnd = new TimeSpan(18, 0, 0);
        
        var offHoursLogs = logs.Where(l => 
            l.Timestamp.TimeOfDay < businessStart || 
            l.Timestamp.TimeOfDay > businessEnd ||
            l.Timestamp.DayOfWeek == DayOfWeek.Saturday ||
            l.Timestamp.DayOfWeek == DayOfWeek.Sunday)
            .GroupBy(l => l.UserId)
            .Where(g => g.Count() > 10) // More than 10 off-hours operations
            .ToList();
        
        foreach (var userGroup in offHoursLogs)
        {
            alerts.Add(new SecurityAlert
            {
                AlertType = "OffHoursActivity",
                UserId = userGroup.Key,
                Description = $"User performed {userGroup.Count()} operations outside business hours",
                Severity = "Medium",
                DetectedAt = DateTime.UtcNow,
                Evidence = new Dictionary<string, object>
                {
                    ["offHoursCount"] = userGroup.Count(),
                    ["businessHours"] = $"{businessStart:hh\\:mm} - {businessEnd:hh\\:mm}"
                }
            });
        }
        
        return alerts;
    }

    private List<SecurityAlert> DetectFailedOperationSpikes(List<AdminAuditLog> logs)
    {
        var alerts = new List<SecurityAlert>();
        
        var failedOps = logs.Where(l => !l.Success)
            .GroupBy(l => l.UserId)
            .Where(g => g.Count() > 5) // More than 5 failed operations
            .ToList();
        
        foreach (var userGroup in failedOps)
        {
            alerts.Add(new SecurityAlert
            {
                AlertType = "FailedOperationSpike",
                UserId = userGroup.Key,
                Description = $"User had {userGroup.Count()} failed operations",
                Severity = userGroup.Count() > 20 ? "High" : "Medium",
                DetectedAt = DateTime.UtcNow,
                Evidence = new Dictionary<string, object>
                {
                    ["failedCount"] = userGroup.Count(),
                    ["failureRate"] = (double)userGroup.Count() / logs.Count(l => l.UserId == userGroup.Key)
                }
            });
        }
        
        return alerts;
    }

    private List<SecurityAlert> DetectPrivilegeEscalation(List<AdminAuditLog> logs)
    {
        var alerts = new List<SecurityAlert>();
        
        // Look for users performing increasingly sensitive operations
        var privilegedOperations = new[] { "assign-license", "create-security-group", "reset-user-password" };
        
        var userPrivilegedOps = logs.Where(l => privilegedOperations.Contains(l.TemplateId))
            .GroupBy(l => l.UserId)
            .Where(g => g.Count() > 3)
            .ToList();
        
        foreach (var userGroup in userPrivilegedOps)
        {
            alerts.Add(new SecurityAlert
            {
                AlertType = "PrivilegeEscalation",
                UserId = userGroup.Key,
                Description = $"User performed multiple high-privilege operations",
                Severity = "High",
                DetectedAt = DateTime.UtcNow,
                Evidence = new Dictionary<string, object>
                {
                    ["privilegedOperations"] = userGroup.Select(l => l.Operation).ToList(),
                    ["count"] = userGroup.Count()
                }
            });
        }
        
        return alerts;
    }

    private List<SecurityAlert> DetectSuspiciousIPActivity(List<AdminAuditLog> logs)
    {
        var alerts = new List<SecurityAlert>();
        
        // Look for IPs with unusually high activity or multiple user associations
        var ipActivity = logs.GroupBy(l => l.ClientIp)
            .Select(g => new 
            { 
                IP = g.Key, 
                UserCount = g.Select(l => l.UserId).Distinct().Count(),
                OperationCount = g.Count()
            })
            .Where(x => x.UserCount > 3 || x.OperationCount > 50)
            .ToList();
        
        foreach (var ipInfo in ipActivity)
        {
            alerts.Add(new SecurityAlert
            {
                AlertType = "SuspiciousIPActivity",
                Description = $"IP {ipInfo.IP} associated with {ipInfo.UserCount} users and {ipInfo.OperationCount} operations",
                Severity = ipInfo.UserCount > 5 ? "High" : "Medium",
                DetectedAt = DateTime.UtcNow,
                Evidence = new Dictionary<string, object>
                {
                    ["clientIp"] = ipInfo.IP,
                    ["uniqueUsers"] = ipInfo.UserCount,
                    ["totalOperations"] = ipInfo.OperationCount
                }
            });
        }
        
        return alerts;
    }

    // Additional helper methods for audit reporting...
    private string GetMostActiveUser(List<AdminAuditLog> logs)
    {
        return logs.GroupBy(l => l.UserName)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault()?.Key ?? "None";
    }

    private List<string> GetTopOperations(List<AdminAuditLog> logs)
    {
        return logs.GroupBy(l => l.Operation)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => $"{g.Key} ({g.Count()})")
            .ToList();
    }

    private List<string> GetSecurityAlerts(List<AdminAuditLog> logs)
    {
        var alerts = new List<string>();
        
        var failureRate = logs.Count > 0 ? (double)logs.Count(l => !l.Success) / logs.Count : 0;
        if (failureRate > 0.1)
        {
            alerts.Add($"High failure rate: {failureRate:P1}");
        }
        
        var offHoursCount = logs.Count(l => 
            l.Timestamp.TimeOfDay < TimeSpan.FromHours(9) || 
            l.Timestamp.TimeOfDay > TimeSpan.FromHours(18));
        
        if (offHoursCount > logs.Count * 0.2)
        {
            alerts.Add($"Significant off-hours activity: {offHoursCount} operations");
        }
        
        return alerts;
    }

    private List<string> GetComplianceViolations(List<AdminAuditLog> logs)
    {
        // This would implement specific compliance checks
        return new List<string>();
    }

    private Dictionary<string, object> CalculatePerformanceMetrics(List<AdminAuditLog> logs)
    {
        if (!logs.Any()) return new Dictionary<string, object>();
        
        return new Dictionary<string, object>
        {
            ["AverageExecutionTime"] = logs.Average(l => l.Duration.TotalSeconds),
            ["MaxExecutionTime"] = logs.Max(l => l.Duration.TotalSeconds),
            ["MinExecutionTime"] = logs.Min(l => l.Duration.TotalSeconds),
            ["OperationsPerHour"] = logs.Count / Math.Max(1, (logs.Max(l => l.Timestamp) - logs.Min(l => l.Timestamp)).TotalHours)
        };
    }

    private List<string> GenerateRecommendations(List<AdminAuditLog> logs)
    {
        var recommendations = new List<string>();
        
        var failureRate = logs.Count > 0 ? (double)logs.Count(l => !l.Success) / logs.Count : 0;
        if (failureRate > 0.05)
        {
            recommendations.Add("Consider reviewing failed operations and providing additional training");
        }
        
        var avgDuration = logs.Any() ? logs.Average(l => l.Duration.TotalSeconds) : 0;
        if (avgDuration > 30)
        {
            recommendations.Add("Operations are taking longer than expected - consider performance optimization");
        }
        
        return recommendations;
    }

    private async Task StoreAuditReportAsync(AdminAuditReport report)
    {
        var reportPath = Path.Combine(_auditLogPath, "reports", $"audit-report-{report.ReportId}.json");
        var reportDirectory = Path.GetDirectoryName(reportPath)!;
        
        if (!Directory.Exists(reportDirectory))
        {
            Directory.CreateDirectory(reportDirectory);
        }
        
        var reportJson = JsonSerializer.Serialize(report, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        
        await File.WriteAllTextAsync(reportPath, reportJson);
    }

    private AdminAuditReport CreateEmptyReport(DateTime startDate, DateTime endDate)
    {
        return new AdminAuditReport
        {
            ReportId = Guid.NewGuid().ToString(),
            GeneratedAt = DateTime.UtcNow,
            PeriodStart = startDate,
            PeriodEnd = endDate,
            GeneratedBy = "AdminAuditService",
            TotalOperations = 0,
            SuccessfulOperations = 0,
            FailedOperations = 0,
            UniqueUsers = 0,
            MostActiveUser = "None",
            TopOperations = new List<string>(),
            SecurityAlerts = new List<string>(),
            ComplianceViolations = new List<string>(),
            PerformanceMetrics = new Dictionary<string, object>(),
            Recommendations = new List<string> { "Insufficient data for analysis" }
        };
    }

    private void PerformCleanup(object? state)
    {
        try
        {
            var retentionDays = _configuration.GetValue<int>("MicrosoftAdmin:AuditRetentionDays", 90);
            var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);
            
            var logFiles = Directory.GetFiles(_auditLogPath, "admin-audit-*.jsonl");
            
            foreach (var file in logFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                if (DateTime.TryParseExact(fileName.Substring("admin-audit-".Length), "yyyy-MM-dd", 
                    null, System.Globalization.DateTimeStyles.None, out var fileDate))
                {
                    if (fileDate < cutoffDate.Date)
                    {
                        File.Delete(file);
                        _logger.LogInformation("Deleted old audit log file: {File}", file);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during audit log cleanup");
        }
    }

    public void Dispose()
    {
        _writeLock?.Dispose();
        _cleanupTimer?.Dispose();
        _rateLimiter?.Dispose();
    }
}

// Supporting classes
public class AdminAuditReport
{
    public string ReportId { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public string GeneratedBy { get; set; } = string.Empty;
    public int TotalOperations { get; set; }
    public int SuccessfulOperations { get; set; }
    public int FailedOperations { get; set; }
    public int UniqueUsers { get; set; }
    public string MostActiveUser { get; set; } = string.Empty;
    public List<string> TopOperations { get; set; } = new();
    public List<string> SecurityAlerts { get; set; } = new();
    public List<string> ComplianceViolations { get; set; } = new();
    public Dictionary<string, object> PerformanceMetrics { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
}

public class SecurityAlert
{
    public string AlertType { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public DateTime DetectedAt { get; set; }
    public Dictionary<string, object> Evidence { get; set; } = new();
}

public class RateLimiter : IDisposable
{
    private readonly Dictionary<string, List<DateTime>> _requestTimes = new();
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly int _maxRequestsPerMinute = 60;

    public bool IsAllowed(string key)
    {
        _lock.Wait();
        try
        {
            var now = DateTime.UtcNow;
            var oneMinuteAgo = now.AddMinutes(-1);

            if (!_requestTimes.ContainsKey(key))
            {
                _requestTimes[key] = new List<DateTime>();
            }

            var requests = _requestTimes[key];
            requests.RemoveAll(time => time < oneMinuteAgo);

            if (requests.Count >= _maxRequestsPerMinute)
            {
                return false;
            }

            requests.Add(now);
            return true;
        }
        finally
        {
            _lock.Release();
        }
    }

    public void Dispose()
    {
        _lock?.Dispose();
    }
}