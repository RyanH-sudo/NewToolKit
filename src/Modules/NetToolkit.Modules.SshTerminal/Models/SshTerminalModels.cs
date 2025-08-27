using System.ComponentModel;
using System.Text;
using Newtonsoft.Json;

namespace NetToolkit.Modules.SshTerminal.Models;

/// <summary>
/// SSH terminal models - the legendary arcana of digital connection wizardry
/// Where bytes become bridges and terminals transform into transcendent gateways
/// </summary>

#region Connection Models

/// <summary>
/// Connection parameters - the sacred geometry of digital bridge construction
/// </summary>
public class ConnectionParams
{
    /// <summary>
    /// Unique identifier for this connection session
    /// </summary>
    public string SessionId { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Connection type - the flavor of digital communion
    /// </summary>
    public ConnectionType Type { get; set; }
    
    /// <summary>
    /// Target hostname or IP address for network connections
    /// </summary>
    public string? Host { get; set; }
    
    /// <summary>
    /// Port number for network connections
    /// </summary>
    public int Port { get; set; } = 22;
    
    /// <summary>
    /// Username for authentication
    /// </summary>
    public string? Username { get; set; }
    
    /// <summary>
    /// Password for authentication (encrypted in storage)
    /// </summary>
    public string? Password { get; set; }
    
    /// <summary>
    /// Private key path for key-based authentication
    /// </summary>
    public string? PrivateKeyPath { get; set; }
    
    /// <summary>
    /// Serial port name (e.g., COM1, /dev/ttyUSB0)
    /// </summary>
    public string? SerialPort { get; set; }
    
    /// <summary>
    /// Baud rate for serial connections
    /// </summary>
    public int BaudRate { get; set; } = 9600;
    
    /// <summary>
    /// Data bits for serial connections
    /// </summary>
    public int DataBits { get; set; } = 8;
    
    /// <summary>
    /// Stop bits for serial connections
    /// </summary>
    public System.IO.Ports.StopBits StopBits { get; set; } = System.IO.Ports.StopBits.One;
    
    /// <summary>
    /// Parity for serial connections
    /// </summary>
    public System.IO.Ports.Parity Parity { get; set; } = System.IO.Ports.Parity.None;
    
    /// <summary>
    /// Bluetooth device address
    /// </summary>
    public string? BluetoothAddress { get; set; }
    
    /// <summary>
    /// Connection timeout in seconds
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;
    
    /// <summary>
    /// Auto-reconnect on disconnect
    /// </summary>
    public bool AutoReconnect { get; set; } = true;
    
    /// <summary>
    /// Enable logging for this session
    /// </summary>
    public bool EnableLogging { get; set; } = true;
    
    /// <summary>
    /// Custom display name for this connection
    /// </summary>
    public string DisplayName { get; set; } = "Untitled Connection";
    
    /// <summary>
    /// Get witty connection description based on type
    /// </summary>
    public string GetConnectionDescription() => Type switch
    {
        ConnectionType.SSH => $"🔐 SSH Portal to {Host}:{Port} - Digital bridge to remote realms!",
        ConnectionType.Serial => $"🔌 Serial Gateway via {SerialPort} @ {BaudRate} - Hardware whispering at the speed of electrons!",
        ConnectionType.Bluetooth => $"📡 Bluetooth Bond with {BluetoothAddress} - Wireless wizardry through ethereal frequencies!",
        ConnectionType.Telnet => $"📞 Telnet Tunnel to {Host}:{Port} - Vintage protocol, timeless connections!",
        _ => "🌟 Mysterious Connection - Exploring unknown digital territories!"
    };
}

/// <summary>
/// Connection type enumeration - the spectrum of digital communion
/// </summary>
public enum ConnectionType
{
    SSH,
    Serial,
    Bluetooth,
    Telnet
}

/// <summary>
/// Connection status - the heartbeat of digital bridges
/// </summary>
public enum ConnectionStatus
{
    Disconnected,
    Connecting,
    Connected,
    Reconnecting,
    Failed,
    Timeout
}

/// <summary>
/// Session handle - the mystical key to active digital realms
/// </summary>
public class SessionHandle
{
    public string SessionId { get; set; } = string.Empty;
    public ConnectionType Type { get; set; }
    public ConnectionStatus Status { get; set; }
    public DateTime ConnectedAt { get; set; }
    public DateTime LastActivity { get; set; }
    public string DisplayName { get; set; } = "Unknown Session";
    public Dictionary<string, object> Properties { get; set; } = new();
    
