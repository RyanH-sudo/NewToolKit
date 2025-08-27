using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using NetToolkit.Core.Interfaces;
using NetToolkit.Core.Models;

namespace NetToolkit.Core.Services;

public class NetworkInfoService
{
    private readonly ILoggerWrapper _logger;

    public NetworkInfoService(ILoggerWrapper logger)
    {
        _logger = logger;
    }

    public async Task<List<NetworkAdapter>> GetNetworkAdaptersAsync()
    {
        var adapters = new List<NetworkAdapter>();

        try
        {
            using var searcher = new ManagementObjectSearcher(
                "SELECT * FROM Win32_NetworkAdapter WHERE NetEnabled = true");
            
            var collection = await Task.Run(() => searcher.Get());

            foreach (ManagementObject adapter in collection)
            {
                var networkAdapter = new NetworkAdapter
                {
                    Name = adapter["Name"]?.ToString() ?? "Unknown",
                    Description = adapter["Description"]?.ToString() ?? "Unknown",
                    MacAddress = adapter["MACAddress"]?.ToString() ?? "Unknown",
                    AdapterType = adapter["AdapterType"]?.ToString() ?? "Unknown",
                    Manufacturer = adapter["Manufacturer"]?.ToString() ?? "Unknown",
                    Speed = Convert.ToUInt64(adapter["Speed"] ?? 0)
                };

                // Get IP configuration
                await PopulateIpConfigurationAsync(networkAdapter, adapter["Index"]?.ToString());
                
                adapters.Add(networkAdapter);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve network adapters");
        }

        return adapters;
    }

    public async Task<List<NetworkDevice>> PingSweepAsync(string subnet, int timeout = 1000)
    {
        var devices = new List<NetworkDevice>();
        var tasks = new List<Task>();

        try
        {
            var network = IPNetwork.Parse(subnet);
            var semaphore = new SemaphoreSlim(50); // Limit concurrent pings

            foreach (var ip in network.ListIPAddress())
            {
                tasks.Add(PingDeviceAsync(ip, timeout, devices, semaphore));
            }

            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to perform ping sweep on subnet {Subnet}", subnet);
        }

        return devices.OrderBy(d => d.IPAddress).ToList();
    }

    private async Task PingDeviceAsync(IPAddress ipAddress, int timeout, List<NetworkDevice> devices, SemaphoreSlim semaphore)
    {
        await semaphore.WaitAsync();
        
        try
        {
            using var ping = new Ping();
            var reply = await ping.SendPingAsync(ipAddress, timeout);

            if (reply.Status == IPStatus.Success)
            {
                var device = new NetworkDevice
                {
                    IPAddress = ipAddress.ToString(),
                    IsOnline = true,
                    ResponseTime = reply.RoundtripTime,
                    HostName = await GetHostNameAsync(ipAddress)
                };

                lock (devices)
                {
                    devices.Add(device);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to ping {IPAddress}", ipAddress);
        }
        finally
        {
            semaphore.Release();
        }
    }

    private async Task<string> GetHostNameAsync(IPAddress ipAddress)
    {
        try
        {
            var hostEntry = await Dns.GetHostEntryAsync(ipAddress);
            return hostEntry.HostName;
        }
        catch
        {
            return "Unknown";
        }
    }

    private async Task PopulateIpConfigurationAsync(NetworkAdapter adapter, string? adapterIndex)
    {
        if (string.IsNullOrEmpty(adapterIndex)) return;

        try
        {
            using var searcher = new ManagementObjectSearcher(
                $"SELECT * FROM Win32_NetworkAdapterConfiguration WHERE Index = {adapterIndex}");
            
            var collection = await Task.Run(() => searcher.Get());

            foreach (ManagementObject config in collection)
            {
                adapter.IPAddresses = ConvertToStringArray(config["IPAddress"]);
                adapter.SubnetMasks = ConvertToStringArray(config["IPSubnet"]);
                adapter.DefaultGateways = ConvertToStringArray(config["DefaultIPGateway"]);
                adapter.DnsServers = ConvertToStringArray(config["DNSServerSearchOrder"]);
                adapter.DhcpEnabled = Convert.ToBoolean(config["DHCPEnabled"]);
                break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get IP configuration for adapter index {Index}", adapterIndex);
        }
    }

    private static string[] ConvertToStringArray(object? value)
    {
        return value switch
        {
            string[] array => array,
            string single => new[] { single },
            _ => Array.Empty<string>()
        };
    }
}

// Simple IP network helper
public readonly struct IPNetwork
{
    public IPAddress NetworkAddress { get; }
    public int PrefixLength { get; }

    public IPNetwork(IPAddress networkAddress, int prefixLength)
    {
        NetworkAddress = networkAddress;
        PrefixLength = prefixLength;
    }

    public static IPNetwork Parse(string cidr)
    {
        var parts = cidr.Split('/');
        var networkAddress = IPAddress.Parse(parts[0]);
        var prefixLength = int.Parse(parts[1]);
        return new IPNetwork(networkAddress, prefixLength);
    }

    public IEnumerable<IPAddress> ListIPAddress()
    {
        var networkBytes = NetworkAddress.GetAddressBytes();
        var hostBits = 32 - PrefixLength;
        var hostCount = (uint)(1 << hostBits) - 2; // Exclude network and broadcast

        for (uint i = 1; i <= hostCount; i++)
        {
            var hostBytes = BitConverter.GetBytes(i);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(hostBytes);

            var ipBytes = new byte[4];
            for (int j = 0; j < 4; j++)
            {
                ipBytes[j] = (byte)(networkBytes[j] | hostBytes[j]);
            }

            yield return new IPAddress(ipBytes);
        }
    }
}