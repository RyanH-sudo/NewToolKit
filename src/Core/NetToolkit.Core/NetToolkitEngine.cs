using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using NetToolkit.Core.Interfaces;
using NetToolkit.Core.Services;
using NetToolkit.Core.Data;
using NLog.Extensions.Logging;

namespace NetToolkit.Core;

public class NetToolkitEngine
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHost _host;
    private readonly List<IModule> _modules = new();

    public NetToolkitEngine()
    {
        _host = CreateHostBuilder().Build();
        _serviceProvider = _host.Services;
    }

    private static IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                      .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", 
                                   optional: true, reloadOnChange: true);
            })
            .ConfigureLogging((context, logging) =>
            {
                logging.ClearProviders();
                logging.AddNLog();
                logging.SetMinimumLevel(LogLevel.Debug);
            })
            .ConfigureServices((context, services) =>
            {
                // Core services
                services.AddSingleton<IEventBus, EventBusService>();
                services.AddSingleton<ILoggerWrapper, LoggerWrapperService>();
                services.AddSingleton<INetToolkitConfiguration, ConfigurationProviderService>();
                services.AddTransient<NetworkInfoService>();

                // Database context
                var connectionString = context.Configuration.GetConnectionString("DefaultConnection") 
                    ?? "Data Source=nettoolkit.db";
                
                services.AddDbContext<NetToolkitContext>(options =>
                {
                    options.UseSqlite(connectionString);
                    options.EnableSensitiveDataLogging(context.HostingEnvironment.IsDevelopment());
                });

                // MediatR for CQRS
                services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(NetToolkitEngine).Assembly));
            });
    }

    public async Task StartAsync()
    {
        await _host.StartAsync();
        
        // Initialize database
        await InitializeDatabaseAsync();
        
        // Initialize modules
        await InitializeModulesAsync();
        
        var logger = _serviceProvider.GetRequiredService<ILoggerWrapper>();
        logger.LogInfo("NetToolkit Engine started successfully");
    }

    public async Task StopAsync()
    {
        // Dispose modules
        foreach (var module in _modules)
        {
            try
            {
                await module.DisposeAsync();
            }
            catch (Exception ex)
            {
                var logger = _serviceProvider.GetRequiredService<ILoggerWrapper>();
                logger.LogError(ex, "Error disposing module {ModuleName}", module.Name);
            }
        }

        await _host.StopAsync();
        _host.Dispose();
    }

    public async Task RegisterModuleAsync(IModule module)
    {
        try
        {
            await module.InitializeAsync(_serviceProvider);
            _modules.Add(module);
            
            var logger = _serviceProvider.GetRequiredService<ILoggerWrapper>();
            logger.LogInfo("Module {ModuleName} v{Version} registered successfully", 
                          module.Name, module.Version);
        }
        catch (Exception ex)
        {
            var logger = _serviceProvider.GetRequiredService<ILoggerWrapper>();
            logger.LogError(ex, "Failed to register module {ModuleName}", module.Name);
            throw;
        }
    }

    public T GetService<T>() where T : notnull
    {
        return _serviceProvider.GetRequiredService<T>();
    }

    private async Task InitializeDatabaseAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<NetToolkitContext>();
            await context.Database.EnsureCreatedAsync();
            
            var logger = _serviceProvider.GetRequiredService<ILoggerWrapper>();
            logger.LogInfo("Database initialized successfully");
        }
        catch (Exception ex)
        {
            var logger = _serviceProvider.GetRequiredService<ILoggerWrapper>();
            logger.LogError(ex, "Failed to initialize database");
            throw;
        }
    }

    private async Task InitializeModulesAsync()
    {
        // This will be called when modules are loaded dynamically
        await Task.CompletedTask;
    }
}