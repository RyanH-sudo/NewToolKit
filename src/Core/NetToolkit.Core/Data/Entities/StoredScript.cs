namespace NetToolkit.Core.Data.Entities;

public class StoredScript
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string ScriptType { get; set; } = string.Empty; // PowerShell, Bash, etc.
    public string Parameters { get; set; } = string.Empty; // JSON serialized parameters
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastModified { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
}