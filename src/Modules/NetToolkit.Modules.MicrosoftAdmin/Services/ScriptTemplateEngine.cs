using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using NetToolkit.Modules.MicrosoftAdmin.Interfaces;
using NetToolkit.Modules.MicrosoftAdmin.Models;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Management.Automation;
using System.Text;
using System.IO;

namespace NetToolkit.Modules.MicrosoftAdmin.Services;

/// <summary>
/// PowerShell script template engine - the alchemical transmutator of admin intentions
/// Transforms template wisdom into executable PowerShell mastery
/// </summary>
public class ScriptTemplateEngine : IScriptTemplateEngine
{
    private readonly ILogger<ScriptTemplateEngine> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _templatesPath;
    private readonly Dictionary<string, AdminTemplate> _templateCache = new();
    private readonly SemaphoreSlim _cacheLock = new(1, 1);

    public ScriptTemplateEngine(
        ILogger<ScriptTemplateEngine> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _templatesPath = _configuration["MicrosoftAdmin:TemplatesPath"] ?? 
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");
        
        _logger.LogInformation("Script template engine initialized - admin alchemy laboratory ready at {Path}", _templatesPath);
        
        // Ensure templates directory exists
        if (!Directory.Exists(_templatesPath))
        {
            Directory.CreateDirectory(_templatesPath);
            _logger.LogInformation("Templates directory created - admin spell repository established");
        }
    }

