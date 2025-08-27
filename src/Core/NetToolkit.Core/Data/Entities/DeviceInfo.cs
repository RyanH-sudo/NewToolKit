namespace NetToolkit.Core.Data.Entities;

public class DeviceInfo
{
    public int Id { get; set; }
    public string IPAddress { get; set; } = string.Empty;
    public string HostName { get; set; } = string.Empty;
    public string MacAddress { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty; // Router, Switch, Server, Workstation, etc.
    public string OperatingSystem { get; set; } = string.Empty;
    public string OpenPorts { get; set; } = string.Empty; // JSON array of ports
    public bool IsOnline { get; set; }
    public DateTime LastSeen { get; set; } = DateTime.UtcNow;
    public DateTime FirstSeen { get; set; } = DateTime.UtcNow;
    public string Notes { get; set; } = string.Empty;
}