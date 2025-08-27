namespace NetToolkit.Core.Interfaces;

public interface IModule
{
    string Name { get; }
    string Version { get; }
    Task InitializeAsync(IServiceProvider serviceProvider);
    Task DisposeAsync();
}