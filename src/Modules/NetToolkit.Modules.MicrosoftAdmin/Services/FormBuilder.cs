using Microsoft.Extensions.Logging;
using NetToolkit.Modules.MicrosoftAdmin.Interfaces;
using NetToolkit.Modules.MicrosoftAdmin.Models;

namespace NetToolkit.Modules.MicrosoftAdmin.Services;

/// <summary>
/// Dynamic form builder - the UI alchemist of administrative interfaces
/// Transforms template parameters into beautiful, interactive form elements
/// </summary>
public class FormBuilder : IFormBuilder
{
    private readonly ILogger<FormBuilder> _logger;
    private readonly Dictionary<string, Func<TemplateParameter, UiControl>> _controlGenerators;

    public FormBuilder(ILogger<FormBuilder> logger)
    {
        _logger = logger;
        
        // Initialize control generators for different parameter types
        _controlGenerators = new Dictionary<string, Func<TemplateParameter, UiControl>>
        {
            [nameof(ParameterType.String)] = GenerateTextBoxControl,
            [nameof(ParameterType.Email)] = GenerateEmailControl,
            [nameof(ParameterType.Password)] = GeneratePasswordControl,
            [nameof(ParameterType.Integer)] = GenerateNumericControl,
            [nameof(ParameterType.Double)] = GenerateNumericControl,
            [nameof(ParameterType.Boolean)] = GenerateCheckBoxControl,
            [nameof(ParameterType.MultilineText)] = GenerateMultilineTextControl,
            [nameof(ParameterType.DropdownList)] = GenerateDropdownControl,
            [nameof(ParameterType.MultiSelect)] = GenerateMultiSelectControl,
            [nameof(ParameterType.FilePath)] = GenerateFilePathControl,
            [nameof(ParameterType.Date)] = GenerateDateControl,
            [nameof(ParameterType.DateTime)] = GenerateDateTimeControl,
            [nameof(ParameterType.Guid)] = GenerateGuidControl,
            [nameof(ParameterType.Json)] = GenerateJsonControl,
            [nameof(ParameterType.Url)] = GenerateUrlControl
        };
        
        _logger.LogInformation("Form builder initialized - UI alchemy laboratory ready for admin interface transmutation");
    }

    public async Task<FormDefinition> GenerateFormAsync(AdminTemplate template)
    {
        try
        {
            _logger.LogInformation("Generating form for template {TemplateId} - UI transmutation commencing", template.Id);
            
            var formDefinition = new FormDefinition
            {
                FormId = $"form-{template.Id}",
                Title = template.Name,
                Description = template.Description
            };
            
            var controls = new List<UiControl>();
            var sections = new List<FormSection>();
            
            // Group parameters by category or create default sections
            var parameterGroups = GroupParametersBySection(template.Parameters);
            int tabOrder = 1;
            
            foreach (var group in parameterGroups)
            {
                var section = new FormSection
                {
                    Name = group.Key,
                    Title = FormatSectionTitle(group.Key),
                    Description = GetSectionDescription(group.Key),
                    IsCollapsible = group.Key != "General",
                    IsExpanded = group.Key == "General",
                    IconName = GetSectionIcon(group.Key)
                };
                
                foreach (var parameter in group.Value.OrderBy(p => GetParameterPriority(p)))
                {
                    var control = await GenerateControlForParameter(parameter, tabOrder++);
                    if (control != null)
                    {
                        control.Group = group.Key;
                        controls.Add(control);
                        section.ControlNames.Add(control.Name);
                    }
                }
                
                if (section.ControlNames.Count > 0)
                {
                    sections.Add(section);
                }
            }
            
            formDefinition.Controls = controls;
            formDefinition.Sections = sections;
            
            // Add form-level validation rules
            formDefinition.ValidationRules = GenerateFormValidationRules(template);
            
            // Configure form properties
            formDefinition.FormProperties = new Dictionary<string, object>
            {
                ["templateId"] = template.Id,
                ["complexity"] = template.Complexity.ToString(),
                ["estimatedDuration"] = template.EstimatedDuration,
                ["requiredScopes"] = template.RequiredScopes,
                ["wittyHint"] = GetWittyFormHint(template.Id),
                ["helpUrl"] = template.HelpUrl,
                ["canSaveAsDraft"] = true,
                ["autoValidate"] = true
            };
            
            _logger.LogInformation("Form generated successfully for template {TemplateId} - {ControlCount} controls, {SectionCount} sections created", 
                template.Id, controls.Count, sections.Count);
                
            return formDefinition;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate form for template {TemplateId} - UI alchemy failed", template.Id);
            return CreateErrorForm(template, ex.Message);
        }
    }

