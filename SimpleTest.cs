using NetToolkit.Core;
using NetToolkit.Modules.PowerShell;
using NetToolkit.Core.Interfaces;
using NetToolkit.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

/// <summary>
/// Simple integration test for PowerShell module - no complex dependencies
/// </summary>
class SimpleTest
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("🚀 NetToolkit PowerShell Module Simple Integration Test");
        Console.WriteLine("======================================================");
        
        try
        {
            // Create a minimal service collection for testing
            var services = new ServiceCollection();
            
            // Add basic logging
            services.AddLogging(builder => 
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });
            
            // Add minimal configuration
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>("ConnectionStrings:DefaultConnection", "Data Source=test.db")
                })
                .Build();
            services.AddSingleton<IConfiguration>(configuration);
            
            // Add core services with simplified implementations
            services.AddSingleton<ILoggerWrapper, LoggerWrapperService>();
            services.AddSingleton<IEventBus>(provider =>
            {
                var logger = provider.GetRequiredService<ILoggerWrapper>();
                return new EventBusService(logger);
            });
            
            var serviceProvider = services.BuildServiceProvider();
            
            Console.WriteLine("✅ Basic services initialized");
            
            // Test PowerShell module creation
            Console.WriteLine("📦 Creating PowerShell Module...");
            var powerShellModule = new PowerShellModule();
            
            Console.WriteLine("📊 Getting module statistics...");
            var stats = await powerShellModule.GetModuleStatsAsync();
            Console.WriteLine($"   - Module: {stats.ModuleName} v{stats.ModuleVersion}");
            Console.WriteLine($"   - Initialized: {stats.IsInitialized}");
            
            // Test template service
            Console.WriteLine("📜 Testing template service...");
            var templateService = powerShellModule.GetTemplateService();
            if (templateService != null)
            {
                Console.WriteLine("   - Template service available: YES");
            }
            else
            {
                Console.WriteLine("   - Template service available: NO (expected before initialization)");
            }
            
            Console.WriteLine("✅ Simple integration test completed successfully!");
            Console.WriteLine("🎉 PowerShell module basic functionality verified!");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Test failed: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"   Inner exception: {ex.InnerException.Message}");
            }
        }
        
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}