using NetToolkit.Core.Models;

namespace NetToolkit.Core.Interfaces;

public interface INetworkScanModule : IModule
{
    Task<List<NetworkDevice>> QuickScanAsync(string networkRange);
    Task<List<NetworkDevice>> DeepScanAsync(string networkRange, List<int>? ports = null);
    Task<NetworkDevice?> ScanDeviceAsync(string ipAddress, List<int>? ports = null);
    Task<List<NetworkAdapter>> GetLocalNetworkAdaptersAsync();
    Task<bool> TestConnectivityAsync(string host, int port, int timeout = 5000);
}

public class PortScanResult
{
    public int Port { get; set; }
    public bool IsOpen { get; set; }
    public string Service { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
}