    public async Task<ValidationResult> ValidateFormInputAsync(FormDefinition formDefinition, Dictionary<string, object> inputValues)
    {
        try
        {
            _logger.LogDebug("Validating form input for {FormId} - ensuring admin precision", formDefinition.FormId);
            
            var result = new ValidationResult();
            var messages = new List<ValidationMessage>();
            
            // Validate each control's input
            foreach (var control in formDefinition.Controls)
            {
                var validationMessage = await ValidateControlInput(control, inputValues);
                if (validationMessage != null)
                {
                    messages.Add(validationMessage);
                }
            }
            
            // Apply form-level validation rules
            foreach (var rule in formDefinition.ValidationRules)
            {
                var ruleValidation = await ApplyValidationRule(rule, inputValues, formDefinition.Controls);
                if (ruleValidation != null)
                {
                    messages.Add(ruleValidation);
                }
            }
            
            result.Messages = messages;
            result.IsValid = !result.HasErrors;
            result.ValidationData = new Dictionary<string, object>
            {
                ["validatedAt"] = DateTime.UtcNow,
                ["formId"] = formDefinition.FormId,
                ["inputCount"] = inputValues.Count
            };
            
            _logger.LogDebug("Form validation completed for {FormId} - {ErrorCount} errors, {WarningCount} warnings", 
                formDefinition.FormId, 
                messages.Count(m => m.Severity == ValidationSeverity.Error),
                messages.Count(m => m.Severity == ValidationSeverity.Warning));
                
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Form validation failed catastrophically for {FormId}", formDefinition.FormId);
            
            return new ValidationResult
            {
                IsValid = false,
                Messages = new List<ValidationMessage>
                {
                    new()
                    {
                        Field = "validation",
                        Message = $"Validation process crashed: {ex.Message}",
                        Severity = ValidationSeverity.Error,
                        Code = "VALIDATION_CRASH"
                    }
                }
            };
        }
    }

