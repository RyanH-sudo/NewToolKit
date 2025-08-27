using Microsoft.Extensions.Configuration;
using NetToolkit.Core.Interfaces;
using System.Text.Json;

namespace NetToolkit.Core.Services;

public class ConfigurationProviderService : INetToolkitConfiguration
{
    private readonly IConfiguration _configuration;
    private readonly Dictionary<string, object> _cachedValues = new();
    private readonly string _configFilePath;

    public ConfigurationProviderService(IConfiguration configuration)
    {
        _configuration = configuration;
        _configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
    }

    public T GetValue<T>(string key, T defaultValue = default!)
    {
        if (_cachedValues.TryGetValue(key, out var cachedValue) && cachedValue is T)
        {
            return (T)cachedValue;
        }

        var value = _configuration.GetValue<T>(key, defaultValue);
        _cachedValues[key] = value!;
        return value;
    }

    public string GetConnectionString(string name)
    {
        return _configuration.GetConnectionString(name) ?? string.Empty;
    }

    public void SetValue<T>(string key, T value)
    {
        _cachedValues[key] = value!;
    }

    public async Task SaveAsync()
    {
        try
        {
            var json = JsonSerializer.Serialize(_cachedValues, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            
            await File.WriteAllTextAsync(_configFilePath, json);
        }
        catch (Exception)
        {
            // Log error but don't throw - configuration saving is non-critical
        }
    }
}