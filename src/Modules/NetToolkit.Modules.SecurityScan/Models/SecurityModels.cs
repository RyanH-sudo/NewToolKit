using NetToolkit.Modules.ScannerAndTopography.Models;

namespace NetToolkit.Modules.SecurityScan.Models;

/// <summary>
/// Security scan result - the vigilant chronicle of digital fortress assessments
/// Where vulnerabilities are catalogued with forensic precision and witty commentary
/// </summary>
public class ScanResult
{
    public string ScanId { get; set; } = Guid.NewGuid().ToString();
    public DateTime ScanTimestamp { get; set; } = DateTime.UtcNow;
    public ScanType ScanType { get; set; }
    public List<ScanTarget> Targets { get; set; } = new();
    public List<VulnerabilityEntry> Vulnerabilities { get; set; } = new();
    public ScanStatistics Statistics { get; set; } = new();
    public ScanSeveritySummary SeveritySummary { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
    public TimeSpan Duration { get; set; }
    public ScanStatus Status { get; set; } = ScanStatus.Completed;
    
    /// <summary>
    /// Get witty security assessment summary - because humor lightens heavy security burdens
    /// </summary>
    public string GetSecuritySummary()
    {
        var totalVulns = Vulnerabilities.Count;
        var criticalCount = Vulnerabilities.Count(v => v.Severity == VulnerabilitySeverity.Critical);
        var highCount = Vulnerabilities.Count(v => v.Severity == VulnerabilitySeverity.High);
        
        return (criticalCount, highCount, totalVulns) switch
        {
            (0, 0, 0) => "🛡️ Fortress secured - your network's wearing digital chainmail! ✨",
            (0, 0, var low) when low < 5 => $"🟢 {totalVulns} minor chinks in the armor - easily patched! 🔧",
            (0, var h, var total) when h < 3 => $"🟡 {total} vulnerabilities found - some attention needed but no panic! 📋",
            (0, var h, var total) when h >= 3 => $"🟠 {total} vulnerabilities including {h} high-risk - time for some security housekeeping! 🧹",
            (var c, var h, var total) when c < 3 => $"🔴 {total} vulnerabilities with {c} critical - urgent patching party required! 🚨",
            (var c, var h, var total) => $"💀 {total} vulnerabilities including {c} critical - DEFCON 1, all hands on deck! ⚡"
        };
    }
}

/// <summary>
/// Deep scan result - comprehensive vulnerability excavation with detailed forensics
/// The archaeological dig of digital security, unearthing buried threats
/// </summary>
public class DeepScanResult : ScanResult
{
    public List<ServiceFingerprint> ServiceFingerprints { get; set; } = new();
    public List<OperatingSystemInfo> OSFingerprints { get; set; } = new();
    public List<ExploitInfo> PotentialExploits { get; set; } = new();
    public List<NetworkService> RunningServices { get; set; } = new();
    public List<SecurityMisconfiguration> Misconfigurations { get; set; } = new();
    public List<ComplianceCheck> ComplianceResults { get; set; } = new();
    
