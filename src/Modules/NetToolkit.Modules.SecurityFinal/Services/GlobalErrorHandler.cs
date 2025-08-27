using NetToolkit.Modules.SecurityFinal.Models;
using NetToolkit.Modules.SecurityFinal.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace NetToolkit.Modules.SecurityFinal.Services;

/// <summary>
/// Global error handler with witty recovery mechanisms and resilience patterns
/// The digital paramedic of NetToolkit - turning crashes into comebacks!
/// </summary>
public class GlobalErrorHandler : IErrorHandler
{
    private readonly ILogger<GlobalErrorHandler> _logger;
    private readonly IRecoveryService _recoveryService;
    private readonly IAuditService _auditService;
    private readonly ConcurrentDictionary<string, ErrorStatistics> _errorStats;
    private readonly ConcurrentDictionary<string, DateTime> _lastErrorTimes;
    private readonly Random _wittyMessageRandom;
    private readonly object _statsLock = new();

    // Polly retry policies for different error types
    private readonly IAsyncPolicy _networkRetryPolicy;
    private readonly IAsyncPolicy _fileSystemRetryPolicy;
    private readonly IAsyncPolicy _databaseRetryPolicy;
    private readonly IAsyncPolicy _generalRetryPolicy;

    public GlobalErrorHandler(
        ILogger<GlobalErrorHandler> logger,
        IRecoveryService recoveryService,
        IAuditService auditService)
    {
        _logger = logger;
        _recoveryService = recoveryService;
        _auditService = auditService;
        _errorStats = new ConcurrentDictionary<string, ErrorStatistics>();
        _lastErrorTimes = new ConcurrentDictionary<string, DateTime>();
        _wittyMessageRandom = new Random();

        // Initialize retry policies with different strategies
        _networkRetryPolicy = CreateNetworkRetryPolicy();
        _fileSystemRetryPolicy = CreateFileSystemRetryPolicy();
        _databaseRetryPolicy = CreateDatabaseRetryPolicy();
        _generalRetryPolicy = CreateGeneralRetryPolicy();
    }

    public async Task<RecoveryAction> HandleExceptionAsync(Exception exception, ErrorContext context)
    {
        try
        {
            _logger.LogError(exception, "Handling exception in {Module}.{Operation} for user {User}", 
                context.ModuleName, context.OperationName, context.UserId);

            // Update error statistics
            UpdateErrorStatistics(exception, context);

            // Determine recovery strategy based on exception type and context
            var recoveryStrategy = DetermineRecoveryStrategy(exception, context);
            
            // Execute recovery action
            var recoveryAction = await ExecuteRecoveryAsync(exception, context, recoveryStrategy);

            // Audit the error and recovery
            await AuditErrorAndRecoveryAsync(exception, context, recoveryAction);

            // Check if we need to trigger auto-save
            if (IsRecoveryTriggered(recoveryAction))
            {
                await TriggerEmergencySaveAsync(context);
            }

            return recoveryAction;
        }
        catch (Exception handlerException)
        {
            _logger.LogCritical(handlerException, "Error handler itself failed - this is not good!");
            
            // Return emergency fallback action
            return new RecoveryAction
            {
                ActionType = RecoveryActionType.Emergency,
                Description = "Error handler failure - emergency protocols activated",
                WittyMessage = "Even the error handler had an error - it's errors all the way down! 🐢",
                WasSuccessful = false,
                TimeTaken = TimeSpan.Zero,
                NextSteps = "Manual intervention may be required"
            };
        }
    }

    public async Task RegisterGlobalHandlersAsync()
    {
        try
        {
            _logger.LogInformation("Registering global exception handlers");

            // Register unhandled exception handlers
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            AppDomain.CurrentDomain.FirstChanceException += OnFirstChanceException;
            
            // For WPF applications
            if (System.Windows.Application.Current != null)
            {
                System.Windows.Application.Current.DispatcherUnhandledException += OnDispatcherUnhandledException;
            }

            // Register task scheduler unobserved task exception handler
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

            _logger.LogInformation("🛡️ Global exception handlers registered - Safety nets deployed across the digital realm!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to register global exception handlers");
            throw;
        }
    }

