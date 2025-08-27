namespace NetToolkit.Modules.PowerShell.Interfaces;

/// <summary>
/// Script template service - turning mundane tasks into magnificent one-click wonders
/// </summary>
public interface IScriptTemplateService
{
    /// <summary>
    /// Load all templates from the sacred scriptorium
    /// </summary>
    Task<List<ScriptTemplate>> LoadTemplatesAsync();
    
    /// <summary>
    /// Get a specific template by ID - like summoning a specific spell
    /// </summary>
    Task<ScriptTemplate?> GetTemplateAsync(string templateId);
    
    /// <summary>
    /// Generate a script from template with parameters - alchemy at its finest
    /// </summary>
    Task<string> GenerateScriptAsync(string templateId, Dictionary<string, object> parameters);
    
    /// <summary>
    /// Get template categories - because organization is the key to sanity
    /// </summary>
    Task<List<string>> GetCategoriesAsync();
    
    /// <summary>
    /// Search templates by name or description - finding needles in digital haystacks
    /// </summary>
    Task<List<ScriptTemplate>> SearchTemplatesAsync(string searchTerm);
    
    /// <summary>
    /// Validate template parameters - preventing catastrophic parameter disasters
    /// </summary>
    Task<ValidationResult> ValidateParametersAsync(string templateId, Dictionary<string, object> parameters);
    
    /// <summary>
    /// Save custom template - because sometimes you create magic worth keeping
    /// </summary>
    Task<bool> SaveTemplateAsync(ScriptTemplate template);
}

/// <summary>
/// Script template definition - the blueprint for automation excellence
/// </summary>
public class ScriptTemplate
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Template { get; set; } = string.Empty;
    public List<TemplateParameter> Parameters { get; set; } = new();
    public string Author { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0.0";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastModified { get; set; } = DateTime.UtcNow;
    public List<string> Tags { get; set; } = new();
    public TemplateRisk RiskLevel { get; set; } = TemplateRisk.Low;
    public string HelpText { get; set; } = string.Empty;
    public List<string> Prerequisites { get; set; } = new();
}

/// <summary>
/// Template parameter definition - because parameters need structure too
/// </summary>
public class TemplateParameter
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ParameterType Type { get; set; } = ParameterType.String;
    public bool IsRequired { get; set; } = true;
    public object? DefaultValue { get; set; }
    public List<string> ValidValues { get; set; } = new(); // For dropdowns
    public string ValidationPattern { get; set; } = string.Empty; // Regex validation
    public string PlaceholderText { get; set; } = string.Empty;
    public string HelpText { get; set; } = string.Empty;
}

/// <summary>
/// Parameter types - because not all data is created equal
/// </summary>
public enum ParameterType
{
    String,
    Integer,
    Boolean,
    Email,
    IPAddress,
    FilePath,
    Password,
    MultilineString,
    Selection, // Dropdown
    MultiSelection, // Multiple choice
    Date,
    Number
}

/// <summary>
/// Template risk levels - because with great power comes great responsibility
/// </summary>
public enum TemplateRisk
{
    Low,      // Safe operations like Get- commands
    Medium,   // Modify operations like Set- commands  
    High,     // Potentially destructive like Remove- commands
    Critical  // System-level changes that could cause outages
}

/// <summary>
/// Validation result - success or a list of reasons why things went sideways
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public Dictionary<string, string> SanitizedValues { get; set; } = new();
}