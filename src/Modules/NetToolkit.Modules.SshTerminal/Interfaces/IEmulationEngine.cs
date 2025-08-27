using NetToolkit.Modules.SshTerminal.Models;

namespace NetToolkit.Modules.SshTerminal.Interfaces;

/// <summary>
/// Emulation engine interface - the mystical transformer of digital consciousness
/// Where raw bytes become beautiful terminal experiences and keystrokes orchestrate symphonies
/// </summary>
public interface IEmulationEngine
{
    /// <summary>
    /// Parse raw output into colored terminal display - the alchemical transformation of bytes to beauty
    /// </summary>
    /// <param name="rawOutput">Raw terminal output with ANSI escape sequences</param>
    /// <param name="sessionId">Session identifier for context</param>
    /// <returns>Beautifully colored output ready for display</returns>
    Task<List<ColoredOutput>> ParseOutputAsync(string rawOutput, string sessionId);
    
    /// <summary>
    /// Parse raw output synchronously - swift transformation for immediate display
    /// </summary>
    /// <param name="rawOutput">Raw terminal output</param>
    /// <param name="sessionId">Session identifier</param>
    /// <returns>Parsed colored output</returns>
    List<ColoredOutput> ParseOutput(string rawOutput, string sessionId);
    
    /// <summary>
    /// Handle keyboard input - the symphony conductor of digital keystrokes
    /// </summary>
    /// <param name="keyEvent">Keyboard event to process</param>
    /// <param name="sessionId">Target session identifier</param>
    /// <returns>Processed input string ready for transmission</returns>
    Task<string> HandleInputAsync(KeyEvent keyEvent, string sessionId);
    
    /// <summary>
    /// Process special key combinations - the decoder of mystical key sequences
    /// </summary>
    /// <param name="keyEvent">Special key event</param>
    /// <param name="sessionId">Session context</param>
    /// <returns>Special command or control sequence</returns>
    string ProcessSpecialKey(KeyEvent keyEvent, string sessionId);
    
    /// <summary>
    /// Get command history - the scroll of digital incantations past
    /// </summary>
    /// <param name="sessionId">Session to query</param>
    /// <param name="count">Number of history entries to retrieve</param>
    /// <returns>List of historical command entries</returns>
    Task<List<CommandHistoryEntry>> GetCommandHistoryAsync(string sessionId, int count = 50);
    
    /// <summary>
    /// Add command to history - inscribing new incantations in the eternal scroll
    /// </summary>
    /// <param name="sessionId">Target session</param>
    /// <param name="command">Command to remember</param>
    /// <param name="response">Optional command response</param>
    /// <param name="success">Whether command was successful</param>
    /// <returns>Task representing the history update</returns>
    Task AddCommandToHistoryAsync(string sessionId, string command, string? response = null, bool success = true);
    
    /// <summary>
    /// Navigate command history - temporal voyages through previous incantations
    /// </summary>
    /// <param name="sessionId">Session context</param>
    /// <param name="direction">Navigation direction (up/down)</param>
    /// <returns>Historical command or empty string if at boundary</returns>
    Task<string> NavigateHistoryAsync(string sessionId, HistoryDirection direction);
    
    /// <summary>
    /// Auto-complete command - the prescient oracle of command completion
    /// </summary>
    /// <param name="partialCommand">Partially typed command</param>
    /// <param name="sessionId">Session context for environment-specific completion</param>
    /// <returns>List of completion suggestions</returns>
    Task<List<string>> GetAutoCompleteAsync(string partialCommand, string sessionId);
    
    /// <summary>
    /// Set terminal size - configuring the digital viewport dimensions
    /// </summary>
    /// <param name="sessionId">Target session</param>
    /// <param name="columns">Terminal width in characters</param>
    /// <param name="rows">Terminal height in characters</param>
    /// <returns>Success status of resize operation</returns>
    Task<bool> SetTerminalSizeAsync(string sessionId, int columns, int rows);
    
    /// <summary>
    /// Clear terminal buffer - the great digital cleansing
    /// </summary>
    /// <param name="sessionId">Session to clear</param>
    /// <returns>Task representing the clear operation</returns>
    Task ClearTerminalAsync(string sessionId);
    
    /// <summary>
    /// Reset terminal state - returning to primordial digital purity
    /// </summary>
    /// <param name="sessionId">Session to reset</param>
    /// <returns>Task representing the reset operation</returns>
    Task ResetTerminalAsync(string sessionId);
    
    /// <summary>
    /// Enable/disable echo mode - controlling the mirror of digital input
    /// </summary>
    /// <param name="sessionId">Target session</param>
    /// <param name="enabled">Whether to echo input back to terminal</param>
    /// <returns>Task representing the echo configuration</returns>
    Task SetEchoModeAsync(string sessionId, bool enabled);
    
    /// <summary>
    /// Get terminal configuration - the sacred scrolls of terminal customization
    /// </summary>
    /// <param name="sessionId">Session to query</param>
    /// <returns>Current terminal configuration</returns>
    Task<TerminalConfig> GetTerminalConfigAsync(string sessionId);
    
    /// <summary>
    /// Set terminal configuration - inscribing new customization parameters
    /// </summary>
    /// <param name="sessionId">Target session</param>
    /// <param name="config">New terminal configuration</param>
    /// <returns>Success status of configuration update</returns>
    Task<bool> SetTerminalConfigAsync(string sessionId, TerminalConfig config);
    
    /// <summary>
    /// Convert output to xterm.js format - bridging to web terminal rendering
    /// </summary>
    /// <param name="coloredOutput">Colored output to convert</param>
    /// <returns>xterm.js compatible string</returns>
    string ConvertToXtermFormat(ColoredOutput coloredOutput);
    
    /// <summary>
    /// Convert output to Three.js visualization data - transforming text to 3D artistry
    /// </summary>
    /// <param name="sessionId">Session context</param>
    /// <param name="recentLines">Number of recent lines to visualize</param>
    /// <returns>Three.js compatible visualization data</returns>
    Task<Dictionary<string, object>> ConvertToThreeJsDataAsync(string sessionId, int recentLines = 100);
}

/// <summary>
/// History navigation direction enumeration
/// </summary>
public enum HistoryDirection
{
    Up,
    Down
}