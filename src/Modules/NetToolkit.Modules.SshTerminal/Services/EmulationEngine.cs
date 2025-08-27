using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using NetToolkit.Modules.SshTerminal.Models;
using NetToolkit.Modules.SshTerminal.Interfaces;

namespace NetToolkit.Modules.SshTerminal.Services;

/// <summary>
/// Terminal emulation engine - the mystical transformer of digital consciousness
/// Where raw bytes become beautiful terminal experiences with cosmic color parsing
/// </summary>
public class EmulationEngine : IEmulationEngine
{
    private readonly ILogger<EmulationEngine> _logger;
    
    // Session-specific data
    private readonly ConcurrentDictionary<string, List<CommandHistoryEntry>> _commandHistory = new();
    private readonly ConcurrentDictionary<string, int> _historyPosition = new();
    private readonly ConcurrentDictionary<string, TerminalConfig> _terminalConfigs = new();
    private readonly ConcurrentDictionary<string, bool> _echoMode = new();
    
    // ANSI escape sequence patterns - the magical incantations of terminal formatting
    private static readonly Regex AnsiColorPattern = new(
        @"\x1B\[([0-9;]+)m",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    private static readonly Regex AnsiCursorPattern = new(
        @"\x1B\[([0-9]*);?([0-9]*)([HfABCDsuJK])",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    private static readonly Regex AnsiErasePattern = new(
        @"\x1B\[([0-2]?)J",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    // Color mapping for ANSI codes
    private static readonly Dictionary<int, ConsoleColor> AnsiColorMap = new()
    {
        [30] = ConsoleColor.Black,
        [31] = ConsoleColor.Red,
        [32] = ConsoleColor.Green,
        [33] = ConsoleColor.Yellow,
        [34] = ConsoleColor.Blue,
        [35] = ConsoleColor.Magenta,
        [36] = ConsoleColor.Cyan,
        [37] = ConsoleColor.White,
        [90] = ConsoleColor.DarkGray,
        [91] = ConsoleColor.Red,
        [92] = ConsoleColor.Green,
        [93] = ConsoleColor.Yellow,
        [94] = ConsoleColor.Blue,
        [95] = ConsoleColor.Magenta,
        [96] = ConsoleColor.Cyan,
        [97] = ConsoleColor.White
    };
    
    private static readonly Dictionary<int, ConsoleColor> AnsiBgColorMap = new()
    {
        [40] = ConsoleColor.Black,
        [41] = ConsoleColor.Red,
        [42] = ConsoleColor.Green,
        [43] = ConsoleColor.Yellow,
        [44] = ConsoleColor.Blue,
        [45] = ConsoleColor.Magenta,
        [46] = ConsoleColor.Cyan,
        [47] = ConsoleColor.White,
        [100] = ConsoleColor.DarkGray,
        [101] = ConsoleColor.Red,
        [102] = ConsoleColor.Green,
        [103] = ConsoleColor.Yellow,
        [104] = ConsoleColor.Blue,
        [105] = ConsoleColor.Magenta,
        [106] = ConsoleColor.Cyan,
        [107] = ConsoleColor.White
    };
    
    public EmulationEngine(ILogger<EmulationEngine> logger)
    {
        _logger = logger;
        
        _logger.LogInformation("🎨 Terminal Emulation Engine initialized - ready to transform bytes into beautiful experiences!");
    }
    
    #region IEmulationEngine Implementation
    
    /// <summary>
    /// Parse raw output into colored terminal display - alchemical byte transformation
    /// </summary>
    public async Task<List<ColoredOutput>> ParseOutputAsync(string rawOutput, string sessionId)
    {
        return await Task.FromResult(ParseOutput(rawOutput, sessionId));
    }
    
    /// <summary>
    /// Parse raw output synchronously - swift transformation for immediate display
    /// </summary>
    public List<ColoredOutput> ParseOutput(string rawOutput, string sessionId)
    {
        if (string.IsNullOrEmpty(rawOutput))
            return new List<ColoredOutput>();
        
        try
        {
            var outputs = new List<ColoredOutput>();
            var lines = rawOutput.Split(new[] { '\n', '\r' }, StringSplitOptions.None);
            
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;
                
                var parsedOutputs = ParseAnsiLine(line, sessionId);
                outputs.AddRange(parsedOutputs);
            }
            
            _logger.LogTrace("🎨 Parsed {LineCount} lines into {OutputCount} colored outputs for session {SessionId}",
                lines.Length, outputs.Count, sessionId);
            
            return outputs;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error parsing output for session {SessionId} - raw bytes resist transformation!", sessionId);
            
            // Return raw output as fallback
            return new List<ColoredOutput>
            {
                new ColoredOutput { Text = rawOutput }
            };
        }
    }
    
    /// <summary>
    /// Handle keyboard input - symphony conductor of digital keystrokes
    /// </summary>
    public async Task<string> HandleInputAsync(KeyEvent keyEvent, string sessionId)
    {
        try
        {
            // Handle special keys first
            if (keyEvent.IsSpecialKey)
            {
                var specialResult = ProcessSpecialKey(keyEvent, sessionId);
                if (!string.IsNullOrEmpty(specialResult))
                    return specialResult;
            }
            
            // Handle regular character input
            var input = keyEvent.KeyChar.ToString();
            
            // Process control characters
            if (keyEvent.Modifiers.HasFlag(ConsoleModifiers.Control))
            {
                input = ProcessControlKey(keyEvent.Key);
            }
            
            // Echo handling
            var echoEnabled = _echoMode.GetValueOrDefault(sessionId, true);
            if (echoEnabled)
            {
                _logger.LogTrace("⌨️ Input processed for session {SessionId}: Key={Key}, Char={Char}",
                    sessionId, keyEvent.Key, keyEvent.KeyChar);
            }
            
            return await Task.FromResult(input);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error handling input for session {SessionId}", sessionId);
            return string.Empty;
        }
    }
    
    /// <summary>
    /// Process special key combinations
    /// </summary>
    public string ProcessSpecialKey(KeyEvent keyEvent, string sessionId)
    {
        return keyEvent.Key switch
        {
            ConsoleKey.UpArrow => HandleHistoryNavigation(sessionId, HistoryDirection.Up),
            ConsoleKey.DownArrow => HandleHistoryNavigation(sessionId, HistoryDirection.Down),
            ConsoleKey.Tab => HandleTabCompletion(sessionId),
            ConsoleKey.Enter => "\r\n",
            ConsoleKey.Escape => "\x1B",
            ConsoleKey.Backspace => "\b",
            ConsoleKey.Delete => "\x7F",
            ConsoleKey.Home => "\x1B[H",
            ConsoleKey.End => "\x1B[F",
            ConsoleKey.PageUp => "\x1B[5~",
            ConsoleKey.PageDown => "\x1B[6~",
            ConsoleKey.F1 => "\x1B[OP",
            ConsoleKey.F2 => "\x1B[OQ",
            ConsoleKey.F3 => "\x1B[OR",
            ConsoleKey.F4 => "\x1B[OS",
            _ when keyEvent.Modifiers.HasFlag(ConsoleModifiers.Control) => ProcessControlKey(keyEvent.Key),
            _ => string.Empty
        };
    }
    
    /// <summary>
    /// Get command history
    /// </summary>
    public async Task<List<CommandHistoryEntry>> GetCommandHistoryAsync(string sessionId, int count = 50)
    {
        await Task.CompletedTask;
        
        if (!_commandHistory.TryGetValue(sessionId, out var history))
            return new List<CommandHistoryEntry>();
        
        return history.TakeLast(count).ToList();
    }
    
    /// <summary>
    /// Add command to history
    /// </summary>
    public async Task AddCommandToHistoryAsync(string sessionId, string command, string? response = null, bool success = true)
    {
        await Task.CompletedTask;
        
        var entry = new CommandHistoryEntry
        {
            Command = command,
            Response = response,
            WasSuccessful = success,
            Timestamp = DateTime.Now
        };
        
        var history = _commandHistory.GetOrAdd(sessionId, _ => new List<CommandHistoryEntry>());
        history.Add(entry);
        
        // Limit history size
        const int maxHistorySize = 1000;
        if (history.Count > maxHistorySize)
        {
            history.RemoveRange(0, history.Count - maxHistorySize);
        }
        
        // Reset history position
        _historyPosition[sessionId] = history.Count;
        
        _logger.LogTrace("📜 Added command to history for session {SessionId}: {Command}", sessionId, command);
    }
    
    /// <summary>
    /// Navigate command history
    /// </summary>
    public async Task<string> NavigateHistoryAsync(string sessionId, HistoryDirection direction)
    {
        await Task.CompletedTask;
        
        if (!_commandHistory.TryGetValue(sessionId, out var history) || history.Count == 0)
            return string.Empty;
        
        var currentPosition = _historyPosition.GetValueOrDefault(sessionId, history.Count);
        
        var newPosition = direction switch
        {
            HistoryDirection.Up => Math.Max(0, currentPosition - 1),
            HistoryDirection.Down => Math.Min(history.Count, currentPosition + 1),
            _ => currentPosition
        };
        
        _historyPosition[sessionId] = newPosition;
        
        if (newPosition >= history.Count)
            return string.Empty; // At the end, return empty for new command
        
        var command = history[newPosition].Command;
        
        _logger.LogTrace("🔄 History navigation for session {SessionId}: {Direction} to position {Position} = {Command}",
            sessionId, direction, newPosition, command);
        
        return command;
    }
    
    /// <summary>
    /// Auto-complete command
    /// </summary>
    public async Task<List<string>> GetAutoCompleteAsync(string partialCommand, string sessionId)
    {
        await Task.CompletedTask;
        
        var suggestions = new List<string>();
        
        try
        {
            // Get history-based suggestions
            if (_commandHistory.TryGetValue(sessionId, out var history))
            {
                var historySuggestions = history
                    .Where(h => h.Command.StartsWith(partialCommand, StringComparison.OrdinalIgnoreCase))
                    .Select(h => h.Command)
                    .Distinct()
                    .Take(10)
                    .ToList();
                
                suggestions.AddRange(historySuggestions);
            }
            
            // Add common command suggestions
            var commonCommands = GetCommonCommands()
                .Where(cmd => cmd.StartsWith(partialCommand, StringComparison.OrdinalIgnoreCase))
                .Take(5)
                .ToList();
            
            suggestions.AddRange(commonCommands);
            
            // Remove duplicates and sort
            suggestions = suggestions.Distinct().OrderBy(s => s).ToList();
            
            _logger.LogTrace("💡 Auto-complete for '{PartialCommand}' in session {SessionId}: {SuggestionCount} suggestions",
                partialCommand, sessionId, suggestions.Count);
            
            return suggestions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error generating auto-complete suggestions for session {SessionId}", sessionId);
            return new List<string>();
        }
    }
    
    /// <summary>
    /// Set terminal size
    /// </summary>
    public async Task<bool> SetTerminalSizeAsync(string sessionId, int columns, int rows)
    {
        await Task.CompletedTask;
        
        try
        {
            var config = GetOrCreateConfig(sessionId);
            config.BufferSize = Math.Max(rows * columns, 1000); // Ensure minimum buffer size
            
            _logger.LogDebug("📐 Terminal size set for session {SessionId}: {Columns}x{Rows}",
                sessionId, columns, rows);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error setting terminal size for session {SessionId}", sessionId);
            return false;
        }
    }
    
    /// <summary>
    /// Clear terminal
    /// </summary>
    public async Task ClearTerminalAsync(string sessionId)
    {
        await Task.CompletedTask;
        
        // This would be handled by the terminal buffer in the host service
        _logger.LogDebug("🧹 Terminal clear requested for session {SessionId}", sessionId);
    }
    
    /// <summary>
    /// Reset terminal state
    /// </summary>
    public async Task ResetTerminalAsync(string sessionId)
    {
        await Task.CompletedTask;
        
        // Reset terminal configuration to defaults
        var config = GetOrCreateConfig(sessionId);
        config.DefaultForeground = ConsoleColor.White;
        config.DefaultBackground = ConsoleColor.Black;
        
        _logger.LogDebug("🔄 Terminal reset for session {SessionId} - returned to primordial digital purity!", sessionId);
    }
    
    /// <summary>
    /// Set echo mode
    /// </summary>
    public async Task SetEchoModeAsync(string sessionId, bool enabled)
    {
        await Task.CompletedTask;
        
        _echoMode[sessionId] = enabled;
        
        _logger.LogDebug("🔊 Echo mode {Status} for session {SessionId}",
            enabled ? "enabled" : "disabled", sessionId);
    }
    
    /// <summary>
    /// Get terminal configuration
    /// </summary>
    public async Task<TerminalConfig> GetTerminalConfigAsync(string sessionId)
    {
        await Task.CompletedTask;
        return GetOrCreateConfig(sessionId);
    }
    
    /// <summary>
    /// Set terminal configuration
    /// </summary>
    public async Task<bool> SetTerminalConfigAsync(string sessionId, TerminalConfig config)
    {
        await Task.CompletedTask;
        
        try
        {
            _terminalConfigs[sessionId] = config;
            
            _logger.LogDebug("⚙️ Terminal configuration updated for session {SessionId}: {ConfigSummary}",
                sessionId, config.GetConfigSummary());
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error setting terminal configuration for session {SessionId}", sessionId);
            return false;
        }
    }
    
    /// <summary>
    /// Convert output to xterm.js format
    /// </summary>
    public string ConvertToXtermFormat(ColoredOutput coloredOutput)
    {
        return coloredOutput.ToXtermString();
    }
    
    /// <summary>
    /// Convert output to Three.js visualization data
    /// </summary>
    public async Task<Dictionary<string, object>> ConvertToThreeJsDataAsync(string sessionId, int recentLines = 100)
    {
        await Task.CompletedTask;
        
        try
        {
            var history = await GetCommandHistoryAsync(sessionId, recentLines);
            
            var visualizationData = new Dictionary<string, object>
            {
                ["session_id"] = sessionId,
                ["total_commands"] = history.Count,
                ["success_rate"] = history.Count > 0 ? (double)history.Count(h => h.WasSuccessful) / history.Count : 0.0,
                ["activity_timeline"] = history.Select(h => new
                {
                    timestamp = h.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    command_length = h.Command.Length,
                    success = h.WasSuccessful
                }).ToList(),
                ["command_patterns"] = history.GroupBy(h => h.Command.Split(' ')[0])
                    .Select(g => new { command = g.Key, count = g.Count() })
                    .OrderByDescending(x => x.count)
                    .Take(10)
                    .ToList(),
                ["color_palette"] = new[]
                {
                    "#00ff41", // Matrix green for success
                    "#ff073a", // Alert red for errors
                    "#39c5bb", // Cyan for info
                    "#ffd700", // Gold for warnings
                    "#8a2be2"  // Purple for special commands
                },
                ["effects"] = new
                {
                    glow_intensity = 0.7,
                    particle_density = history.Count * 0.1,
                    connection_strength = Math.Min(1.0, history.Count / 100.0)
                }
            };
            
            _logger.LogTrace("🎭 Three.js visualization data generated for session {SessionId}: {DataPoints} points",
                sessionId, history.Count);
            
            return visualizationData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error generating Three.js visualization data for session {SessionId}", sessionId);
            
            return new Dictionary<string, object>
            {
                ["error"] = ex.Message,
                ["session_id"] = sessionId
            };
        }
    }
    
    #endregion
    
    #region ANSI Parsing Implementation
    
    /// <summary>
    /// Parse a single line with ANSI escape sequences
    /// </summary>
    private List<ColoredOutput> ParseAnsiLine(string line, string sessionId)
    {
        var outputs = new List<ColoredOutput>();
        
        if (string.IsNullOrEmpty(line))
        {
            outputs.Add(new ColoredOutput { Text = line });
            return outputs;
        }
        
        var position = 0;
        var currentForeground = GetOrCreateConfig(sessionId).DefaultForeground;
        var currentBackground = GetOrCreateConfig(sessionId).DefaultBackground;
        var isBold = false;
        var isUnderline = false;
        var isItalic = false;
        
        // Find all ANSI escape sequences
        var matches = AnsiColorPattern.Matches(line);
        
        foreach (Match match in matches)
        {
            // Add text before this escape sequence
            if (match.Index > position)
            {
                var textBefore = line[position..match.Index];
                if (!string.IsNullOrEmpty(textBefore))
                {
                    outputs.Add(new ColoredOutput
                    {
                        Text = textBefore,
                        Foreground = currentForeground,
                        Background = currentBackground,
                        IsBold = isBold,
                        IsUnderline = isUnderline,
                        IsItalic = isItalic
                    });
                }
            }
            
            // Process the ANSI code
            var codes = match.Groups[1].Value.Split(';');
            foreach (var codeStr in codes)
            {
                if (int.TryParse(codeStr, out var code))
                {
                    switch (code)
                    {
                        case 0: // Reset
                            currentForeground = GetOrCreateConfig(sessionId).DefaultForeground;
                            currentBackground = GetOrCreateConfig(sessionId).DefaultBackground;
                            isBold = isUnderline = isItalic = false;
                            break;
                        case 1: // Bold
                            isBold = true;
                            break;
                        case 3: // Italic
                            isItalic = true;
                            break;
                        case 4: // Underline
                            isUnderline = true;
                            break;
                        case 22: // Normal intensity
                            isBold = false;
                            break;
                        case 23: // Not italic
                            isItalic = false;
                            break;
                        case 24: // Not underlined
                            isUnderline = false;
                            break;
                        default:
                            // Foreground colors
                            if (AnsiColorMap.TryGetValue(code, out var fgColor))
                                currentForeground = fgColor;
                            // Background colors
                            else if (AnsiBgColorMap.TryGetValue(code, out var bgColor))
                                currentBackground = bgColor;
                            break;
                    }
                }
            }
            
            position = match.Index + match.Length;
        }
        
        // Add remaining text
        if (position < line.Length)
        {
            var remainingText = line[position..];
            if (!string.IsNullOrEmpty(remainingText))
            {
                outputs.Add(new ColoredOutput
                {
                    Text = remainingText,
                    Foreground = currentForeground,
                    Background = currentBackground,
                    IsBold = isBold,
                    IsUnderline = isUnderline,
                    IsItalic = isItalic
                });
            }
        }
        
        // If no ANSI codes found, add the whole line
        if (outputs.Count == 0)
        {
            outputs.Add(new ColoredOutput
            {
                Text = line,
                Foreground = currentForeground,
                Background = currentBackground
            });
        }
        
        return outputs;
    }
    
    #endregion
    
    #region Helper Methods
    
    private TerminalConfig GetOrCreateConfig(string sessionId)
    {
        return _terminalConfigs.GetOrAdd(sessionId, _ => new TerminalConfig());
    }
    
    private string HandleHistoryNavigation(string sessionId, HistoryDirection direction)
    {
        var task = NavigateHistoryAsync(sessionId, direction);
        return task.Result;
    }
    
    private string HandleTabCompletion(string sessionId)
    {
        // This would need current input context to work properly
        // For now, return tab character
        return "\t";
    }
    
    private string ProcessControlKey(ConsoleKey key)
    {
        return key switch
        {
            ConsoleKey.C => "\x03", // Ctrl+C (SIGINT)
            ConsoleKey.D => "\x04", // Ctrl+D (EOF)
            ConsoleKey.Z => "\x1A", // Ctrl+Z (SUSP)
            ConsoleKey.L => "\x0C", // Ctrl+L (Clear screen)
            ConsoleKey.U => "\x15", // Ctrl+U (Kill line)
            ConsoleKey.K => "\x0B", // Ctrl+K (Kill to end of line)
            ConsoleKey.A => "\x01", // Ctrl+A (Beginning of line)
            ConsoleKey.E => "\x05", // Ctrl+E (End of line)
            _ => string.Empty
        };
    }
    
    private List<string> GetCommonCommands()
    {
        return new List<string>
        {
            "ls", "cd", "pwd", "cat", "grep", "find", "ps", "top", "kill",
            "mkdir", "rmdir", "cp", "mv", "rm", "chmod", "chown", "sudo",
            "ssh", "scp", "rsync", "curl", "wget", "ping", "netstat",
            "ifconfig", "ip", "route", "iptables", "systemctl", "service",
            "docker", "kubectl", "git", "vim", "nano", "less", "more",
            "tail", "head", "sort", "uniq", "wc", "awk", "sed", "tar",
            "gzip", "unzip", "df", "du", "free", "uptime", "whoami", "who"
        };
    }
    
    #endregion
}