    public async Task<FormDefinition> GenerateCustomFormAsync(List<TemplateParameter> parameters)
    {
        try
        {
            _logger.LogInformation("Generating custom form from {ParameterCount} parameters - bespoke UI creation", parameters.Count);
            
            var customTemplate = new AdminTemplate
            {
                Id = $"custom-{Guid.NewGuid():N}",
                Name = "Custom Admin Form",
                Description = "Dynamically generated form for custom administrative operations",
                Category = "Custom",
                Parameters = parameters,
                IsCustomTemplate = true
            };
            
            return await GenerateFormAsync(customTemplate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate custom form - bespoke UI creation failed");
            return CreateErrorForm(null, ex.Message);
        }
    }

    private async Task<UiControl?> GenerateControlForParameter(TemplateParameter parameter, int tabOrder)
    {
        try
        {
            var parameterType = parameter.Type.ToString();
            
            if (_controlGenerators.TryGetValue(parameterType, out var generator))
            {
                var control = generator(parameter);
                control.TabOrder = tabOrder;
                
                // Apply common properties
                ApplyCommonControlProperties(control, parameter);
                
                _logger.LogDebug("Generated {ControlType} control for parameter {ParameterName}", 
                    control.Type, parameter.Name);
                    
                return await Task.FromResult(control);
            }
            else
            {
                _logger.LogWarning("No control generator found for parameter type {ParameterType}, using default TextBox", parameterType);
                return GenerateTextBoxControl(parameter);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate control for parameter {ParameterName}", parameter.Name);
            return null;
        }
    }

    private void ApplyCommonControlProperties(UiControl control, TemplateParameter parameter)
    {
        control.Name = parameter.Name;
        control.DisplayName = parameter.DisplayName.IsNotEmpty() ? parameter.DisplayName : parameter.Name;
        control.Required = parameter.Required;
        control.Tooltip = parameter.HelpText.IsNotEmpty() ? parameter.HelpText : parameter.Description;
        control.Value = parameter.DefaultValue;
        
        // Add validation rules
        if (!string.IsNullOrEmpty(parameter.ValidationPattern))
        {
            control.ValidationRules.Add($"regex:{parameter.ValidationPattern}");
            control.ValidationMessage = parameter.ValidationMessage.IsNotEmpty() ? 
                parameter.ValidationMessage : $"Invalid format for {parameter.DisplayName}";
        }
        
        if (parameter.MinLength.HasValue)
            control.ValidationRules.Add($"minlength:{parameter.MinLength.Value}");
            
        if (parameter.MaxLength.HasValue)
            control.ValidationRules.Add($"maxlength:{parameter.MaxLength.Value}");
            
        if (parameter.MinValue.HasValue)
            control.ValidationRules.Add($"min:{parameter.MinValue.Value}");
            
        if (parameter.MaxValue.HasValue)
            control.ValidationRules.Add($"max:{parameter.MaxValue.Value}");
        
        // Add witty placeholders
        if (string.IsNullOrEmpty(control.Properties.GetValueOrDefault("placeholder")?.ToString()))
        {
            control.Properties["placeholder"] = GetWittyPlaceholder(parameter);
        }
    }

    private UiControl GenerateTextBoxControl(TemplateParameter parameter)
    {
        return new UiControl
        {
            Type = UiControlType.TextBox,
            Properties = new Dictionary<string, object>
            {
                ["placeholder"] = parameter.Placeholder.IsNotEmpty() ? parameter.Placeholder : GetWittyPlaceholder(parameter),
                ["maxLength"] = parameter.MaxLength ?? 256,
                ["autocomplete"] = GetAutoCompleteHint(parameter)
            }
        };
    }

    private UiControl GenerateEmailControl(TemplateParameter parameter)
    {
        return new UiControl
        {
            Type = UiControlType.TextBox,
            Properties = new Dictionary<string, object>
            {
                ["inputType"] = "email",
                ["placeholder"] = parameter.Placeholder.IsNotEmpty() ? parameter.Placeholder : "user@company.com",
                ["pattern"] = @"^[^\s@]+@[^\s@]+\.[^\s@]+$",
                ["autocomplete"] = "email",
                ["spellcheck"] = false
            },
            ValidationRules = new List<string> { "email" }
        };
    }

    private UiControl GeneratePasswordControl(TemplateParameter parameter)
    {
        return new UiControl
        {
            Type = UiControlType.PasswordBox,
            Properties = new Dictionary<string, object>
            {
                ["placeholder"] = "Enter secure password...",
                ["showStrengthIndicator"] = true,
                ["minLength"] = parameter.MinLength ?? 8,
                ["requireComplexity"] = true,
                ["autocomplete"] = "new-password"
            }
        };
    }

    private UiControl GenerateNumericControl(TemplateParameter parameter)
    {
        return new UiControl
        {
            Type = UiControlType.NumericUpDown,
            Properties = new Dictionary<string, object>
            {
                ["minimum"] = parameter.MinValue ?? (parameter.Type == ParameterType.Integer ? int.MinValue : double.MinValue),
                ["maximum"] = parameter.MaxValue ?? (parameter.Type == ParameterType.Integer ? int.MaxValue : double.MaxValue),
                ["increment"] = parameter.Type == ParameterType.Integer ? 1 : 0.1,
                ["decimalPlaces"] = parameter.Type == ParameterType.Integer ? 0 : 2,
                ["thousandsSeparator"] = true
            }
        };
    }

    private UiControl GenerateCheckBoxControl(TemplateParameter parameter)
    {
        return new UiControl
        {
            Type = UiControlType.CheckBox,
            Properties = new Dictionary<string, object>
            {
                ["content"] = parameter.DisplayName,
                ["isThreeState"] = false
            }
        };
    }

    private UiControl GenerateMultilineTextControl(TemplateParameter parameter)
    {
        return new UiControl
        {
            Type = UiControlType.MultilineTextBox,
            Properties = new Dictionary<string, object>
            {
                ["rows"] = 5,
                ["maxLength"] = parameter.MaxLength ?? 4000,
                ["placeholder"] = parameter.Placeholder.IsNotEmpty() ? parameter.Placeholder : "Enter detailed information...",
                ["spellCheck"] = true,
                ["wordWrap"] = true
            }
        };
    }

    private UiControl GenerateDropdownControl(TemplateParameter parameter)
    {
        return new UiControl
        {
            Type = UiControlType.ComboBox,
            Options = parameter.ValidValues.Select(v => new SelectOption
            {
                Value = v,
                Text = v,
                IsSelected = v.Equals(parameter.DefaultValue?.ToString())
            }).ToList(),
            Properties = new Dictionary<string, object>
            {
                ["isEditable"] = false,
                ["placeholder"] = "Select an option..."
            }
        };
    }

    private UiControl GenerateMultiSelectControl(TemplateParameter parameter)
    {
        return new UiControl
        {
            Type = UiControlType.ComboBox,
            Options = parameter.ValidValues.Select(v => new SelectOption
            {
                Value = v,
                Text = v,
                IsSelected = false
            }).ToList(),
            Properties = new Dictionary<string, object>
            {
                ["multiSelect"] = true,
                ["placeholder"] = "Select multiple options..."
            }
        };
    }

    private UiControl GenerateFilePathControl(TemplateParameter parameter)
    {
        return new UiControl
        {
            Type = UiControlType.FileSelector,
            Properties = new Dictionary<string, object>
            {
                ["filter"] = GetFileFilter(parameter),
                ["multiSelect"] = false,
                ["placeholder"] = "Click to browse for file...",
                ["buttonText"] = "Browse..."
            }
        };
    }

    private UiControl GenerateDateControl(TemplateParameter parameter)
    {
        return new UiControl
        {
            Type = UiControlType.DatePicker,
            Properties = new Dictionary<string, object>
            {
                ["format"] = "yyyy-MM-dd",
                ["showCalendar"] = true,
                ["minimumDate"] = DateTime.Today,
                ["maximumDate"] = DateTime.Today.AddYears(5)
            }
        };
    }

    private UiControl GenerateDateTimeControl(TemplateParameter parameter)
    {
        return new UiControl
        {
            Type = UiControlType.DatePicker,
            Properties = new Dictionary<string, object>
            {
                ["format"] = "yyyy-MM-dd HH:mm",
                ["showCalendar"] = true,
                ["showTimePicker"] = true,
                ["minimumDate"] = DateTime.Now,
                ["maximumDate"] = DateTime.Now.AddYears(5)
            }
        };
    }

    private UiControl GenerateGuidControl(TemplateParameter parameter)
    {
        return new UiControl
        {
            Type = UiControlType.TextBox,
            Properties = new Dictionary<string, object>
            {
                ["pattern"] = @"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
                ["placeholder"] = "00000000-0000-0000-0000-000000000000",
                ["font"] = "monospace",
                ["generateButton"] = true
            }
        };
    }

    private UiControl GenerateJsonControl(TemplateParameter parameter)
    {
        return new UiControl
        {
            Type = UiControlType.MultilineTextBox,
            Properties = new Dictionary<string, object>
            {
                ["rows"] = 8,
                ["placeholder"] = "{\n  \"key\": \"value\"\n}",
                ["font"] = "monospace",
                ["jsonValidation"] = true,
                ["syntaxHighlighting"] = true,
                ["formatButton"] = true
            }
        };
    }

    private UiControl GenerateUrlControl(TemplateParameter parameter)
    {
        return new UiControl
        {
            Type = UiControlType.TextBox,
            Properties = new Dictionary<string, object>
            {
                ["inputType"] = "url",
                ["placeholder"] = "https://example.com",
                ["pattern"] = @"^https?://.*",
                ["spellcheck"] = false
            },
            ValidationRules = new List<string> { "url" }
        };
    }

    private Dictionary<string, List<TemplateParameter>> GroupParametersBySection(List<TemplateParameter> parameters)
    {
        var groups = new Dictionary<string, List<TemplateParameter>>();
        
        foreach (var parameter in parameters)
        {
            var sectionName = GetParameterSection(parameter);
            
            if (!groups.ContainsKey(sectionName))
            {
                groups[sectionName] = new List<TemplateParameter>();
            }
            
            groups[sectionName].Add(parameter);
        }
        
        return groups;
    }

    private string GetParameterSection(TemplateParameter parameter)
    {
        // Group parameters by type or naming conventions
        if (parameter.Name.Contains("email", StringComparison.OrdinalIgnoreCase) || 
            parameter.Name.Contains("user", StringComparison.OrdinalIgnoreCase))
            return "User Information";
            
        if (parameter.Name.Contains("quota", StringComparison.OrdinalIgnoreCase) || 
            parameter.Name.Contains("size", StringComparison.OrdinalIgnoreCase) ||
            parameter.Name.Contains("limit", StringComparison.OrdinalIgnoreCase))
            return "Resource Settings";
            
        if (parameter.Name.Contains("password", StringComparison.OrdinalIgnoreCase) ||
            parameter.Name.Contains("security", StringComparison.OrdinalIgnoreCase) ||
            parameter.Name.Contains("auth", StringComparison.OrdinalIgnoreCase))
            return "Security Settings";
            
        if (parameter.Name.Contains("file", StringComparison.OrdinalIgnoreCase) ||
            parameter.Name.Contains("path", StringComparison.OrdinalIgnoreCase) ||
            parameter.Name.Contains("csv", StringComparison.OrdinalIgnoreCase))
            return "File Operations";
            
        return "General";
    }

    private string FormatSectionTitle(string sectionName)
    {
        return sectionName switch
        {
            "General" => "Basic Configuration",
            "User Information" => "User Details",
            "Resource Settings" => "Resource Configuration",
            "Security Settings" => "Security & Authentication",
            "File Operations" => "File & Data Operations",
            _ => sectionName
        };
    }

    private string GetSectionDescription(string sectionName)
    {
        return sectionName switch
        {
            "General" => "Essential parameters for the administrative operation",
            "User Information" => "Target user details and identification",
            "Resource Settings" => "Resource allocation and capacity settings",
            "Security Settings" => "Security policies and authentication options",
            "File Operations" => "File handling and data import/export options",
            _ => "Configuration options for this section"
        };
    }

    private string GetSectionIcon(string sectionName)
    {
        return sectionName switch
        {
            "General" => "settings",
            "User Information" => "user",
            "Resource Settings" => "server",
            "Security Settings" => "shield",
            "File Operations" => "folder",
            _ => "settings"
        };
    }

    private int GetParameterPriority(TemplateParameter parameter)
    {
        // Required parameters first
        if (parameter.Required) return 1;
        
        // Common parameters next
        if (parameter.Name.Contains("email", StringComparison.OrdinalIgnoreCase) ||
            parameter.Name.Contains("name", StringComparison.OrdinalIgnoreCase))
            return 2;
            
        return 3;
    }

    private List<FormValidationRule> GenerateFormValidationRules(AdminTemplate template)
    {
        var rules = new List<FormValidationRule>();
        
        // Add template-specific validation rules
        if (template.Id == "create-shared-mailbox")
        {
            rules.Add(new FormValidationRule
            {
                RuleId = "unique-mailbox-name",
                Description = "Ensure mailbox name is unique",
                AffectedControls = new List<string> { "name", "email" },
                ValidationExpression = "name && email && name.length > 0 && email.length > 0",
                ErrorMessage = "Both mailbox name and email address must be provided",
                Severity = ValidationSeverity.Error
            });
        }
        
        if (template.Id == "extend-mailbox")
        {
            rules.Add(new FormValidationRule
            {
                RuleId = "quota-consistency",
                Description = "Maximum quota must be greater than regular quota",
                AffectedControls = new List<string> { "quota", "quotaMax" },
                ValidationExpression = "quota < quotaMax",
                ErrorMessage = "Maximum quota must be larger than regular quota - like a safety buffer!",
                Severity = ValidationSeverity.Error
            });
        }
        
        return rules;
    }

    private string GetWittyFormHint(string templateId)
    {
        return templateId switch
        {
            "extend-mailbox" => "Expanding digital storage - like Marie Kondo for mailboxes!",
            "create-shared-mailbox" => "Creating collaboration nexus - where teamwork magic happens!",
            "add-email-alias" => "Multiplying digital identity - like having multiple superhero aliases!",
            "bulk-user-create" => "Mass user spawning - population explosion in the digital realm!",
            "reset-user-password" => "Password resurrection - breathing new life into access credentials!",
            _ => "Administrative precision awaits - configure with confidence!"
        };
    }

    private string GetWittyPlaceholder(TemplateParameter parameter)
    {
        return parameter.Type switch
        {
            ParameterType.Email => "your.email@company.com",
            ParameterType.String when parameter.Name.Contains("name", StringComparison.OrdinalIgnoreCase) => 
                "Enter a memorable name...",
            ParameterType.Integer when parameter.Name.Contains("quota", StringComparison.OrdinalIgnoreCase) => 
                "Size in GB (be generous!)",
            ParameterType.Password => "Secure password (like Fort Knox!)",
            ParameterType.FilePath => "Path to your data treasure...",
            _ => $"Enter {parameter.DisplayName.ToLower()}..."
        };
    }

    private string GetAutoCompleteHint(TemplateParameter parameter)
    {
        return parameter.Type switch
        {
            ParameterType.Email => "email",
            ParameterType.String when parameter.Name.Contains("name", StringComparison.OrdinalIgnoreCase) => "name",
            ParameterType.String when parameter.Name.Contains("user", StringComparison.OrdinalIgnoreCase) => "username",
            _ => "off"
        };
    }

    private string GetFileFilter(TemplateParameter parameter)
    {
        if (parameter.Name.Contains("csv", StringComparison.OrdinalIgnoreCase))
            return "CSV Files|*.csv|All Files|*.*";
        if (parameter.Name.Contains("json", StringComparison.OrdinalIgnoreCase))
            return "JSON Files|*.json|All Files|*.*";
        if (parameter.Name.Contains("xml", StringComparison.OrdinalIgnoreCase))
            return "XML Files|*.xml|All Files|*.*";
        
        return "All Files|*.*";
    }

    private async Task<ValidationMessage?> ValidateControlInput(UiControl control, Dictionary<string, object> inputValues)
    {
        if (!inputValues.TryGetValue(control.Name, out var value))
        {
            if (control.Required)
            {
                return new ValidationMessage
                {
                    Field = control.Name,
                    Message = $"{control.DisplayName} is required - don't leave admin fields empty!",
                    Severity = ValidationSeverity.Error,
                    Code = "REQUIRED_FIELD"
                };
            }
            return null;
        }
        
        var stringValue = value?.ToString() ?? string.Empty;
        
        // Apply validation rules
        foreach (var rule in control.ValidationRules)
        {
            var validationResult = await ApplyValidationRule(rule, stringValue, control);
            if (validationResult != null)
            {
                return validationResult;
            }
        }
        
        return null;
    }

    private async Task<ValidationMessage?> ApplyValidationRule(string rule, string value, UiControl control)
    {
        var parts = rule.Split(':', 2);
        if (parts.Length != 2) return null;
        
        var ruleType = parts[0];
        var ruleValue = parts[1];
        
        return ruleType switch
        {
            "regex" => ValidateRegex(value, ruleValue, control),
            "minlength" => ValidateMinLength(value, int.Parse(ruleValue), control),
            "maxlength" => ValidateMaxLength(value, int.Parse(ruleValue), control),
            "min" => ValidateMinValue(value, double.Parse(ruleValue), control),
            "max" => ValidateMaxValue(value, double.Parse(ruleValue), control),
            "email" => ValidateEmail(value, control),
            "url" => ValidateUrl(value, control),
            _ => await Task.FromResult<ValidationMessage?>(null)
        };
    }

    private async Task<ValidationMessage?> ApplyValidationRule(FormValidationRule rule, Dictionary<string, object> inputValues, List<UiControl> controls)
    {
        // This would implement complex cross-field validation
        // For now, return a simple implementation
        await Task.CompletedTask;
        return null;
    }

    private ValidationMessage? ValidateRegex(string value, string pattern, UiControl control)
    {
        try
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(value, pattern))
            {
                return new ValidationMessage
                {
                    Field = control.Name,
                    Message = control.ValidationMessage.IsNotEmpty() ? control.ValidationMessage : 
                        $"{control.DisplayName} format is invalid - pattern matching failed!",
                    Severity = ValidationSeverity.Error,
                    Code = "REGEX_VALIDATION"
                };
            }
        }
        catch (Exception ex)
        {
            return new ValidationMessage
            {
                Field = control.Name,
                Message = $"Validation pattern error: {ex.Message}",
                Severity = ValidationSeverity.Error,
                Code = "REGEX_ERROR"
            };
        }
        
        return null;
    }

