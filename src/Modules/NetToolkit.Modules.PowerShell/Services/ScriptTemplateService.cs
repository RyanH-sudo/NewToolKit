using System.Text.Json;
using System.Text.RegularExpressions;
using NetToolkit.Modules.PowerShell.Interfaces;
using NetToolkit.Core.Interfaces;

namespace NetToolkit.Modules.PowerShell.Services;

/// <summary>
/// Script Template Service - The digital alchemist transforming templates into executable gold!
/// Where mundane admin tasks become one-click wonders through the magic of templating.
/// </summary>
public class ScriptTemplateService : IScriptTemplateService
{
    private readonly ILoggerWrapper _logger;
    private readonly string _templatesPath;
    private readonly Dictionary<string, ScriptTemplate> _templateCache;
    private readonly SemaphoreSlim _cacheLock;
    
    public ScriptTemplateService(ILoggerWrapper logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _templatesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "ScriptTemplates");
        _templateCache = new Dictionary<string, ScriptTemplate>();
        _cacheLock = new SemaphoreSlim(1, 1);
        
        // Ensure templates directory exists - even digital alchemists need a workshop
        Directory.CreateDirectory(_templatesPath);
        
        // Initialize with built-in templates - the sacred scrolls of MSP wisdom
        _ = InitializeBuiltInTemplatesAsync();
        
