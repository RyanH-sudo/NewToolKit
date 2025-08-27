using NetToolkit.Modules.SshTerminal.Models;

namespace NetToolkit.Modules.SshTerminal.Interfaces;

/// <summary>
/// Device detector interface - the omniscient sensor of hardware companions
/// Where the physical world reveals its digital secrets through mystical scanning
/// </summary>
public interface IDeviceDetector
{
    /// <summary>
    /// Scan for available devices of specified type - digital reconnaissance of hardware realm
    /// </summary>
    /// <param name="deviceType">Type of devices to discover</param>
    /// <param name="cancellationToken">Cancellation token for graceful termination</param>
    /// <returns>Comprehensive scan results with discovered devices</returns>
    Task<DeviceScanResult> ScanDevicesAsync(DeviceType deviceType, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Scan all device types - omnipotent hardware census across all realms
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Complete scan results for all device types</returns>
    Task<DeviceScanResult> ScanAllDevicesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get device details by identifier - deep interrogation of specific hardware
    /// </summary>
    /// <param name="deviceId">Device identifier to investigate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Detailed device information or null if not found</returns>
    Task<DeviceInfo?> GetDeviceDetailsAsync(string deviceId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Check if device is currently available - real-time availability oracle
    /// </summary>
    /// <param name="deviceId">Device identifier to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if device is ready for connection</returns>
    Task<bool> IsDeviceAvailableAsync(string deviceId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Start continuous device monitoring - vigilant watching of hardware changes
    /// </summary>
    /// <param name="deviceType">Type of devices to monitor (null for all types)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the monitoring operation</returns>
    Task StartDeviceMonitoringAsync(DeviceType? deviceType = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Stop device monitoring - ending the vigilant watch
    /// </summary>
    /// <returns>Task representing the stop operation</returns>
    Task StopDeviceMonitoringAsync();
    
    /// <summary>
    /// Get cached device list - swift retrieval from memory palace
    /// </summary>
    /// <param name="deviceType">Type of devices to retrieve (null for all)</param>
    /// <returns>List of cached devices</returns>
    List<DeviceInfo> GetCachedDevices(DeviceType? deviceType = null);
    
    /// <summary>
    /// Refresh device cache - forcing new reconnaissance mission
    /// </summary>
    /// <param name="deviceType">Type of devices to refresh (null for all)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated scan results</returns>
    Task<DeviceScanResult> RefreshDeviceCacheAsync(DeviceType? deviceType = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Test device connectivity - preliminary connection probe
    /// </summary>
    /// <param name="deviceInfo">Device to test</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Connection test results with success status and details</returns>
    Task<(bool Success, string Message)> TestDeviceConnectivityAsync(DeviceInfo deviceInfo, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get device capabilities - discovering the full potential of hardware
    /// </summary>
    /// <param name="deviceId">Device identifier to analyze</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary of device capabilities and supported features</returns>
    Task<Dictionary<string, object>> GetDeviceCapabilitiesAsync(string deviceId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Event fired when a new device is detected
    /// </summary>
    event EventHandler<DeviceDetectedEvent>? DeviceDetected;
    
    /// <summary>
    /// Event fired when a device becomes unavailable
    /// </summary>
    event EventHandler<DeviceDetectedEvent>? DeviceLost;
}