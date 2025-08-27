using NetToolkit.Modules.ScannerAndTopography.Models;

namespace NetToolkit.Modules.ScannerAndTopography.Interfaces;

/// <summary>
/// Network scanner interface - the divine blueprint for digital reconnaissance
/// Where IP ranges transform into mapped territories of cyber enlightenment
/// </summary>
public interface INetworkScanner
{
    /// <summary>
    /// Scan network range with surgical precision - unveiling the hidden cosmos
    /// </summary>
    /// <param name="range">IP range to explore - the digital frontier</param>
    /// <param name="options">Scanning configuration - fine-tuning the cosmic probe</param>
    /// <returns>Complete topology result - the mapped universe revealed</returns>
    Task<TopologyResult> ScanAsync(string range, ScanOptions options, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Harvest local NIC information - excavating the machine's digital DNA
    /// </summary>
    /// <returns>List of network interface details - the fingerprints of connectivity</returns>
    Task<List<NicDetail>> GetNicInfoAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Perform targeted ping sweep - gentle whispers across the digital void
    /// </summary>
    /// <param name="range">IP range for ping reconnaissance</param>
    /// <param name="timeout">Response timeout in milliseconds</param>
    /// <param name="maxConcurrent">Maximum concurrent ping operations</param>
    /// <returns>List of responsive network nodes - the digital heartbeats</returns>
    Task<List<NetworkNode>> PingSweepAsync(string range, int timeout = 1000, int maxConcurrent = 50, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Scan ports on discovered nodes - knocking on digital doors
    /// </summary>
    /// <param name="nodes">Nodes to probe for open ports</param>
    /// <param name="ports">Port numbers to scan</param>
    /// <param name="timeout">Connection timeout per port</param>
    /// <returns>Nodes enriched with port information - the full digital portrait</returns>
    Task<List<NetworkNode>> ScanPortsAsync(List<NetworkNode> nodes, List<int> ports, int timeout = 2000, CancellationToken cancellationToken = default);
}