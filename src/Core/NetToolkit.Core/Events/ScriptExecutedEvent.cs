namespace NetToolkit.Core.Events;

public class ScriptExecutedEvent
{
    public string ScriptId { get; set; } = string.Empty;
    public string ScriptName { get; set; } = string.Empty;
    public string ScriptType { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string Output { get; set; } = string.Empty;
    public string ErrorOutput { get; set; } = string.Empty;
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
    public TimeSpan Duration { get; set; }
    public string ExecutedBy { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
}