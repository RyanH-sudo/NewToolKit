namespace NetToolkit.Core.Models;

public class NetworkDevice
{
    public string IPAddress { get; set; } = string.Empty;
    public string HostName { get; set; } = string.Empty;
    public bool IsOnline { get; set; }
    public long ResponseTime { get; set; }
    public string MacAddress { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public List<int> OpenPorts { get; set; } = new();
    public DateTime LastSeen { get; set; } = DateTime.UtcNow;
}