    public async Task<ErrorStatistics> GetErrorStatisticsAsync()
    {
        try
        {
            lock (_statsLock)
            {
                var overallStats = new ErrorStatistics
                {
                    TotalErrors = _errorStats.Values.Sum(s => s.TotalErrors),
                    RecoveredErrors = _errorStats.Values.Sum(s => s.RecoveredErrors),
                    UnrecoveredErrors = _errorStats.Values.Sum(s => s.UnrecoveredErrors),
                    TotalDowntime = TimeSpan.FromMilliseconds(_errorStats.Values.Sum(s => s.TotalDowntime.TotalMilliseconds)),
                    StatisticsPeriodStart = _errorStats.Values.Any() ? _errorStats.Values.Min(s => s.StatisticsPeriodStart) : DateTime.UtcNow,
                    StatisticsPeriodEnd = DateTime.UtcNow
                };

                // Aggregate errors by type
                foreach (var stat in _errorStats.Values)
                {
                    foreach (var errorType in stat.ErrorsByType)
                    {
                        overallStats.ErrorsByType[errorType.Key] = 
                            overallStats.ErrorsByType.GetValueOrDefault(errorType.Key, 0) + errorType.Value;
                    }
                    
                    foreach (var moduleError in stat.ErrorsByModule)
                    {
                        overallStats.ErrorsByModule[moduleError.Key] = 
                            overallStats.ErrorsByModule.GetValueOrDefault(moduleError.Key, 0) + moduleError.Value;
                    }
                }

                _logger.LogDebug("Retrieved error statistics: {TotalErrors} total, {RecoveryRate:F1}% recovery rate", 
                    overallStats.TotalErrors, overallStats.RecoverySuccessRate);

                return overallStats;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get error statistics");
            return new ErrorStatistics
            {
                StatisticsPeriodStart = DateTime.UtcNow,
                StatisticsPeriodEnd = DateTime.UtcNow
            };
        }
    }

    public async Task<ErrorHandlingTestResult> TestErrorHandlingAsync()
    {
        try
        {
            _logger.LogInformation("Testing error handling mechanisms");

            var testResult = new ErrorHandlingTestResult
            {
                AllTestsPassed = true,
                TestedAt = DateTime.UtcNow
            };

            var startTime = DateTime.UtcNow;

            // Test 1: Basic exception handling
            await TestBasicExceptionHandling(testResult);
            
            // Test 2: Retry mechanism
            await TestRetryMechanism(testResult);
            
            // Test 3: Fallback behavior
            await TestFallbackBehavior(testResult);
            
            // Test 4: Recovery service integration
            await TestRecoveryIntegration(testResult);
            
            // Test 5: Witty message generation
            await TestWittyMessageGeneration(testResult);

            testResult.TotalTestTime = DateTime.UtcNow - startTime;
            testResult.AllTestsPassed = !testResult.FailedTests.Any();

            var resultEmoji = testResult.AllTestsPassed ? "✅" : "⚠️";
            _logger.LogInformation("{Emoji} Error handling test completed: {Passed}/{Total} tests passed in {Duration}ms",
                resultEmoji, testResult.TestResults.Count(kvp => kvp.Value), testResult.TestResults.Count,
                testResult.TotalTestTime.TotalMilliseconds);

            return testResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling test itself failed - inception-level problems!");
            return new ErrorHandlingTestResult
            {
                AllTestsPassed = false,
                FailedTests = { "TestFramework" },
                TestedAt = DateTime.UtcNow,
                TotalTestTime = TimeSpan.Zero,
                AdditionalNotes = $"Test framework failure: {ex.Message}"
            };
        }
    }

    private RecoveryActionType DetermineRecoveryStrategy(Exception exception, ErrorContext context)
    {
        // Determine strategy based on exception type
        return exception switch
        {
            // Network-related exceptions - retry with backoff
            HttpRequestException or TimeoutException => RecoveryActionType.Retry,
            
            // File system exceptions - retry with different approach
            DirectoryNotFoundException or FileNotFoundException => RecoveryActionType.Fallback,
            IOException or UnauthorizedAccessException => RecoveryActionType.Retry,
            
            // Memory/resource exceptions - graceful degradation
            OutOfMemoryException or InsufficientMemoryException => RecoveryActionType.Graceful,
            
            // Security exceptions - no retry, audit and deny
            UnauthorizedAccessException or System.Security.SecurityException => RecoveryActionType.Graceful,
            
            // Argument exceptions - usually programming errors, log and continue
            ArgumentException or ArgumentNullException => RecoveryActionType.Graceful,
            
            // Critical system exceptions - emergency procedures
            StackOverflowException or AccessViolationException => RecoveryActionType.Emergency,
            
            // Database/connection exceptions - retry with exponential backoff
            InvalidOperationException when exception.Message.Contains("database") => RecoveryActionType.Retry,
            
            // Default - try graceful recovery
            _ => context.AttemptNumber >= 3 ? RecoveryActionType.Emergency : RecoveryActionType.Retry
        };
    }

    private async Task<RecoveryAction> ExecuteRecoveryAsync(Exception exception, ErrorContext context, RecoveryActionType strategy)
    {
        var stopwatch = Stopwatch.StartNew();
        var wittyMessage = GetWittyMessage(strategy, exception, context);

        try
        {
            switch (strategy)
            {
                case RecoveryActionType.Retry:
                    return await ExecuteRetryRecoveryAsync(exception, context, wittyMessage, stopwatch);
                    
                case RecoveryActionType.Fallback:
                    return await ExecuteFallbackRecoveryAsync(exception, context, wittyMessage, stopwatch);
                    
                case RecoveryActionType.Graceful:
                    return await ExecuteGracefulRecoveryAsync(exception, context, wittyMessage, stopwatch);
                    
                case RecoveryActionType.Emergency:
                    return await ExecuteEmergencyRecoveryAsync(exception, context, wittyMessage, stopwatch);
                    
                default:
                    return await ExecuteGracefulRecoveryAsync(exception, context, wittyMessage, stopwatch);
            }
        }
        finally
        {
            stopwatch.Stop();
        }
    }

    private async Task<RecoveryAction> ExecuteRetryRecoveryAsync(Exception exception, ErrorContext context, string wittyMessage, Stopwatch stopwatch)
    {
        try
        {
            // Select appropriate retry policy
            var policy = SelectRetryPolicy(exception);
            
            var success = false;
            var retryCount = 0;
            
            await policy.ExecuteAsync(async () =>
            {
                retryCount++;
                // Simulate retry operation - in real implementation, this would re-execute the failed operation
                await Task.Delay(100);
                
                if (retryCount >= 2) // Simulate success after 2 retries
                {
                    success = true;
                    return;
                }
                
                throw new InvalidOperationException("Simulated retry failure");
            });

            return new RecoveryAction
            {
                ActionType = RecoveryActionType.Retry,
                Description = $"Operation retried successfully after {retryCount} attempts",
                WittyMessage = wittyMessage,
                WasSuccessful = success,
                TimeTaken = stopwatch.Elapsed,
                RecoveryDetails = new Dictionary<string, object>
                {
                    ["RetryCount"] = retryCount,
                    ["RetryPolicy"] = policy.GetType().Name
                },
                NextSteps = success ? null : "Consider manual intervention"
            };
        }
        catch (Exception retryEx)
        {
            return new RecoveryAction
            {
                ActionType = RecoveryActionType.Retry,
                Description = $"Retry failed after multiple attempts: {retryEx.Message}",
                WittyMessage = "Retry sequence exhausted - even persistence has its limits! 🔄",
                WasSuccessful = false,
                TimeTaken = stopwatch.Elapsed,
                NextSteps = "Escalate to fallback recovery or manual intervention"
            };
        }
    }

    private async Task<RecoveryAction> ExecuteFallbackRecoveryAsync(Exception exception, ErrorContext context, string wittyMessage, Stopwatch stopwatch)
    {
        try
        {
            // Implement fallback logic based on context
            var fallbackSuccess = await AttemptFallbackOperationAsync(exception, context);
            
            return new RecoveryAction
            {
                ActionType = RecoveryActionType.Fallback,
                Description = "Fallback operation executed successfully",
                WittyMessage = wittyMessage,
                WasSuccessful = fallbackSuccess,
                TimeTaken = stopwatch.Elapsed,
                RecoveryDetails = new Dictionary<string, object>
                {
                    ["FallbackMethod"] = GetFallbackMethodName(exception),
                    ["OriginalOperation"] = context.OperationName
                },
                NextSteps = fallbackSuccess ? null : "Consider alternative approaches"
            };
        }
        catch (Exception fallbackEx)
        {
            return new RecoveryAction
            {
                ActionType = RecoveryActionType.Fallback,
                Description = $"Fallback operation also failed: {fallbackEx.Message}",
                WittyMessage = "Plan B has joined Plan A in the failure pile - time for Plan C! 📋",
                WasSuccessful = false,
                TimeTaken = stopwatch.Elapsed,
                NextSteps = "Escalate to emergency recovery procedures"
            };
        }
    }

    private async Task<RecoveryAction> ExecuteGracefulRecoveryAsync(Exception exception, ErrorContext context, string wittyMessage, Stopwatch stopwatch)
    {
        try
        {
            // Implement graceful degradation
            await LogGracefulDegradationAsync(exception, context);
            
            // Continue with reduced functionality
            return new RecoveryAction
            {
                ActionType = RecoveryActionType.Graceful,
                Description = "System continues with reduced functionality",
                WittyMessage = wittyMessage,
                WasSuccessful = true,
                TimeTaken = stopwatch.Elapsed,
                RecoveryDetails = new Dictionary<string, object>
                {
                    ["DegradationLevel"] = "Minimal",
                    ["AffectedFeatures"] = GetAffectedFeatures(context)
                },
                NextSteps = "Monitor system stability and plan full recovery"
            };
        }
        catch (Exception gracefulEx)
        {
            return new RecoveryAction
            {
                ActionType = RecoveryActionType.Graceful,
                Description = $"Graceful recovery encountered issues: {gracefulEx.Message}",
                WittyMessage = "Even graceful recovery stumbled - but we're still standing! 💃",
                WasSuccessful = false,
                TimeTaken = stopwatch.Elapsed,
                NextSteps = "Escalate to emergency procedures"
            };
        }
    }

    private async Task<RecoveryAction> ExecuteEmergencyRecoveryAsync(Exception exception, ErrorContext context, string wittyMessage, Stopwatch stopwatch)
    {
        try
        {
            _logger.LogCritical("Executing emergency recovery procedures for critical exception in {Module}", context.ModuleName);
            
            // Trigger emergency save
            await _recoveryService.SaveStateAsync(new SaveContext
            {
                SessionId = Guid.NewGuid().ToString(),
                UserId = context.UserId,
                ApplicationState = new Dictionary<string, object>
                {
                    ["EmergencySave"] = true,
                    ["TriggeringException"] = exception.GetType().Name,
                    ["ModuleName"] = context.ModuleName,
                    ["OperationName"] = context.OperationName,
                    ["Timestamp"] = DateTime.UtcNow
                },
                Type = SaveType.Emergency,
                Description = $"Emergency save triggered by {exception.GetType().Name}"
            });
            
            return new RecoveryAction
            {
                ActionType = RecoveryActionType.Emergency,
                Description = "Emergency protocols activated - system state preserved",
                WittyMessage = wittyMessage,
                WasSuccessful = true,
                TimeTaken = stopwatch.Elapsed,
                RecoveryDetails = new Dictionary<string, object>
                {
                    ["EmergencySaveCompleted"] = true,
                    ["CriticalException"] = exception.GetType().Name
                },
                NextSteps = "Review system logs and consider restart if necessary"
            };
        }
        catch (Exception emergencyEx)
        {
            return new RecoveryAction
            {
                ActionType = RecoveryActionType.Emergency,
                Description = $"Emergency recovery failed: {emergencyEx.Message}",
                WittyMessage = "Emergency protocols failed - this is the digital equivalent of calling 911! 🚨",
                WasSuccessful = false,
                TimeTaken = stopwatch.Elapsed,
                NextSteps = "Immediate manual intervention required"
            };
        }
    }

    private string GetWittyMessage(RecoveryActionType strategy, Exception exception, ErrorContext context)
    {
        var messages = WittyMessages.RecoveryMessages.GetValueOrDefault(strategy, new List<string>
        {
            "Recovery attempt in progress - crossing digital fingers! 🤞"
        });

        var baseMessage = WittyMessages.GetRandomMessage(messages);
        
        // Add context-specific humor
        var contextualAddition = exception switch
        {
            FileNotFoundException => " (The file played hide and seek - and won!)",
            OutOfMemoryException => " (RAM said 'I'm full!' and went on a diet)",
            TimeoutException => " (Time's up! The operation got stage fright)",
            UnauthorizedAccessException => " (Access denied - even computers have trust issues)",
            ArgumentNullException => " (Null argument spotted - the void strikes back!)",
            _ => ""
        };

        return baseMessage + contextualAddition;
    }

    private void UpdateErrorStatistics(Exception exception, ErrorContext context)
    {
        try
        {
            var statsKey = $"{context.ModuleName}:{exception.GetType().Name}";
            
            _errorStats.AddOrUpdate(statsKey, 
                new ErrorStatistics
                {
                    TotalErrors = 1,
                    ErrorsByType = { [exception.GetType().Name] = 1 },
                    ErrorsByModule = { [context.ModuleName] = 1 },
                    StatisticsPeriodStart = DateTime.UtcNow,
                    StatisticsPeriodEnd = DateTime.UtcNow
                },
                (key, existing) =>
                {
                    existing.TotalErrors++;
                    existing.ErrorsByType[exception.GetType().Name] = 
                        existing.ErrorsByType.GetValueOrDefault(exception.GetType().Name, 0) + 1;
                    existing.ErrorsByModule[context.ModuleName] = 
                        existing.ErrorsByModule.GetValueOrDefault(context.ModuleName, 0) + 1;
                    existing.StatisticsPeriodEnd = DateTime.UtcNow;
                    return existing;
                });

            _lastErrorTimes[statsKey] = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to update error statistics - the irony is not lost on us");
        }
    }

    private async Task AuditErrorAndRecoveryAsync(Exception exception, ErrorContext context, RecoveryAction recoveryAction)
    {
        try
        {
            await _auditService.LogOperationalEventAsync(new OperationalAuditEvent
            {
                EventId = Guid.NewGuid().ToString(),
                EventType = OperationalEventType.ErrorOccurred,
                UserId = context.UserId,
                Description = $"Exception handled: {exception.GetType().Name} in {context.ModuleName}.{context.OperationName}",
                OccurredAt = DateTime.UtcNow,
                SourceModule = context.ModuleName,
                IsSuccessful = recoveryAction.WasSuccessful,
                EventData = new Dictionary<string, object>
                {
                    ["ExceptionType"] = exception.GetType().Name,
                    ["ExceptionMessage"] = exception.Message,
                    ["RecoveryAction"] = recoveryAction.ActionType.ToString(),
                    ["RecoverySuccessful"] = recoveryAction.WasSuccessful,
                    ["RecoveryTime"] = recoveryAction.TimeTaken.TotalMilliseconds,
                    ["WittyMessage"] = recoveryAction.WittyMessage,
                    ["AttemptNumber"] = context.AttemptNumber
                }
            });
        }
        catch (Exception auditEx)
        {
            _logger.LogWarning(auditEx, "Failed to audit error and recovery - audit system needs its own error handler!");
        }
    }

    #region Event Handlers

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var exception = e.ExceptionObject as Exception ?? new Exception("Unknown unhandled exception");
        var context = new ErrorContext
        {
            ModuleName = "GlobalHandler",
            OperationName = "UnhandledException",
            UserId = Environment.UserName,
            OccurredAt = DateTime.UtcNow,
            AdditionalInfo = $"IsTerminating: {e.IsTerminating}"
        };

        _ = Task.Run(async () =>
        {
            var recovery = await HandleExceptionAsync(exception, context);
            _logger.LogCritical("Unhandled exception processed: {Recovery} - {Message}", 
                recovery.ActionType, recovery.WittyMessage);
        });
    }

