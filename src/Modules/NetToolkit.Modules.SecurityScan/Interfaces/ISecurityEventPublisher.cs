using NetToolkit.Core.Interfaces;
using NetToolkit.Modules.SecurityScan.Models;

namespace NetToolkit.Modules.SecurityScan.Interfaces;

/// <summary>
/// Security event publisher interface - the vigilant herald of digital fortress intelligence
/// Where vulnerability discoveries cascade across the modular cosmos with urgent wisdom
/// </summary>
public interface ISecurityEventPublisher : IEventBus
{
    /// <summary>
    /// Publish scan launch event - announcing the beginning of security reconnaissance
    /// </summary>
    /// <param name="scanId">Unique scan identifier</param>
    /// <param name="scanType">Type of security scan</param>
    /// <param name="targets">List of scan targets</param>
    Task PublishScanLaunchedAsync(string scanId, ScanType scanType, List<ScanTarget> targets);
    
    /// <summary>
    /// Publish vulnerability discovery event - sounding alarms for digital threats
    /// </summary>
    /// <param name="vulnerability">Discovered vulnerability</param>
    Task PublishVulnerabilityUnearthedAsync(VulnerabilityEntry vulnerability);
    
    /// <summary>
    /// Publish critical vulnerability alert - maximum priority digital emergency
    /// </summary>
    /// <param name="vulnerability">Critical vulnerability requiring immediate attention</param>
    Task PublishCriticalVulnerabilityAlertAsync(VulnerabilityEntry vulnerability);
    
    /// <summary>
    /// Publish scan report generation completion - celebrating intelligence compilation
    /// </summary>
    /// <param name="scanId">Scan identifier</param>
    /// <param name="reportFormat">Generated report format</param>
    /// <param name="summary">Scan summary statistics</param>
    Task PublishReportCraftedAsync(string scanId, ReportFormat reportFormat, ScanSeveritySummary summary);
    
    /// <summary>
    /// Publish remediation proposal event - offering solutions for digital ailments
    /// </summary>
    /// <param name="vulnerabilityId">Vulnerability requiring remediation</param>
    /// <param name="suggestion">Proposed remediation strategy</param>
    Task PublishMitigationProposedAsync(string vulnerabilityId, RemediationSuggestion suggestion);
    
    /// <summary>
    /// Publish security anomaly resolution - celebrating fixed vulnerabilities
    /// </summary>
    /// <param name="vulnerabilityId">Resolved vulnerability identifier</param>
    /// <param name="resolutionMethod">Method used for resolution</param>
    /// <param name="verificationResult">Post-fix verification status</param>
    Task PublishAnomalyResolvedAsync(string vulnerabilityId, string resolutionMethod, bool verificationResult);
    
    /// <summary>
    /// Publish scan progress update - chronicling the journey through digital assessment
    /// </summary>
    /// <param name="scanId">Scan identifier</param>
    /// <param name="progress">Current scan progress</param>
    Task PublishScanProgressUpdateAsync(string scanId, ScanProgress progress);
    
    /// <summary>
    /// Publish scan completion event - celebrating the conclusion of security assessment
    /// </summary>
    /// <param name="scanId">Completed scan identifier</param>
    /// <param name="result">Final scan results</param>
    /// <param name="summary">Witty security summary</param>
    Task PublishScanCompletedAsync(string scanId, ScanResult result, string summary);
    
    /// <summary>
    /// Publish vulnerability heatmap data for 3D visualization - painting threats in living color
    /// </summary>
    /// <param name="scanId">Scan identifier</param>
    /// <param name="heatmapData">Vulnerability visualization data</param>
    Task PublishVulnerabilityHeatmapAsync(string scanId, VulnerabilityHeatmapData heatmapData);
    
    /// <summary>
    /// Publish compliance assessment result - reporting adherence to digital governance
    /// </summary>
    /// <param name="scanId">Scan identifier</param>
    /// <param name="complianceResults">Compliance check results</param>
    Task PublishComplianceAssessmentAsync(string scanId, List<ComplianceCheck> complianceResults);
    
    /// <summary>
    /// Publish security metric update - quantifying digital fortress strength
    /// </summary>
    /// <param name="scanId">Scan identifier</param>
    /// <param name="metrics">Security metrics and KPIs</param>
    Task PublishSecurityMetricsAsync(string scanId, Dictionary<string, object> metrics);
    
    /// <summary>
    /// Publish exploit intelligence - warning of active threat vectors
    /// </summary>
    /// <param name="exploitInfo">Discovered exploit information</param>
    /// <param name="threatLevel">Assessed threat level</param>
    Task PublishExploitIntelligenceAsync(ExploitInfo exploitInfo, ThreatLevel threatLevel);
}

