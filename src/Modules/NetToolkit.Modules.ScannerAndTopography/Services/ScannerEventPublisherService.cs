using Microsoft.Extensions.Logging;
using NetToolkit.Core.Interfaces;
using NetToolkit.Modules.ScannerAndTopography.Interfaces;
using NetToolkit.Modules.ScannerAndTopography.Models;

namespace NetToolkit.Modules.ScannerAndTopography.Services;

/// <summary>
/// Scanner event publisher service - the herald of digital discoveries
/// Where network revelations cascade across the modular cosmos with witty charm
/// </summary>
public class ScannerEventPublisherService : IScannerEventPublisher
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<ScannerEventPublisherService> _logger;
    private readonly Dictionary<string, int> _eventCounts = new();
    private readonly object _lockObject = new();

    public ScannerEventPublisherService(IEventBus eventBus, ILogger<ScannerEventPublisherService> logger)
    {
        _eventBus = eventBus;
        _logger = logger;
        
        _logger.LogInformation("📡 Scanner Event Publisher initialized - ready to broadcast digital discoveries across the cosmos!");
    }

    #region IEventBus Implementation

    /// <summary>
    /// Publish async event with base event bus functionality
    /// </summary>
    public async Task PublishAsync<T>(T eventData, CancellationToken cancellationToken = default) where T : class
    {
        await _eventBus.PublishAsync(eventData, cancellationToken);
    }

    public async Task SubscribeAsync<T>(Func<T, Task> handler) where T : class
    {
        await _eventBus.SubscribeAsync(handler);
    }
    
    public async Task SubscribeAsync<T>(string eventName, Func<T, Task> handler) where T : class
    {
        await _eventBus.SubscribeAsync(eventName, handler);
    }
    
    public async Task UnsubscribeAsync<T>() where T : class
    {
        await _eventBus.UnsubscribeAsync<T>();
    }
    
    public async Task UnsubscribeAsync<T>(string eventName, Func<T, Task> handler) where T : class
    {
        await _eventBus.UnsubscribeAsync(eventName, handler);
    }

    /// <summary>
    /// Subscribe to events with base event bus functionality
    /// </summary>
    public void Subscribe<T>(Func<T, Task> handler) where T : class
    {
        _eventBus.Subscribe(handler);
    }

    /// <summary>
    /// Unsubscribe from events with base event bus functionality
    /// </summary>
    public void Unsubscribe<T>(Func<T, Task> handler) where T : class
    {
        _eventBus.Unsubscribe(handler);
    }
    
    #endregion

    /// <summary>
    /// Publish scan initiation event - announcing the beginning of digital reconnaissance
    /// </summary>
    public async Task PublishScanInitiatedAsync(string scanId, string networkRange, ScanOptions options)
    {
        var eventData = new
        {
            EventType = "ScanInitiated",
            ScanId = scanId,
            NetworkRange = networkRange,
            Options = new
            {
                options.PingTimeout,
                options.MaxConcurrentPings,
                options.PortsToScan,
                options.EnablePortScan,
                options.EnableWmiDiscovery,
                options.EnableAnomalyDetection
            },
            Timestamp = DateTime.UtcNow,
            Source = "NetworkScanner"
        };

        await PublishWithLogging("ScanInitiated", eventData);
        
        _logger.LogInformation("🚀 Scan {ScanId} initiated on range {NetworkRange} - digital expedition begins! Target ports: {PortCount}",
            scanId, networkRange, options.PortsToScan.Count);
    }

    /// <summary>
    /// Publish port discovery event - celebrating each opened digital doorway
    /// </summary>
    public async Task PublishPortDiscoveredAsync(string nodeId, int port, string? service = null)
    {
        var eventData = new
        {
            EventType = "PortDiscovered",
            NodeId = nodeId,
            Port = port,
            Service = service ?? DetermineServiceByPort(port),
            ServicePersonality = GetServicePersonality(port),
            Timestamp = DateTime.UtcNow,
            Source = "PortScanner"
        };

        await PublishWithLogging("PortDiscovered", eventData);
        
        _logger.LogDebug("🚪 Port {Port} discovered on node {NodeId} - {ServicePersonality}",
            port, nodeId, GetServicePersonality(port));
    }

    /// <summary>
    /// Publish topology update event - chronicling the evolution of network maps
    /// </summary>
    public async Task PublishTopologyUpdatedAsync(TopologyResult topology, TopologyChangeType changeType)
    {
        var eventData = new
        {
            EventType = "TopologyUpdated",
            ScanId = topology.ScanId,
            ChangeType = changeType.ToString(),
            NetworkRange = topology.NetworkRange,
            NodeCount = topology.Nodes.Count,
            EdgeCount = topology.Edges.Count,
            ActiveNodes = topology.Nodes.Count(n => n.IsOnline),
            AnomalyCount = topology.Anomalies.Count,
            Statistics = new
            {
                topology.Statistics.ScanDuration,
                topology.Statistics.ScanEfficiency,
                topology.Statistics.TotalHostsScanned,
                topology.Statistics.OpenPortsFound,
                Commentary = topology.Statistics.GetPerformanceCommentary()
            },
            Summary = topology.GetWittyScanSummary(),
            Timestamp = DateTime.UtcNow,
            Source = "TopologyEngine"
        };

        await PublishWithLogging("TopologyUpdated", eventData);
        
        _logger.LogInformation("🗺️ Topology updated ({ChangeType}) - {NodeCount} nodes, {EdgeCount} connections, {AnomalyCount} anomalies detected",
            changeType, topology.Nodes.Count, topology.Edges.Count, topology.Anomalies.Count);
    }

    /// <summary>
    /// Publish configuration applied event - announcing successful network modifications
    /// </summary>
    public async Task PublishConfigAppliedAsync(string nodeId, string command, bool success, string? message = null)
    {
        var eventData = new
        {
            EventType = "ConfigurationApplied",
            NodeId = nodeId,
            Command = command,
            Success = success,
            Message = message ?? (success ? "Configuration applied successfully! ✨" : "Configuration encountered cosmic turbulence! 💫"),
            Timestamp = DateTime.UtcNow,
            Source = "ConfigurationUtility"
        };

        await PublishWithLogging("ConfigurationApplied", eventData);
        
        var emoji = success ? "✅" : "❌";
        _logger.LogInformation("{Emoji} Configuration '{Command}' on node {NodeId} - {Status}",
            emoji, command, nodeId, success ? "Success" : "Failed");
    }

    /// <summary>
    /// Publish anomaly detection event - sounding alarms for digital peculiarities
    /// </summary>
    public async Task PublishAnomalyDetectedAsync(NetworkAnomaly anomaly)
    {
        var eventData = new
        {
            EventType = "AnomalyDetected",
            AnomalyId = anomaly.Id,
            NodeId = anomaly.NodeId,
            Type = anomaly.Type.ToString(),
            Severity = anomaly.Severity.ToString(),
            Description = anomaly.Description,
            WittyDescription = anomaly.GetWittyDescription(),
            RecommendedAction = anomaly.RecommendedAction,
            DetectedAt = anomaly.DetectedAt,
            SeverityLevel = GetSeverityLevel(anomaly.Severity),
            Timestamp = DateTime.UtcNow,
            Source = "AnomalyDetector"
        };

        await PublishWithLogging("AnomalyDetected", eventData);
        
        var severityEmoji = GetSeverityEmoji(anomaly.Severity);
        _logger.LogWarning("{Emoji} Anomaly detected on node {NodeId}: {WittyDescription} (Severity: {Severity})",
            severityEmoji, anomaly.NodeId, anomaly.GetWittyDescription(), anomaly.Severity);
    }

    /// <summary>
    /// Publish scan completion event - celebrating the conclusion of digital exploration
    /// </summary>
    public async Task PublishScanCompletedAsync(string scanId, ScanStatistics statistics, string summary)
    {
        var eventData = new
        {
            EventType = "ScanCompleted",
            ScanId = scanId,
            Statistics = new
            {
                statistics.ScanDuration,
                statistics.TotalHostsScanned,
                statistics.ActiveHosts,
                statistics.OpenPortsFound,
                statistics.AnomaliesDetected,
                statistics.ScanEfficiency,
                EfficiencyPercentage = statistics.ScanEfficiency * 100,
                Commentary = statistics.GetPerformanceCommentary()
            },
            Summary = summary,
            CompletionTime = DateTime.UtcNow,
            ScanStartTime = statistics.ScanStartTime,
            ScanEndTime = statistics.ScanEndTime,
            Timestamp = DateTime.UtcNow,
            Source = "NetworkScanner"
        };

        await PublishWithLogging("ScanCompleted", eventData);
        
        _logger.LogInformation("🏁 Scan {ScanId} completed! Duration: {Duration:mm\\:ss}, Efficiency: {Efficiency:F1}% - {Summary}",
            scanId, statistics.ScanDuration, statistics.ScanEfficiency * 100, summary);
    }

    /// <summary>
    /// Publish node status change event - tracking the vital signs of digital citizens
    /// </summary>
    public async Task PublishNodeStatusChangedAsync(string nodeId, NodeStatus oldStatus, NodeStatus newStatus, string? reason = null)
    {
        var eventData = new
        {
            EventType = "NodeStatusChanged",
            NodeId = nodeId,
            OldStatus = oldStatus.ToString(),
            NewStatus = newStatus.ToString(),
            StatusTransition = $"{oldStatus} → {newStatus}",
            Reason = reason ?? "Status change detected through cosmic monitoring",
            StatusPersonality = GetStatusPersonality(newStatus),
            Timestamp = DateTime.UtcNow,
            Source = "NodeMonitor"
        };

        await PublishWithLogging("NodeStatusChanged", eventData);
        
        var statusEmoji = GetStatusEmoji(newStatus);
        _logger.LogInformation("{Emoji} Node {NodeId} status changed: {OldStatus} → {NewStatus} ({Reason})",
            statusEmoji, nodeId, oldStatus, newStatus, reason ?? "Cosmic monitoring");
    }

    /// <summary>
    /// Publish network performance metrics - the heartbeat of digital infrastructure
    /// </summary>
    public async Task PublishPerformanceMetricsAsync(string nodeId, Dictionary<string, object> metrics)
    {
        var eventData = new
        {
            EventType = "PerformanceMetrics",
            NodeId = nodeId,
            Metrics = metrics,
            MetricsCount = metrics.Count,
            MetricsPersonality = GetMetricsPersonality(metrics),
            Timestamp = DateTime.UtcNow,
            Source = "PerformanceMonitor"
        };

        await PublishWithLogging("PerformanceMetrics", (object)eventData);
        
        _logger.LogDebug("📊 Performance metrics for node {NodeId} - {MetricCount} measurements: {Personality}",
            nodeId, metrics.Count, GetMetricsPersonality(metrics));
    }

    #region Private Helper Methods

    private async Task PublishWithLogging(string eventType, object eventData)
    {
        try
        {
            // Track event statistics
            lock (_lockObject)
            {
                _eventCounts[eventType] = _eventCounts.GetValueOrDefault(eventType, 0) + 1;
            }

            await _eventBus.PublishAsync(eventData, CancellationToken.None);
            
            _logger.LogDebug("📤 Event published: {EventType} (Total: {Count})", eventType, _eventCounts[eventType]);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Failed to publish {EventType} event - cosmic communication failure!", eventType);
            throw;
        }
    }

    private string DetermineServiceByPort(int port)
    {
        return port switch
        {
            21 => "FTP",
            22 => "SSH",
            23 => "Telnet",
            25 => "SMTP",
            53 => "DNS",
            80 => "HTTP",
            110 => "POP3",
            143 => "IMAP",
            443 => "HTTPS",
            993 => "IMAPS",
            995 => "POP3S",
            3389 => "RDP",
            1433 => "SQL Server",
            3306 => "MySQL",
            5432 => "PostgreSQL",
            _ => "Unknown Service"
        };
    }

    private string GetServicePersonality(int port)
    {
        return port switch
        {
            21 => "🗂️ FTP - The file transfer faithful",
            22 => "🔒 SSH - The secure shell guardian",
            23 => "📡 Telnet - The nostalgic communicator",
            25 => "📧 SMTP - The email messenger",
            53 => "🗺️ DNS - The address book keeper",
            80 => "🌐 HTTP - The web world's doorkeeper",
            110 => "📬 POP3 - The mailbox collector",
            143 => "📨 IMAP - The mail synchronizer",
            443 => "🛡️ HTTPS - The secure web sentinel",
            993 => "🔐 IMAPS - The encrypted mail guardian",
            995 => "🔒 POP3S - The secure postal worker",
            3389 => "🖥️ RDP - The remote desktop wizard",
            1433 => "🗄️ SQL Server - The data warehouse keeper",
            3306 => "🐬 MySQL - The dolphin database",
            5432 => "🐘 PostgreSQL - The elephant database",
            _ => "❓ Unknown Service - Mystery port detective needed!"
        };
    }

    private string GetSeverityEmoji(AnomalySeverity severity)
    {
        return severity switch
        {
            AnomalySeverity.Info => "ℹ️",
            AnomalySeverity.Low => "⚠️",
            AnomalySeverity.Medium => "🟡",
            AnomalySeverity.High => "🟠",
            AnomalySeverity.Critical => "🔴",
            _ => "❓"
        };
    }

    private int GetSeverityLevel(AnomalySeverity severity)
    {
        return severity switch
        {
            AnomalySeverity.Info => 1,
            AnomalySeverity.Low => 2,
            AnomalySeverity.Medium => 3,
            AnomalySeverity.High => 4,
            AnomalySeverity.Critical => 5,
            _ => 0
        };
    }

    private string GetStatusEmoji(NodeStatus status)
    {
        return status switch
        {
            NodeStatus.Normal => "✅",
            NodeStatus.Warning => "⚠️",
            NodeStatus.Critical => "🔴",
            NodeStatus.Anomaly => "🚨",
            NodeStatus.Offline => "💤",
            NodeStatus.Unknown => "❓",
            _ => "🤔"
        };
    }

    private string GetStatusPersonality(NodeStatus status)
    {
        return status switch
        {
            NodeStatus.Normal => "🌟 All systems operational - digital zen achieved!",
            NodeStatus.Warning => "⚠️ Caution advised - minor cosmic disturbances detected",
            NodeStatus.Critical => "🆘 Critical status - immediate attention required!",
            NodeStatus.Anomaly => "👽 Anomalous behavior - investigation warranted",
            NodeStatus.Offline => "😴 Gone dark - digital hibernation mode",
            NodeStatus.Unknown => "🔮 Status mysterious - cosmic divination needed",
            _ => "🤷 Status unclear - consulting digital oracles"
        };
    }

    private string GetMetricsPersonality(Dictionary<string, object> metrics)
    {
        if (!metrics.Any()) return "📊 Empty metrics - digital silence reigns";
        
        var metricTypes = metrics.Keys.ToList();
        
        return metricTypes.Count switch
        {
            1 => $"📈 Solo metric: {metricTypes[0]} - focused monitoring",
            2 => $"📊 Dual metrics: {string.Join(" & ", metricTypes)} - balanced observation",
            <= 5 => $"📈 Multi-metric monitoring: {metricTypes.Count} measurements - comprehensive view",
            _ => $"📊 Metric bonanza: {metricTypes.Count} measurements - data scientist's dream!"
        };
    }

    /// <summary>
    /// Get event statistics for monitoring and debugging
    /// </summary>
    public Dictionary<string, int> GetEventStatistics()
    {
        lock (_lockObject)
        {
            return new Dictionary<string, int>(_eventCounts);
        }
    }

    /// <summary>
    /// Reset event statistics
    /// </summary>
    public void ResetEventStatistics()
    {
        lock (_lockObject)
        {
            _eventCounts.Clear();
        }
        
        _logger.LogInformation("🔄 Event statistics reset - cosmic counters zeroed!");
    }

    #endregion
}