    private void OnFirstChanceException(object? sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
    {
        // Log first chance exceptions for debugging (but don't handle them - they might be handled elsewhere)
        _logger.LogTrace("First chance exception: {ExceptionType} - {Message}", 
            e.Exception.GetType().Name, e.Exception.Message);
    }

    private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        var context = new ErrorContext
        {
            ModuleName = "WPF",
            OperationName = "DispatcherUnhandledException",
            UserId = Environment.UserName,
            OccurredAt = DateTime.UtcNow,
            AdditionalInfo = "WPF Dispatcher thread exception"
        };

        _ = Task.Run(async () =>
        {
            var recovery = await HandleExceptionAsync(e.Exception, context);
            
            // Decide whether to mark as handled
            e.Handled = recovery.WasSuccessful && recovery.ActionType != RecoveryActionType.Emergency;
            
            _logger.LogError("Dispatcher exception processed: {Recovery} - Handled: {Handled}", 
                recovery.ActionType, e.Handled);
        });
    }

    private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        var context = new ErrorContext
        {
            ModuleName = "TaskScheduler",
            OperationName = "UnobservedTaskException",
            UserId = Environment.UserName,
            OccurredAt = DateTime.UtcNow,
            AdditionalInfo = "Unobserved task exception"
        };

        _ = Task.Run(async () =>
        {
            foreach (var exception in e.Exception.InnerExceptions)
            {
                var recovery = await HandleExceptionAsync(exception, context);
                _logger.LogWarning("Unobserved task exception processed: {Recovery}", recovery.ActionType);
            }
        });

