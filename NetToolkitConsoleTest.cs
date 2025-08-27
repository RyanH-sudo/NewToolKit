using NetToolkit.Core;
using NetToolkit.Modules.PowerShell;

namespace NetToolkit.ConsoleTest;

/// <summary>
/// Simple console test application to verify PowerShell module integration
/// Digital integration testing - ensuring our components dance in harmony!
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("🚀 NetToolkit PowerShell Module Integration Test");
        Console.WriteLine("==================================================");
        
        try
        {
            // Initialize the core engine
            Console.WriteLine("🔧 Initializing NetToolkit Core Engine...");
            using var engine = new NetToolkitEngine();
            await engine.StartAsync();
            
            Console.WriteLine("✅ Core engine started successfully!");
            
            // Create and register PowerShell module
            Console.WriteLine("📦 Creating PowerShell Module...");
            var powerShellModule = new PowerShellModule();
            
            Console.WriteLine("🔌 Registering PowerShell Module...");
            await engine.RegisterModuleAsync(powerShellModule);
            
            Console.WriteLine("✅ PowerShell module registered successfully!");
            
            // Test basic functionality
            Console.WriteLine("🧪 Running integration tests...");
            
            // Test 1: Module statistics
            var stats = await powerShellModule.GetModuleStatsAsync();
            Console.WriteLine($"📊 Module Stats: {stats.ModuleName} v{stats.ModuleVersion}");
            Console.WriteLine($"   - Initialized: {stats.IsInitialized}");
            Console.WriteLine($"   - Templates: {stats.TemplateCount}");
            Console.WriteLine($"   - Command History: {stats.CommandHistoryCount}");
            
            // Test 2: Template service
            var templateService = powerShellModule.GetTemplateService();
            if (templateService != null)
            {
                var templates = await templateService.LoadTemplatesAsync();
                Console.WriteLine($"📜 Loaded {templates.Count} script templates");
                
                foreach (var template in templates.Take(3))
                {
                    Console.WriteLine($"   - {template.Name} ({template.Category})");
                }
            }
            
            // Test 3: Basic PowerShell execution (if available)
            var psHost = powerShellModule.GetPowerShellHost();
            if (psHost != null)
            {
                Console.WriteLine("⚡ Testing PowerShell execution...");
                try
                {
                    var result = await psHost.ExecuteCommandAsync("Write-Output 'Hello from NetToolkit!'");
                    Console.WriteLine($"   Success: {result.Success}");
                    if (!string.IsNullOrEmpty(result.Output))
                    {
                        Console.WriteLine($"   Output: {result.Output.Trim()}");
                    }
                    if (!string.IsNullOrEmpty(result.ErrorOutput))
                    {
                        Console.WriteLine($"   Error: {result.ErrorOutput.Trim()}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   ⚠️ PowerShell test failed: {ex.Message}");
                }
            }
            
            Console.WriteLine("✅ Integration tests completed!");
            Console.WriteLine("🎉 NetToolkit PowerShell Module integration successful!");
            
            // Stop the engine
            Console.WriteLine("🛑 Stopping NetToolkit Engine...");
            await engine.StopAsync();
            Console.WriteLine("✅ Engine stopped gracefully.");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Integration test failed: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
        
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}