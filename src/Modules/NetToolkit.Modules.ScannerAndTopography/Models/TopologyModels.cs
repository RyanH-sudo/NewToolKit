namespace NetToolkit.Modules.ScannerAndTopography.Models;

/// <summary>
/// Network topology result - the digital cosmos mapped in exquisite detail!
/// Where networks transform into navigable 3D universes of metallic wonder.
/// </summary>
public class TopologyResult
{
    public string ScanId { get; set; } = Guid.NewGuid().ToString();
    public DateTime ScanTimestamp { get; set; } = DateTime.UtcNow;
    public string NetworkRange { get; set; } = string.Empty;
    public List<NetworkNode> Nodes { get; set; } = new();
    public List<NetworkEdge> Edges { get; set; } = new();
    public ScanStatistics Statistics { get; set; } = new();
    public List<NetworkAnomaly> Anomalies { get; set; } = new();
    
    /// <summary>
    /// Get witty scan summary based on results - because data deserves personality!
    /// </summary>
    public string GetWittyScanSummary()
    {
        var activeNodes = Nodes.Count(n => n.IsOnline);
        var totalPorts = Nodes.SelectMany(n => n.OpenPorts).Count();
        
        return activeNodes switch
        {
            0 => "Digital wasteland detected - not a soul in sight! 👻",
            1 => "Lone digital warrior discovered - a hermit in the network! 🏰",
            < 5 => $"{activeNodes} digital citizens found - a cozy neighborhood! 🏘️",
            < 20 => $"{activeNodes} network nodes mapped - bustling digital metropolis! 🌆",
            < 50 => $"{activeNodes} devices charted - a thriving cyber empire! 👑",
            _ => $"{activeNodes} network entities cataloged - digital universe conquered! 🌌"
        };
    }
}

/// <summary>
/// Network node - celestial orbs in our 3D digital cosmos
/// Each node gleams like a star with its own unique signature
/// </summary>
public class NetworkNode
{
    public string Id { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string MacAddress { get; set; } = string.Empty;
    public string HostName { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public bool IsOnline { get; set; }
    public bool IsDynamic { get; set; } = true; // DHCP assigned
    public string SubnetMask { get; set; } = string.Empty;
    public string Gateway { get; set; } = string.Empty;
    public List<int> OpenPorts { get; set; } = new();
    public long ResponseTime { get; set; } // Ping response in milliseconds
    public DateTime LastSeen { get; set; } = DateTime.UtcNow;
    public DateTime FirstSeen { get; set; } = DateTime.UtcNow;
    
    // 3D Visualization properties - for the cosmic display
    public Vector3D Position { get; set; } = new();
    public string Color { get; set; } = "#00FF00"; // Emerald for vitality by default
    public NodeStatus Status { get; set; } = NodeStatus.Normal;
    public string Notes { get; set; } = string.Empty;
    
    /// <summary>
    /// Get the digital personality of this node - because every device has character!
    /// </summary>
    public string GetNodePersonality()
    {
        var portCount = OpenPorts.Count;
        var isRouter = OpenPorts.Contains(22) || OpenPorts.Contains(23) || OpenPorts.Contains(80);
        var isServer = OpenPorts.Any(p => new[] { 80, 443, 21, 22, 3389 }.Contains(p));
        
        return (isRouter, isServer, portCount) switch
        {
            (true, _, _) => "🌉 Network Gateway - The digital bridge keeper",
            (_, true, > 5) => "🖥️ Mighty Server - The workhorse of the digital realm",
            (_, _, > 10) => "🔌 Port Bonanza - This device loves company!",
            (_, _, 0) => "🔒 Digital Hermit - Keeps to itself in cyber solitude",
            (_, _, < 3) => "🏠 Simple Device - Humble and unassuming",
            _ => "🤖 Digital Citizen - Another node in the matrix"
        };
    }
    
    /// <summary>
    /// Determine node color based on status and characteristics
    /// </summary>
    public string GetStatusColor()
    {
        return Status switch
        {
            NodeStatus.Anomaly => "#FF4444", // Crimson for anomalies
            NodeStatus.Critical => "#FF8800", // Orange for critical
            NodeStatus.Warning => "#FFFF00", // Yellow for warnings
            NodeStatus.Normal when IsOnline => "#00FF00", // Emerald for vitality
            NodeStatus.Normal when !IsOnline => "#888888", // Gray for offline
            _ => "#FFFFFF" // White for unknown
        };
    }
}

/// <summary>
/// Network edge - the luminous conduits connecting our digital stars
/// Like laser-threaded connections in the cyber cosmos
/// </summary>
public class NetworkEdge
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FromNodeId { get; set; } = string.Empty;
    public string ToNodeId { get; set; } = string.Empty;
    public EdgeType ConnectionType { get; set; } = EdgeType.Subnet;
    public string Label { get; set; } = string.Empty;
    public double Strength { get; set; } = 1.0; // Connection strength for layout algorithms
    public string Color { get; set; } = "#00CCFF"; // Cyan laser threads
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Get witty connection description
    /// </summary>
    public string GetConnectionPersonality()
    {
        return ConnectionType switch
        {
            EdgeType.Subnet => "🏘️ Subnet Neighbors - Digital next-door buddies",
            EdgeType.Gateway => "🚪 Gateway Connection - The path to other realms",
            EdgeType.Direct => "🔗 Direct Link - Best friends in the network",
            EdgeType.VPN => "🔒 VPN Tunnel - Secret passage through cyberspace",
            EdgeType.Wireless => "📡 Wireless Bridge - Invisible threads of connection",
            _ => "❓ Mysterious Link - Connection type unknown to mortals"
        };
    }
}

/// <summary>
/// 3D Vector for positioning nodes in the digital cosmos
/// </summary>
public class Vector3D
{
    public double X { get; set; } = 0;
    public double Y { get; set; } = 0;
    public double Z { get; set; } = 0;
    