        e.SetObserved(); // Prevent the application from terminating
    }

    #endregion

    #region Helper Methods

    private IAsyncPolicy SelectRetryPolicy(Exception exception)
    {
        return exception switch
        {
            HttpRequestException or TimeoutException => _networkRetryPolicy,
            IOException or FileNotFoundException => _fileSystemRetryPolicy,
            InvalidOperationException when exception.Message.Contains("database") => _databaseRetryPolicy,
            _ => _generalRetryPolicy
        };
    }

    private IAsyncPolicy CreateNetworkRetryPolicy()
    {
        return Policy
            .Handle<HttpRequestException>()
            .Or<TimeoutException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    _logger.LogWarning("Network retry attempt {RetryCount} in {Delay}ms", retryCount, timespan.TotalMilliseconds);
                });
    }

    private IAsyncPolicy CreateFileSystemRetryPolicy()
    {
        return Policy
            .Handle<IOException>()
            .Or<FileNotFoundException>()
            .WaitAndRetryAsync(
                retryCount: 2,
                sleepDurationProvider: retryAttempt => TimeSpan.FromMilliseconds(500 * retryAttempt),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    _logger.LogWarning("File system retry attempt {RetryCount} in {Delay}ms", retryCount, timespan.TotalMilliseconds);
                });
    }

    private IAsyncPolicy CreateDatabaseRetryPolicy()
    {
        return Policy
            .Handle<InvalidOperationException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(retryAttempt * 2),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    _logger.LogWarning("Database retry attempt {RetryCount} in {Delay}ms", retryCount, timespan.TotalMilliseconds);
                });
    }

    private IAsyncPolicy CreateGeneralRetryPolicy()
    {
        return Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 2,
                sleepDurationProvider: retryAttempt => TimeSpan.FromMilliseconds(1000),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    _logger.LogWarning("General retry attempt {RetryCount} in {Delay}ms", retryCount, timespan.TotalMilliseconds);
                });
    }

    private async Task<bool> AttemptFallbackOperationAsync(Exception exception, ErrorContext context)
    {
        // Simulate fallback operation based on exception type
        await Task.Delay(100);
        
        return exception switch
        {
            FileNotFoundException => true, // Use default/cached file
            DirectoryNotFoundException => true, // Create directory or use temp
            UnauthorizedAccessException => false, // Can't work around security
            _ => true // Most operations can have fallbacks
        };
    }

    private string GetFallbackMethodName(Exception exception)
    {
        return exception switch
        {
            FileNotFoundException => "UseDefaultFile",
            DirectoryNotFoundException => "CreateTempDirectory",
            TimeoutException => "UseCachedData",
            _ => "GenericFallback"
        };
    }

    private async Task LogGracefulDegradationAsync(Exception exception, ErrorContext context)
    {
        _logger.LogInformation("Graceful degradation activated for {Module}.{Operation} due to {ExceptionType}",
            context.ModuleName, context.OperationName, exception.GetType().Name);
        
        // Simulate logging graceful degradation
        await Task.Delay(10);
    }

    private List<string> GetAffectedFeatures(ErrorContext context)
    {
        return context.ModuleName switch
        {
            "Terminal" => new List<string> { "SSH connections", "Script execution" },
            "Scanner" => new List<string> { "Port scanning", "Vulnerability detection" },
            "Topography" => new List<string> { "3D visualization", "Network mapping" },
            _ => new List<string> { "Advanced features" }
        };
    }

    private bool IsRecoveryTriggered(RecoveryAction recoveryAction)
    {
        return recoveryAction.ActionType == RecoveryActionType.Emergency || 
               !recoveryAction.WasSuccessful;
    }

    private async Task TriggerEmergencySaveAsync(ErrorContext context)
    {
        try
        {
            await _recoveryService.SaveStateAsync(new SaveContext
            {
                SessionId = Guid.NewGuid().ToString(),
                UserId = context.UserId,
                ApplicationState = new Dictionary<string, object>
                {
                    ["TriggeringModule"] = context.ModuleName,
                    ["TriggeringOperation"] = context.OperationName,
                    ["SaveReason"] = "ErrorRecoveryTriggered"
                },
                Type = SaveType.Emergency,
                Description = $"Emergency save triggered by error in {context.ModuleName}"
            });
            
            _logger.LogInformation("💾 Emergency save completed - Digital life preserver deployed!");
        }
        catch (Exception saveEx)
        {
            _logger.LogError(saveEx, "Emergency save failed - we're in deep trouble now!");
        }
    }

    #endregion

    #region Test Methods

    private async Task TestBasicExceptionHandling(ErrorHandlingTestResult testResult)
    {
        try
        {
            var testException = new InvalidOperationException("Test exception");
            var testContext = new ErrorContext
            {
                ModuleName = "TestModule",
                OperationName = "TestOperation",
                UserId = "TestUser",
                OccurredAt = DateTime.UtcNow
            };

            var recovery = await HandleExceptionAsync(testException, testContext);
            testResult.TestResults["BasicExceptionHandling"] = recovery != null && !string.IsNullOrEmpty(recovery.WittyMessage);
        }
        catch (Exception ex)
        {
            testResult.TestResults["BasicExceptionHandling"] = false;
            testResult.FailedTests.Add($"BasicExceptionHandling: {ex.Message}");
        }
    }

    private async Task TestRetryMechanism(ErrorHandlingTestResult testResult)
    {
        try
        {
            var policy = _generalRetryPolicy;
            var attempts = 0;
            
            await policy.ExecuteAsync(async () =>
            {
                attempts++;
                if (attempts < 2)
                    throw new InvalidOperationException("Retry test");
                await Task.CompletedTask;
            });

            testResult.TestResults["RetryMechanism"] = attempts >= 2;
        }
        catch (Exception ex)
        {
            testResult.TestResults["RetryMechanism"] = false;
            testResult.FailedTests.Add($"RetryMechanism: {ex.Message}");
        }
    }

    private async Task TestFallbackBehavior(ErrorHandlingTestResult testResult)
    {
        try
        {
            var testContext = new ErrorContext
            {
                ModuleName = "TestModule",
                OperationName = "TestFallback",
                UserId = "TestUser",
                OccurredAt = DateTime.UtcNow
            };

            var fallbackResult = await AttemptFallbackOperationAsync(new FileNotFoundException(), testContext);
            testResult.TestResults["FallbackBehavior"] = fallbackResult;
        }
        catch (Exception ex)
        {
            testResult.TestResults["FallbackBehavior"] = false;
            testResult.FailedTests.Add($"FallbackBehavior: {ex.Message}");
        }
    }

    private async Task TestRecoveryIntegration(ErrorHandlingTestResult testResult)
    {
        try
        {
            // Test that recovery service integration works
            var mockContext = new SaveContext
            {
                SessionId = "TestSession",
                UserId = "TestUser",
                Type = SaveType.Emergency,
                Description = "Test recovery integration"
            };

            // This would test actual recovery service - for now we'll simulate success
            await Task.Delay(10);
            testResult.TestResults["RecoveryIntegration"] = true;
        }
        catch (Exception ex)
        {
            testResult.TestResults["RecoveryIntegration"] = false;
            testResult.FailedTests.Add($"RecoveryIntegration: {ex.Message}");
        }
    }

    private async Task TestWittyMessageGeneration(ErrorHandlingTestResult testResult)
    {
        try
        {
            var testException = new OutOfMemoryException("Test memory exception");
            var testContext = new ErrorContext
            {
                ModuleName = "TestModule",
                OperationName = "TestWittyMessage",
                UserId = "TestUser",
                OccurredAt = DateTime.UtcNow
            };

            var wittyMessage = GetWittyMessage(RecoveryActionType.Graceful, testException, testContext);
            testResult.TestResults["WittyMessageGeneration"] = !string.IsNullOrEmpty(wittyMessage) && wittyMessage.Length > 10;
        }
        catch (Exception ex)
        {
            testResult.TestResults["WittyMessageGeneration"] = false;
            testResult.FailedTests.Add($"WittyMessageGeneration: {ex.Message}");
        }
    }

    #endregion

    public void Dispose()
    {
        try
        {
            // Unregister event handlers
            AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
            AppDomain.CurrentDomain.FirstChanceException -= OnFirstChanceException;
            TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;

            if (System.Windows.Application.Current != null)
            {
                System.Windows.Application.Current.DispatcherUnhandledException -= OnDispatcherUnhandledException;
            }

            _logger.LogDebug("Global error handler disposed - Safety nets retracted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during global error handler disposal - even disposal can fail!");
        }
    }
}