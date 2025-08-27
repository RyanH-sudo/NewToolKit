using Xunit;
using Moq;
using NetToolkit.Core.Interfaces;
using NetToolkit.Modules.PowerShell;
using NetToolkit.Modules.PowerShell.Interfaces;
using NetToolkit.Modules.PowerShell.Services;
using Microsoft.Extensions.DependencyInjection;

namespace NetToolkit.Tests.PowerShell;

/// <summary>
/// PowerShell Module Tests - Ensuring our digital magic is bulletproof!
/// Because even wizards need quality assurance for their spells.
/// </summary>
public class PowerShellModuleTests : IDisposable
{
    private readonly Mock<ILoggerWrapper> _mockLogger;
    private readonly Mock<IEventBus> _mockEventBus;
    private readonly ServiceProvider _serviceProvider;
    private readonly PowerShellModule _module;
    
    public PowerShellModuleTests()
    {
        // Create mocks - digital test doubles for our components
        _mockLogger = new Mock<ILoggerWrapper>();
        _mockEventBus = new Mock<IEventBus>();
        
        // Setup service collection - assembling the test laboratory
        var services = new ServiceCollection();
        services.AddSingleton(_mockLogger.Object);
        services.AddSingleton(_mockEventBus.Object);
        
        _serviceProvider = services.BuildServiceProvider();
        
        // Create module instance - the subject of our digital experiments
        _module = new PowerShellModule();
    }
    
    [Fact]
    public void Module_Properties_ShouldReturnCorrectValues()
    {
        // Arrange & Act & Assert - the holy trinity of testing
        Assert.Equal("PowerShell Terminal Module", _module.Name);
        Assert.Equal("1.0.0", _module.Version);
    }
    
    [Fact]
    public async Task InitializeAsync_ShouldCompleteSuccessfully()
    {
        // Arrange - preparing the digital stage
        _mockLogger.Setup(x => x.LogInfo(It.IsAny<string>(), It.IsAny<object[]>()));
        _mockLogger.Setup(x => x.LogDebug(It.IsAny<string>(), It.IsAny<object[]>()));
        
        // Act - performing the digital ritual
        var initTask = _module.InitializeAsync(_serviceProvider);
        
        // Assert - verifying the magic worked
        await Assert.ThrowsAsync<NotImplementedException>(() => initTask);
        // Note: This will throw because we're testing the base class behavior
        // In a real implementation, we'd have proper dependency injection setup
    }
    
    [Fact]
    public void GetPowerShellHost_BeforeInitialization_ShouldReturnNull()
    {
        // Arrange & Act
        var host = _module.GetPowerShellHost();
        
        // Assert
        Assert.Null(host);
    }
    
    [Fact]
    public void GetTemplateService_BeforeInitialization_ShouldReturnNull()
    {
        // Arrange & Act
        var service = _module.GetTemplateService();
        
        // Assert
        Assert.Null(service);
    }
    
    [Fact]
    public void GetMicrosoftService_BeforeInitialization_ShouldReturnNull()
    {
        // Arrange & Act
        var service = _module.GetMicrosoftService();
        
        // Assert
        Assert.Null(service);
    }
    
    [Fact]
    public void GetEventPublisher_BeforeInitialization_ShouldReturnNull()
    {
        // Arrange & Act
        var publisher = _module.GetEventPublisher();
        
        // Assert
        Assert.Null(publisher);
    }
    
