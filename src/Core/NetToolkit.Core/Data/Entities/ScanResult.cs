namespace NetToolkit.Core.Data.Entities;

public class ScanResult
{
    public int Id { get; set; }
    public string NetworkRange { get; set; } = string.Empty;
    public string ResultsJson { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string ScanType { get; set; } = string.Empty;
    public int DeviceCount { get; set; }
}