    /// <summary>
    /// Get session duration with cosmic flair
    /// </summary>
    public string GetSessionDuration()
    {
        var duration = DateTime.Now - ConnectedAt;
        return duration.TotalHours >= 1
            ? $"{duration:hh\\:mm\\:ss} - Epic journey through digital time!"
            : $"{duration:mm\\:ss} - Swift exploration of cyber realms!";
    }
}

#endregion

#region Device Detection Models

/// <summary>
/// Device information - the catalog of digital companions
/// </summary>
public class DeviceInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DeviceType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new();
    
    /// <summary>
    /// Get device status icon with personality
    /// </summary>
    public string GetStatusIcon() => (Type, IsAvailable) switch
    {
        (DeviceType.Serial, true) => "🔌 Serial Port Ready - Hardware handshake awaits!",
        (DeviceType.Serial, false) => "⚡ Serial Port Busy - Another process claimed the electrons!",
        (DeviceType.USB, true) => "🔗 USB Device Available - Plug and play paradise!",
        (DeviceType.USB, false) => "⛔ USB Device Occupied - Digital traffic jam!",
        (DeviceType.Bluetooth, true) => "📡 Bluetooth Device Paired - Wireless wonderland ready!",
        (DeviceType.Bluetooth, false) => "🔍 Bluetooth Device Searching - Hunting for radio signals!",
        _ => "❓ Mystery Device - Enigmatic hardware awaits discovery!"
    };
}

/// <summary>
/// Device type enumeration - the taxonomy of digital peripherals
/// </summary>
public enum DeviceType
{
    Serial,
    USB,
    Bluetooth,
    Network
}

/// <summary>
/// Device scan result - the bounty of hardware reconnaissance
/// </summary>
public class DeviceScanResult
{
    public List<DeviceInfo> Devices { get; set; } = new();
    public DateTime ScanTimestamp { get; set; } = DateTime.Now;
    public TimeSpan ScanDuration { get; set; }
    public string ScanSummary => $"🔍 Discovered {Devices.Count} devices in {ScanDuration.TotalMilliseconds:F0}ms - Digital archaeology complete!";
}

#endregion

#region Terminal Emulation Models

/// <summary>
/// Colored output - the rainbow of terminal artistry
/// </summary>
public class ColoredOutput
{
    public string Text { get; set; } = string.Empty;
    public ConsoleColor? Foreground { get; set; }
    public ConsoleColor? Background { get; set; }
    public bool IsBold { get; set; }
    public bool IsUnderline { get; set; }
    public bool IsItalic { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    
    /// <summary>
    /// Convert to xterm.js compatible string
    /// </summary>
    public string ToXtermString()
    {
        var escape = new StringBuilder();
        
        if (Foreground.HasValue)
            escape.Append($"\\x1b[{GetAnsiColor(Foreground.Value, false)}m");
            
        if (Background.HasValue)
            escape.Append($"\\x1b[{GetAnsiColor(Background.Value, true)}m");
            
        if (IsBold) escape.Append("\\x1b[1m");
        if (IsUnderline) escape.Append("\\x1b[4m");
        if (IsItalic) escape.Append("\\x1b[3m");
        
        escape.Append(Text);
        escape.Append("\\x1b[0m"); // Reset
        
        return escape.ToString();
    }
    
    private static int GetAnsiColor(ConsoleColor color, bool background) => color switch
    {
        ConsoleColor.Black => background ? 40 : 30,
        ConsoleColor.Red => background ? 41 : 31,
        ConsoleColor.Green => background ? 42 : 32,
        ConsoleColor.Yellow => background ? 43 : 33,
        ConsoleColor.Blue => background ? 44 : 34,
        ConsoleColor.Magenta => background ? 45 : 35,
        ConsoleColor.Cyan => background ? 46 : 36,
        ConsoleColor.White => background ? 47 : 37,
        _ => background ? 40 : 37
    };
}

/// <summary>
/// Key event - the symphony of keyboard orchestration
/// </summary>
public class KeyEvent
{
    public ConsoleKey Key { get; set; }
    public char KeyChar { get; set; }
    public ConsoleModifiers Modifiers { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    
    /// <summary>
    /// Check if this is a special key combination
    /// </summary>
    public bool IsSpecialKey => Key switch
    {
        ConsoleKey.UpArrow or ConsoleKey.DownArrow or ConsoleKey.LeftArrow or ConsoleKey.RightArrow => true,
        ConsoleKey.F1 or ConsoleKey.F2 or ConsoleKey.F3 or ConsoleKey.F4 => true,
        ConsoleKey.Tab or ConsoleKey.Enter or ConsoleKey.Escape => true,
        ConsoleKey.Home or ConsoleKey.End or ConsoleKey.PageUp or ConsoleKey.PageDown => true,
        _ => Modifiers.HasFlag(ConsoleModifiers.Control)
    };
}

/// <summary>
/// Command history entry - the scroll of digital incantations
/// </summary>
public class CommandHistoryEntry
{
    public string Command { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string? Response { get; set; }
    public bool WasSuccessful { get; set; }
    
    /// <summary>
    /// Get history entry with cosmic timestamp
    /// </summary>
    public string GetFormattedEntry()
    {
        var status = WasSuccessful ? "✨" : "💥";
        return $"{status} [{Timestamp:HH:mm:ss}] {Command}";
    }
}

/// <summary>
/// Terminal buffer - the infinite scroll of digital consciousness
/// </summary>
public class TerminalBuffer
{
    private readonly Queue<ColoredOutput> _buffer = new();
    private readonly int _maxLines;
    
