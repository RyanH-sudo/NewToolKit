using System.Collections.Concurrent;
using System.IO.Ports;
using System.Management;
using Microsoft.Extensions.Logging;
using NetToolkit.Modules.SshTerminal.Models;
using NetToolkit.Modules.SshTerminal.Interfaces;

namespace NetToolkit.Modules.SshTerminal.Services;

/// <summary>
/// Device scanner service - the omniscient sentinel of hardware reconnaissance
/// Where physical devices reveal their digital secrets through systematic scanning
/// </summary>
public class DeviceScanner : IDeviceDetector, IDisposable
{
    private readonly ILogger<DeviceScanner> _logger;
    private readonly ISshEventPublisher _eventPublisher;
    
    private readonly ConcurrentDictionary<DeviceType, List<DeviceInfo>> _deviceCache = new();
    private readonly Timer? _monitoringTimer;
    private readonly SemaphoreSlim _scanSemaphore = new(1, 1);
    
    private volatile bool _isMonitoring;
    private volatile bool _disposed;
    private DeviceType? _monitoringType;
    
    // Windows Management Instrumentation watchers
    private ManagementEventWatcher? _usbWatcher;
    private ManagementEventWatcher? _serialWatcher;
    
    public DeviceScanner(ILogger<DeviceScanner> logger, ISshEventPublisher eventPublisher)
    {
        _logger = logger;
        _eventPublisher = eventPublisher;
        
        // Initialize device cache
        foreach (DeviceType deviceType in Enum.GetValues<DeviceType>())
        {
            _deviceCache[deviceType] = new List<DeviceInfo>();
        }
        
        _logger.LogInformation("🔍 Device Scanner initialized - ready to discover hardware companions across digital realms!");
    }
    
    #region IDeviceDetector Implementation
    
    /// <summary>
    /// Scan for devices of specified type - targeted hardware reconnaissance
    /// </summary>
    public async Task<DeviceScanResult> ScanDevicesAsync(DeviceType deviceType, CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.Now;
        
        try
        {
            await _scanSemaphore.WaitAsync(cancellationToken);
            
            _logger.LogInformation("🔍 Initiating {DeviceType} device scan - hunting for hardware treasures!", deviceType);
            
            var devices = deviceType switch
            {
                DeviceType.Serial => await ScanSerialDevicesAsync(cancellationToken),
                DeviceType.USB => await ScanUsbDevicesAsync(cancellationToken),
                DeviceType.Bluetooth => await ScanBluetoothDevicesAsync(cancellationToken),
                DeviceType.Network => await ScanNetworkDevicesAsync(cancellationToken),
                _ => new List<DeviceInfo>()
            };
            
            // Update cache
            _deviceCache[deviceType] = devices;
            
            // Fire device detected events
            foreach (var device in devices.Where(d => d.IsAvailable))
            {
                DeviceDetected?.Invoke(this, new DeviceDetectedEvent
                {
                    SessionId = Guid.NewGuid().ToString(),
                    Device = device,
                    DeviceType = deviceType
                });
                
                await _eventPublisher.PublishDeviceDetectedAsync(device, deviceType);
            }
            
            var scanDuration = DateTime.Now - startTime;
            var result = new DeviceScanResult
            {
                Devices = devices,
                ScanTimestamp = DateTime.Now,
                ScanDuration = scanDuration
            };
            
            _logger.LogInformation("✨ {DeviceType} scan completed - discovered {DeviceCount} devices in {Duration:F2}s!",
                deviceType, devices.Count, scanDuration.TotalSeconds);
            
            return result;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("⏰ Device scan cancelled for {DeviceType} - reconnaissance interrupted!", deviceType);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error scanning {DeviceType} devices - hardware remains elusive!", deviceType);
            
            return new DeviceScanResult
            {
                Devices = new List<DeviceInfo>(),
                ScanTimestamp = DateTime.Now,
                ScanDuration = DateTime.Now - startTime
            };
        }
        finally
        {
            _scanSemaphore.Release();
        }
    }
    