    public async Task<List<AdminTemplate>> LoadTemplatesAsync()
    {
        await _cacheLock.WaitAsync();
        try
        {
            _logger.LogInformation("Loading admin templates - gathering administrative spellbook");
            
            var templates = new List<AdminTemplate>();
            
            // Load built-in templates first
            templates.AddRange(await LoadBuiltInTemplatesAsync());
            
            // Load custom templates from file system
            templates.AddRange(await LoadCustomTemplatesAsync());
            
            // Update cache
            _templateCache.Clear();
            foreach (var template in templates)
            {
                _templateCache[template.Id] = template;
            }
            
            _logger.LogInformation("Loaded {Count} admin templates - arsenal fully stocked!", templates.Count);
            return templates;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load admin templates - spell repository corrupted!");
            return new List<AdminTemplate>();
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    public async Task<List<UiControl>> GenerateFormControlsAsync(AdminTemplate template)
    {
        try
        {
            _logger.LogDebug("Generating form controls for template {TemplateId} - UI alchemy commencing", template.Id);
            
            var controls = new List<UiControl>();
            
            foreach (var param in template.Parameters.OrderBy(p => p.Name))
            {
                var control = new UiControl
                {
                    Name = param.Name,
                    DisplayName = param.DisplayName.IsNotEmpty() ? param.DisplayName : param.Name,
                    Type = MapParameterTypeToUiControl(param.Type),
                    Required = param.Required,
                    Tooltip = param.HelpText.IsNotEmpty() ? param.HelpText : param.Description,
                    ValidationMessage = param.ValidationMessage,
                    Value = param.DefaultValue
                };
                
                // Configure control properties based on parameter type
                ConfigureControlProperties(control, param);
                
                // Add validation rules
                if (!string.IsNullOrEmpty(param.ValidationPattern))
                {
                    control.ValidationRules.Add($"regex:{param.ValidationPattern}");
                }
                
                if (param.MinLength.HasValue)
                {
                    control.ValidationRules.Add($"minlength:{param.MinLength.Value}");
                }
                
                if (param.MaxLength.HasValue)
                {
                    control.ValidationRules.Add($"maxlength:{param.MaxLength.Value}");
                }
                
                if (param.MinValue.HasValue)
                {
                    control.ValidationRules.Add($"min:{param.MinValue.Value}");
                }
                
                if (param.MaxValue.HasValue)
                {
                    control.ValidationRules.Add($"max:{param.MaxValue.Value}");
                }
                
                controls.Add(control);
            }
            
            _logger.LogDebug("Generated {Count} form controls for template {TemplateId} - UI elements materialized", 
                controls.Count, template.Id);
                
            return await Task.FromResult(controls);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate form controls for template {TemplateId}", template.Id);
            return new List<UiControl>();
        }
    }

    public async Task<string> BuildScriptAsync(AdminTemplate template, Dictionary<string, object> parameters)
    {
        try
        {
            _logger.LogDebug("Building PowerShell script for template {TemplateId} - admin incantation weaving", template.Id);
            
            var script = template.ScriptTemplate;
            if (string.IsNullOrWhiteSpace(script))
            {
                _logger.LogWarning("Template {TemplateId} has empty script content", template.Id);
                return string.Empty;
            }
            
            // Replace parameter placeholders with actual values
            var processedScript = ProcessScriptTemplate(script, parameters, template.Parameters);
            
            // Add error handling wrapper
            var finalScript = WrapScriptWithErrorHandling(processedScript, template);
            
            _logger.LogDebug("Script built successfully for template {TemplateId} - {Length} characters of admin power", 
                template.Id, finalScript.Length);
                
            return await Task.FromResult(finalScript);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to build script for template {TemplateId}", template.Id);
            return string.Empty;
        }
    }

    public async Task<ValidationResult> ValidateTemplateAsync(AdminTemplate template)
    {
        var result = new ValidationResult();
        var messages = new List<ValidationMessage>();
        
        try
        {
            _logger.LogDebug("Validating template {TemplateId} - administrative spell checking", template.Id);
            
            // Basic template validation
            if (string.IsNullOrWhiteSpace(template.Id))
            {
                messages.Add(new ValidationMessage
                {
                    Field = "Id",
                    Message = "Template ID is required - like a spell needs a name!",
                    Severity = ValidationSeverity.Error
                });
            }
            
            if (string.IsNullOrWhiteSpace(template.Name))
            {
                messages.Add(new ValidationMessage
                {
                    Field = "Name",
                    Message = "Template name is required - anonymous admin spells are forbidden!",
                    Severity = ValidationSeverity.Error
                });
            }
            
            if (string.IsNullOrWhiteSpace(template.ScriptTemplate))
            {
                messages.Add(new ValidationMessage
                {
                    Field = "ScriptTemplate",
                    Message = "Script content is required - empty spells cast nothing but disappointment!",
                    Severity = ValidationSeverity.Error
                });
            }
            
            // Validate PowerShell syntax
            await ValidatePowerShellSyntax(template.ScriptTemplate, messages);
            
            // Validate parameter placeholders
            ValidateParameterPlaceholders(template, messages);
            
            // Validate required scopes
            if (template.RequiredScopes.Count == 0)
            {
                messages.Add(new ValidationMessage
                {
                    Field = "RequiredScopes",
                    Message = "At least one required scope must be specified - admin spells need permission boundaries!",
                    Severity = ValidationSeverity.Warning
                });
            }
            
            result.Messages = messages;
            result.IsValid = !result.HasErrors;
            
            _logger.LogDebug("Template {TemplateId} validation complete - {ErrorCount} errors, {WarningCount} warnings", 
                template.Id, messages.Count(m => m.Severity == ValidationSeverity.Error),
                messages.Count(m => m.Severity == ValidationSeverity.Warning));
                
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Template validation failed catastrophically for {TemplateId}", template.Id);
            
            messages.Add(new ValidationMessage
            {
                Field = "validation",
                Message = $"Validation process failed: {ex.Message}",
                Severity = ValidationSeverity.Error
            });
            
            result.Messages = messages;
            result.IsValid = false;
            return result;
        }
    }

    public async Task<(bool Success, string TemplateId)> SaveCustomTemplateAsync(AdminTemplate template)
    {
        await _cacheLock.WaitAsync();
        try
        {
            _logger.LogInformation("Saving custom template {TemplateName} - inscribing admin wisdom", template.Name);
            
            // Assign ID if not provided
            if (string.IsNullOrWhiteSpace(template.Id))
            {
                template.Id = $"custom-{Guid.NewGuid():N}";
            }
            
            template.IsCustomTemplate = true;
            template.CreatedAt = DateTime.UtcNow;
            template.UpdatedAt = DateTime.UtcNow;
            
            // Validate template before saving
            var validation = await ValidateTemplateAsync(template);
            if (!validation.IsValid)
            {
                _logger.LogWarning("Custom template {TemplateId} validation failed - {ErrorCount} errors found", 
                    template.Id, validation.Messages.Count(m => m.Severity == ValidationSeverity.Error));
                return (false, string.Empty);
            }
            
            // Serialize and save template
            var templatePath = Path.Combine(_templatesPath, "Custom", $"{template.Id}.json");
            var templateDirectory = Path.GetDirectoryName(templatePath)!;
            
            if (!Directory.Exists(templateDirectory))
            {
                Directory.CreateDirectory(templateDirectory);
            }
            
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            var templateJson = JsonSerializer.Serialize(template, jsonOptions);
            await File.WriteAllTextAsync(templatePath, templateJson);
            
            // Update cache
            _templateCache[template.Id] = template;
            
            _logger.LogInformation("Custom template {TemplateId} saved successfully - admin spell inscribed in tome", 
                template.Id);
                
            return (true, template.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save custom template {TemplateName}", template.Name);
            return (false, string.Empty);
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    public async Task<AdminTemplate?> GetTemplateByIdAsync(string templateId)
    {
        await _cacheLock.WaitAsync();
        try
        {
            if (_templateCache.TryGetValue(templateId, out var cachedTemplate))
            {
                return cachedTemplate;
            }
            
            // Template not in cache, try loading from file system
            var customTemplatePath = Path.Combine(_templatesPath, "Custom", $"{templateId}.json");
            if (File.Exists(customTemplatePath))
            {
                var templateJson = await File.ReadAllTextAsync(customTemplatePath);
                var template = JsonSerializer.Deserialize<AdminTemplate>(templateJson, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                
                if (template != null)
                {
                    _templateCache[templateId] = template;
                    return template;
                }
            }
            
            _logger.LogWarning("Template {TemplateId} not found - spell missing from admin grimoire", templateId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve template {TemplateId}", templateId);
            return null;
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    private async Task<List<AdminTemplate>> LoadBuiltInTemplatesAsync()
    {
        var templates = new List<AdminTemplate>();
        
        try
        {
            _logger.LogDebug("Loading built-in admin templates - consulting ancient admin wisdom");
            
            // Create built-in templates based on the predefined list
            foreach (var (templateId, (name, description, category)) in BuiltInTemplates.Templates)
            {
                var template = await CreateBuiltInTemplate(templateId, name, description, category);
                if (template != null)
                {
                    templates.Add(template);
                }
            }
            
            _logger.LogDebug("Loaded {Count} built-in templates - foundational spells acquired", templates.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load built-in templates - ancient wisdom corrupted");
        }
        
        return templates;
    }

    private async Task<List<AdminTemplate>> LoadCustomTemplatesAsync()
    {
        var templates = new List<AdminTemplate>();
        
        try
        {
            var customTemplatesPath = Path.Combine(_templatesPath, "Custom");
            if (!Directory.Exists(customTemplatesPath))
            {
                return templates;
            }
            
            var templateFiles = Directory.GetFiles(customTemplatesPath, "*.json");
            
            foreach (var file in templateFiles)
            {
                try
                {
                    var templateJson = await File.ReadAllTextAsync(file);
                    var template = JsonSerializer.Deserialize<AdminTemplate>(templateJson, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                    
                    if (template != null)
                    {
                        template.IsCustomTemplate = true;
                        templates.Add(template);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to load custom template from {File}", file);
                }
            }
            
            _logger.LogDebug("Loaded {Count} custom templates - user-crafted spells imported", templates.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load custom templates - spell collection corrupted");
        }
        
        return templates;
    }

    private async Task<AdminTemplate?> CreateBuiltInTemplate(string id, string name, string description, string category)
    {
        try
        {
            // Load template script from embedded resource or create default
            var scriptContent = await GetBuiltInScriptContent(id);
            var parameters = GetBuiltInParameters(id);
            
            return new AdminTemplate
            {
                Id = id,
                Name = name,
                Description = description,
                Category = category,
                ScriptTemplate = scriptContent,
                Parameters = parameters,
                RequiredScopes = GetRequiredScopes(id),
                Complexity = GetTemplateComplexity(id),
                EstimatedDuration = GetEstimatedDuration(id),
                Tags = GetTemplateTags(id),
                IconName = GetTemplateIcon(id),
                IsCustomTemplate = false,
                Author = "NetToolkit Team",
                Version = "1.0.0",
                Examples = GetTemplateExamples(id)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create built-in template {TemplateId}", id);
            return null;
        }
    }

    private async Task<string> GetBuiltInScriptContent(string templateId)
    {
        // Return template-specific PowerShell scripts
        return templateId switch
        {
            "extend-mailbox" => @"
# Extend user mailbox quota - storage expansion spell
Connect-ExchangeOnline -ShowProgress $false
Set-Mailbox -Identity '{email}' -ProhibitSendQuota '{quota}GB' -ProhibitSendReceiveQuota '{quotaMax}GB'
Write-Output ""Mailbox quota extended for {email} - storage expanded to {quota}GB""
Disconnect-ExchangeOnline -Confirm:$false",

            "create-shared-mailbox" => @"
# Create shared mailbox - collaboration portal manifestation
Connect-ExchangeOnline -ShowProgress $false
New-Mailbox -Name '{name}' -DisplayName '{displayName}' -Shared -PrimarySmtpAddress '{email}'
Write-Output ""Shared mailbox '{name}' created successfully at {email}""
Disconnect-ExchangeOnline -Confirm:$false",

            "add-email-alias" => @"
# Add email alias - identity multiplication spell
Connect-ExchangeOnline -ShowProgress $false
Set-Mailbox -Identity '{email}' -EmailAddresses @{Add='{alias}'}
Write-Output ""Email alias '{alias}' added to {email} - identity enhanced""
Disconnect-ExchangeOnline -Confirm:$false",

            "bulk-user-create" => @"
# Bulk user creation - digital population expansion
Connect-MgGraph -Scopes 'User.ReadWrite.All'
$users = Import-Csv '{csvPath}'
foreach ($user in $users) {
    New-MgUser -DisplayName $user.DisplayName -UserPrincipalName $user.UPN -MailNickname $user.MailNickname -PasswordProfile @{Password=$user.Password; ForceChangePasswordNextSignIn=$true}
    Write-Output ""User created: $($user.DisplayName)""
}
Write-Output ""Bulk user creation completed - {0} users spawned"" -f $users.Count
Disconnect-MgGraph",

            "reset-user-password" => @"
# Reset user password - access restoration ritual
Connect-MgGraph -Scopes 'User.ReadWrite.All'
$newPassword = '{newPassword}'
Update-MgUser -UserId '{email}' -PasswordProfile @{Password=$newPassword; ForceChangePasswordNextSignIn='{forceChange}'}
Write-Output ""Password reset for {email} - access gates refreshed""
Disconnect-MgGraph",

            _ => await Task.FromResult(@"
# Generic admin template - customize as needed
Write-Output ""Admin operation completed successfully""")
        };
    }

    private List<TemplateParameter> GetBuiltInParameters(string templateId)
    {
        return templateId switch
        {
            "extend-mailbox" => new List<TemplateParameter>
            {
                new() { Name = "email", DisplayName = "User Email", Type = ParameterType.Email, Required = true, Description = "Email address of the user whose mailbox to extend" },
                new() { Name = "quota", DisplayName = "New Quota (GB)", Type = ParameterType.Integer, Required = true, MinValue = 1, MaxValue = 100, DefaultValue = 10, Description = "New mailbox quota in gigabytes" },
                new() { Name = "quotaMax", DisplayName = "Max Quota (GB)", Type = ParameterType.Integer, Required = true, MinValue = 1, MaxValue = 150, DefaultValue = 15, Description = "Maximum mailbox quota before blocking" }
            },

            "create-shared-mailbox" => new List<TemplateParameter>
            {
                new() { Name = "name", DisplayName = "Mailbox Name", Type = ParameterType.String, Required = true, MinLength = 3, MaxLength = 64, Description = "Internal name for the shared mailbox" },
                new() { Name = "displayName", DisplayName = "Display Name", Type = ParameterType.String, Required = true, MinLength = 3, MaxLength = 128, Description = "Friendly display name" },
                new() { Name = "email", DisplayName = "Email Address", Type = ParameterType.Email, Required = true, Description = "Primary email address for the shared mailbox" }
            },

            "add-email-alias" => new List<TemplateParameter>
            {
                new() { Name = "email", DisplayName = "User Email", Type = ParameterType.Email, Required = true, Description = "Primary email address of the user" },
                new() { Name = "alias", DisplayName = "New Alias", Type = ParameterType.Email, Required = true, Description = "New email alias to add" }
            },

            "bulk-user-create" => new List<TemplateParameter>
            {
                new() { Name = "csvPath", DisplayName = "CSV File Path", Type = ParameterType.FilePath, Required = true, Description = "Path to CSV file containing user data" }
            },

            "reset-user-password" => new List<TemplateParameter>
            {
                new() { Name = "email", DisplayName = "User Email", Type = ParameterType.Email, Required = true, Description = "Email address of the user" },
                new() { Name = "newPassword", DisplayName = "New Password", Type = ParameterType.Password, Required = true, MinLength = 8, Description = "New password for the user" },
                new() { Name = "forceChange", DisplayName = "Force Password Change", Type = ParameterType.Boolean, Required = true, DefaultValue = true, Description = "Require password change on next login" }
            },

            _ => new List<TemplateParameter>()
        };
    }

    private List<string> GetRequiredScopes(string templateId)
    {
        return templateId switch
        {
            "extend-mailbox" or "create-shared-mailbox" or "add-email-alias" => 
                new List<string> { GraphScopes.MailboxSettingsReadWrite },
            "bulk-user-create" or "reset-user-password" => 
                new List<string> { GraphScopes.UserReadWriteAll },
            _ => new List<string> { GraphScopes.UserRead }
        };
    }

    private TemplateComplexity GetTemplateComplexity(string templateId)
    {
        return templateId switch
        {
            "bulk-user-create" => TemplateComplexity.Advanced,
            "tenant-report" => TemplateComplexity.Expert,
            "enable-mfa" or "create-security-group" => TemplateComplexity.Intermediate,
            _ => TemplateComplexity.Basic
        };
    }

    private string GetEstimatedDuration(string templateId)
    {
        return templateId switch
        {
            "bulk-user-create" => "5-15 minutes",
            "tenant-report" => "10-30 minutes", 
            "create-shared-mailbox" => "2-5 minutes",
            _ => "1-3 minutes"
        };
    }

    private List<string> GetTemplateTags(string templateId)
    {
        return templateId switch
        {
            "extend-mailbox" or "create-shared-mailbox" or "add-email-alias" => 
                new List<string> { "mailbox", "email", "exchange" },
            "bulk-user-create" or "reset-user-password" => 
                new List<string> { "users", "identity", "bulk" },
            "enable-mfa" => 
                new List<string> { "security", "mfa", "authentication" },
            _ => new List<string> { "admin", "management" }
        };
    }

    private string GetTemplateIcon(string templateId)
    {
        return templateId switch
        {
            "extend-mailbox" or "create-shared-mailbox" or "add-email-alias" => "mail",
            "bulk-user-create" or "reset-user-password" => "users",
            "enable-mfa" => "shield",
            "create-security-group" => "group",
            "tenant-report" => "chart",
            _ => "script"
        };
    }

    private List<TemplateExample> GetTemplateExamples(string templateId)
    {
        return templateId switch
        {
            "extend-mailbox" => new List<TemplateExample>
            {
                new()
                {
                    Name = "Standard Extension",
                    Description = "Extend mailbox to 25GB with 30GB max",
                    Parameters = new Dictionary<string, object>
                    {
                        ["email"] = "john.doe@company.com",
                        ["quota"] = 25,
                        ["quotaMax"] = 30
                    },
                    ExpectedOutcome = "Mailbox quota increased, user can receive more emails"
                }
            },
            _ => new List<TemplateExample>()
        };
    }

    private UiControlType MapParameterTypeToUiControl(ParameterType parameterType)
    {
        return parameterType switch
        {
            ParameterType.String => UiControlType.TextBox,
            ParameterType.Integer => UiControlType.NumericUpDown,
            ParameterType.Double => UiControlType.NumericUpDown,
            ParameterType.Boolean => UiControlType.CheckBox,
            ParameterType.Email => UiControlType.TextBox,
            ParameterType.Password => UiControlType.PasswordBox,
            ParameterType.MultilineText => UiControlType.MultilineTextBox,
            ParameterType.DropdownList => UiControlType.ComboBox,
            ParameterType.FilePath => UiControlType.FileSelector,
            ParameterType.Date => UiControlType.DatePicker,
            ParameterType.DateTime => UiControlType.DatePicker,
            _ => UiControlType.TextBox
        };
    }

    private void ConfigureControlProperties(UiControl control, TemplateParameter param)
    {
        // Add dropdown options if specified
        if (param.ValidValues.Count > 0)
        {
            control.Type = UiControlType.ComboBox;
            control.Options = param.ValidValues.Select(v => new SelectOption
            {
                Value = v,
                Text = v,
                IsSelected = v.Equals(param.DefaultValue?.ToString())
            }).ToList();
        }
        
        // Configure numeric controls
        if (param.Type is ParameterType.Integer or ParameterType.Double)
        {
            if (param.MinValue.HasValue)
                control.Properties["minimum"] = param.MinValue.Value;
            if (param.MaxValue.HasValue)
                control.Properties["maximum"] = param.MaxValue.Value;
        }
        
        // Configure string controls
        if (param.Type == ParameterType.String)
        {
            if (param.MaxLength.HasValue)
                control.Properties["maxLength"] = param.MaxLength.Value;
            if (!string.IsNullOrEmpty(param.Placeholder))
                control.Properties["placeholder"] = param.Placeholder;
        }
        
        // Configure secure controls
        if (param.IsSecret)
        {
            control.Type = UiControlType.PasswordBox;
            control.Properties["isSecret"] = true;
        }
    }

    private string ProcessScriptTemplate(string script, Dictionary<string, object> parameters, List<TemplateParameter> templateParams)
    {
        var processedScript = script;
        
        // Replace parameter placeholders {paramName} with actual values
        foreach (var param in parameters)
        {
            var placeholder = $"{{{param.Key}}}";
            var value = param.Value?.ToString() ?? string.Empty;
            
            // Apply type-specific formatting
            var templateParam = templateParams.FirstOrDefault(p => p.Name == param.Key);
            if (templateParam != null)
            {
                value = FormatParameterValue(value, templateParam.Type);
            }
            
            processedScript = processedScript.Replace(placeholder, value);
        }
        
        return processedScript;
    }

    private string FormatParameterValue(string value, ParameterType type)
    {
        return type switch
        {
            ParameterType.Boolean => value.ToLower() == "true" ? "$true" : "$false",
            ParameterType.String or ParameterType.Email => $"'{value.Replace("'", "''")}'", // Escape single quotes
            _ => value
        };
    }

    private string WrapScriptWithErrorHandling(string script, AdminTemplate template)
    {
        var wrapper = new StringBuilder();
        
        wrapper.AppendLine("# NetToolkit Admin Script Execution Wrapper");
        wrapper.AppendLine($"# Template: {template.Name} ({template.Id})");
        wrapper.AppendLine($"# Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        wrapper.AppendLine();
        wrapper.AppendLine("try {");
        wrapper.AppendLine("    # Enable strict error handling");
        wrapper.AppendLine("    $ErrorActionPreference = 'Stop'");
        wrapper.AppendLine();
        wrapper.AppendLine("    # Admin script execution begins");
        
        // Indent the actual script
        var indentedScript = string.Join(Environment.NewLine, 
            script.Split(Environment.NewLine).Select(line => "    " + line));
        wrapper.AppendLine(indentedScript);
        
        wrapper.AppendLine();
        wrapper.AppendLine("    Write-Output \"Admin operation completed successfully - NetToolkit mastery achieved!\"");
        wrapper.AppendLine("}");
        wrapper.AppendLine("catch {");
        wrapper.AppendLine("    Write-Error \"Admin operation failed: $($_.Exception.Message)\"");
        wrapper.AppendLine("    throw");
        wrapper.AppendLine("}");
        wrapper.AppendLine("finally {");
        wrapper.AppendLine("    # Cleanup operations if needed");
        wrapper.AppendLine("    Write-Output \"Script execution completed at $(Get-Date)\"");
        wrapper.AppendLine("}");
        
        return wrapper.ToString();
    }

    private async Task ValidatePowerShellSyntax(string script, List<ValidationMessage> messages)
    {
        try
        {
            using var powerShell = PowerShell.Create();
            
            // Try to parse the script to check syntax
            var tokens = new List<PSToken>();
            
            // INFERRED: Use PSParser for script validation in System.Management.Automation v7.4.0
            System.Collections.ObjectModel.Collection<PSParseError> parseErrorsCollection;
            var parseResult = PSParser.Tokenize(script, out parseErrorsCollection);
            if (parseResult != null) tokens.AddRange(parseResult);
            var parseErrors = parseErrorsCollection.ToList();
            
            if (parseErrors.Count > 0)
            {
                foreach (var error in parseErrors)
                {
                    messages.Add(new ValidationMessage
                    {
                        Field = "ScriptTemplate",
                        Message = $"PowerShell syntax error at line {error.Token.StartLine}: {error.Message}",
                        Severity = ValidationSeverity.Error,
                        Code = "SYNTAX_ERROR"
                    });
                }
            }
        }
        catch (Exception ex)
        {
            messages.Add(new ValidationMessage
            {
                Field = "ScriptTemplate",
                Message = $"Script validation failed: {ex.Message}",
                Severity = ValidationSeverity.Warning
            });
        }
        
        await Task.CompletedTask;
    }

    private void ValidateParameterPlaceholders(AdminTemplate template, List<ValidationMessage> messages)
    {
        var script = template.ScriptTemplate;
        var parameterNames = template.Parameters.Select(p => p.Name).ToHashSet();
        
        // Find all placeholders in script
        var placeholderRegex = new Regex(@"\{(\w+)\}", RegexOptions.IgnoreCase);
        var matches = placeholderRegex.Matches(script);
        
        foreach (Match match in matches)
        {
            var parameterName = match.Groups[1].Value;
            
            if (!parameterNames.Contains(parameterName))
            {
                messages.Add(new ValidationMessage
                {
                    Field = "ScriptTemplate",
                    Message = $"Script references undefined parameter '{parameterName}' - like calling a function that doesn't exist!",
                    Severity = ValidationSeverity.Error,
                    Code = "UNDEFINED_PARAMETER"
                });
            }
        }
        
        // Check for unused parameters
        foreach (var param in template.Parameters)
        {
            var placeholder = $"{{{param.Name}}}";
            if (!script.Contains(placeholder))
            {
                messages.Add(new ValidationMessage
                {
                    Field = param.Name,
                    Message = $"Parameter '{param.Name}' is defined but not used in script - like unused variables cluttering the namespace!",
                    Severity = ValidationSeverity.Warning,
                    Code = "UNUSED_PARAMETER"
                });
            }
        }
    }

    public void Dispose()
    {
        _cacheLock?.Dispose();
    }
}