using Microsoft.Extensions.Logging;
using NetToolkit.Modules.AiOrb.Interfaces;
using NetToolkit.Modules.AiOrb.Models;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Net.Http;

namespace NetToolkit.Modules.AiOrb.Services;

/// <summary>
/// Secure configuration manager for AI Orb settings and API keys
/// Handles encrypted storage and validation of sensitive configuration data
/// </summary>
public class ConfigManager : IConfigManager
{
    private readonly ILogger<ConfigManager> _logger;
    private readonly string _configDirectory;
    private readonly string _configFilePath;
    private readonly string _encryptionScope = "NetToolkit.AiOrb.Config";

    public ConfigManager(ILogger<ConfigManager> logger)
    {
        _logger = logger;
        _configDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NetToolkit", "AiOrb");
        _configFilePath = Path.Combine(_configDirectory, "config.encrypted");
        
        // Ensure config directory exists
        Directory.CreateDirectory(_configDirectory);
        
        _logger.LogInformation("Configuration manager initialized - Orb wisdom secured! 🔐");
    }

    /// <summary>
    /// Load the current AI orb configuration from secure storage
    /// </summary>
    public async Task<ApiConfig> LoadConfigAsync()
    {
        try
        {
            if (!File.Exists(_configFilePath))
            {
                _logger.LogInformation("No existing configuration found - providing cosmic defaults! ✨");
                return GetDefaultConfig();
            }

            var encryptedData = await File.ReadAllBytesAsync(_configFilePath);
            var decryptedData = ProtectedData.Unprotect(encryptedData, null, DataProtectionScope.CurrentUser);
            var jsonString = Encoding.UTF8.GetString(decryptedData);
            
            var config = JsonConvert.DeserializeObject<ApiConfig>(jsonString) ?? GetDefaultConfig();
            
            _logger.LogInformation("Configuration loaded successfully - Orb consciousness restored! 🧠");
            return config;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load configuration - Orb temporarily confused! 😵‍💫");
            return GetDefaultConfig();
        }
    }

    /// <summary>
    /// Save API configuration to secure encrypted storage
    /// </summary>
    public async Task<bool> SaveConfigAsync(ApiConfig config)
    {
        try
        {
            // Validate configuration before saving
            var validationResult = await ValidateConfigAsync(config);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Configuration validation failed - Orb refuses invalid wisdom! ❌ Errors: {Errors}", 
                    string.Join(", ", validationResult.Errors));
                return false;
            }

            var jsonString = JsonConvert.SerializeObject(config, Formatting.Indented);
            var dataToEncrypt = Encoding.UTF8.GetBytes(jsonString);
            var encryptedData = ProtectedData.Protect(dataToEncrypt, null, DataProtectionScope.CurrentUser);
            
            await File.WriteAllBytesAsync(_configFilePath, encryptedData);
            
