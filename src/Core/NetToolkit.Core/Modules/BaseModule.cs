using NetToolkit.Core.Interfaces;

namespace NetToolkit.Core.Modules;

public abstract class BaseModule : IModule
{
    protected IServiceProvider ServiceProvider { get; private set; } = null!;
    protected ILoggerWrapper Logger { get; private set; } = null!;
    protected IEventBus EventBus { get; private set; } = null!;

    public abstract string Name { get; }
    public abstract string Version { get; }

    public virtual async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        Logger = serviceProvider.GetService(typeof(ILoggerWrapper)) as ILoggerWrapper 
            ?? throw new InvalidOperationException("ILoggerWrapper not registered");
        EventBus = serviceProvider.GetService(typeof(IEventBus)) as IEventBus 
            ?? throw new InvalidOperationException("IEventBus not registered");

        Logger.LogInfo("Initializing module {ModuleName} v{Version}", Name, Version);
        
        await OnInitializeAsync();
        
        Logger.LogInfo("Module {ModuleName} initialized successfully", Name);
    }

    public virtual async Task DisposeAsync()
    {
        Logger?.LogInfo("Disposing module {ModuleName}", Name);
        await OnDisposeAsync();
    }

    protected abstract Task OnInitializeAsync();
    protected abstract Task OnDisposeAsync();
}