    public Vector3D() { }
    
    public Vector3D(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }
    
    /// <summary>
    /// Generate random position for initial 3D layout - cosmic distribution
    /// </summary>
    public static Vector3D RandomPosition(Random? random = null)
    {
        var rng = random ?? new Random();
        return new Vector3D(
            (rng.NextDouble() - 0.5) * 200, // Spread across 200 units
            (rng.NextDouble() - 0.5) * 200,
            (rng.NextDouble() - 0.5) * 200
        );
    }
}

/// <summary>
/// Network Interface Card details - the digital fingerprints of devices
/// Harvested from the depths of WMI with surgical precision
/// </summary>
public class NicDetail
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MacAddress { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string SubnetMask { get; set; } = string.Empty;
    public string Gateway { get; set; } = string.Empty;
    public List<string> DnsServers { get; set; } = new();
    public bool IsDhcpEnabled { get; set; } = true;
    public bool IsEnabled { get; set; } = true;
    public string AdapterType { get; set; } = string.Empty;
    public ulong Speed { get; set; } = 0; // Connection speed in bits per second
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Get human-readable speed - because numbers mean more with context
    /// </summary>
    public string GetHumanReadableSpeed()
    {
        return Speed switch
        {
            >= 1_000_000_000 => $"{Speed / 1_000_000_000.0:F1} Gbps - Warp speed achieved! 🚀",
            >= 1_000_000 => $"{Speed / 1_000_000.0:F1} Mbps - Cruising at highway speeds! 🛣️",
            >= 1_000 => $"{Speed / 1_000.0:F1} Kbps - Dial-up nostalgia detected! 📞",
            > 0 => $"{Speed} bps - Carrier pigeon might be faster! 🐦",
            _ => "Speed unknown - Quantum entanglement perhaps? ⚛️"
        };
    }
}

/// <summary>
/// Scan configuration options - fine-tuning the digital exploration
/// </summary>
public class ScanOptions
{
    public int PingTimeout { get; set; } = 1000; // Milliseconds
    public int MaxConcurrentPings { get; set; } = 50;
    public List<int> PortsToScan { get; set; } = new() { 21, 22, 23, 25, 53, 80, 110, 143, 443, 993, 995, 3389 };
    public int PortTimeout { get; set; } = 2000;
    public bool ScanAllPorts { get; set; } = false; // If true, scan 1-65535
    public bool EnableWmiDiscovery { get; set; } = true;
    public bool EnablePortScan { get; set; } = true;
    public bool EnableAnomalyDetection { get; set; } = true;
    public bool SaveResults { get; set; } = true;
    public string OutputFormat { get; set; } = "json"; // json, pdf, csv
}

/// <summary>
/// Scan statistics - the numerical poetry of network exploration
/// </summary>
public class ScanStatistics
{
    public TimeSpan ScanDuration { get; set; }
    public int TotalHostsScanned { get; set; }
    public int ActiveHosts { get; set; }
    public int TotalPortsProbed { get; set; }
    public int OpenPortsFound { get; set; }
    public int AnomaliesDetected { get; set; }
    public double ScanEfficiency { get; set; } // Active hosts / Total scanned
    public DateTime ScanStartTime { get; set; }
    public DateTime ScanEndTime { get; set; }
    