            _logger.LogInformation("Configuration saved securely - Orb wisdom preserved! 💾");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save configuration - Orb memory malfunction! 🚨");
            return false;
        }
    }

    /// <summary>
    /// Validate API configuration for completeness and correctness
    /// </summary>
    public async Task<ConfigValidationResult> ValidateConfigAsync(ApiConfig config)
    {
        var result = new ConfigValidationResult { IsValid = true };
        
        try
        {
            // Validate API key
            if (string.IsNullOrWhiteSpace(config.ApiKey))
            {
                result.Errors.Add("API key is required for orb enlightenment");
                result.IsValid = false;
            }
            else if (config.ApiKey.Length < 10)
            {
                result.Warnings.Add("API key seems suspiciously short - are you sure it's correct?");
            }

            // Validate base URL
            if (string.IsNullOrWhiteSpace(config.BaseUrl))
            {
                result.Errors.Add("Base URL is required for cosmic communication");
                result.IsValid = false;
            }
            else if (!Uri.TryCreate(config.BaseUrl, UriKind.Absolute, out var uri))
            {
                result.Errors.Add("Base URL format is invalid - orb cannot reach the cosmic API");
                result.IsValid = false;
            }
            else if (uri.Scheme != "https")
            {
                result.Warnings.Add("Non-HTTPS URL detected - security spirits are displeased");
            }

            // Validate model name
            if (string.IsNullOrWhiteSpace(config.Model))
            {
                result.Errors.Add("Model name is required for AI consciousness selection");
                result.IsValid = false;
            }

            // Validate numeric ranges
            if (config.MaxTokens <= 0 || config.MaxTokens > 100000)
            {
                result.Warnings.Add("Max tokens should be between 1 and 100,000 for optimal cosmic balance");
            }

            if (config.Temperature < 0 || config.Temperature > 2)
            {
                result.Warnings.Add("Temperature should be between 0 and 2 for stable orb personality");
            }

            if (config.TimeoutSeconds <= 0 || config.TimeoutSeconds > 300)
            {
                result.Warnings.Add("Timeout should be between 1 and 300 seconds for patient wisdom");
            }

            if (config.RateLimitPerMinute <= 0 || config.RateLimitPerMinute > 1000)
            {
                result.Warnings.Add("Rate limit should be between 1 and 1000 requests per minute");
            }

            // Test API connectivity if basic validation passes
            if (result.IsValid)
            {
                var connectivityTest = await TestApiConnectivityAsync(config);
                if (!connectivityTest)
                {
                    result.Warnings.Add("API connectivity test failed - check your cosmic connection");
                }
            }

            // Add recommendations
            if (config.EnableCaching)
            {
                result.Recommendations.Add("Caching enabled - orb will remember recent wisdom for faster responses");
            }
            else
            {
                result.Recommendations.Add("Consider enabling caching for improved performance");
            }

            _logger.LogInformation("Configuration validation completed - Orb wisdom verified! ✅ Valid: {IsValid}", result.IsValid);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Configuration validation failed - Orb confusion detected! 🤔");
            result.IsValid = false;
            result.Errors.Add($"Validation error: {ex.Message}");
            return result;
        }
    }

    /// <summary>
    /// Get default configuration values for first-time setup
    /// </summary>
    public ApiConfig GetDefaultConfig()
    {
        var defaultConfig = new ApiConfig
        {
            Provider = "OpenAI",
            ApiKey = string.Empty,
            BaseUrl = "https://api.openai.com",
            Model = "gpt-4",
            MaxTokens = 2000,
            Temperature = 0.7,
            TimeoutSeconds = 30,
            EnableCaching = true,
            RateLimitPerMinute = 60,
            CustomHeaders = new Dictionary<string, string>
            {
                ["User-Agent"] = "NetToolkit-AiOrb/1.0"
            },
            ProviderSpecificSettings = new Dictionary<string, object>
            {
                ["frequency_penalty"] = 0.0,
                ["presence_penalty"] = 0.0,
                ["top_p"] = 1.0
            }
        };

        _logger.LogDebug("Default configuration generated - Orb ready for cosmic awakening! 🌟");
        return defaultConfig;
    }

    /// <summary>
    /// Check if valid configuration exists and is properly set up
    /// </summary>
    public async Task<bool> IsConfiguredAsync()
    {
        try
        {
            var config = await LoadConfigAsync();
            var validationResult = await ValidateConfigAsync(config);
            
            var isConfigured = validationResult.IsValid && !string.IsNullOrWhiteSpace(config.ApiKey);
            
            _logger.LogInformation("Configuration check completed - Orb readiness: {IsConfigured} 🔍", 
                isConfigured ? "READY" : "NEEDS SETUP");
            
            return isConfigured;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Configuration check failed - Orb status unknown! ❓");
            return false;
        }
    }

    /// <summary>
    /// Reset configuration to defaults (useful for troubleshooting)
    /// </summary>
    public async Task<bool> ResetConfigAsync()
    {
        try
        {
            if (File.Exists(_configFilePath))
            {
                // Backup existing config
                var backupPath = _configFilePath + $".backup.{DateTime.UtcNow:yyyyMMdd-HHmmss}";
                File.Copy(_configFilePath, backupPath);
                File.Delete(_configFilePath);
                
                _logger.LogInformation("Configuration reset completed - Previous orb wisdom backed up to: {BackupPath} 🔄", backupPath);
            }

            var defaultConfig = GetDefaultConfig();
            // Don't save the default config immediately - let user configure it first
            
            _logger.LogInformation("Configuration reset to defaults - Orb awaits new cosmic instructions! 🎭");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Configuration reset failed - Orb memory persistent! 🧲");
            return false;
        }
    }

    /// <summary>
    /// Test API connectivity with given configuration
    /// </summary>
    private async Task<bool> TestApiConnectivityAsync(ApiConfig config)
    {
        try
        {
            using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds) };
            
            // Add authentication header
            if (config.Provider.Equals("OpenAI", StringComparison.OrdinalIgnoreCase))
            {
                httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", config.ApiKey);
            }

            // Add custom headers
            foreach (var header in config.CustomHeaders)
            {
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            // Simple connectivity test - try to reach the API endpoint
            var response = await httpClient.GetAsync($"{config.BaseUrl.TrimEnd('/')}/models");
            
            var isConnected = response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.Unauthorized;
            
            _logger.LogDebug("API connectivity test - Cosmic connection status: {Status} ({StatusCode}) 📡", 
                isConnected ? "CONNECTED" : "FAILED", response.StatusCode);
            
            return isConnected;
        }
        catch (Exception ex)
        {
            _logger.LogDebug("API connectivity test failed - Cosmic signals blocked! 📵 Error: {Error}", ex.Message);
            return false;
        }
    }
}