using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Management;
using Microsoft.Extensions.Logging;
using NetToolkit.Modules.ScannerAndTopography.Interfaces;
using NetToolkit.Modules.ScannerAndTopography.Models;
using NetToolkit.Core.Interfaces;
using Polly;

namespace NetToolkit.Modules.ScannerAndTopography.Services;

/// <summary>
/// Network scanner service - the supreme conductor of digital reconnaissance
/// Where IP ranges transform into mapped universes through async sorcery
/// </summary>
public class NetworkScannerService : INetworkScanner
{
    private readonly ILogger<NetworkScannerService> _logger;
    private readonly IScannerEventPublisher _eventPublisher;
    private readonly Random _random = new();
    
    // Resilience policies for network operations
    private readonly IAsyncPolicy _pingPolicy;
    private readonly IAsyncPolicy _portScanPolicy;
    
    public NetworkScannerService(ILogger<NetworkScannerService> logger, IScannerEventPublisher eventPublisher)
    {
        _logger = logger;
        _eventPublisher = eventPublisher;
        
        // Cosmic resilience patterns - because networks are unpredictable beasts
        _pingPolicy = Policy
            .Handle<PingException>()
            .Or<SocketException>()
            .Or<TimeoutException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    _logger.LogDebug("Ping retry {RetryCount} after {Delay}ms - persistence is key in the digital realm! 🔄", 
                        retryCount, timespan.TotalMilliseconds);
                });
                
        _portScanPolicy = Policy
            .Handle<SocketException>()
            .Or<TimeoutException>()
            .WaitAndRetryAsync(
                retryCount: 2,
                sleepDurationProvider: retryAttempt => TimeSpan.FromMilliseconds(50 * retryAttempt),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    _logger.LogDebug("Port scan retry {RetryCount} - knocking harder on digital doors! 🚪", retryCount);
                });
    }

    /// <summary>
    /// Scan network range with comprehensive reconnaissance - the full digital symphony
    /// </summary>
    public async Task<TopologyResult> ScanAsync(string range, ScanOptions options, CancellationToken cancellationToken = default)
    {
        var scanId = Guid.NewGuid().ToString();
        var startTime = DateTime.UtcNow;
        
        _logger.LogInformation("🚀 Initiating cosmic network scan {ScanId} on range {Range} - prepare for digital enlightenment!", scanId, range);
        await _eventPublisher.PublishScanInitiatedAsync(scanId, range, options);
        
        try
        {
            var result = new TopologyResult
            {
                ScanId = scanId,
                ScanTimestamp = startTime,
                NetworkRange = range,
                Statistics = new ScanStatistics { ScanStartTime = startTime }
            };

            // Phase 1: Ping sweep - discovering the digital heartbeats
            _logger.LogInformation("🔍 Phase 1: Ping sweep across {Range} - listening for digital whispers...", range);
            var aliveNodes = await PingSweepAsync(range, options.PingTimeout, options.MaxConcurrentPings, cancellationToken);
            result.Nodes.AddRange(aliveNodes);

            _logger.LogInformation("💓 Discovered {Count} responsive nodes - the network pulse is strong!", aliveNodes.Count);

            // Phase 2: Port scanning - knocking on digital doors
            if (options.EnablePortScan && aliveNodes.Any())
            {
                _logger.LogInformation("🚪 Phase 2: Port scanning on {Count} nodes - exploring digital doorways...", aliveNodes.Count);
                var enrichedNodes = await ScanPortsAsync(aliveNodes, options.PortsToScan, options.PortTimeout, cancellationToken);
                result.Nodes = enrichedNodes;
            }

            // Phase 3: NIC enrichment - harvesting digital fingerprints
            if (options.EnableWmiDiscovery)
            {
                _logger.LogInformation("🧬 Phase 3: WMI enrichment - harvesting digital DNA...");
                await EnrichNodesWithNicInfo(result.Nodes, cancellationToken);
            }

            // Phase 4: Topology construction - weaving the network web
            _logger.LogInformation("🕸️ Phase 4: Topology construction - weaving the cosmic web...");
            result.Edges = ConstructNetworkEdges(result.Nodes);

            // Phase 5: Anomaly detection - uncovering digital peculiarities
            if (options.EnableAnomalyDetection)
            {
                _logger.LogInformation("🕵️ Phase 5: Anomaly detection - seeking digital mysteries...");
                result.Anomalies = await DetectAnomalies(result.Nodes, cancellationToken);
            }

            // Finalize statistics
            var endTime = DateTime.UtcNow;
            result.Statistics = CalculateStatistics(result, startTime, endTime);
            
            var summary = result.GetWittyScanSummary();
            _logger.LogInformation("✨ Scan completed: {Summary}", summary);
            
            await _eventPublisher.PublishScanCompletedAsync(scanId, result.Statistics, summary);
            await _eventPublisher.PublishTopologyUpdatedAsync(result, TopologyChangeType.TopologyRefresh);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Scan {ScanId} encountered cosmic turbulence - but we shall persevere!", scanId);
            throw;
        }
    }

    /// <summary>
    /// Perform ping sweep across IP range - gentle whispers across the digital void
    /// </summary>
    public async Task<List<NetworkNode>> PingSweepAsync(string range, int timeout = 1000, int maxConcurrent = 50, CancellationToken cancellationToken = default)
    {
        var ipAddresses = ParseIpRange(range);
        var semaphore = new SemaphoreSlim(maxConcurrent, maxConcurrent);
        var aliveNodes = new List<NetworkNode>();
        var lockObject = new object();

        _logger.LogDebug("🎯 Ping sweep targeting {Count} addresses with {MaxConcurrent} concurrent operations", ipAddresses.Count, maxConcurrent);

        var pingTasks = ipAddresses.Select(async ip =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                var node = await PingHostAsync(ip, timeout, cancellationToken);
                if (node != null)
                {
                    lock (lockObject)
                    {
                        aliveNodes.Add(node);
                    }
                }
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(pingTasks);
        
        _logger.LogInformation("🎉 Ping sweep complete - {AliveCount}/{TotalCount} nodes responded to our digital greeting!", 
            aliveNodes.Count, ipAddresses.Count);

        return aliveNodes;
    }

    /// <summary>
    /// Scan ports on discovered nodes - systematic exploration of digital doorways
    /// </summary>
    public async Task<List<NetworkNode>> ScanPortsAsync(List<NetworkNode> nodes, List<int> ports, int timeout = 2000, CancellationToken cancellationToken = default)
    {
        var enrichedNodes = new List<NetworkNode>();
        var semaphore = new SemaphoreSlim(10, 10); // Limit concurrent port scans per node

        var nodeTasks = nodes.Select(async node =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                var enrichedNode = await ScanNodePortsAsync(node, ports, timeout, cancellationToken);
                lock (enrichedNodes)
                {
                    enrichedNodes.Add(enrichedNode);
                }
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(nodeTasks);
        
        var totalOpenPorts = enrichedNodes.SelectMany(n => n.OpenPorts).Count();
        _logger.LogInformation("🔐 Port scanning complete - discovered {OpenPorts} open ports across {NodeCount} nodes!", 
            totalOpenPorts, enrichedNodes.Count);

        return enrichedNodes;
    }

    /// <summary>
    /// Get local NIC information - harvesting the machine's digital fingerprints
    /// </summary>
    public async Task<List<NicDetail>> GetNicInfoAsync(CancellationToken cancellationToken = default)
    {
        var nicDetails = new List<NicDetail>();
        
        try
        {
            _logger.LogDebug("🧬 Harvesting local NIC information - extracting digital DNA...");
            
            await Task.Run(() =>
            {
                using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = True");
                foreach (ManagementObject nic in searcher.Get())
                {
                    var detail = ExtractNicDetail(nic);
                    if (detail != null)
                    {
                        nicDetails.Add(detail);
                    }
                }
            }, cancellationToken);
            
            _logger.LogInformation("📡 Discovered {Count} active network interfaces - the digital fingerprints of connectivity!", nicDetails.Count);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠️ WMI harvesting encountered resistance - some digital secrets remain hidden!");
        }

        return nicDetails;
    }

    #region Private Helper Methods

    private async Task<NetworkNode?> PingHostAsync(IPAddress ip, int timeout, CancellationToken cancellationToken)
    {
        try
        {
            return await _pingPolicy.ExecuteAsync(async () =>
            {
                using var ping = new Ping();
                var reply = await ping.SendPingAsync(ip, timeout);
                
                if (reply.Status == IPStatus.Success)
                {
                    var node = new NetworkNode
                    {
                        Id = Guid.NewGuid().ToString(),
                        IpAddress = ip.ToString(),
                        IsOnline = true,
                        ResponseTime = reply.RoundtripTime,
                        LastSeen = DateTime.UtcNow,
                        FirstSeen = DateTime.UtcNow,
                        Position = Vector3D.RandomPosition(_random)
                    };

                    // Attempt reverse DNS lookup for hostname
                    try
                    {
                        var hostEntry = await Dns.GetHostEntryAsync(ip);
                        node.HostName = hostEntry.HostName;
                    }
                    catch
                    {
                        // Hostname lookup failed - no digital nameplate found
                        node.HostName = $"Unknown-{ip}";
                    }

                    _logger.LogDebug("💚 Node {IP} responded in {ResponseTime}ms - digital pulse detected!", ip, reply.RoundtripTime);
                    return node;
                }
                
                return null;
            });
        }
        catch (Exception ex)
        {
            _logger.LogDebug("💔 Node {IP} remains silent - {Error}", ip, ex.Message);
            return null;
        }
    }

    private async Task<NetworkNode> ScanNodePortsAsync(NetworkNode node, List<int> ports, int timeout, CancellationToken cancellationToken)
    {
        var enrichedNode = node;
        var openPorts = new List<int>();

        var portTasks = ports.Select(async port =>
        {
            var isOpen = await _portScanPolicy.ExecuteAsync(async () => await IsPortOpenAsync(node.IpAddress, port, timeout, cancellationToken));
            if (isOpen)
            {
                lock (openPorts)
                {
                    openPorts.Add(port);
                }
                await _eventPublisher.PublishPortDiscoveredAsync(node.Id, port);
                _logger.LogDebug("🚪 Port {Port} open on {IP} - digital doorway discovered!", port, node.IpAddress);
            }
        });

        await Task.WhenAll(portTasks);
        enrichedNode.OpenPorts = openPorts;

        return enrichedNode;
    }

    private async Task<bool> IsPortOpenAsync(string ipAddress, int port, int timeout, CancellationToken cancellationToken)
    {
        try
        {
            using var tcpClient = new TcpClient();
            var connectTask = tcpClient.ConnectAsync(ipAddress, port);
            var timeoutTask = Task.Delay(timeout, cancellationToken);
            
            var completedTask = await Task.WhenAny(connectTask, timeoutTask);
            
            if (completedTask == connectTask && tcpClient.Connected)
            {
                return true;
            }
        }
        catch
        {
            // Port connection failed - door remains closed
        }

        return false;
    }

    private List<IPAddress> ParseIpRange(string range)
    {
        var addresses = new List<IPAddress>();
        
        if (range.Contains('/'))
        {
            // CIDR notation (e.g., 192.168.1.0/24)
            addresses.AddRange(ParseCidrRange(range));
        }
        else if (range.Contains('-'))
        {
            // Range notation (e.g., 192.168.1.1-192.168.1.254)
            addresses.AddRange(ParseDashRange(range));
        }
        else if (IPAddress.TryParse(range, out var singleIp))
        {
            // Single IP address
            addresses.Add(singleIp);
        }
        else
        {
            _logger.LogWarning("🤔 Unrecognized IP range format: {Range} - digital cartography confused!", range);
        }

        return addresses;
    }

    private List<IPAddress> ParseCidrRange(string cidr)
    {
        var addresses = new List<IPAddress>();
        var parts = cidr.Split('/');
        
        if (parts.Length == 2 && IPAddress.TryParse(parts[0], out var networkAddress) && int.TryParse(parts[1], out var prefixLength))
        {
            var networkBytes = networkAddress.GetAddressBytes();
            var hostBits = 32 - prefixLength;
            var hostCount = (uint)(1 << hostBits) - 2; // Exclude network and broadcast

            for (uint i = 1; i <= hostCount; i++)
            {
                var hostBytes = BitConverter.GetBytes(BitConverter.ToUInt32(networkBytes, 0) + i);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(hostBytes);
                addresses.Add(new IPAddress(hostBytes));
            }
        }

        return addresses;
    }

    private List<IPAddress> ParseDashRange(string range)
    {
        var addresses = new List<IPAddress>();
        var parts = range.Split('-');
        
        if (parts.Length == 2 && IPAddress.TryParse(parts[0], out var startIp) && IPAddress.TryParse(parts[1], out var endIp))
        {
            var start = BitConverter.ToUInt32(startIp.GetAddressBytes().Reverse().ToArray(), 0);
            var end = BitConverter.ToUInt32(endIp.GetAddressBytes().Reverse().ToArray(), 0);

            for (uint i = start; i <= end; i++)
            {
                var bytes = BitConverter.GetBytes(i).Reverse().ToArray();
                addresses.Add(new IPAddress(bytes));
            }
        }

        return addresses;
    }

    private async Task EnrichNodesWithNicInfo(List<NetworkNode> nodes, CancellationToken cancellationToken)
    {
        try
        {
            var localNics = await GetNicInfoAsync(cancellationToken);
            
            foreach (var node in nodes)
            {
                var matchingNic = localNics.FirstOrDefault(nic => nic.IpAddress == node.IpAddress);
                if (matchingNic != null)
                {
                    node.MacAddress = matchingNic.MacAddress;
                    node.Manufacturer = matchingNic.Manufacturer;
                    node.SubnetMask = matchingNic.SubnetMask;
                    node.Gateway = matchingNic.Gateway;
                    node.IsDynamic = matchingNic.IsDhcpEnabled;
                    node.DeviceType = DetermineDeviceType(matchingNic, node.OpenPorts);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠️ NIC enrichment encountered cosmic interference - proceeding with basic data!");
        }
    }

    private List<NetworkEdge> ConstructNetworkEdges(List<NetworkNode> nodes)
    {
        var edges = new List<NetworkEdge>();
        
        // Simple subnet-based edge construction
        var subnetGroups = nodes.GroupBy(n => GetSubnetAddress(n.IpAddress, n.SubnetMask));
        
        foreach (var subnet in subnetGroups)
        {
            var subnetNodes = subnet.ToList();
            for (int i = 0; i < subnetNodes.Count; i++)
            {
                for (int j = i + 1; j < subnetNodes.Count; j++)
                {
                    edges.Add(new NetworkEdge
                    {
                        FromNodeId = subnetNodes[i].Id,
                        ToNodeId = subnetNodes[j].Id,
                        ConnectionType = EdgeType.Subnet,
                        Label = "Subnet Connection"
                    });
                }
            }
        }

        return edges;
    }

    private async Task<List<NetworkAnomaly>> DetectAnomalies(List<NetworkNode> nodes, CancellationToken cancellationToken)
    {
        var anomalies = new List<NetworkAnomaly>();
        
        await Task.Run(() =>
        {
            foreach (var node in nodes)
            {
                // Detect unusual port patterns
                if (node.OpenPorts.Count > 20)
                {
                    anomalies.Add(new NetworkAnomaly
                    {
                        NodeId = node.Id,
                        Type = AnomalyType.UnusualPortPattern,
                        Severity = AnomalySeverity.Medium,
                        Description = $"Excessive open ports detected ({node.OpenPorts.Count})",
                        RecommendedAction = "Review port configuration and close unnecessary services"
                    });
                }

                // Detect suspicious services
                var suspiciousPorts = new[] { 135, 139, 445, 1433, 3389 };
                var foundSuspicious = node.OpenPorts.Intersect(suspiciousPorts).ToList();
                
                if (foundSuspicious.Any())
                {
                    anomalies.Add(new NetworkAnomaly
                    {
                        NodeId = node.Id,
                        Type = AnomalyType.SuspiciousService,
                        Severity = AnomalySeverity.High,
                        Description = $"Potentially risky ports open: {string.Join(", ", foundSuspicious)}",
                        RecommendedAction = "Verify services are legitimate and properly secured"
                    });
                }
            }
        }, cancellationToken);

        foreach (var anomaly in anomalies)
        {
            await _eventPublisher.PublishAnomalyDetectedAsync(anomaly);
        }

        return anomalies;
    }

    private NicDetail? ExtractNicDetail(ManagementObject nic)
    {
        try
        {
            var ipAddresses = nic["IPAddress"] as string[];
            if (ipAddresses?.Length > 0)
            {
                return new NicDetail
                {
                    Name = nic["Description"]?.ToString() ?? "Unknown",
                    Description = nic["Description"]?.ToString() ?? "Unknown NIC",
                    MacAddress = nic["MACAddress"]?.ToString() ?? "Unknown",
                    IpAddress = ipAddresses[0],
                    SubnetMask = (nic["IPSubnet"] as string[])?[0] ?? "Unknown",
                    Gateway = (nic["DefaultIPGateway"] as string[])?[0] ?? "Unknown",
                    IsDhcpEnabled = (bool)(nic["DHCPEnabled"] ?? false),
                    IsEnabled = (bool)(nic["IPEnabled"] ?? false)
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug("Failed to extract NIC detail: {Error}", ex.Message);
        }

        return null;
    }

    private string DetermineDeviceType(NicDetail nic, List<int> openPorts)
    {
        if (openPorts.Contains(22) || openPorts.Contains(23)) return "Network Device";
        if (openPorts.Contains(80) || openPorts.Contains(443)) return "Web Server";
        if (openPorts.Contains(3389)) return "Windows Server";
        if (openPorts.Contains(21)) return "FTP Server";
        return "Workstation";
    }

    private string GetSubnetAddress(string ipAddress, string subnetMask)
    {
        try
        {
            var ip = IPAddress.Parse(ipAddress);
            var mask = IPAddress.Parse(subnetMask);
            
            var ipBytes = ip.GetAddressBytes();
            var maskBytes = mask.GetAddressBytes();
            var subnetBytes = new byte[4];
            
            for (int i = 0; i < 4; i++)
            {
                subnetBytes[i] = (byte)(ipBytes[i] & maskBytes[i]);
            }
            
            return new IPAddress(subnetBytes).ToString();
        }
        catch
        {
            return "Unknown";
        }
    }

    private ScanStatistics CalculateStatistics(TopologyResult result, DateTime startTime, DateTime endTime)
    {
        var duration = endTime - startTime;
        var activeHosts = result.Nodes.Count(n => n.IsOnline);
        
        return new ScanStatistics
        {
            ScanDuration = duration,
            TotalHostsScanned = result.Nodes.Count,
            ActiveHosts = activeHosts,
            TotalPortsProbed = result.Nodes.SelectMany(n => n.OpenPorts).Count(),
            OpenPortsFound = result.Nodes.SelectMany(n => n.OpenPorts).Count(),
            AnomaliesDetected = result.Anomalies.Count,
            ScanEfficiency = result.Nodes.Count > 0 ? (double)activeHosts / result.Nodes.Count : 0,
            ScanStartTime = startTime,
            ScanEndTime = endTime
        };
    }

    #endregion
}