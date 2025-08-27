using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NetToolkit.Modules.AiOrb.Models;
using NetToolkit.Modules.AiOrb.Services;
using Xunit;

namespace NetToolkit.Modules.AiOrb.Tests;

/// <summary>
/// Comprehensive tests for ConfigManager - ensuring orb wisdom is properly secured! 🔐
/// </summary>
public class ConfigManagerTests : IDisposable
{
    private readonly ConfigManager _configManager;
    private readonly Mock<ILogger<ConfigManager>> _mockLogger;
    private readonly string _testConfigDirectory;

    public ConfigManagerTests()
    {
        _mockLogger = new Mock<ILogger<ConfigManager>>();
        _configManager = new ConfigManager(_mockLogger.Object);
        
        // Create unique test directory to avoid conflicts
        _testConfigDirectory = Path.Combine(Path.GetTempPath(), $"NetToolkit.Tests.{Guid.NewGuid()}");
        Directory.CreateDirectory(_testConfigDirectory);
    }

    public void Dispose()
    {
        // Cleanup test directory
        if (Directory.Exists(_testConfigDirectory))
        {
            Directory.Delete(_testConfigDirectory, true);
        }
    }

    [Fact]
    public async Task LoadConfigAsync_WhenNoConfigExists_ReturnsDefaultConfig()
    {
        // Act
        var config = await _configManager.LoadConfigAsync();

        // Assert
        config.Should().NotBeNull();
        config.Provider.Should().Be("OpenAI");
        config.ApiKey.Should().BeEmpty();
        config.BaseUrl.Should().Be("https://api.openai.com");
        config.Model.Should().Be("gpt-4");
    }

    [Fact]
    public async Task SaveConfigAsync_WithValidConfig_SavesSuccessfully()
    {
        // Arrange
        var config = new ApiConfig
        {
            Provider = "OpenAI",
            ApiKey = "test-key-12345",
            BaseUrl = "https://api.openai.com",
            Model = "gpt-4",
            MaxTokens = 2000,
            Temperature = 0.7
        };

        // Act
        var result = await _configManager.SaveConfigAsync(config);

        // Assert
        result.Should().BeTrue("valid configuration should save successfully");
    }

    [Fact]
    public async Task ValidateConfigAsync_WithEmptyApiKey_ReturnsInvalidResult()
    {
        // Arrange
        var config = new ApiConfig
        {
            ApiKey = string.Empty,
            BaseUrl = "https://api.openai.com",
            Model = "gpt-4"
        };

        // Act
        var result = await _configManager.ValidateConfigAsync(config);

        // Assert
        result.IsValid.Should().BeFalse("empty API key should be invalid");
        result.Errors.Should().Contain(e => e.Contains("API key is required"));
    }

    [Fact]
    public async Task ValidateConfigAsync_WithInvalidBaseUrl_ReturnsInvalidResult()
    {
        // Arrange
        var config = new ApiConfig
        {
            ApiKey = "valid-key",
            BaseUrl = "not-a-valid-url",
            Model = "gpt-4"
        };

        // Act
        var result = await _configManager.ValidateConfigAsync(config);

        // Assert
        result.IsValid.Should().BeFalse("invalid base URL should be invalid");
        result.Errors.Should().Contain(e => e.Contains("Base URL format is invalid"));
    }

    [Fact]
    public async Task ValidateConfigAsync_WithHttpUrl_ReturnsWarning()
    {
        // Arrange
        var config = new ApiConfig
        {
            ApiKey = "valid-key",
            BaseUrl = "http://insecure-api.com",
            Model = "gpt-4"
        };

        // Act
        var result = await _configManager.ValidateConfigAsync(config);

        // Assert
        result.Warnings.Should().Contain(w => w.Contains("Non-HTTPS URL"));
    }

    [Fact]
    public async Task GetDefaultConfig_ReturnsValidDefaultConfiguration()
    {
        // Act
        var defaultConfig = _configManager.GetDefaultConfig();

        // Assert
        defaultConfig.Should().NotBeNull();
        defaultConfig.Provider.Should().Be("OpenAI");
        defaultConfig.BaseUrl.Should().Be("https://api.openai.com");
        defaultConfig.Model.Should().Be("gpt-4");
        defaultConfig.MaxTokens.Should().Be(2000);
        defaultConfig.Temperature.Should().Be(0.7);
        defaultConfig.EnableCaching.Should().BeTrue();
        defaultConfig.CustomHeaders.Should().ContainKey("User-Agent");
    }

    [Fact]
    public async Task IsConfiguredAsync_WithoutSavedConfig_ReturnsFalse()
    {
        // Act
        var isConfigured = await _configManager.IsConfiguredAsync();

        // Assert
        isConfigured.Should().BeFalse("no saved configuration should return false");
    }

    [Fact]
    public async Task ResetConfigAsync_RemovesExistingConfiguration()
    {
        // Arrange - First save a config
        var config = new ApiConfig { ApiKey = "test-key", BaseUrl = "https://api.openai.com", Model = "gpt-4" };
        await _configManager.SaveConfigAsync(config);

        // Act
        var resetResult = await _configManager.ResetConfigAsync();

        // Assert
        resetResult.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("short")]
    [InlineData("a")]
    public async Task ValidateConfigAsync_WithShortApiKey_ReturnsWarning(string shortKey)
    {
        // Arrange
        var config = new ApiConfig
        {
            ApiKey = shortKey,
            BaseUrl = "https://api.openai.com",
            Model = "gpt-4"
        };

        // Act
        var result = await _configManager.ValidateConfigAsync(config);

        // Assert
        if (string.IsNullOrEmpty(shortKey))
        {
            result.Errors.Should().Contain(e => e.Contains("API key is required"));
        }
        else
        {
            result.Warnings.Should().Contain(w => w.Contains("suspiciously short"));
        }
    }

    [Fact]
    public async Task ValidateConfigAsync_WithValidConfiguration_ReturnsValidResult()
    {
        // Arrange
        var config = new ApiConfig
        {
            ApiKey = "sk-1234567890abcdef",
            BaseUrl = "https://api.openai.com",
            Model = "gpt-4",
            MaxTokens = 2000,
            Temperature = 0.7,
            TimeoutSeconds = 30,
            RateLimitPerMinute = 60
        };

        // Act
        var result = await _configManager.ValidateConfigAsync(config);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0, "Max tokens should be between")]
    [InlineData(-100, "Max tokens should be between")]
    [InlineData(200000, "Max tokens should be between")]
    public async Task ValidateConfigAsync_WithInvalidMaxTokens_ReturnsWarning(int maxTokens, string expectedWarning)
    {
        // Arrange
        var config = new ApiConfig
        {
            ApiKey = "valid-key",
            BaseUrl = "https://api.openai.com", 
            Model = "gpt-4",
            MaxTokens = maxTokens
        };

        // Act
        var result = await _configManager.ValidateConfigAsync(config);

        // Assert
        result.Warnings.Should().Contain(w => w.Contains(expectedWarning));
    }

    [Theory]
    [InlineData(-0.1, "Temperature should be between")]
    [InlineData(3.0, "Temperature should be between")]
    public async Task ValidateConfigAsync_WithInvalidTemperature_ReturnsWarning(double temperature, string expectedWarning)
    {
        // Arrange
        var config = new ApiConfig
        {
            ApiKey = "valid-key",
            BaseUrl = "https://api.openai.com",
            Model = "gpt-4", 
            Temperature = temperature
        };

        // Act
        var result = await _configManager.ValidateConfigAsync(config);

        // Assert
        result.Warnings.Should().Contain(w => w.Contains(expectedWarning));
    }
}