    public TerminalBuffer(int maxLines = 10000)
    {
        _maxLines = maxLines;
    }
    
    public void AddLine(ColoredOutput output)
    {
        _buffer.Enqueue(output);
        while (_buffer.Count > _maxLines)
            _buffer.Dequeue();
    }
    
    public void AddLine(string text, ConsoleColor? foreground = null, ConsoleColor? background = null)
    {
        AddLine(new ColoredOutput
        {
            Text = text,
            Foreground = foreground,
            Background = background
        });
    }
    
    public IEnumerable<ColoredOutput> GetLines() => _buffer.ToArray();
    public int LineCount => _buffer.Count;
    
    public void Clear() => _buffer.Clear();
}

#endregion

#region Session Management Models

/// <summary>
/// Session state - the memory palace of digital connections
/// </summary>
public class SessionState
{
    public string SessionId { get; set; } = string.Empty;
    public ConnectionParams ConnectionParams { get; set; } = new();
    public ConnectionStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? ConnectedAt { get; set; }
    public DateTime? LastActivity { get; set; }
    public int ReconnectAttempts { get; set; }
    public List<CommandHistoryEntry> CommandHistory { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
    
    /// <summary>
    /// Serialize session state for persistence
    /// </summary>
    public string Serialize()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });
    }
    
    /// <summary>
    /// Deserialize session state from JSON
    /// </summary>
    public static SessionState? Deserialize(string json)
    {
        try
        {
            return JsonConvert.DeserializeObject<SessionState>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
        }
        catch
        {
            return null;
        }
    }
    
