using NetToolkit.Core.Interfaces;
using NetToolkit.Modules.ScannerAndTopography.Models;

namespace NetToolkit.Modules.ScannerAndTopography.Interfaces;

/// <summary>
/// Scanner event publisher interface - the herald of digital discoveries
/// Where network revelations cascade across the modular cosmos
/// </summary>
public interface IScannerEventPublisher : IEventBus
{
    /// <summary>
    /// Publish scan initiation event - announcing the beginning of digital reconnaissance
    /// </summary>
    /// <param name="scanId">Unique scan identifier</param>
    /// <param name="networkRange">Target network range</param>
    /// <param name="options">Scan configuration options</param>
    Task PublishScanInitiatedAsync(string scanId, string networkRange, ScanOptions options);
    
    /// <summary>
    /// Publish port discovery event - celebrating each opened digital doorway
    /// </summary>
    /// <param name="nodeId">Node identifier</param>
    /// <param name="port">Discovered open port</param>
    /// <param name="service">Detected service if known</param>
    Task PublishPortDiscoveredAsync(string nodeId, int port, string? service = null);
    
    /// <summary>
    /// Publish topology update event - chronicling the evolution of network maps
    /// </summary>
    /// <param name="topology">Updated topology result</param>
    /// <param name="changeType">Type of topology change</param>
    Task PublishTopologyUpdatedAsync(TopologyResult topology, TopologyChangeType changeType);
    
    /// <summary>
    /// Publish configuration applied event - announcing successful network modifications
    /// </summary>
    /// <param name="nodeId">Configured node identifier</param>
    /// <param name="command">Applied configuration command</param>
    /// <param name="result">Configuration execution result</param>
    Task PublishConfigAppliedAsync(string nodeId, string command, bool success, string? message = null);
    
    /// <summary>
    /// Publish anomaly detection event - sounding alarms for digital peculiarities
    /// </summary>
    /// <param name="anomaly">Detected network anomaly</param>
    Task PublishAnomalyDetectedAsync(NetworkAnomaly anomaly);
    
    /// <summary>
    /// Publish scan completion event - celebrating the conclusion of digital exploration
    /// </summary>
    /// <param name="scanId">Completed scan identifier</param>
    /// <param name="statistics">Scan performance statistics</param>
    /// <param name="summary">Witty scan summary</summary>
    Task PublishScanCompletedAsync(string scanId, ScanStatistics statistics, string summary);
    
    /// <summary>
    /// Publish node status change event - tracking the vital signs of digital citizens
    /// </summary>
    /// <param name="nodeId">Node identifier</param>
    /// <param name="oldStatus">Previous node status</param>
    /// <param name="newStatus">Current node status</param>
    /// <param name="reason">Reason for status change</param>
    Task PublishNodeStatusChangedAsync(string nodeId, NodeStatus oldStatus, NodeStatus newStatus, string? reason = null);
    
    /// <summary>
    /// Publish network performance metrics - the heartbeat of digital infrastructure
    /// </summary>
    /// <param name="nodeId">Node identifier</param>
    /// <param name="metrics">Performance metrics</param>
    Task PublishPerformanceMetricsAsync(string nodeId, Dictionary<string, object> metrics);
}

/// <summary>
/// Topology change types - the categories of network evolution
/// </summary>
public enum TopologyChangeType
{
    NodeAdded,          // New device discovered
    NodeRemoved,        // Device went offline permanently  
    NodeModified,       // Device properties changed
    EdgeAdded,          // New connection established
    EdgeRemoved,        // Connection lost
    EdgeModified,       // Connection properties changed
    TopologyRefresh,    // Complete topology regeneration
    AnomalyDetected,    // Security or configuration anomaly
    PerformanceIssue    // Performance degradation detected
}