    private ValidationMessage? ValidateMinLength(string value, int minLength, UiControl control)
    {
        if (value.Length < minLength)
        {
            return new ValidationMessage
            {
                Field = control.Name,
                Message = $"{control.DisplayName} must be at least {minLength} characters - brevity isn't always virtue!",
                Severity = ValidationSeverity.Error,
                Code = "MIN_LENGTH"
            };
        }
        
        return null;
    }

    private ValidationMessage? ValidateMaxLength(string value, int maxLength, UiControl control)
    {
        if (value.Length > maxLength)
        {
            return new ValidationMessage
            {
                Field = control.Name,
                Message = $"{control.DisplayName} must not exceed {maxLength} characters - verbosity has limits!",
                Severity = ValidationSeverity.Error,
                Code = "MAX_LENGTH"
            };
        }
        
        return null;
    }

    private ValidationMessage? ValidateMinValue(string value, double minValue, UiControl control)
    {
        if (double.TryParse(value, out var numericValue) && numericValue < minValue)
        {
            return new ValidationMessage
            {
                Field = control.Name,
                Message = $"{control.DisplayName} must be at least {minValue} - aim higher!",
                Severity = ValidationSeverity.Error,
                Code = "MIN_VALUE"
            };
        }
        
        return null;
    }

    private ValidationMessage? ValidateMaxValue(string value, double maxValue, UiControl control)
    {
        if (double.TryParse(value, out var numericValue) && numericValue > maxValue)
        {
            return new ValidationMessage
            {
                Field = control.Name,
                Message = $"{control.DisplayName} must not exceed {maxValue} - even admins have limits!",
                Severity = ValidationSeverity.Error,
                Code = "MAX_VALUE"
            };
        }
        
        return null;
    }