        _logger.LogInfo("Script Template Service initialized - The digital spellbook is open! 📚");
    }
    
    /// <summary>
    /// Load all templates from the sacred scriptorium
    /// </summary>
    public async Task<List<ScriptTemplate>> LoadTemplatesAsync()
    {
        await _cacheLock.WaitAsync();
        try
        {
            if (_templateCache.Count == 0)
            {
                await LoadTemplatesFromFileSystemAsync();
            }
            
            var templates = _templateCache.Values.ToList();
            _logger.LogDebug("Loaded {Count} templates - The grimoire grows! 📜", templates.Count);
            
            return templates;
        }
        finally
        {
            _cacheLock.Release();
        }
    }
    
    /// <summary>
    /// Get a specific template by ID - summoning a specific spell from the grimoire
    /// </summary>
    public async Task<ScriptTemplate?> GetTemplateAsync(string templateId)
    {
        await _cacheLock.WaitAsync();
        try
        {
            if (_templateCache.Count == 0)
            {
                await LoadTemplatesFromFileSystemAsync();
            }
            
            _templateCache.TryGetValue(templateId, out var template);
            
            if (template != null)
            {
                _logger.LogDebug("Template '{TemplateId}' retrieved - Spell found in grimoire! ✨", templateId);
            }
            else
            {
                _logger.LogWarn("Template '{TemplateId}' not found - The spell remains hidden! 👻", templateId);
            }
            
            return template;
        }
        finally
        {
            _cacheLock.Release();
        }
    }
    
    /// <summary>
    /// Generate a script from template with parameters - alchemy at its finest
    /// </summary>
    public async Task<string> GenerateScriptAsync(string templateId, Dictionary<string, object> parameters)
    {
        var template = await GetTemplateAsync(templateId);
        if (template == null)
        {
            var errorMsg = $"Template '{templateId}' not found - Cannot conjure what doesn't exist!";
            _logger.LogError(errorMsg);
            throw new ArgumentException(errorMsg, nameof(templateId));
        }
        
        try
        {
            // Validate parameters first - preventing catastrophic incantations
            var validation = await ValidateParametersAsync(templateId, parameters);
            if (!validation.IsValid)
            {
                var errorMsg = $"Parameter validation failed: {string.Join(", ", validation.Errors)}";
                _logger.LogError("Template generation failed - {Error}", errorMsg);
                throw new ArgumentException(errorMsg);
            }
            
            // Use sanitized values for generation
            var sanitizedParams = validation.SanitizedValues.ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value);
            
            // Perform template substitution - the magical transformation
            var generatedScript = PerformTemplateSubstitution(template.Template, sanitizedParams);
            
            _logger.LogInfo("Script generated from template '{TemplateId}' - Digital alchemy complete! ⚗️", templateId);
            
            return generatedScript;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Script generation failed for template '{TemplateId}' - The spell backfired!", templateId);
            throw;
        }
    }
    
    /// <summary>
    /// Get template categories - organizing the digital spellbook
    /// </summary>
    public async Task<List<string>> GetCategoriesAsync()
    {
        var templates = await LoadTemplatesAsync();
        var categories = templates
            .Select(t => t.Category)
            .Where(c => !string.IsNullOrEmpty(c))
            .Distinct()
            .OrderBy(c => c)
            .ToList();
        
        _logger.LogDebug("Retrieved {Count} categories - The spellbook is well organized! 📚", categories.Count);
        return categories;
    }
    
    /// <summary>
    /// Search templates by name or description - finding needles in digital haystacks
    /// </summary>
    public async Task<List<ScriptTemplate>> SearchTemplatesAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await LoadTemplatesAsync();
        
        var templates = await LoadTemplatesAsync();
        var searchTermLower = searchTerm.ToLowerInvariant();
        
        var matchingTemplates = templates
            .Where(t => 
                t.Name.ToLowerInvariant().Contains(searchTermLower) ||
                t.Description.ToLowerInvariant().Contains(searchTermLower) ||
                t.Tags.Any(tag => tag.ToLowerInvariant().Contains(searchTermLower)))
            .ToList();
        
        _logger.LogDebug("Found {Count} templates matching '{SearchTerm}' - The search yields fruit! 🔍", 
                        matchingTemplates.Count, searchTerm);
        
        return matchingTemplates;
    }
    
    /// <summary>
    /// Validate template parameters - preventing catastrophic parameter disasters
    /// </summary>
    public async Task<ValidationResult> ValidateParametersAsync(string templateId, Dictionary<string, object> parameters)
    {
        var template = await GetTemplateAsync(templateId);
        if (template == null)
        {
            return new ValidationResult
            {
                IsValid = false,
                Errors = { $"Template '{templateId}' not found" }
            };
        }
        
        var result = new ValidationResult { IsValid = true };
        
        // Check required parameters
        foreach (var param in template.Parameters.Where(p => p.IsRequired))
        {
            if (!parameters.ContainsKey(param.Name) || parameters[param.Name] == null)
            {
                result.Errors.Add($"Required parameter '{param.DisplayName}' is missing");
                continue;
            }
            
            var value = parameters[param.Name].ToString() ?? string.Empty;
            
            // Validate parameter type and format
            var paramValidation = ValidateParameterValue(param, value);
            if (!paramValidation.IsValid)
            {
                result.Errors.AddRange(paramValidation.Errors);
            }
            else
            {
                result.SanitizedValues[param.Name] = paramValidation.SanitizedValue;
            }
        }
        
        // Process optional parameters
        foreach (var param in template.Parameters.Where(p => !p.IsRequired))
        {
            if (parameters.ContainsKey(param.Name) && parameters[param.Name] != null)
            {
                var value = parameters[param.Name].ToString() ?? string.Empty;
                var paramValidation = ValidateParameterValue(param, value);
                
                if (!paramValidation.IsValid)
                {
                    result.Warnings.AddRange(paramValidation.Errors);
                    // Use default value if validation fails for optional params
                    result.SanitizedValues[param.Name] = param.DefaultValue?.ToString() ?? string.Empty;
                }
                else
                {
                    result.SanitizedValues[param.Name] = paramValidation.SanitizedValue;
                }
            }
            else
            {
                // Use default value for optional parameters
                result.SanitizedValues[param.Name] = param.DefaultValue?.ToString() ?? string.Empty;
            }
        }
        
        result.IsValid = result.Errors.Count == 0;
        
        if (result.IsValid)
        {
            _logger.LogDebug("Parameter validation successful for template '{TemplateId}' - All spells components are pure! ✅", templateId);
        }
        else
        {
            _logger.LogWarn("Parameter validation failed for template '{TemplateId}' - {ErrorCount} errors detected", 
                           templateId, result.Errors.Count);
        }
        
        return result;
    }
    
    /// <summary>
    /// Save custom template - preserving digital magic for posterity
    /// </summary>
    public async Task<bool> SaveTemplateAsync(ScriptTemplate template)
    {
        try
        {
            var templateJson = JsonSerializer.Serialize(template, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            
            var filePath = Path.Combine(_templatesPath, $"{template.Id}.json");
            await File.WriteAllTextAsync(filePath, templateJson);
            
            // Update cache
            await _cacheLock.WaitAsync();
            try
            {
                _templateCache[template.Id] = template;
            }
            finally
            {
                _cacheLock.Release();
            }
            
            _logger.LogInfo("Template '{TemplateId}' saved successfully - The spell is preserved! 💾", template.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save template '{TemplateId}' - The spell remains ephemeral!", template.Id);
            return false;
        }
    }
    
    /// <summary>
    /// Load templates from file system - reading the ancient scrolls
    /// </summary>
    private async Task LoadTemplatesFromFileSystemAsync()
    {
        try
        {
            var templateFiles = Directory.GetFiles(_templatesPath, "*.json");
            
            foreach (var file in templateFiles)
            {
                try
                {
                    var json = await File.ReadAllTextAsync(file);
                    var template = JsonSerializer.Deserialize<ScriptTemplate>(json);
                    
                    if (template != null && !string.IsNullOrEmpty(template.Id))
                    {
                        _templateCache[template.Id] = template;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to load template from file {File} - Corrupted scroll detected!", file);
                }
            }
            
            _logger.LogInfo("Loaded {Count} templates from file system - The grimoire is refreshed! 📖", _templateCache.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load templates from file system - The scriptorium is inaccessible!");
        }
    }
    
    /// <summary>
    /// Initialize built-in templates - the fundamental spells of MSP mastery
    /// </summary>
    private async Task InitializeBuiltInTemplatesAsync()
    {
        var builtInTemplates = new List<ScriptTemplate>
        {
            // Mailbox Extension Template - because everyone needs more digital space
            new ScriptTemplate
            {
                Id = "extend-mailbox",
                Name = "Extend Mailbox Quota",
                Description = "Expand mailbox quota for a user - because digital hoarding is a real thing!",
                Category = "Microsoft 365",
                RiskLevel = TemplateRisk.Medium,
                Template = @"
# Extending mailbox quota for {UserEmail} to {QuotaGB}GB
# Because everyone deserves more digital storage space! 📦

try {
    Write-Host ""Connecting to Exchange Online - Opening the gates to the cloud kingdom..."" -ForegroundColor Cyan
    Connect-ExchangeOnline -ShowProgress $false
    
    Write-Host ""Extending mailbox for {UserEmail} to {QuotaGB}GB..."" -ForegroundColor Yellow
    Set-Mailbox -Identity ""{UserEmail}"" -ProhibitSendQuota ""{QuotaGB}GB"" -ProhibitSendReceiveQuota ""{QuotaGB}GB"" -IssueWarningQuota ""{WarningGB}GB""
    
    Write-Host ""✅ Mailbox successfully extended! Digital horizons have been expanded!"" -ForegroundColor Green
    
    # Get updated mailbox stats
    $mailbox = Get-Mailbox -Identity ""{UserEmail}""
    $stats = Get-MailboxStatistics -Identity ""{UserEmail}""
    
    Write-Host ""📊 Updated Mailbox Statistics:"" -ForegroundColor Cyan
    Write-Host ""   Current Size: $($stats.TotalItemSize)"" -ForegroundColor White
    Write-Host ""   New Quota: $($mailbox.ProhibitSendReceiveQuota)"" -ForegroundColor White
    Write-Host ""   Warning Level: $($mailbox.IssueWarningQuota)"" -ForegroundColor White
}
catch {
    Write-Error ""❌ Mailbox extension failed: $($_.Exception.Message)""
    Write-Host ""The digital storage gods are displeased! Check permissions and try again."" -ForegroundColor Red
}
finally {
    Disconnect-ExchangeOnline -Confirm:$false -ErrorAction SilentlyContinue
    Write-Host ""🚪 Disconnected from Exchange Online - The gates are sealed."" -ForegroundColor Gray
}",
                Parameters = new List<TemplateParameter>
                {
                    new()
                    {
                        Name = "UserEmail",
                        DisplayName = "User Email",
                        Description = "The email address of the user whose mailbox will be extended",
                        Type = ParameterType.Email,
                        IsRequired = true,
                        PlaceholderText = "user@company.com",
                        HelpText = "Enter the full email address of the user"
                    },
                    new()
                    {
                        Name = "QuotaGB", 
                        DisplayName = "New Quota (GB)",
                        Description = "The new mailbox quota size in gigabytes",
                        Type = ParameterType.Integer,
                        IsRequired = true,
                        DefaultValue = 100,
                        PlaceholderText = "100",
                        HelpText = "Enter the new quota size (recommended: 50-500 GB)"
                    },
                    new()
                    {
                        Name = "WarningGB",
                        DisplayName = "Warning Quota (GB)", 
                        Description = "Warning threshold (typically 90% of quota)",
                        Type = ParameterType.Integer,
                        IsRequired = false,
                        DefaultValue = 90,
                        PlaceholderText = "90",
                        HelpText = "Warning will be sent when mailbox reaches this size"
                    }
                },
                Tags = { "exchange", "mailbox", "quota", "microsoft365" },
                Author = "NetToolkit",
                HelpText = "This template extends a user's mailbox quota in Exchange Online. Ensure you have Exchange admin privileges before running."
            },
            
            // Convert to Shared Mailbox Template - the communal transformation spell
            new ScriptTemplate
            {
                Id = "convert-shared-mailbox",
                Name = "Convert to Shared Mailbox",
                Description = "Transform a user mailbox into a shared mailbox - sharing is caring!",
                Category = "Microsoft 365",
                RiskLevel = TemplateRisk.High,
                Template = @"
# Converting {UserEmail} to shared mailbox
# Transformation magic: From personal to communal! 🤝

try {
    Write-Host ""Connecting to Exchange Online - Preparing for magical transformation..."" -ForegroundColor Cyan
    Connect-ExchangeOnline -ShowProgress $false
    
    Write-Host ""🔄 Converting {UserEmail} to shared mailbox..."" -ForegroundColor Yellow
    Set-Mailbox -Identity ""{UserEmail}"" -Type Shared
    
    Write-Host ""✅ Conversion complete! The mailbox is now shared with the world!"" -ForegroundColor Green
    
    # Remove the license if requested
    if (""{RemoveLicense}"" -eq ""true"") {
        Write-Host ""🎫 Removing user license..."" -ForegroundColor Yellow
        Connect-MsolService
        Set-MsolUserLicense -UserPrincipalName ""{UserEmail}"" -RemoveLicenses (Get-MsolUser -UserPrincipalName ""{UserEmail}"").Licenses.AccountSkuId
        Write-Host ""✅ License removed - Communism in the digital age!"" -ForegroundColor Green
    }
    
    # Set permissions if delegates specified
    if (![string]::IsNullOrEmpty(""{Delegates}"")) {
        $delegates = ""{Delegates}"" -split "",""
        foreach ($delegate in $delegates) {
            $delegate = $delegate.Trim()
            Write-Host ""👥 Adding delegate: $delegate"" -ForegroundColor Cyan
            Add-MailboxPermission -Identity ""{UserEmail}"" -User $delegate -AccessRights FullAccess -InheritanceType All
            Add-RecipientPermission -Identity ""{UserEmail}"" -Trustee $delegate -AccessRights SendAs -Confirm:$false
        }
        Write-Host ""✅ Delegates configured - The fellowship is complete!"" -ForegroundColor Green
    }
}
catch {
    Write-Error ""❌ Conversion failed: $($_.Exception.Message)""
    Write-Host ""The sharing spirits are reluctant! Check permissions and try again."" -ForegroundColor Red
}
finally {
    Disconnect-ExchangeOnline -Confirm:$false -ErrorAction SilentlyContinue
    Write-Host ""🚪 Transformation complete - The ritual ends."" -ForegroundColor Gray
}",
                Parameters = new List<TemplateParameter>
                {
                    new()
                    {
                        Name = "UserEmail",
                        DisplayName = "User Email",
                        Description = "Email address of the mailbox to convert",
                        Type = ParameterType.Email,
                        IsRequired = true,
                        PlaceholderText = "departing.user@company.com"
                    },
                    new()
                    {
                        Name = "RemoveLicense",
                        DisplayName = "Remove License",
                        Description = "Remove the user's Microsoft 365 license after conversion",
                        Type = ParameterType.Boolean,
                        IsRequired = false,
                        DefaultValue = true,
                        HelpText = "Shared mailboxes under 50GB don't require licenses"
                    },
                    new()
                    {
                        Name = "Delegates",
                        DisplayName = "Delegate Emails",
                        Description = "Comma-separated list of users to grant access (optional)",
                        Type = ParameterType.String,
                        IsRequired = false,
                        PlaceholderText = "manager@company.com, assistant@company.com",
                        HelpText = "Users who will have full access to this shared mailbox"
                    }
                },
                Tags = { "exchange", "shared-mailbox", "conversion", "microsoft365" },
                Author = "NetToolkit"
            },
            
            // Add Email Alias Template - the identity multiplier
            new ScriptTemplate
            {
                Id = "add-email-alias",
                Name = "Add Email Alias",
                Description = "Give a mailbox a secret identity - like a superhero cape for email!",
                Category = "Microsoft 365", 
                RiskLevel = TemplateRisk.Low,
                Template = @"
# Adding email alias {NewAlias} to {UserEmail}
# Secret identity activation - every hero needs an alter ego! 🎭

try {
    Write-Host ""Connecting to Exchange Online - Summoning the identity forge..."" -ForegroundColor Cyan
    Connect-ExchangeOnline -ShowProgress $false
    
    Write-Host ""🎭 Adding alias '{NewAlias}' to {UserEmail}..."" -ForegroundColor Yellow
    Set-Mailbox -Identity ""{UserEmail}"" -EmailAddresses @{add=""{NewAlias}""}
    
    Write-Host ""✅ Alias successfully added! The secret identity is born!"" -ForegroundColor Green
    
    # Verify the alias was added
    $mailbox = Get-Mailbox -Identity ""{UserEmail}""
    Write-Host ""📧 Current email addresses:"" -ForegroundColor Cyan
    $mailbox.EmailAddresses | ForEach-Object {
        if ($_.ToString().StartsWith(""SMTP:"")) {
            $address = $_.ToString().Substring(5)
            if ($address -eq $mailbox.PrimarySmtpAddress) {
                Write-Host ""   ✓ $address (Primary)"" -ForegroundColor Green
            } else {
                Write-Host ""   ✓ $address (Alias)"" -ForegroundColor White
            }
        }
    }
}
catch {
    Write-Error ""❌ Alias addition failed: $($_.Exception.Message)""
    Write-Host ""The identity forge is jammed! Check domain validity and permissions."" -ForegroundColor Red
}
finally {
    Disconnect-ExchangeOnline -Confirm:$false -ErrorAction SilentlyContinue
    Write-Host ""🚪 Identity forge sealed - The magic is complete."" -ForegroundColor Gray
}",
                Parameters = new List<TemplateParameter>
                {
                    new()
                    {
                        Name = "UserEmail",
                        DisplayName = "User Email",
                        Description = "Primary email address of the user",
                        Type = ParameterType.Email,
                        IsRequired = true,
                        PlaceholderText = "john.doe@company.com"
                    },
                    new()
                    {
                        Name = "NewAlias", 
                        DisplayName = "New Alias",
                        Description = "The new email alias to add",
                        Type = ParameterType.Email,
                        IsRequired = true,
                        PlaceholderText = "j.doe@company.com",
                        HelpText = "Must be in the same domain or verified domain"
                    }
                },
                Tags = { "exchange", "alias", "email", "identity" },
                Author = "NetToolkit"
            }
        };
        
        // Save built-in templates to file system
        foreach (var template in builtInTemplates)
        {
            await SaveTemplateAsync(template);
        }
        
        _logger.LogInfo("Built-in templates initialized - The sacred scrolls are inscribed! 📜");
    }
    
    /// <summary>
    /// Validate individual parameter value - ensuring digital purity
    /// </summary>
    private static ParameterValidationResult ValidateParameterValue(TemplateParameter param, string value)
    {
        var result = new ParameterValidationResult { IsValid = true, SanitizedValue = value };
        
        // Type-specific validation
        switch (param.Type)
        {
            case ParameterType.Email:
                if (!IsValidEmail(value))
                {
                    result.IsValid = false;
                    result.Errors.Add($"'{param.DisplayName}' must be a valid email address");
                }
                break;
                
            case ParameterType.Integer:
                if (!int.TryParse(value, out var intValue))
                {
                    result.IsValid = false;
                    result.Errors.Add($"'{param.DisplayName}' must be a valid integer");
                }
                else
                {
                    result.SanitizedValue = intValue.ToString();
                }
                break;
                
            case ParameterType.Boolean:
                if (!bool.TryParse(value, out var boolValue))
                {
                    result.IsValid = false;
                    result.Errors.Add($"'{param.DisplayName}' must be true or false");
                }
                else
                {
                    result.SanitizedValue = boolValue.ToString().ToLowerInvariant();
                }
                break;
                
            case ParameterType.IPAddress:
                if (!System.Net.IPAddress.TryParse(value, out _))
                {
                    result.IsValid = false;
                    result.Errors.Add($"'{param.DisplayName}' must be a valid IP address");
                }
                break;
        }
        
        // Pattern validation
        if (result.IsValid && !string.IsNullOrEmpty(param.ValidationPattern))
        {
            try
            {
                if (!Regex.IsMatch(value, param.ValidationPattern))
                {
                    result.IsValid = false;
                    result.Errors.Add($"'{param.DisplayName}' does not match the required format");
                }
            }
            catch (RegexMatchTimeoutException)
            {
                result.IsValid = false;
                result.Errors.Add($"'{param.DisplayName}' validation timed out");
            }
        }
        
        // Valid values check
        if (result.IsValid && param.ValidValues.Count > 0)
        {
            if (!param.ValidValues.Contains(value, StringComparer.OrdinalIgnoreCase))
            {
                result.IsValid = false;
                result.Errors.Add($"'{param.DisplayName}' must be one of: {string.Join(", ", param.ValidValues)}");
            }
        }
        
        return result;
    }
    
    /// <summary>
    /// Perform template substitution - the magical transformation process
    /// </summary>
    private static string PerformTemplateSubstitution(string template, Dictionary<string, object> parameters)
    {
        var result = template;
        
        foreach (var param in parameters)
        {
            var placeholder = $"{{{param.Key}}}";
            var value = param.Value?.ToString() ?? string.Empty;
            result = result.Replace(placeholder, value);
        }
        
        return result;
    }
    
    /// <summary>
    /// Validate email address format - because not all strings are emails
    /// </summary>
    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;
        
        try
        {
            var mailAddress = new System.Net.Mail.MailAddress(email);
            return mailAddress.Address == email;
        }
        catch
        {
            return false;
        }
    }
}

/// <summary>
/// Parameter validation result - the verdict of digital purity inspection
/// </summary>
internal class ParameterValidationResult
{
    public bool IsValid { get; set; }
    public string SanitizedValue { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
}