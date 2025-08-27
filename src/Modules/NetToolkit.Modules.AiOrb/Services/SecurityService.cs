using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.RegularExpressions;

namespace NetToolkit.Modules.AiOrb.Services;

/// <summary>
/// Security service for input sanitization and threat protection
/// Guards the orb's cosmic consciousness from malicious inputs and attacks
/// </summary>
public class SecurityService
{
    private readonly ILogger<SecurityService> _logger;
    private readonly Dictionary<string, DateTime> _rateLimitTracker;
    private readonly Dictionary<string, int> _failedAttempts;
    private readonly SemaphoreSlim _lockSemaphore;

    // Security patterns for detection
    private static readonly Regex[] MaliciousPatterns = {
        new Regex(@"<script[^>]*>.*?</script>", RegexOptions.IgnoreCase | RegexOptions.Singleline),
        new Regex(@"javascript:", RegexOptions.IgnoreCase),
        new Regex(@"vbscript:", RegexOptions.IgnoreCase),
        new Regex(@"onload\s*=", RegexOptions.IgnoreCase),
        new Regex(@"onclick\s*=", RegexOptions.IgnoreCase),
        new Regex(@"eval\s*\(", RegexOptions.IgnoreCase),
        new Regex(@"cmd\.exe|powershell\.exe|bash|sh\s+", RegexOptions.IgnoreCase),
        new Regex(@"\b(DROP|DELETE|INSERT|UPDATE|SELECT)\s+(TABLE|FROM|INTO)", RegexOptions.IgnoreCase),
        new Regex(@"union\s+select", RegexOptions.IgnoreCase),
        new Regex(@"(\.|%2e)(\.|%2e)(\/|%2f|\\|%5c)", RegexOptions.IgnoreCase), // Directory traversal
        new Regex(@"[<>\"'&]", RegexOptions.None) // Basic HTML injection characters
    };

    // Sensitive information patterns
    private static readonly Regex[] SensitivePatterns = {
        new Regex(@"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b"), // Email
        new Regex(@"\b\d{3}-?\d{2}-?\d{4}\b"), // SSN
        new Regex(@"\b\d{4}[- ]?\d{4}[- ]?\d{4}[- ]?\d{4}\b"), // Credit card
        new Regex(@"\b(?:sk-[a-zA-Z0-9]{48}|pk_[a-zA-Z0-9]+)\b"), // API keys
        new Regex(@"password\s*[:=]\s*\S+", RegexOptions.IgnoreCase),
        new Regex(@"token\s*[:=]\s*\S+", RegexOptions.IgnoreCase),
        new Regex(@"secret\s*[:=]\s*\S+", RegexOptions.IgnoreCase)
    };

    // Rate limiting constants
    private const int MaxRequestsPerMinute = 60;
    private const int MaxFailedAttempts = 5;
    private const int LockoutDurationMinutes = 15;

    public SecurityService(ILogger<SecurityService> logger)
    {
        _logger = logger;
        _rateLimitTracker = new Dictionary<string, DateTime>();
        _failedAttempts = new Dictionary<string, int>();
        _lockSemaphore = new SemaphoreSlim(1, 1);
        
        _logger.LogInformation("Security service initialized - Orb cosmic defenses activated! 🛡️");
    }

    /// <summary>
    /// Sanitize and validate input for AI processing
    /// </summary>
    public async Task<(bool IsValid, string SanitizedInput, List<string> Issues)> SanitizeInputAsync(string input, string userId = "unknown")
    {
        try
        {
            await _lockSemaphore.WaitAsync();

            var issues = new List<string>();
            
            // Check rate limiting
            if (!await CheckRateLimitAsync(userId))
            {
                issues.Add("Rate limit exceeded - please slow down your cosmic queries! 🚫");
                return (false, string.Empty, issues);
            }

            // Check for account lockout
            if (IsUserLockedOut(userId))
            {
                issues.Add("Account temporarily locked due to suspicious activity - cosmic security protocol engaged! 🔒");
                return (false, string.Empty, issues);
            }

            // Validate input is not null or empty
            if (string.IsNullOrWhiteSpace(input))
            {
                issues.Add("Empty input detected - orb needs cosmic content to process! 📝");
                return (false, string.Empty, issues);
            }

            // Check input length
            if (input.Length > 10000)
            {
                issues.Add("Input too long - please keep cosmic queries under 10,000 characters! 📏");
                return (false, input.Substring(0, 10000), issues);
            }

            // Scan for malicious patterns
            var maliciousIssues = ScanForMaliciousContent(input);
            if (maliciousIssues.Any())
            {
                await RecordFailedAttemptAsync(userId);
                issues.AddRange(maliciousIssues);
                return (false, string.Empty, issues);
            }

            // Scan for sensitive information
            var sensitiveIssues = ScanForSensitiveInformation(input);
            if (sensitiveIssues.Any())
            {
                issues.AddRange(sensitiveIssues);
                // Don't fail validation, but warn user and sanitize
            }

            // Sanitize the input
            var sanitizedInput = SanitizeString(input);

            _logger.LogDebug("Input sanitized successfully for user {UserId} - Cosmic content approved! ✅", userId);
            return (true, sanitizedInput, issues);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Input sanitization failed - Cosmic security malfunction! ⚠️");
            return (false, string.Empty, new List<string> { "Security validation failed - please try again!" });
        }
        finally
        {
            _lockSemaphore.Release();
        }
    }