    /// <summary>
    /// Get session status summary with personality
    /// </summary>
    public string GetStatusSummary() => Status switch
    {
        ConnectionStatus.Connected => $"🌟 Session thriving since {ConnectedAt:HH:mm:ss} - Digital harmony achieved!",
        ConnectionStatus.Connecting => "🔄 Establishing connection - Handshaking with distant realms!",
        ConnectionStatus.Reconnecting => $"🔧 Reconnecting (attempt {ReconnectAttempts}) - Persistent digital spirit!",
        ConnectionStatus.Failed => "💥 Connection failed - The digital void claimed another attempt!",
        ConnectionStatus.Timeout => "⏰ Connection timeout - Time's arrow defeated our digital quest!",
        _ => "😴 Session dormant - Awaiting the spark of connection!"
    };
}

/// <summary>
/// Log entry - the chronicles of digital adventures
/// </summary>
public class LogEntry
{
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public LogLevel Level { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? SessionId { get; set; }
    public string? Source { get; set; }
    public Exception? Exception { get; set; }
    
    /// <summary>
    /// Format log entry with cosmic flair
    /// </summary>
    public string Format()
    {
        var levelIcon = Level switch
        {
            LogLevel.Trace => "🔍",
            LogLevel.Debug => "🐛",
            LogLevel.Information => "ℹ️",
            LogLevel.Warning => "⚠️",
            LogLevel.Error => "💥",
            LogLevel.Critical => "🚨",
            _ => "❓"
        };
        
        var sessionPart = !string.IsNullOrEmpty(SessionId) ? $"[{SessionId[..8]}]" : "";
        return $"{levelIcon} [{Timestamp:HH:mm:ss.fff}]{sessionPart} {Message}";
    }
}

/// <summary>
/// Log level enumeration
/// </summary>
public enum LogLevel
{
    Trace,
    Debug,
    Information,
    Warning,
    Error,
    Critical
}

#endregion

#region Event Models

/// <summary>
/// SSH terminal event base class - the foundation of digital proclamations
/// </summary>
public abstract class SshTerminalEventBase
{
    public string SessionId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public Dictionary<string, object> Properties { get; set; } = new();
}

/// <summary>
/// Connection established event
/// </summary>
public class ConnectionEstablishedEvent : SshTerminalEventBase
{
    public ConnectionParams ConnectionParams { get; set; } = new();
    public TimeSpan ConnectionDuration { get; set; }
}

/// <summary>
/// Data received event
/// </summary>
public class DataReceivedEvent : SshTerminalEventBase
{
    public string Data { get; set; } = string.Empty;
    public int ByteCount { get; set; }
    public ConnectionType ConnectionType { get; set; }
}

/// <summary>
/// Command executed event
/// </summary>
public class CommandExecutedEvent : SshTerminalEventBase
{
    public string Command { get; set; } = string.Empty;
    public string? Response { get; set; }
    public bool Success { get; set; }
    public TimeSpan ExecutionTime { get; set; }
}

/// <summary>
/// Error encountered event
/// </summary>
public class ErrorEncounteredEvent : SshTerminalEventBase
{
    public string ErrorMessage { get; set; } = string.Empty;
    public Exception? Exception { get; set; }
    public string? Context { get; set; }
}

/// <summary>
/// Device detected event
/// </summary>
public class DeviceDetectedEvent : SshTerminalEventBase
{
    public DeviceInfo Device { get; set; } = new();
    public DeviceType DeviceType { get; set; }
}

#endregion

#region Configuration Models

/// <summary>
/// Terminal configuration - the sacred scrolls of customization
/// </summary>
public class TerminalConfig
{
    /// <summary>
    /// Default font family for terminal display
    /// </summary>
    public string FontFamily { get; set; } = "Consolas";
    
    /// <summary>
    /// Default font size
    /// </summary>
    public int FontSize { get; set; } = 12;
    
    /// <summary>
    /// Default foreground color
    /// </summary>
    public ConsoleColor DefaultForeground { get; set; } = ConsoleColor.White;
    
    /// <summary>
    /// Default background color
    /// </summary>
    public ConsoleColor DefaultBackground { get; set; } = ConsoleColor.Black;
    
    /// <summary>
    /// Terminal buffer size (number of lines)
    /// </summary>
    public int BufferSize { get; set; } = 10000;
    
    /// <summary>
    /// Command history size
    /// </summary>
    public int HistorySize { get; set; } = 1000;
    
    /// <summary>
    /// Enable bell sound
    /// </summary>
    public bool EnableBell { get; set; } = true;
    
    /// <summary>
    /// Auto-save sessions
    /// </summary>
    public bool AutoSaveSessions { get; set; } = true;
    
    /// <summary>
    /// Session timeout in minutes
    /// </summary>
    public int SessionTimeoutMinutes { get; set; } = 60;
    
    /// <summary>
    /// Enable Three.js effects
    /// </summary>
    public bool EnableThreeJsEffects { get; set; } = true;
    
    /// <summary>
    /// Custom CSS for terminal styling
    /// </summary>
    public string CustomCss { get; set; } = string.Empty;
    
    /// <summary>
    /// Get configuration summary with cosmic style
    /// </summary>
    public string GetConfigSummary()
    {
        return $"🎨 Terminal Config: {FontFamily} {FontSize}pt, buffer={BufferSize} lines, " +
               $"history={HistorySize} commands, effects={EnableThreeJsEffects} - Digital artistry perfected!";
    }
}

#endregion