    /// <summary>
    /// Get performance commentary with typical wit
    /// </summary>
    public string GetPerformanceCommentary()
    {
        var efficiency = ScanEfficiency * 100;
        var hostsPerSecond = TotalHostsScanned / Math.Max(ScanDuration.TotalSeconds, 1);
        
        return efficiency switch
        {
            > 80 => $"🎯 {efficiency:F1}% efficiency - Network scanning ninja level achieved!",
            > 60 => $"👍 {efficiency:F1}% efficiency - Solid reconnaissance work!",
            > 40 => $"📊 {efficiency:F1}% efficiency - Room for improvement, but we're getting there!",
            > 20 => $"🔍 {efficiency:F1}% efficiency - Like finding needles in digital haystacks!",
            _ => $"🌵 {efficiency:F1}% efficiency - Welcome to the digital wasteland!"
        };
    }
}

/// <summary>
/// Network anomaly detection - the cyber sleuth findings
/// </summary>
public class NetworkAnomaly
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string NodeId { get; set; } = string.Empty;
    public AnomalyType Type { get; set; }
    public AnomalySeverity Severity { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
    public bool IsResolved { get; set; } = false;
    public string RecommendedAction { get; set; } = string.Empty;
    
    /// <summary>
    /// Get witty anomaly description with appropriate humor
    /// </summary>
    public string GetWittyDescription()
    {
        return Type switch
        {
            AnomalyType.UnusualPortPattern => "🕵️ This device has more open doors than a hospitable inn!",
            AnomalyType.SuspiciousService => "🤔 Something fishy is running here - digital seafood alert!",
            AnomalyType.UnexpectedDevice => "👽 Alien technology detected - or just misconfigured hardware!",
            AnomalyType.SecurityRisk => "🚨 Security concern spotted - this needs your attention!",
            AnomalyType.ConfigurationIssue => "⚙️ Configuration quirk detected - someone's been experimenting!",
            AnomalyType.PerformanceIssue => "🐌 Performance sluggishness observed - digital molasses detected!",
            _ => "❓ Mysterious anomaly discovered - the plot thickens!"
        };
    }
}

/// <summary>
/// Node status enumeration - the health indicators of digital citizens
/// </summary>
public enum NodeStatus
{
    Normal,
    Warning,
    Critical,
    Anomaly,
    Offline,
    Unknown
}

/// <summary>
/// Edge type enumeration - the varieties of digital connections
/// </summary>
public enum EdgeType
{
    Subnet,     // Same subnet connection
    Gateway,    // Gateway/router connection
    Direct,     // Direct connection
    VPN,        // VPN tunnel
    Wireless,   // Wireless connection
    Unknown     // Connection type unknown
}

/// <summary>
/// Anomaly type enumeration - the categories of digital peculiarities
/// </summary>
public enum AnomalyType
{
    UnusualPortPattern,    // Unexpected open ports
    SuspiciousService,     // Suspicious service detected
    UnexpectedDevice,      // Device type doesn't match expectations
    SecurityRisk,          // Potential security vulnerability
    ConfigurationIssue,    // Configuration anomaly
    PerformanceIssue,      // Performance-related anomaly
    NetworkTrend          // Unusual network traffic patterns
}

/// <summary>
/// Anomaly severity enumeration - the urgency levels of digital concerns
/// </summary>
public enum AnomalySeverity
{
    Info,       // Informational - just FYI
    Low,        // Low priority - worth noting
    Medium,     // Medium priority - should investigate
    High,       // High priority - needs attention
    Critical    // Critical - immediate action required
}