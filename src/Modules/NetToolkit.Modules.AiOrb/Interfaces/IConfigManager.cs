using NetToolkit.Modules.AiOrb.Models;

namespace NetToolkit.Modules.AiOrb.Interfaces;

/// <summary>
/// Configuration manager for AI Orb settings and API keys
/// Handles secure storage and retrieval of sensitive configuration data
/// </summary>
public interface IConfigManager
{
    /// <summary>
    /// Load the current AI orb configuration from secure storage
    /// </summary>
    /// <returns>Current configuration or default values if none exists</returns>
    Task<ApiConfig> LoadConfigAsync();

    /// <summary>
    /// Save API configuration to secure encrypted storage
    /// </summary>
    /// <param name="config">Configuration to save securely</param>
    /// <returns>Success status of the save operation</returns>
    Task<bool> SaveConfigAsync(ApiConfig config);

    /// <summary>
    /// Validate API configuration for completeness and correctness
    /// </summary>
    /// <param name="config">Configuration to validate</param>
    /// <returns>Validation result with any issues found</returns>
    Task<ConfigValidationResult> ValidateConfigAsync(ApiConfig config);

    /// <summary>
    /// Get default configuration values for first-time setup
    /// </summary>
    /// <returns>Default configuration template</returns>
    ApiConfig GetDefaultConfig();

    /// <summary>
    /// Check if valid configuration exists and is properly set up
    /// </summary>
    /// <returns>True if configuration is ready for use</returns>
    Task<bool> IsConfiguredAsync();

    /// <summary>
    /// Reset configuration to defaults (useful for troubleshooting)
    /// </summary>
    /// <returns>Success status of reset operation</returns>
    Task<bool> ResetConfigAsync();
}