    /// <summary>
    /// Scan all device types - omnipotent hardware census
    /// </summary>
    public async Task<DeviceScanResult> ScanAllDevicesAsync(CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.Now;
        var allDevices = new List<DeviceInfo>();
        
        _logger.LogInformation("🌟 Initiating comprehensive device scan - seeking all hardware across digital realms!");
        
        try
        {
            var deviceTypes = Enum.GetValues<DeviceType>();
            var scanTasks = deviceTypes.Select(type => ScanDevicesAsync(type, cancellationToken));
            
            var results = await Task.WhenAll(scanTasks);
            
            foreach (var result in results)
            {
                allDevices.AddRange(result.Devices);
            }
            
            var totalDuration = DateTime.Now - startTime;
            
            _logger.LogInformation("🎯 Comprehensive scan completed - catalogued {TotalDevices} devices across {TypeCount} types in {Duration:F2}s!",
                allDevices.Count, deviceTypes.Length, totalDuration.TotalSeconds);
            
            return new DeviceScanResult
            {
                Devices = allDevices,
                ScanTimestamp = DateTime.Now,
                ScanDuration = totalDuration
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error during comprehensive device scan - some hardware mysteries remain unsolved!");
            
            return new DeviceScanResult
            {
                Devices = allDevices,
                ScanTimestamp = DateTime.Now,
                ScanDuration = DateTime.Now - startTime
            };
        }
    }
    
    /// <summary>
    /// Get device details by identifier
    /// </summary>
    public async Task<DeviceInfo?> GetDeviceDetailsAsync(string deviceId, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        
        foreach (var deviceList in _deviceCache.Values)
        {
            var device = deviceList.FirstOrDefault(d => d.Id == deviceId);
            if (device != null)
            {
                _logger.LogDebug("🔍 Device details retrieved for {DeviceId}: {DeviceName}", deviceId, device.Name);
                return device;
            }
        }
        
        _logger.LogWarning("❓ Device {DeviceId} not found in cache - mysterious hardware!", deviceId);
        return null;
    }
    
    /// <summary>
    /// Check if device is currently available
    /// </summary>
    public async Task<bool> IsDeviceAvailableAsync(string deviceId, CancellationToken cancellationToken = default)
    {
        var device = await GetDeviceDetailsAsync(deviceId, cancellationToken);
        
        if (device == null) return false;
        
        // Perform real-time availability check based on device type
        var isAvailable = device.Type switch
        {
            DeviceType.Serial => await CheckSerialAvailabilityAsync(device.Id),
            DeviceType.USB => await CheckUsbAvailabilityAsync(device.Id),
            DeviceType.Bluetooth => await CheckBluetoothAvailabilityAsync(device.Id),
            DeviceType.Network => await CheckNetworkAvailabilityAsync(device.Id),
            _ => false
        };
        
        device.IsAvailable = isAvailable;
        
        _logger.LogTrace("📊 Device {DeviceId} availability check: {Status}",
            deviceId, isAvailable ? "Available" : "Unavailable");
        
        return isAvailable;
    }
    
    /// <summary>
    /// Start continuous device monitoring
    /// </summary>
    public async Task StartDeviceMonitoringAsync(DeviceType? deviceType = null, CancellationToken cancellationToken = default)
    {
        if (_isMonitoring)
        {
            _logger.LogWarning("⚠️ Device monitoring already active - vigilance continues!");
            return;
        }
        
        _isMonitoring = true;
        _monitoringType = deviceType;
        
        _logger.LogInformation("👁️ Starting device monitoring for {DeviceType} - vigilant watch begins!",
            deviceType?.ToString() ?? "All Types");
        
        try
        {
            // Set up WMI watchers for hardware changes
            await SetupWmiWatchersAsync();
            
            // Start periodic scanning for missed events
            _ = Task.Run(async () => await PeriodicMonitoringAsync(cancellationToken), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error starting device monitoring - hardware surveillance compromised!");
            _isMonitoring = false;
            throw;
        }
    }
    
    /// <summary>
    /// Stop device monitoring
    /// </summary>
    public async Task StopDeviceMonitoringAsync()
    {
        if (!_isMonitoring) return;
        
        _isMonitoring = false;
        
        try
        {
            // Stop WMI watchers
            _usbWatcher?.Stop();
            _usbWatcher?.Dispose();
            _usbWatcher = null;
            
            _serialWatcher?.Stop();
            _serialWatcher?.Dispose();
            _serialWatcher = null;
            
            _logger.LogInformation("🛑 Device monitoring stopped - vigilant watch concluded!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error stopping device monitoring - surveillance shutdown incomplete!");
        }
        
        await Task.CompletedTask;
    }
    
    /// <summary>
    /// Get cached device list
    /// </summary>
    public List<DeviceInfo> GetCachedDevices(DeviceType? deviceType = null)
    {
        if (deviceType.HasValue)
        {
            return _deviceCache.TryGetValue(deviceType.Value, out var devices) 
                ? new List<DeviceInfo>(devices) 
                : new List<DeviceInfo>();
        }
        
        var allDevices = new List<DeviceInfo>();
        foreach (var deviceList in _deviceCache.Values)
        {
            allDevices.AddRange(deviceList);
        }
        
        _logger.LogTrace("💾 Retrieved {DeviceCount} devices from cache for {DeviceType}",
            allDevices.Count, deviceType?.ToString() ?? "All Types");
        
        return allDevices;
    }
    
    /// <summary>
    /// Refresh device cache
    /// </summary>
    public async Task<DeviceScanResult> RefreshDeviceCacheAsync(DeviceType? deviceType = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("🔄 Refreshing device cache for {DeviceType} - reconnaissance renewal!",
            deviceType?.ToString() ?? "All Types");
        
        return deviceType.HasValue
            ? await ScanDevicesAsync(deviceType.Value, cancellationToken)
            : await ScanAllDevicesAsync(cancellationToken);
    }
    
    /// <summary>
    /// Test device connectivity
    /// </summary>
    public async Task<(bool Success, string Message)> TestDeviceConnectivityAsync(DeviceInfo deviceInfo, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = deviceInfo.Type switch
            {
                DeviceType.Serial => await TestSerialConnectivityAsync(deviceInfo),
                DeviceType.USB => await TestUsbConnectivityAsync(deviceInfo),
                DeviceType.Bluetooth => await TestBluetoothConnectivityAsync(deviceInfo),
                DeviceType.Network => await TestNetworkConnectivityAsync(deviceInfo),
                _ => (false, "Unsupported device type")
            };
            
            var status = result.Item1 ? "✅" : "❌";
            _logger.LogInformation("{Status} Connectivity test for {DeviceName}: {Message}",
                status, deviceInfo.Name, result.Item2);
            
            return result;
        }
        catch (Exception ex)
        {
            var message = $"Connectivity test failed: {ex.Message}";
            _logger.LogError(ex, "💥 Error testing connectivity for device {DeviceName}", deviceInfo.Name);
            return (false, message);
        }
    }
    
    /// <summary>
    /// Get device capabilities
    /// </summary>
    public async Task<Dictionary<string, object>> GetDeviceCapabilitiesAsync(string deviceId, CancellationToken cancellationToken = default)
    {
        var device = await GetDeviceDetailsAsync(deviceId, cancellationToken);
        
        if (device == null)
        {
            return new Dictionary<string, object>
            {
                ["error"] = "Device not found"
            };
        }
        
        var capabilities = new Dictionary<string, object>
        {
            ["device_type"] = device.Type.ToString(),
            ["device_name"] = device.Name,
            ["is_available"] = device.IsAvailable,
            ["supports_bidirectional"] = true,
            ["supports_raw_data"] = true
        };
        
        // Add type-specific capabilities
        switch (device.Type)
        {
            case DeviceType.Serial:
                capabilities["supports_flow_control"] = true;
                capabilities["supports_break_signal"] = true;
                capabilities["configurable_baud_rates"] = new[] { 9600, 19200, 38400, 57600, 115200 };
                break;
                
            case DeviceType.USB:
                capabilities["supports_bulk_transfer"] = true;
                capabilities["supports_interrupt_transfer"] = true;
                break;
                
            case DeviceType.Bluetooth:
                capabilities["supports_pairing"] = true;
                capabilities["supports_rfcomm"] = true;
                break;
                
            case DeviceType.Network:
                capabilities["supports_tcp"] = true;
                capabilities["supports_encryption"] = true;
                break;
        }
        
        // Merge with device-specific properties
        foreach (var prop in device.Properties)
        {
            capabilities[$"device_{prop.Key}"] = prop.Value;
        }
        
        _logger.LogTrace("📋 Retrieved capabilities for device {DeviceId}: {CapabilityCount} capabilities",
            deviceId, capabilities.Count);
        
        return capabilities;
    }
    
    #endregion
    
    #region Device Type Scanning Implementations
    
    /// <summary>
    /// Scan for serial devices
    /// </summary>
    private async Task<List<DeviceInfo>> ScanSerialDevicesAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        
        var devices = new List<DeviceInfo>();
        
        try
        {
            var portNames = SerialPort.GetPortNames();
            
            _logger.LogDebug("🔌 Found {PortCount} serial ports: {Ports}",
                portNames.Length, string.Join(", ", portNames));
            
            foreach (var portName in portNames)
            {
                if (cancellationToken.IsCancellationRequested) break;
                
                var device = new DeviceInfo
                {
                    Id = portName,
                    Name = $"Serial Port {portName}",
                    Type = DeviceType.Serial,
                    Description = await GetSerialPortDescriptionAsync(portName),
                    IsAvailable = await CheckSerialAvailabilityAsync(portName)
                };
                
                // Add serial-specific properties
                device.Properties["port_name"] = portName;
                device.Properties["supports_dtr_rts"] = true;
                device.Properties["max_baud_rate"] = 115200;
                
                devices.Add(device);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error scanning serial devices - hardware enumeration failed!");
        }
        
        return devices;
    }
    
    /// <summary>
    /// Scan for USB devices
    /// </summary>
    private async Task<List<DeviceInfo>> ScanUsbDevicesAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        
        var devices = new List<DeviceInfo>();
        
        try
        {
            // Query USB devices using WMI
            var query = "SELECT * FROM Win32_PnPEntity WHERE ClassGuid='{36fc9e60-c465-11cf-8056-444553540000}'"; // USB devices
            
            using var searcher = new ManagementObjectSearcher(query);
            using var collection = searcher.Get();
            
            foreach (ManagementObject device in collection)
            {
                if (cancellationToken.IsCancellationRequested) break;
                
                try
                {
                    var deviceId = device["DeviceID"]?.ToString() ?? "";
                    var name = device["Name"]?.ToString() ?? "Unknown USB Device";
                    var description = device["Description"]?.ToString() ?? "";
                    var status = device["Status"]?.ToString() ?? "";
                    
                    var deviceInfo = new DeviceInfo
                    {
                        Id = deviceId,
                        Name = name,
                        Type = DeviceType.USB,
                        Description = description,
                        IsAvailable = status == "OK"
                    };
                    
                    // Add USB-specific properties
                    deviceInfo.Properties["device_id"] = deviceId;
                    deviceInfo.Properties["status"] = status;
                    deviceInfo.Properties["pnp_class"] = device["PNPClass"]?.ToString() ?? "";
                    deviceInfo.Properties["manufacturer"] = device["Manufacturer"]?.ToString() ?? "";
                    
                    devices.Add(deviceInfo);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "⚠️ Error processing USB device - skipping problematic hardware!");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error scanning USB devices - WMI query failed!");
        }
        
        return devices;
    }
    
    /// <summary>
    /// Scan for Bluetooth devices
    /// </summary>
    private async Task<List<DeviceInfo>> ScanBluetoothDevicesAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        
        var devices = new List<DeviceInfo>();
        
        _logger.LogWarning("📡 Bluetooth device scanning - Full implementation pending for Windows.Devices.Bluetooth integration!");
        
        // Placeholder implementation - would use Windows.Devices.Bluetooth APIs
        // This requires UWP/WinRT integration which needs additional setup
        
        return devices;
    }
    
