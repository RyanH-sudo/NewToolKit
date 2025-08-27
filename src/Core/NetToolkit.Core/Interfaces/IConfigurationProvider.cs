namespace NetToolkit.Core.Interfaces;

public interface INetToolkitConfiguration
{
    T GetValue<T>(string key, T defaultValue = default!);
    string GetConnectionString(string name);
    void SetValue<T>(string key, T value);
    Task SaveAsync();
}