    /// <summary>
    /// Validate API configuration for security issues
    /// </summary>
    public async Task<(bool IsSecure, List<string> SecurityIssues)> ValidateApiConfigurationAsync(string apiKey, string baseUrl)
    {
        var issues = new List<string>();
        
        try
        {
            // Check API key format and strength
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                issues.Add("API key is required for secure cosmic communication");
                return (false, issues);
            }

            if (apiKey.Length < 10)
            {
                issues.Add("API key appears too short - cosmic security compromised");
            }

            if (apiKey.Contains("test") || apiKey.Contains("demo") || apiKey.Contains("example"))
            {
                issues.Add("API key appears to be a test/demo key - use production credentials");
            }

            // Check base URL security
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                issues.Add("Base URL is required for secure API communication");
                return (false, issues);
            }

            if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri))
            {
                issues.Add("Invalid base URL format");
                return (false, issues);
            }

            if (uri.Scheme != "https")
            {
                issues.Add("Non-HTTPS URL detected - cosmic security spirits are displeased! Use HTTPS for secure communication");
            }

            if (uri.Host.Contains("localhost") || uri.Host.Contains("127.0.0.1"))
            {
                issues.Add("Localhost URL detected - ensure this is intentional for development");
            }

            var isSecure = issues.Count(i => !i.Contains("localhost") && !i.Contains("development")) == 0;
            
            _logger.LogInformation("API configuration security validation completed - Security level: {SecurityLevel} 🔒", 
                isSecure ? "SECURE" : "NEEDS ATTENTION");

            return (isSecure, issues);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API configuration security validation failed - Cosmic security analysis error! ⚠️");
            issues.Add("Security validation failed - please review configuration manually");
            return (false, issues);
        }
    }

    /// <summary>
    /// Sanitize output before displaying to user
    /// </summary>
    public string SanitizeOutput(string output)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(output))
            {
                return string.Empty;
            }

            var sanitized = output;

            // Remove any potential script tags that might have been injected
            foreach (var pattern in MaliciousPatterns.Take(5)) // Only HTML/JS patterns for output
            {
                sanitized = pattern.Replace(sanitized, "[BLOCKED CONTENT]");
            }

            // Mask sensitive information in output
            sanitized = MaskSensitiveInformation(sanitized);

            return sanitized;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Output sanitization failed - Cosmic filter malfunction! 🧹");
            return "[OUTPUT SANITIZATION ERROR]";
        }
    }

    /// <summary>
    /// Generate security report for monitoring
    /// </summary>
    public async Task<SecurityReport> GenerateSecurityReportAsync()
    {
        try
        {
            await _lockSemaphore.WaitAsync();

            var report = new SecurityReport
            {
                GeneratedAt = DateTime.UtcNow,
                TotalUsers = _rateLimitTracker.Count,
                ActiveUsers = _rateLimitTracker.Count(kvp => DateTime.UtcNow - kvp.Value < TimeSpan.FromHours(1)),
                LockedUsers = _failedAttempts.Count(kvp => kvp.Value >= MaxFailedAttempts),
                TotalFailedAttempts = _failedAttempts.Values.Sum(),
                SecurityEvents = GetRecentSecurityEvents()
            };

            return report;
        }
        finally
        {
            _lockSemaphore.Release();
        }
    }

    // Private helper methods

    private async Task<bool> CheckRateLimitAsync(string userId)
    {
        var now = DateTime.UtcNow;
        
        if (_rateLimitTracker.TryGetValue(userId, out var lastRequest))
        {
            var timeSinceLastRequest = now - lastRequest;
            if (timeSinceLastRequest < TimeSpan.FromMinutes(1.0 / MaxRequestsPerMinute))
            {
                _logger.LogWarning("Rate limit exceeded for user {UserId} - Cosmic throttle engaged! 🚦", userId);
                await RecordFailedAttemptAsync(userId);
                return false;
            }
        }

        _rateLimitTracker[userId] = now;
        return true;
    }

    private bool IsUserLockedOut(string userId)
    {
        if (!_failedAttempts.TryGetValue(userId, out var attempts))
        {
            return false;
        }

        return attempts >= MaxFailedAttempts;
    }

    private async Task RecordFailedAttemptAsync(string userId)
    {
        _failedAttempts[userId] = _failedAttempts.GetValueOrDefault(userId, 0) + 1;
        
        if (_failedAttempts[userId] >= MaxFailedAttempts)
        {
            _logger.LogWarning("User {UserId} locked out due to {AttemptCount} failed attempts - Cosmic security lockdown activated! 🔒", 
                userId, _failedAttempts[userId]);
        }
    }

    private List<string> ScanForMaliciousContent(string input)
    {
        var issues = new List<string>();

        foreach (var pattern in MaliciousPatterns)
        {
            if (pattern.IsMatch(input))
            {
                var patternDescription = GetPatternDescription(pattern);
                issues.Add($"Potentially malicious content detected: {patternDescription} - Cosmic security alert! 🚨");
                
                _logger.LogWarning("Malicious pattern detected: {Pattern} in input - Orb defenses activated! 🛡️", 
                    patternDescription);
            }
        }

        return issues;
    }

    private List<string> ScanForSensitiveInformation(string input)
    {
        var issues = new List<string>();

        foreach (var pattern in SensitivePatterns)
        {
            if (pattern.IsMatch(input))
            {
                var patternDescription = GetSensitivePatternDescription(pattern);
                issues.Add($"Sensitive information detected: {patternDescription} - Consider removing for privacy! 🔐");
                
                _logger.LogInformation("Sensitive information detected: {Pattern} - Privacy protection advised! 🛡️", 
                    patternDescription);
            }
        }

        return issues;
    }

    private string SanitizeString(string input)
    {
        var sanitized = input;

        // HTML encode dangerous characters
        sanitized = sanitized
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&#39;");

        // Remove null bytes and control characters
        sanitized = Regex.Replace(sanitized, @"[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]", string.Empty);

        // Normalize whitespace
        sanitized = Regex.Replace(sanitized, @"\s+", " ").Trim();

        return sanitized;
    }

    private string MaskSensitiveInformation(string output)
    {
        var masked = output;

        // Mask email addresses
        masked = Regex.Replace(masked, @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b", 
            "[EMAIL_MASKED]");

        // Mask potential API keys
        masked = Regex.Replace(masked, @"\b(?:sk-[a-zA-Z0-9]{48}|pk_[a-zA-Z0-9]+)\b", 
            "[API_KEY_MASKED]");

        // Mask potential passwords in output
        masked = Regex.Replace(masked, @"password\s*[:=]\s*\S+", 
            "password: [MASKED]", RegexOptions.IgnoreCase);

        return masked;
    }

    private string GetPatternDescription(Regex pattern)
    {
        var patternStr = pattern.ToString();
        
        return patternStr switch
        {
            var p when p.Contains("script") => "Script injection attempt",
            var p when p.Contains("javascript") => "JavaScript execution attempt",
            var p when p.Contains("eval") => "Code evaluation attempt", 
            var p when p.Contains("cmd.exe") => "Command execution attempt",
            var p when p.Contains("DROP|DELETE") => "SQL injection attempt",
            var p when p.Contains("union") => "SQL union injection attempt",
            var p when p.Contains("2e.*2f") => "Directory traversal attempt",
            _ => "Suspicious pattern"
        };
    }

    private string GetSensitivePatternDescription(Regex pattern)
    {
        var patternStr = pattern.ToString();
        
        return patternStr switch
        {
            var p when p.Contains("@") => "Email address",
            var p when p.Contains("\\d{3}-?\\d{2}") => "Social Security Number",
            var p when p.Contains("\\d{4}[- ]?") => "Credit card number",
            var p when p.Contains("sk-|pk_") => "API key",
            var p when p.Contains("password") => "Password",
            var p when p.Contains("token") => "Authentication token",
            var p when p.Contains("secret") => "Secret key",
            _ => "Sensitive data"
        };
    }

    private List<string> GetRecentSecurityEvents()
    {
        var events = new List<string>();
        
        var now = DateTime.UtcNow;
        var recentThreshold = now.AddHours(-24);

        // Get recent rate limit violations
        var recentRateLimit = _rateLimitTracker.Count(kvp => kvp.Value > recentThreshold);
        if (recentRateLimit > 0)
        {
            events.Add($"Rate limit violations in last 24h: {recentRateLimit}");
        }

        // Get recent lockouts
        var currentLockouts = _failedAttempts.Count(kvp => kvp.Value >= MaxFailedAttempts);
        if (currentLockouts > 0)
        {
            events.Add($"Currently locked out users: {currentLockouts}");
        }

        return events;
    }
}

/// <summary>
/// Security report for monitoring and auditing
/// </summary>
public class SecurityReport
{
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int LockedUsers { get; set; }
    public int TotalFailedAttempts { get; set; }
    public List<string> SecurityEvents { get; set; } = new();
    public Dictionary<string, object> AdditionalMetrics { get; set; } = new();
}