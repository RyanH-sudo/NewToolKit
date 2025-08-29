namespace NetToolkit.Modules.Education.Events;

/// <summary>
/// Event fired when an education module is loaded and ready
/// </summary>
public class ModuleLoadedEvent
{
    /// <summary>
    /// Name of the loaded module
    /// </summary>
    public required string ModuleName { get; init; }

    /// <summary>
    /// Module identifier
    /// </summary>
    public required int ModuleId { get; init; }

    /// <summary>
    /// When the module was loaded
    /// </summary>
    public DateTime LoadedAt { get; init; }

    /// <summary>
    /// Whether the module is ready for interaction
    /// </summary>
    public bool IsReady { get; init; }

    /// <summary>
    /// Module status or additional information
    /// </summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Number of lessons in the loaded module
    /// </summary>
    public int LessonCount { get; init; }
}