    private ValidationMessage? ValidateEmail(string value, UiControl control)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(value);
            if (addr.Address != value)
            {
                return new ValidationMessage
                {
                    Field = control.Name,
                    Message = $"{control.DisplayName} must be a valid email address - like a proper digital letter!",
                    Severity = ValidationSeverity.Error,
                    Code = "INVALID_EMAIL"
                };
            }
        }
        catch
        {
            return new ValidationMessage
            {
                Field = control.Name,
                Message = $"{control.DisplayName} must be a valid email address - check your @ and dots!",
                Severity = ValidationSeverity.Error,
                Code = "INVALID_EMAIL"
            };
        }
        
        return null;
    }

    private ValidationMessage? ValidateUrl(string value, UiControl control)
    {
        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) || 
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            return new ValidationMessage
            {
                Field = control.Name,
                Message = $"{control.DisplayName} must be a valid URL - starting with http:// or https://",
                Severity = ValidationSeverity.Error,
                Code = "INVALID_URL"
            };
        }
        
        return null;
    }

    private FormDefinition CreateErrorForm(AdminTemplate? template, string errorMessage)
    {
        return new FormDefinition
        {
            FormId = $"error-form-{Guid.NewGuid():N}",
            Title = template?.Name ?? "Form Error",
            Description = $"Form generation failed: {errorMessage}",
            Controls = new List<UiControl>
            {
                new()
                {
                    Name = "error",
                    DisplayName = "Error",
                    Type = UiControlType.Label,
                    Value = $"Failed to generate form: {errorMessage}",
                    Enabled = false
                }
            }
        };
    }
}