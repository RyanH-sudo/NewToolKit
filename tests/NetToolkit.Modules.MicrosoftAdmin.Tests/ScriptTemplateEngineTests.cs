using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using NetToolkit.Modules.MicrosoftAdmin.Services;
using NetToolkit.Modules.MicrosoftAdmin.Models;

namespace NetToolkit.Modules.MicrosoftAdmin.Tests;

/// <summary>
/// Test suite for Script Template Engine - validating the alchemy of admin automation
/// These tests ensure that PowerShell spells are crafted with precision and wisdom
/// </summary>
public class ScriptTemplateEngineTests : IDisposable
{
    private readonly Mock<ILogger<ScriptTemplateEngine>> _mockLogger;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly ScriptTemplateEngine _engine;
    private readonly string _testTemplatesPath;

    public ScriptTemplateEngineTests()
    {
        _mockLogger = new Mock<ILogger<ScriptTemplateEngine>>();
        _mockConfiguration = new Mock<IConfiguration>();
        
        // Setup test templates path
        _testTemplatesPath = Path.Combine(Path.GetTempPath(), "NetToolkit_Tests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testTemplatesPath);
        
        _mockConfiguration.Setup(c => c["MicrosoftAdmin:TemplatesPath"])
            .Returns(_testTemplatesPath);

        _engine = new ScriptTemplateEngine(_mockLogger.Object, _mockConfiguration.Object);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task LoadTemplatesAsync_WithBuiltInTemplates_ReturnsExpectedTemplates()
    {
        // Act - load the administrative arsenal
        var templates = await _engine.LoadTemplatesAsync();

        // Assert - verify spell collection
        Assert.NotEmpty(templates);
        
        // Check for essential built-in templates
        var essentialTemplates = new[] { "extend-mailbox", "create-shared-mailbox", "add-email-alias", "reset-user-password" };
        foreach (var essentialTemplate in essentialTemplates)
        {
            Assert.Contains(templates, t => t.Id == essentialTemplate);
        }
        
        // Verify template properties
        var mailboxTemplate = templates.FirstOrDefault(t => t.Id == "extend-mailbox");
        Assert.NotNull(mailboxTemplate);
        Assert.Equal("Extend Mailbox Quota", mailboxTemplate.Name);
        Assert.Equal("Mailbox Management", mailboxTemplate.Category);
        Assert.False(mailboxTemplate.IsCustomTemplate);
        Assert.NotEmpty(mailboxTemplate.Parameters);
        Assert.Contains(mailboxTemplate.Parameters, p => p.Name == "email" && p.Type == ParameterType.Email);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task LoadTemplatesAsync_WithCustomTemplates_IncludesCustomTemplates()
    {
        // Arrange - create custom template file
        var customTemplatesDir = Path.Combine(_testTemplatesPath, "Custom");
        Directory.CreateDirectory(customTemplatesDir);
        
        var customTemplate = AdminTestHelpers.CreateTestTemplate("custom-test", "Custom Test Template", "Custom Category");
        customTemplate.IsCustomTemplate = true;
        
        var templateJson = System.Text.Json.JsonSerializer.Serialize(customTemplate, new System.Text.Json.JsonSerializerOptions
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });
        
        await File.WriteAllTextAsync(Path.Combine(customTemplatesDir, "custom-test.json"), templateJson);

        // Act - load templates including custom ones
        var templates = await _engine.LoadTemplatesAsync();

        // Assert - verify custom template inclusion
        Assert.Contains(templates, t => t.Id == "custom-test" && t.IsCustomTemplate);
        
        var customLoadedTemplate = templates.FirstOrDefault(t => t.Id == "custom-test");
        Assert.NotNull(customLoadedTemplate);
        Assert.Equal("Custom Test Template", customLoadedTemplate.Name);
        Assert.True(customLoadedTemplate.IsCustomTemplate);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task BuildScriptAsync_WithValidTemplate_ReturnsProcessedScript()
    {
        // Arrange - prepare template and parameters for script alchemy
        var template = new AdminTemplate
        {
            Id = "test-build",
            Name = "Test Build Template",
            ScriptTemplate = @"
# Test PowerShell script
Set-Mailbox -Identity '{email}' -ProhibitSendQuota '{quota}GB'
Write-Output ""Mailbox for {email} expanded to {quota}GB""",
            Parameters = new List<TemplateParameter>
            {
                new() { Name = "email", Type = ParameterType.Email },
                new() { Name = "quota", Type = ParameterType.Integer }
            }
        };

        var parameters = new Dictionary<string, object>
        {
            ["email"] = "test@example.com",
            ["quota"] = 25
        };

        // Act - weave the PowerShell spell
        var builtScript = await _engine.BuildScriptAsync(template, parameters);

        // Assert - verify script transmutation
        Assert.NotEmpty(builtScript);
        Assert.Contains("'test@example.com'", builtScript); // Email should be properly quoted
        Assert.Contains("25", builtScript); // Quota should be substituted
        Assert.Contains("Set-Mailbox", builtScript);
        Assert.Contains("NetToolkit Admin Script", builtScript); // Should include wrapper
        Assert.Contains("try {", builtScript); // Should have error handling
        Assert.Contains("catch {", builtScript);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task BuildScriptAsync_WithBooleanParameter_FormatsCorrectly()
    {
        // Arrange - test boolean parameter formatting
        var template = new AdminTemplate
        {
            ScriptTemplate = "Set-User -ForceChangePassword {forceChange}",
            Parameters = new List<TemplateParameter>
            {
                new() { Name = "forceChange", Type = ParameterType.Boolean }
            }
        };

        var parametersTrue = new Dictionary<string, object>
        {
            ["forceChange"] = true
        };
        
        var parametersFalse = new Dictionary<string, object>
        {
            ["forceChange"] = false
        };

        // Act - build scripts with different boolean values
        var scriptTrue = await _engine.BuildScriptAsync(template, parametersTrue);
        var scriptFalse = await _engine.BuildScriptAsync(template, parametersFalse);

        // Assert - verify PowerShell boolean formatting
        Assert.Contains("$true", scriptTrue);
        Assert.Contains("$false", scriptFalse);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task BuildScriptAsync_WithStringParameter_EscapesSingleQuotes()
    {
        // Arrange - test string escaping for security
        var template = new AdminTemplate
        {
            ScriptTemplate = "Write-Output 'Message: {message}'",
            Parameters = new List<TemplateParameter>
            {
                new() { Name = "message", Type = ParameterType.String }
            }
        };

        var parameters = new Dictionary<string, object>
        {
            ["message"] = "Don't break the script!"
        };

        // Act - build script with potentially problematic string
        var builtScript = await _engine.BuildScriptAsync(template, parameters);

        // Assert - verify proper escaping
        Assert.Contains("Don''t break", builtScript); // Single quotes should be escaped
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task ValidateTemplateAsync_WithValidTemplate_ReturnsValidResult()
    {
        // Arrange - prepare well-formed template
        var validTemplate = new AdminTemplate
        {
            Id = "valid-test",
            Name = "Valid Test Template",
            ScriptTemplate = @"
Write-Output ""Starting admin operation""
Set-Mailbox -Identity '{email}' -ProhibitSendQuota '{quota}GB'
Write-Output ""Operation completed successfully""",
            Parameters = new List<TemplateParameter>
            {
                new() 
                { 
                    Name = "email", 
                    Type = ParameterType.Email,
                    Required = true,
                    Description = "User email address"
                },
                new() 
                { 
                    Name = "quota", 
                    Type = ParameterType.Integer,
                    Required = true,
                    MinValue = 1,
                    MaxValue = 100,
                    Description = "Mailbox quota in GB"
                }
            },
            RequiredScopes = new List<string> { "Mail.ReadWrite" }
        };

        // Act - validate the administrative wisdom
        var result = await _engine.ValidateTemplateAsync(validTemplate);

        // Assert - verify validation success
        Assert.True(result.IsValid);
        Assert.Empty(result.Messages.Where(m => m.Severity == ValidationSeverity.Error));
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("", "Template ID is required")]
    [InlineData(null, "Template name is required")]
    [InlineData("valid-id", "Script content is required")]
    public async Task ValidateTemplateAsync_WithMissingRequiredFields_ReturnsErrors(
        string? templateId, string expectedErrorSubstring)
    {
        // Arrange - prepare incomplete template
        var incompleteTemplate = new AdminTemplate
        {
            Id = templateId ?? string.Empty,
            Name = templateId == "" ? string.Empty : "Valid Name",
            ScriptTemplate = templateId == "valid-id" ? string.Empty : "Valid Script",
            Parameters = new List<TemplateParameter>()
        };

        // Act - validate incomplete template
        var result = await _engine.ValidateTemplateAsync(incompleteTemplate);

        // Assert - verify validation catches omissions
        Assert.False(result.IsValid);
        Assert.Contains(result.Messages, m => 
            m.Severity == ValidationSeverity.Error && 
            m.Message.Contains(expectedErrorSubstring, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task ValidateTemplateAsync_WithUndefinedParameterPlaceholder_ReturnsError()
    {
        // Arrange - template with undefined parameter reference
        var templateWithUndefinedParam = new AdminTemplate
        {
            Id = "undefined-param-test",
            Name = "Undefined Parameter Test",
            ScriptTemplate = "Set-Mailbox -Identity '{email}' -Quota '{undefinedParam}GB'", // undefinedParam not in parameters
            Parameters = new List<TemplateParameter>
            {
                new() { Name = "email", Type = ParameterType.Email }
                // Missing undefinedParam definition
            }
        };

        // Act - validate template with undefined reference
        var result = await _engine.ValidateTemplateAsync(templateWithUndefinedParam);

        // Assert - verify undefined parameter detection
        Assert.False(result.IsValid);
        Assert.Contains(result.Messages, m => 
            m.Severity == ValidationSeverity.Error &&
            m.Message.Contains("undefined parameter 'undefinedParam'", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task ValidateTemplateAsync_WithUnusedParameter_ReturnsWarning()
    {
        // Arrange - template with unused parameter
        var templateWithUnusedParam = new AdminTemplate
        {
            Id = "unused-param-test",
            Name = "Unused Parameter Test",
            ScriptTemplate = "Set-Mailbox -Identity '{email}' -Quota '25GB'", // unusedParam not referenced
            Parameters = new List<TemplateParameter>
            {
                new() { Name = "email", Type = ParameterType.Email },
                new() { Name = "unusedParam", Type = ParameterType.String } // Not used in script
            }
        };

        // Act - validate template with unused parameter
        var result = await _engine.ValidateTemplateAsync(templateWithUnusedParam);

        // Assert - verify unused parameter warning
        Assert.Contains(result.Messages, m => 
            m.Severity == ValidationSeverity.Warning &&
            m.Message.Contains("unusedParam") &&
            m.Message.Contains("not used", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task ValidateTemplateAsync_WithInvalidPowerShellSyntax_ReturnsError()
    {
        // Arrange - template with malformed PowerShell
        var templateWithBadSyntax = new AdminTemplate
        {
            Id = "bad-syntax-test",
            Name = "Bad Syntax Test",
            ScriptTemplate = @"
Set-Mailbox -Identity '{email}' -Quota '{quota}GB'
if ($true { # Missing closing parenthesis
    Write-Output 'This will cause a syntax error'
}", // Malformed PowerShell syntax
            Parameters = new List<TemplateParameter>
            {
                new() { Name = "email", Type = ParameterType.Email },
                new() { Name = "quota", Type = ParameterType.Integer }
            }
        };

        // Act - validate template with syntax error
        var result = await _engine.ValidateTemplateAsync(templateWithBadSyntax);

        // Assert - verify syntax error detection
        Assert.Contains(result.Messages, m => 
            m.Severity == ValidationSeverity.Error &&
            m.Code == "SYNTAX_ERROR");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GenerateFormControlsAsync_WithVariousParameterTypes_CreatesCorrectControls()
    {
        // Arrange - template with diverse parameter types
        var template = new AdminTemplate
        {
            Parameters = new List<TemplateParameter>
            {
                new() 
                { 
                    Name = "email", 
                    DisplayName = "Email Address",
                    Type = ParameterType.Email,
                    Required = true,
                    Description = "User email address"
                },
                new() 
                { 
                    Name = "quota", 
                    DisplayName = "Storage Quota",
                    Type = ParameterType.Integer,
                    MinValue = 1,
                    MaxValue = 100,
                    DefaultValue = 25
                },
                new() 
                { 
                    Name = "enabled", 
                    DisplayName = "Account Enabled",
                    Type = ParameterType.Boolean,
                    DefaultValue = true
                },
                new() 
                { 
                    Name = "description", 
                    DisplayName = "Description",
                    Type = ParameterType.MultilineText,
                    MaxLength = 500
                }
            }
        };

        // Act - generate form controls from parameters
        var controls = await _engine.GenerateFormControlsAsync(template);

        // Assert - verify control generation
        Assert.Equal(4, controls.Count);

        // Check email control
        var emailControl = controls.FirstOrDefault(c => c.Name == "email");
        Assert.NotNull(emailControl);
        Assert.Equal(UiControlType.TextBox, emailControl.Type);
        Assert.True(emailControl.Required);
        Assert.Equal("Email Address", emailControl.DisplayName);

        // Check integer control
        var quotaControl = controls.FirstOrDefault(c => c.Name == "quota");
        Assert.NotNull(quotaControl);
        Assert.Equal(UiControlType.NumericUpDown, quotaControl.Type);
        Assert.Equal(25, quotaControl.Value);

        // Check boolean control
        var enabledControl = controls.FirstOrDefault(c => c.Name == "enabled");
        Assert.NotNull(enabledControl);
        Assert.Equal(UiControlType.CheckBox, enabledControl.Type);
        Assert.Equal(true, enabledControl.Value);

        // Check multiline text control
        var descriptionControl = controls.FirstOrDefault(c => c.Name == "description");
        Assert.NotNull(descriptionControl);
        Assert.Equal(UiControlType.MultilineTextBox, descriptionControl.Type);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task SaveCustomTemplateAsync_WithValidTemplate_SavesSuccessfully()
    {
        // Arrange - prepare custom template for preservation
        var customTemplate = AdminTestHelpers.CreateTestTemplate("save-test", "Save Test Template", "Custom");
        customTemplate.IsCustomTemplate = true;

        // Act - save the custom spell
        var (success, templateId) = await _engine.SaveCustomTemplateAsync(customTemplate);

        // Assert - verify successful preservation
        Assert.True(success);
        Assert.NotEmpty(templateId);
        Assert.Equal("save-test", templateId);

        // Verify file was created
        var expectedPath = Path.Combine(_testTemplatesPath, "Custom", $"{templateId}.json");
        Assert.True(File.Exists(expectedPath));

        // Verify content
        var savedContent = await File.ReadAllTextAsync(expectedPath);
        Assert.Contains("Save Test Template", savedContent);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task SaveCustomTemplateAsync_WithInvalidTemplate_ReturnsFalse()
    {
        // Arrange - prepare invalid template
        var invalidTemplate = new AdminTemplate
        {
            // Missing required fields - ID, Name, ScriptTemplate
            Parameters = new List<TemplateParameter>()
        };

        // Act - attempt to save invalid template
        var (success, templateId) = await _engine.SaveCustomTemplateAsync(invalidTemplate);

        // Assert - verify rejection of invalid template
        Assert.False(success);
        Assert.Empty(templateId);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetTemplateByIdAsync_WithExistingTemplate_ReturnsTemplate()
    {
        // Arrange - first save a template
        var testTemplate = AdminTestHelpers.CreateTestTemplate("retrieve-test", "Retrieve Test", "Test");
        await _engine.SaveCustomTemplateAsync(testTemplate);

        // Act - retrieve the saved template
        var retrievedTemplate = await _engine.GetTemplateByIdAsync("retrieve-test");

        // Assert - verify successful retrieval
        Assert.NotNull(retrievedTemplate);
        Assert.Equal("retrieve-test", retrievedTemplate.Id);
        Assert.Equal("Retrieve Test", retrievedTemplate.Name);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetTemplateByIdAsync_WithNonexistentTemplate_ReturnsNull()
    {
        // Act - attempt to retrieve non-existent template
        var retrievedTemplate = await _engine.GetTemplateByIdAsync("does-not-exist");

        // Assert - verify graceful handling of missing template
        Assert.Null(retrievedTemplate);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task LoadTemplatesAsync_AfterSavingCustomTemplate_IncludesNewTemplate()
    {
        // Arrange - save a new custom template
        var newCustomTemplate = AdminTestHelpers.CreateTestTemplate("integration-test", "Integration Test", "Integration");
        await _engine.SaveCustomTemplateAsync(newCustomTemplate);

        // Act - reload all templates
        var allTemplates = await _engine.LoadTemplatesAsync();

        // Assert - verify custom template is included
        Assert.Contains(allTemplates, t => t.Id == "integration-test");
        
        var foundTemplate = allTemplates.FirstOrDefault(t => t.Id == "integration-test");
        Assert.NotNull(foundTemplate);
        Assert.True(foundTemplate.IsCustomTemplate);
        Assert.Equal("Integration Test", foundTemplate.Name);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("extend-mailbox", TemplateComplexity.Basic)]
    [InlineData("bulk-user-create", TemplateComplexity.Advanced)]
    [InlineData("tenant-report", TemplateComplexity.Expert)]
    [InlineData("enable-mfa", TemplateComplexity.Intermediate)]
    public async Task LoadTemplatesAsync_BuiltInTemplates_HaveCorrectComplexity(
        string templateId, TemplateComplexity expectedComplexity)
    {
        // Act - load templates and check complexity assignment
        var templates = await _engine.LoadTemplatesAsync();

        // Assert - verify complexity classification
        var template = templates.FirstOrDefault(t => t.Id == templateId);
        Assert.NotNull(template);
        Assert.Equal(expectedComplexity, template.Complexity);
    }

    [Fact]
    [Trait("Category", "Performance")]
    public async Task LoadTemplatesAsync_WithManyTemplates_PerformsEfficiently()
    {
        // Arrange - create multiple custom templates to test performance
        var tasks = new List<Task>();
        for (int i = 0; i < 50; i++)
        {
            var template = AdminTestHelpers.CreateTestTemplate($"perf-test-{i}", $"Performance Test {i}", "Performance");
            tasks.Add(_engine.SaveCustomTemplateAsync(template));
        }
        await Task.WhenAll(tasks);

        // Act - measure template loading performance
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var templates = await _engine.LoadTemplatesAsync();
        stopwatch.Stop();

        // Assert - verify performance expectations
        Assert.True(templates.Count >= 50); // Should have at least our test templates plus built-ins
        Assert.True(stopwatch.ElapsedMilliseconds < 5000); // Should complete within 5 seconds
    }

    public void Dispose()
    {
        // Cleanup test directory
        if (Directory.Exists(_testTemplatesPath))
        {
            Directory.Delete(_testTemplatesPath, recursive: true);
        }
        
        _engine?.Dispose();
    }
}