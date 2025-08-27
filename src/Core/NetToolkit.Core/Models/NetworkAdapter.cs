namespace NetToolkit.Core.Models;

public class NetworkAdapter
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MacAddress { get; set; } = string.Empty;
    public string AdapterType { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public ulong Speed { get; set; }
    public string[] IPAddresses { get; set; } = Array.Empty<string>();
    public string[] SubnetMasks { get; set; } = Array.Empty<string>();
    public string[] DefaultGateways { get; set; } = Array.Empty<string>();
    public string[] DnsServers { get; set; } = Array.Empty<string>();
    public bool DhcpEnabled { get; set; }
}