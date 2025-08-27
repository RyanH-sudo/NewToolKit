using NetToolkit.Core.Models;

namespace NetToolkit.Core.Events;

public class NetworkScanCompletedEvent
{
    public string ScanId { get; set; } = string.Empty;
    public string NetworkRange { get; set; } = string.Empty;
    public List<NetworkDevice> Devices { get; set; } = new();
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    public TimeSpan Duration { get; set; }
    public string ScanType { get; set; } = string.Empty;
}