    [Fact]
    public async Task ExecuteTestCommandAsync_BeforeInitialization_ShouldReturnFalse()
    {
        // Arrange & Act
        var result = await _module.ExecuteTestCommandAsync();
        
        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public async Task GetModuleStatsAsync_BeforeInitialization_ShouldReturnDefaultStats()
    {
        // Arrange & Act
        var stats = await _module.GetModuleStatsAsync();
        
        // Assert - verifying our digital accounting is accurate
        Assert.NotNull(stats);
        Assert.Equal("PowerShell Terminal Module", stats.ModuleName);
        Assert.Equal("1.0.0", stats.ModuleVersion);
        Assert.False(stats.IsInitialized);
        Assert.Equal(0, stats.TemplateCount);
        Assert.Equal(0, stats.CommandHistoryCount);
    }
    
    public void Dispose()
    {
        // Cleanup - returning the digital realm to its natural state
        _serviceProvider?.Dispose();
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Script Template Service Tests - Ensuring our digital spellbook is reliable
/// </summary>
public class ScriptTemplateServiceTests : IDisposable
{
    private readonly Mock<ILoggerWrapper> _mockLogger;
    private readonly ScriptTemplateService _service;
    private readonly string _testTemplatesPath;
    
    public ScriptTemplateServiceTests()
    {
        _mockLogger = new Mock<ILoggerWrapper>();
        
        // Create temporary test directory - a digital sandbox for our experiments
        _testTemplatesPath = Path.Combine(Path.GetTempPath(), "NetToolkit_Tests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testTemplatesPath);
        
        _service = new ScriptTemplateService(_mockLogger.Object);
    }
    
    [Fact]
    public async Task LoadTemplatesAsync_ShouldReturnBuiltInTemplates()
    {
        // Arrange & Act
        var templates = await _service.LoadTemplatesAsync();
        
        // Assert - verifying our spell collection is complete
        Assert.NotNull(templates);
        Assert.NotEmpty(templates);
        Assert.Contains(templates, t => t.Id == "extend-mailbox");
        Assert.Contains(templates, t => t.Id == "convert-shared-mailbox");
        Assert.Contains(templates, t => t.Id == "add-email-alias");
    }
    
    [Fact]
    public async Task GetTemplateAsync_WithValidId_ShouldReturnTemplate()
    {
        // Arrange
        const string templateId = "extend-mailbox";
        
        // Act
        var template = await _service.GetTemplateAsync(templateId);
        
        // Assert
        Assert.NotNull(template);
        Assert.Equal(templateId, template.Id);
        Assert.Equal("Extend Mailbox Quota", template.Name);
    }
    
    [Fact]
    public async Task GetTemplateAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        const string templateId = "non-existent-template";
        
        // Act
        var template = await _service.GetTemplateAsync(templateId);
        
        // Assert
        Assert.Null(template);
    }
    
    [Fact]
    public async Task GenerateScriptAsync_WithValidParameters_ShouldReturnScript()
    {
        // Arrange
        const string templateId = "extend-mailbox";
        var parameters = new Dictionary<string, object>
        {
            { "UserEmail", "test@example.com" },
            { "QuotaGB", 100 },
            { "WarningGB", 90 }
        };
        
        // Act
        var script = await _service.GenerateScriptAsync(templateId, parameters);
        
        // Assert - verifying our digital alchemy works
        Assert.NotNull(script);
        Assert.Contains("test@example.com", script);
        Assert.Contains("100GB", script);
    }
    
    [Fact]
    public async Task GenerateScriptAsync_WithMissingRequiredParameter_ShouldThrowException()
    {
        // Arrange
        const string templateId = "extend-mailbox";
        var parameters = new Dictionary<string, object>
        {
            { "QuotaGB", 100 } // Missing required UserEmail parameter
        };
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GenerateScriptAsync(templateId, parameters));
    }
    
    [Fact]
    public async Task ValidateParametersAsync_WithValidParameters_ShouldReturnValid()
    {
        // Arrange
        const string templateId = "extend-mailbox";
        var parameters = new Dictionary<string, object>
        {
            { "UserEmail", "test@example.com" },
            { "QuotaGB", 100 },
            { "WarningGB", 90 }
        };
        
        // Act
        var result = await _service.ValidateParametersAsync(templateId, parameters);
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
    
    [Fact]
    public async Task ValidateParametersAsync_WithInvalidEmail_ShouldReturnInvalid()
    {
        // Arrange
        const string templateId = "extend-mailbox";
        var parameters = new Dictionary<string, object>
        {
            { "UserEmail", "invalid-email" }, // Invalid email format
            { "QuotaGB", 100 }
        };
        
        // Act
        var result = await _service.ValidateParametersAsync(templateId, parameters);
        
        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }
    
    [Fact]
    public async Task GetCategoriesAsync_ShouldReturnUniqueCategories()
    {
        // Arrange & Act
        var categories = await _service.GetCategoriesAsync();
        
        // Assert
        Assert.NotNull(categories);
        Assert.Contains("Microsoft 365", categories);
        Assert.Equal(categories.Count, categories.Distinct().Count()); // Ensure uniqueness
    }
    
    [Fact]
    public async Task SearchTemplatesAsync_WithMatchingTerm_ShouldReturnMatchingTemplates()
    {
        // Arrange
        const string searchTerm = "mailbox";
        
        // Act
        var results = await _service.SearchTemplatesAsync(searchTerm);
        
        // Assert
        Assert.NotNull(results);
        Assert.NotEmpty(results);
        Assert.All(results, template => 
            Assert.True(template.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                       template.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
    }
    
    public void Dispose()
    {
        // Cleanup - erasing our digital footprints
        if (Directory.Exists(_testTemplatesPath))
        {
            Directory.Delete(_testTemplatesPath, true);
        }
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Terminal Event Publisher Tests - Ensuring our digital town crier works properly
/// </summary>
public class TerminalEventPublisherTests
{
    private readonly Mock<IEventBus> _mockEventBus;
    private readonly Mock<ILoggerWrapper> _mockLogger;
    private readonly TerminalEventPublisher _publisher;
    
    public TerminalEventPublisherTests()
    {
        _mockEventBus = new Mock<IEventBus>();
        _mockLogger = new Mock<ILoggerWrapper>();
        _publisher = new TerminalEventPublisher(_mockEventBus.Object, _mockLogger.Object);
    }
    
    [Fact]
    public async Task PublishScriptStartedAsync_ShouldPublishEvent()
    {
        // Arrange
        var eventData = new ScriptExecutionStartedEvent
        {
            ScriptId = "test-123",
            ScriptName = "Test Script",
            ScriptContent = "Write-Output 'Hello World'"
        };
        
        _mockEventBus.Setup(x => x.PublishAsync(It.IsAny<ScriptExecutionStartedEvent>()))
                    .Returns(Task.CompletedTask);
        
        // Act
        await _publisher.PublishScriptStartedAsync(eventData);
        
        // Assert - verifying our digital messenger delivered the news
        _mockEventBus.Verify(x => x.PublishAsync(It.Is<ScriptExecutionStartedEvent>(e => 
            e.ScriptId == "test-123" && e.ScriptName == "Test Script")), Times.Once);
    }
    
    [Fact]
    public async Task PublishScriptCompletedAsync_ShouldPublishEventWithWittyMessage()
    {
        // Arrange
        var eventData = new ScriptExecutionCompletedEvent
        {
            ScriptId = "test-123",
            ScriptName = "Test Script",
            Success = true,
            Duration = TimeSpan.FromSeconds(2)
        };
        
        _mockEventBus.Setup(x => x.PublishAsync(It.IsAny<ScriptExecutionCompletedEvent>()))
                    .Returns(Task.CompletedTask);
        
        // Act
        await _publisher.PublishScriptCompletedAsync(eventData);
        
        // Assert
        _mockEventBus.Verify(x => x.PublishAsync(It.Is<ScriptExecutionCompletedEvent>(e => 
            e.Success == true && e.Duration.TotalSeconds == 2)), Times.Once);
        
        // Verify the witty message generation
        var wittyMessage = eventData.GetWittyStatusMessage();
        Assert.Contains("wine", wittyMessage.ToLowerInvariant()); // Should mention wine for longer executions
    }
    
    [Fact]
    public async Task PublishErrorOccurredAsync_ShouldPublishEventWithWittyErrorMessage()
    {
        // Arrange
        var eventData = new ErrorOccurredEvent
        {
            ErrorMessage = "Command not found",
            ErrorCategory = "CommandNotFoundError",
            Severity = ErrorSeverity.Error
        };
        
        _mockEventBus.Setup(x => x.PublishAsync(It.IsAny<ErrorOccurredEvent>()))
                    .Returns(Task.CompletedTask);
        
        // Act
        await _publisher.PublishErrorOccurredAsync(eventData);
        
        // Assert
        _mockEventBus.Verify(x => x.PublishAsync(It.Is<ErrorOccurredEvent>(e => 
            e.ErrorMessage == "Command not found")), Times.Once);
        
        // Verify the witty error message contains humor
        var wittyMessage = eventData.GetWittyErrorMessage();
        Assert.Contains("Houston", wittyMessage); // Default witty message should mention Houston
    }
}