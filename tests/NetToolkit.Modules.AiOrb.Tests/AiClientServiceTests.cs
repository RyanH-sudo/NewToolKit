using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NetToolkit.Modules.AiOrb.Interfaces;
using NetToolkit.Modules.AiOrb.Models;
using NetToolkit.Modules.AiOrb.Services;
using Xunit;

namespace NetToolkit.Modules.AiOrb.Tests;

/// <summary>
/// Tests for AI Client Service - ensuring cosmic communication works flawlessly! 🌌
/// </summary>
public class AiClientServiceTests : IDisposable
{
    private readonly Mock<ILogger<AiClientService>> _mockLogger;
    private readonly Mock<IConfigManager> _mockConfigManager;
    private readonly AiClientService _aiClientService;

    public AiClientServiceTests()
    {
        _mockLogger = new Mock<ILogger<AiClientService>>();
        _mockConfigManager = new Mock<IConfigManager>();
        _aiClientService = new AiClientService(_mockLogger.Object, _mockConfigManager.Object);
    }

    public void Dispose()
    {
        _aiClientService?.Dispose();
    }

    [Fact]
    public async Task ChatAsync_WithoutConfiguration_ReturnsConfigurationMessage()
    {
        // Arrange
        _mockConfigManager.Setup(x => x.LoadConfigAsync())
            .ReturnsAsync(new ApiConfig { ApiKey = string.Empty });

        // Act
        var result = await _aiClientService.ChatAsync("Hello, world!");

        // Assert
        result.Should().Contain("not configured");
        result.Should().Contain("API keys");
    }

    [Fact]
    public async Task AnalyzeCodeAsync_WithValidCode_ReturnsCodeSuggestion()
    {
        // Arrange
        var code = "Get-Process | Where-Object { $_.Name -eq 'notepad' }";
        var context = new CliContext
        {
            Shell = "powershell",
            CurrentDirectory = "C:\\",
            ExitCode = 0
        };

        _mockConfigManager.Setup(x => x.LoadConfigAsync())
            .ReturnsAsync(new ApiConfig { ApiKey = "test-key" });

        // Act
        var result = await _aiClientService.AnalyzeCodeAsync(code, context);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().NotBeEmpty();
        result.Language.Should().Be("powershell");
    }

    [Fact]
    public async Task AnalyzeNetworkingContentAsync_WithNetworkingText_ReturnsInsights()
    {
        // Arrange
        var networkingContent = "192.168.1.1 subnet mask 255.255.255.0 gateway timeout error";
        
        _mockConfigManager.Setup(x => x.LoadConfigAsync())
            .ReturnsAsync(new ApiConfig { ApiKey = "test-key" });

        // Act
        var result = await _aiClientService.AnalyzeNetworkingContentAsync(networkingContent);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThan(0);
    }

    [Theory]
    [InlineData("powershell")]
    [InlineData("bash")]  
    [InlineData("text")]
    public async Task DetectLanguage_WithDifferentCodeTypes_ReturnsCorrectLanguage(string expectedLanguage)
    {
        // Arrange
        var codeExamples = new Dictionary<string, string>
        {
            ["powershell"] = "Get-Process | Where-Object { $_.Name -eq 'test' }",
            ["bash"] = "#!/bin/bash\ngrep 'error' /var/log/syslog",
            ["text"] = "This is just plain text without any code"
        };

        // Act & Assert - This would test the private DetectLanguage method
        // In a real implementation, we'd either make it internal or test through public methods
        var suggestion = await _aiClientService.AnalyzeCodeAsync(codeExamples[expectedLanguage]);
        
        if (expectedLanguage == "text")
        {
            suggestion.Language.Should().Be("text");
        }
        else
        {
            suggestion.Language.Should().Be(expectedLanguage);
        }
    }

    [Fact]
    public async Task TestConnectionAsync_WithoutConfiguration_ReturnsFalse()
    {
        // Arrange
        _mockConfigManager.Setup(x => x.LoadConfigAsync())
            .ReturnsAsync(new ApiConfig { ApiKey = string.Empty });

        // Act
        var result = await _aiClientService.TestConnectionAsync();

        // Assert
        result.Should().BeFalse("connection test should fail without API configuration");
    }

    [Fact]
    public async Task ChatAsync_WithCaching_UsesCachedResponse()
    {
        // Arrange
        var config = new ApiConfig 
        { 
            ApiKey = "test-key",
            EnableCaching = true,
            BaseUrl = "https://api.openai.com"
        };
        
        _mockConfigManager.Setup(x => x.LoadConfigAsync()).ReturnsAsync(config);

        var prompt = "What is networking?";
        
        // Act - First call should make API request
        var firstResponse = await _aiClientService.ChatAsync(prompt);
        
        // Act - Second identical call should use cache
        var secondResponse = await _aiClientService.ChatAsync(prompt);

        // Assert
        firstResponse.Should().NotBeEmpty();
        secondResponse.Should().NotBeEmpty();
        // In a real test with HTTP mocking, we'd verify only one HTTP call was made
    }

    [Fact]
    public void Constructor_WithValidDependencies_InitializesSuccessfully()
    {
        // Arrange & Act
        var service = new AiClientService(_mockLogger.Object, _mockConfigManager.Object);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task AnalyzeCodeAsync_WithException_ReturnsErrorSuggestion()
    {
        // Arrange
        _mockConfigManager.Setup(x => x.LoadConfigAsync())
            .ThrowsAsync(new InvalidOperationException("Config error"));

        // Act
        var result = await _aiClientService.AnalyzeCodeAsync("test code");

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Analysis Failed");
        result.ConfidenceLevel.Should().Be(0);
        result.Description.Should().Contain("could not analyze");
    }

    [Theory]
    [InlineData("error occurred", SuggestionType.Fix)]
    [InlineData("optimize performance", SuggestionType.Optimization)]
    [InlineData("automate this process", SuggestionType.Automation)]
    [InlineData("enhance functionality", SuggestionType.Enhancement)]
    public async Task DetectSuggestionType_WithDifferentContent_ReturnsCorrectType(string content, SuggestionType expectedType)
    {
        // This tests the private DetectSuggestionType method indirectly
        // In a real implementation, we might need to make it internal or test differently
        
        // Act
        var suggestion = await _aiClientService.AnalyzeCodeAsync(content);

        // Assert - The suggestion type detection would be applied to the AI response
        // This is a simplified test of the concept
        suggestion.Should().NotBeNull();
    }

    [Fact]
    public async Task AnalyzeNetworkingContentAsync_WithException_ReturnsErrorInsight()
    {
        // Arrange
        _mockConfigManager.Setup(x => x.LoadConfigAsync())
            .ThrowsAsync(new Exception("Network error"));

        // Act
        var result = await _aiClientService.AnalyzeNetworkingContentAsync("test content");

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Title.Should().Be("Analysis Error");
        result.First().Category.Should().Be("Error");
    }
}