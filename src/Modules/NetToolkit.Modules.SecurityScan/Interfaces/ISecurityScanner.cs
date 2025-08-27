using NetToolkit.Modules.SecurityScan.Models;

namespace NetToolkit.Modules.SecurityScan.Interfaces;

/// <summary>
/// Security scanner interface - the omniscient sentinel of digital fortress assessment
/// Where vulnerabilities are unveiled with surgical precision and cosmic intelligence
/// </summary>
public interface ISecurityScanner
{
    /// <summary>
    /// Perform quick security scan - swift reconnaissance of digital perimeters
    /// </summary>
    /// <param name="target">Target specification for scanning</param>
    /// <param name="cancellationToken">Cancellation token for graceful termination</param>
    /// <returns>Quick scan results with immediate vulnerability assessment</returns>
    Task<ScanResult> QuickScanAsync(ScanTarget target, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Perform deep security scan - comprehensive archaeological dig of digital vulnerabilities
    /// </summary>
    /// <param name="target">Target specification for deep analysis</param>
    /// <param name="options">Depth configuration options</param>
    /// <param name="cancellationToken">Cancellation token for graceful termination</param>
    /// <returns>Deep scan results with exhaustive vulnerability intelligence</returns>
    Task<DeepScanResult> DeepScanAsync(ScanTarget target, DepthOptions options, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Perform targeted scan on specific services - focused digital investigation
    /// </summary>
    /// <param name="target">Target with specific services to analyze</param>
    /// <param name="services">List of services to focus on</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Targeted scan results</returns>
    Task<ScanResult> TargetedScanAsync(ScanTarget target, List<string> services, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Generate security report from scan results - transforming data into wisdom
    /// </summary>
    /// <param name="result">Scan results to process</param>
    /// <param name="format">Desired report format</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Generated report as byte array</returns>
    Task<byte[]> GenerateReportAsync(ScanResult result, ReportFormat format, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Validate scan target accessibility - ensuring targets are reachable
    /// </summary>
    /// <param name="target">Target to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if target is accessible</returns>
    Task<bool> ValidateTargetAsync(ScanTarget target, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get scan progress for running operations
    /// </summary>
    /// <param name="scanId">Scan identifier</param>
    /// <returns>Progress information</returns>
    Task<ScanProgress> GetScanProgressAsync(string scanId);
    
    /// <summary>
    /// Cancel running scan operation
    /// </summary>
    /// <param name="scanId">Scan identifier to cancel</param>
    /// <returns>True if cancellation successful</returns>
    Task<bool> CancelScanAsync(string scanId);
}