    /// <summary>
    /// Scan for network devices (placeholder)
    /// </summary>
    private async Task<List<DeviceInfo>> ScanNetworkDevicesAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        
        var devices = new List<DeviceInfo>();
        
        _logger.LogDebug("🌐 Network device scanning - Integration with Scanner module recommended!");
        
        // This would integrate with the Scanner/Topography module for network device discovery
        
        return devices;
    }
    
    #endregion
    
    #region Availability Checking
    
    private async Task<bool> CheckSerialAvailabilityAsync(string portName)
    {
        await Task.CompletedTask;
        
        try
        {
            using var port = new SerialPort(portName);
            port.Open();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    private async Task<bool> CheckUsbAvailabilityAsync(string deviceId)
    {
        await Task.CompletedTask;
        
        try
        {
            var query = $"SELECT * FROM Win32_PnPEntity WHERE DeviceID='{deviceId}'";
            using var searcher = new ManagementObjectSearcher(query);
            using var collection = searcher.Get();
            
            foreach (ManagementObject device in collection)
            {
                var status = device["Status"]?.ToString();
                return status == "OK";
            }
            
            return false;
        }
        catch
        {
            return false;
        }
    }
    
    private async Task<bool> CheckBluetoothAvailabilityAsync(string deviceAddress)
    {
        await Task.CompletedTask;
        // Placeholder - would check Bluetooth device status
        return false;
    }
    
    private async Task<bool> CheckNetworkAvailabilityAsync(string networkId)
    {
        await Task.CompletedTask;
        // Placeholder - would ping or check network connectivity
        return false;
    }
    
    #endregion
    
    #region Connectivity Testing
    
    private async Task<(bool Success, string Message)> TestSerialConnectivityAsync(DeviceInfo deviceInfo)
    {
        try
        {
            using var port = new SerialPort(deviceInfo.Id, 9600);
            await Task.Run(() =>
            {
                port.Open();
                port.Close();
            });
            
            return (true, "Serial port accessible and responding");
        }
        catch (Exception ex)
        {
            return (false, $"Serial connection failed: {ex.Message}");
        }
    }
    
    private async Task<(bool Success, string Message)> TestUsbConnectivityAsync(DeviceInfo deviceInfo)
    {
        await Task.CompletedTask;
        
        var isAvailable = await CheckUsbAvailabilityAsync(deviceInfo.Id);
        return isAvailable
            ? (true, "USB device detected and operational")
            : (false, "USB device not responding or unavailable");
    }
    
    private async Task<(bool Success, string Message)> TestBluetoothConnectivityAsync(DeviceInfo deviceInfo)
    {
        await Task.CompletedTask;
        return (false, "Bluetooth connectivity testing not yet implemented");
    }
    
    private async Task<(bool Success, string Message)> TestNetworkConnectivityAsync(DeviceInfo deviceInfo)
    {
        await Task.CompletedTask;
        return (false, "Network connectivity testing not yet implemented");
    }
    
    #endregion
    
    #region Monitoring and Events
    
    private async Task SetupWmiWatchersAsync()
    {
        try
        {
            // Set up USB device watcher
            var usbQuery = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 2 OR EventType = 3");
            _usbWatcher = new ManagementEventWatcher(usbQuery);
            _usbWatcher.EventArrived += async (sender, e) =>
            {
                _logger.LogDebug("🔗 USB device change detected - refreshing device cache!");
                await RefreshDeviceCacheAsync(DeviceType.USB);
            };
            _usbWatcher.Start();
            
            _logger.LogDebug("👁️ WMI watchers activated - hardware surveillance online!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error setting up WMI watchers - hardware monitoring compromised!");
        }
    }
    
    private async Task PeriodicMonitoringAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("⏰ Starting periodic device monitoring - regular reconnaissance sweeps!");
        
        while (_isMonitoring && !cancellationToken.IsCancellationRequested)
        {
            try
            {
                // Refresh cache periodically
                if (_monitoringType.HasValue)
                {
                    await RefreshDeviceCacheAsync(_monitoringType.Value, cancellationToken);
                }
                else
                {
                    await RefreshDeviceCacheAsync(null, cancellationToken);
                }
                
                // Wait before next scan
                await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Error during periodic monitoring - continuing surveillance!");
            }
        }
        
        _logger.LogDebug("🛑 Periodic monitoring stopped");
    }
    
    #endregion
    
    #region Helper Methods
    
    private async Task<string> GetSerialPortDescriptionAsync(string portName)
    {
        await Task.CompletedTask;
        
        try
        {
            var query = $"SELECT * FROM Win32_SerialPort WHERE DeviceID LIKE '%{portName}%'";
            using var searcher = new ManagementObjectSearcher(query);
            using var collection = searcher.Get();
            
            foreach (ManagementObject port in collection)
            {
                var description = port["Description"]?.ToString();
                if (!string.IsNullOrEmpty(description))
                    return description;
            }
        }
        catch (Exception ex)
        {
            _logger.LogTrace(ex, "Unable to get description for port {PortName}", portName);
        }
        
        return $"Serial communication port ({portName})";
    }
    
    #endregion
    
    #region Events
    
    public event EventHandler<DeviceDetectedEvent>? DeviceDetected;
    public event EventHandler<DeviceDetectedEvent>? DeviceLost;
    
    #endregion
    
    #region IDisposable Implementation
    
    public void Dispose()
    {
        if (_disposed) return;
        
        _disposed = true;
        
        // Stop monitoring
        _ = Task.Run(async () => await StopDeviceMonitoringAsync());
        
        // Dispose resources
        _scanSemaphore?.Dispose();
        
        _logger.LogInformation("🧹 Device Scanner disposed - hardware surveillance concluded!");
    }
    
    #endregion
}