    /// <summary>
    /// Generate comprehensive threat assessment - the oracle speaks of digital perils
    /// </summary>
    public string GenerateThreatAssessment()
    {
        var exploitableCount = PotentialExploits.Count(e => e.Exploitability == ExploitSeverity.High);
        var misconfigCount = Misconfigurations.Count;
        
        return (exploitableCount, misconfigCount) switch
        {
            (0, 0) => "🏰 Digital fortress appears secure - the ramparts hold strong!",
            (0, var m) when m < 3 => $"⚙️ {m} configuration tweaks recommended - fine-tuning time!",
            (0, var m) => $"🔧 {m} misconfigurations detected - housekeeping required!",
            (var e, 0) when e < 2 => $"⚔️ {e} potential exploit paths - shield wall formation!",
            (var e, var m) => $"🚨 {e} exploits + {m} misconfigs - battle stations, everyone!"
        };
    }
}

/// <summary>
/// Vulnerability entry - the detailed dossier of digital dangers
/// Each vulnerability catalogued with precision and personality
/// </summary>
public class VulnerabilityEntry
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string NodeId { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Service { get; set; } = string.Empty;
    public string CVE { get; set; } = string.Empty;
    public VulnerabilitySeverity Severity { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Solution { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public double CVSSScore { get; set; }
    public string ExploitDetails { get; set; } = string.Empty;
    public DateTime DiscoveredAt { get; set; } = DateTime.UtcNow;
    public bool IsExploitable { get; set; }
    public string RemediationScript { get; set; } = string.Empty;
    public VulnerabilityCategory Category { get; set; }
    
    /// <summary>
    /// Get witty vulnerability description - making security awareness memorable
    /// </summary>
    public string GetWittyDescription()
    {
        return Category switch
        {
            VulnerabilityCategory.Authentication => "🔐 Authentication weakness detected - the digital door's lock is rusty!",
            VulnerabilityCategory.Authorization => "🚪 Authorization gap found - someone's using the wrong key!",
            VulnerabilityCategory.Configuration => "⚙️ Configuration issue spotted - someone missed a checkbox!",
            VulnerabilityCategory.Encryption => "🔒 Encryption flaw discovered - secrets aren't so secret!",
            VulnerabilityCategory.InputValidation => "📝 Input validation bypass - the bouncer's asleep at the gate!",
            VulnerabilityCategory.NetworkSecurity => "🌐 Network security hole - there's a gap in the digital fence!",
            VulnerabilityCategory.PrivilegeEscalation => "👑 Privilege escalation risk - peasants trying to be kings!",
            VulnerabilityCategory.InformationDisclosure => "📢 Information leakage - loose lips sink digital ships!",
            VulnerabilityCategory.DenialOfService => "💥 DoS vulnerability - someone can pull the digital fire alarm!",
            _ => "🎯 Security vulnerability detected - time to patch this digital pothole!"
        };
    }
    
    /// <summary>
    /// Get severity color for visualization - painting threats in vivid hues
    /// </summary>
    public string GetSeverityColor()
    {
        return Severity switch
        {
            VulnerabilitySeverity.Critical => "#FF0000", // Blazing red for maximum danger
            VulnerabilitySeverity.High => "#FF4500",     // Orange-red for high concern
            VulnerabilitySeverity.Medium => "#FFA500",   // Orange for moderate attention
            VulnerabilitySeverity.Low => "#FFFF00",      // Yellow for minor issues
            VulnerabilitySeverity.Info => "#87CEEB",     // Sky blue for informational
            _ => "#808080" // Gray for unknown
        };
    }
}

/// <summary>
/// Scan target specification - defining the scope of digital reconnaissance
/// </summary>
public class ScanTarget
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string IpAddress { get; set; } = string.Empty;
    public string HostName { get; set; } = string.Empty;
    public List<int> Ports { get; set; } = new();
    public bool IsAlive { get; set; }
    public string NodeId { get; set; } = string.Empty; // Reference to topology node
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Service fingerprint - the digital DNA of network services
/// </summary>
public class ServiceFingerprint
{
    public string Service { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Banner { get; set; } = string.Empty;
    public string Product { get; set; } = string.Empty;
    public string Vendor { get; set; } = string.Empty;
    public Dictionary<string, string> ExtraInfo { get; set; } = new();
    
    /// <summary>
    /// Generate service personality - giving character to digital services
    /// </summary>
    public string GetServicePersonality()
    {
        return Service.ToLower() switch
        {
            "ssh" => "🔒 SSH Guardian - The secure shell sentinel standing watch",
            "http" => "🌐 HTTP Herald - The web world's faithful messenger",
            "https" => "🛡️ HTTPS Protector - The encrypted web's armored knight",
            "ftp" => "📁 FTP Archivist - The file transfer faithful, showing its age",
            "smtp" => "📧 SMTP Postmaster - The email system's tireless courier",
            "dns" => "🗺️ DNS Navigator - The internet's address book keeper",
            "mysql" => "🐬 MySQL Dolphin - The friendly database swimming in data",
            "postgresql" => "🐘 PostgreSQL Elephant - The memory-rich database giant",
            "rdp" => "🖥️ RDP Portal - The remote desktop's magical gateway",
            "telnet" => "📡 Telnet Veteran - The unencrypted elder, needs retirement",
            _ => $"🤖 {Service} Service - Digital worker bee in the network hive"
        };
    }
}

/// <summary>
/// Operating system information - the digital identity of target systems
/// </summary>
public class OperatingSystemInfo
{
    public string IpAddress { get; set; } = string.Empty;
    public string OSFamily { get; set; } = string.Empty;
    public string OSGeneration { get; set; } = string.Empty;
    public string OSVendor { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public List<string> CPEMatches { get; set; } = new();
    
    /// <summary>
    /// Get OS personality - characterizing digital inhabitants
    /// </summary>
    public string GetOSPersonality()
    {
        return OSFamily.ToLower() switch
        {
            "windows" => "🪟 Windows Dweller - The familiar desktop companion",
            "linux" => "🐧 Linux Penguin - The open-source freedom fighter",
            "macos" => "🍎 macOS Sophisticate - The sleek and stylish resident",
            "unix" => "⚡ Unix Veteran - The battle-tested server guardian",
            "freebsd" => "😈 FreeBSD Daemon - The robust and reliable keeper",
            "android" => "🤖 Android Citizen - The mobile world's green ambassador",
            "ios" => "📱 iOS Inhabitant - The polished mobile perfectionist",
            _ => $"👻 {OSFamily} Spirit - Mysterious digital entity detected"
        };
    }
}

/// <summary>
/// Exploit information - the dark knowledge of attack vectors
/// </summary>
public class ExploitInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ExploitSeverity Exploitability { get; set; }
    public string Platform { get; set; } = string.Empty;
    public List<string> References { get; set; } = new();
    public string PoC { get; set; } = string.Empty; // Proof of Concept
    public DateTime Published { get; set; }
    public bool HasPublicExploit { get; set; }
}

/// <summary>
/// Network service information - the digital workforce census
/// </summary>
public class NetworkService
{
    public int Port { get; set; }
    public string Protocol { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Service { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public Dictionary<string, string> Scripts { get; set; } = new();
}

/// <summary>
/// Security misconfiguration - the accidental chinks in digital armor
/// </summary>
public class SecurityMisconfiguration
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Category { get; set; } = string.Empty;
    public string Issue { get; set; } = string.Empty;
    public string Impact { get; set; } = string.Empty;
    public string Recommendation { get; set; } = string.Empty;
    public VulnerabilitySeverity Severity { get; set; }
    public string Location { get; set; } = string.Empty;
    
    /// <summary>
    /// Get misconfiguration wit - making configuration errors memorable
    /// </summary>
    public string GetConfigurationWit()
    {
        return Category.ToLower() switch
        {
            "authentication" => "🔐 Authentication misconfigured - the digital locks are loose!",
            "ssl/tls" => "🔒 SSL/TLS issue - the encryption envelope has holes!",
            "permissions" => "👥 Permission problem - digital access control gone awry!",
            "default" => "🏭 Default settings detected - someone forgot to customize!",
            "outdated" => "📅 Outdated component - this software predates smartphones!",
            "exposure" => "📢 Information exposed - secrets are shouting from rooftops!",
            _ => "⚙️ Configuration quirk discovered - someone's been experimenting!"
        };
    }
}

/// <summary>
/// Compliance check result - adherence to digital governance standards
/// </summary>
public class ComplianceCheck
{
    public string Standard { get; set; } = string.Empty; // NIST, PCI-DSS, HIPAA, etc.
    public string Control { get; set; } = string.Empty;
    public bool Passed { get; set; }
    public string Finding { get; set; } = string.Empty;
    public string Remediation { get; set; } = string.Empty;
    public string Evidence { get; set; } = string.Empty;
}

/// <summary>
/// Scan depth options - fine-tuning the digital archaeological expedition
/// </summary>
public class DepthOptions
{
    public int MaxHosts { get; set; } = 50;
    public int MaxPorts { get; set; } = 1000;
    public bool IncludeVulnScripts { get; set; } = true;
    public bool IncludeServiceDetection { get; set; } = true;
    public bool IncludeOSDetection { get; set; } = true;
    public bool IncludeBruteForceScripts { get; set; } = false;
    public int TimeoutSeconds { get; set; } = 300;
    public ScanIntensity Intensity { get; set; } = ScanIntensity.Normal;
    public List<string> CustomScripts { get; set; } = new();
    public bool GeneratePDF { get; set; } = true;
    public bool GenerateHTML { get; set; } = true;
}

/// <summary>
/// Scan severity summary - the statistical overview of digital dangers
/// </summary>
public class ScanSeveritySummary
{
    public int Critical { get; set; }
    public int High { get; set; }
    public int Medium { get; set; }
    public int Low { get; set; }
    public int Informational { get; set; }
    public int Total => Critical + High + Medium + Low + Informational;
    
    /// <summary>
    /// Calculate risk score - quantifying digital danger
    /// </summary>
    public double CalculateRiskScore()
    {
        return (Critical * 10) + (High * 7) + (Medium * 4) + (Low * 1) + (Informational * 0.1);
    }
    
    /// <summary>
    /// Get risk level description - translating numbers to narrative
    /// </summary>
    public string GetRiskLevel()
    {
        var score = CalculateRiskScore();
        return score switch
        {
            < 5 => "🟢 Low Risk - Your network's wearing digital bubble wrap!",
            < 20 => "🟡 Moderate Risk - Some patches needed, but no panic!",
            < 50 => "🟠 High Risk - Time for some serious security housekeeping!",
            < 100 => "🔴 Very High Risk - All hands on deck for patching party!",
            _ => "💀 Critical Risk - DEFCON 1, battle stations everyone!"
        };
    }
}

/// <summary>
/// Report format options - choosing the presentation style for security intelligence
/// </summary>
public enum ReportFormat
{
    PDF,        // Professional document format
    HTML,       // Interactive web format
    JSON,       // Machine-readable format
    CSV,        // Spreadsheet-compatible format
    XML,        // Structured data format
    Text        // Simple text format
}

/// <summary>
/// Scan type enumeration - defining the depth of digital excavation
/// </summary>
public enum ScanType
{
    Quick,      // Fast surface scan
    Deep,       // Comprehensive analysis
    Targeted,   // Focused on specific services
    Compliance, // Standards-based assessment
    Custom      // User-defined scope
}

/// <summary>
/// Vulnerability severity levels - the hierarchy of digital dangers
/// </summary>
public enum VulnerabilitySeverity
{
    Info,       // Informational only
    Low,        // Minor security concern
    Medium,     // Moderate security risk
    High,       // Serious security threat
    Critical    // Immediate action required
}

/// <summary>
/// Vulnerability categories - the taxonomy of digital threats
/// </summary>
public enum VulnerabilityCategory
{
    Authentication,         // Identity verification weaknesses
    Authorization,         // Access control failures
    Configuration,         // Setup and settings issues
    Encryption,           // Cryptographic weaknesses
    InputValidation,      // Data sanitization failures
    NetworkSecurity,      // Network layer vulnerabilities
    PrivilegeEscalation,  // Unauthorized permission elevation
    InformationDisclosure, // Data exposure risks
    DenialOfService,      // Availability threats
    CodeExecution,        // Remote execution vulnerabilities
    Unknown               // Unclassified threats
}

/// <summary>
/// Exploit severity levels - measuring the danger of attack vectors
/// </summary>
public enum ExploitSeverity
{
    Theoretical,    // Proof of concept only
    Low,           // Difficult to exploit
    Medium,        // Moderate exploitation difficulty
    High,          // Easy to exploit
    Active         // Exploits seen in the wild
}

/// <summary>
/// Scan intensity levels - balancing thoroughness with performance
/// </summary>
public enum ScanIntensity
{
    Stealthy,      // Slow and quiet
    Normal,        // Balanced approach
    Aggressive,    // Fast and thorough
    Insane         // Maximum speed and coverage
}

/// <summary>
/// Scan status enumeration - tracking the progress of digital reconnaissance
/// </summary>
public enum ScanStatus
{
    Pending,       // Awaiting execution
    Running,       // Currently scanning
    Completed,     // Successfully finished
    Failed,        // Encountered errors
    Cancelled,     // User terminated
    Timeout        // Exceeded time limits
}

/// <summary>
/// Scan progress information - tracking the journey through digital reconnaissance
/// </summary>
public class ScanProgress
{
    public string ScanId { get; set; } = string.Empty;
    public ScanStatus Status { get; set; }
    public double PercentComplete { get; set; }
    public string CurrentOperation { get; set; } = string.Empty;
    public int VulnerabilitiesFound { get; set; }
    public TimeSpan ElapsedTime { get; set; }
    public string LastUpdate { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Remediation suggestion - the wise counsel for vulnerability repair
/// </summary>
public class RemediationSuggestion
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string VulnerabilityId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Steps { get; set; } = new();
    public string PowerShellScript { get; set; } = string.Empty;
    public RemediationPriority Priority { get; set; }
    public RemediationEffort Effort { get; set; } = new();
    public List<string> Prerequisites { get; set; } = new();
    public List<string> RiskFactors { get; set; } = new();
    public bool RequiresReboot { get; set; }
    public bool RequiresElevation { get; set; }
    public string WittyComment { get; set; } = string.Empty;
    
    /// <summary>
    /// Generate witty remediation comment - making security fixes memorable
    /// </summary>
    public string GenerateWittyComment()
    {
        return Priority switch
        {
            RemediationPriority.Critical => "🚨 Drop everything and fix this - it's more urgent than coffee!",
            RemediationPriority.High => "⚡ High priority patch - this vulnerability is doing the digital tango!",
            RemediationPriority.Medium => "📋 Medium priority fix - not panic-worthy but needs attention!",
            RemediationPriority.Low => "🔧 Low priority tweak - like organizing your digital sock drawer!",
            RemediationPriority.Info => "ℹ️ Informational note - just a friendly heads-up from your security friend!",
            _ => "🎯 Remediation recommended - because secure networks are happy networks!"
        };
    }
}

/// <summary>
/// Remediation effort estimation - quantifying the digital repair work
/// </summary>
public class RemediationEffort
{
    public TimeSpan EstimatedDuration { get; set; }
    public RemediationComplexity Complexity { get; set; }
    public int SkillLevel { get; set; } // 1-10 scale
    public List<string> RequiredTools { get; set; } = new();
    public List<string> RequiredPermissions { get; set; } = new();
    public bool RequiresDowntime { get; set; }
    public string EffortDescription { get; set; } = string.Empty;
}

/// <summary>
/// Remediation template - reusable patterns for common fixes
/// </summary>
public class RemediationTemplate
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public VulnerabilityCategory Category { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ScriptTemplate { get; set; } = string.Empty;
    public List<string> Parameters { get; set; } = new();
    public List<string> Examples { get; set; } = new();
    public string Usage { get; set; } = string.Empty;
}

/// <summary>
/// Script validation result - ensuring safety of digital remediation
/// </summary>
public class ScriptValidationResult
{
    public bool IsValid { get; set; }
    public bool IsSafe { get; set; }
    public List<string> Warnings { get; set; } = new();
    public List<string> Errors { get; set; } = new();
    public List<string> SecurityConcerns { get; set; } = new();
    public string ValidationMessage { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Vulnerability heatmap data - visual representation of security threats in 3D space
/// </summary>
public class VulnerabilityHeatmapData
{
    public string ScanId { get; set; } = string.Empty;
    public List<VulnerabilityVisualization> Visualizations { get; set; } = new();
    public Dictionary<string, string> ColorMapping { get; set; } = new();
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Vulnerability visualization data for 3D rendering - cosmic threat representation
/// </summary>
public class VulnerabilityVisualization
{
    public string NodeId { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public VulnerabilitySeverity Severity { get; set; }
    public string Color { get; set; } = string.Empty;
    public double Intensity { get; set; } // 0.0 to 1.0
    public List<string> VulnerabilityIds { get; set; } = new();
    public string VisualizationType { get; set; } = "glow"; // glow, pulse, particle, etc.
    public Dictionary<string, object> EffectParameters { get; set; } = new();
    
    /// <summary>
    /// Generate visualization personality - giving character to threat displays
    /// </summary>
    public string GetVisualizationPersonality()
    {
        return Severity switch
        {
            VulnerabilitySeverity.Critical => "💀 Critical Threat - Blazing red beacon of digital doom!",
            VulnerabilitySeverity.High => "🔥 High Risk - Orange flames of concern dancing!",
            VulnerabilitySeverity.Medium => "⚠️ Medium Risk - Yellow caution lights blinking steadily!",
            VulnerabilitySeverity.Low => "🟡 Low Risk - Gentle amber glow of minor concern!",
            VulnerabilitySeverity.Info => "ℹ️ Information - Cool blue pulse of knowledge!",
            _ => "❓ Unknown Threat - Mysterious purple enigma swirling!"
        };
    }
}

/// <summary>
/// Threat level enumeration - the hierarchy of digital dangers
/// </summary>
public enum ThreatLevel
{
    Informational,  // No immediate threat
    Low,           // Minimal risk
    Elevated,      // Increased awareness needed
    High,          // Significant concern
    Severe,        // Major threat detected
    Critical       // Imminent danger present
}

/// <summary>
/// Remediation priority levels - the urgency hierarchy of digital repairs
/// </summary>
public enum RemediationPriority
{
    Info,       // Informational only
    Low,        // Can wait for maintenance window
    Medium,     // Should be addressed soon
    High,       // Needs immediate attention
    Critical    // Drop everything and fix now
}

/// <summary>
/// Remediation script types - varieties of digital healing incantations
/// </summary>
public enum RemediationScriptType
{
    Detection,      // Script to detect the issue
    Mitigation,     // Script to temporarily mitigate
    Remediation,    // Script to permanently fix
    Validation,     // Script to verify the fix
    Rollback        // Script to undo changes if needed
}

/// <summary>
/// Remediation complexity levels - measuring the difficulty of digital repairs
/// </summary>
public enum RemediationComplexity
{
    Trivial,        // One-click fix
    Simple,         // Basic commands
    Moderate,       // Multiple steps required
    Complex,        // Advanced configuration needed
    Expert          // Requires specialist knowledge
}