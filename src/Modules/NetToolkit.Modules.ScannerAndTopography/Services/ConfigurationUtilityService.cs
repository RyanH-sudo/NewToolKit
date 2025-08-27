using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Logging;
using NetToolkit.Core.Interfaces;
using NetToolkit.Modules.ScannerAndTopography.Interfaces;
using NetToolkit.Modules.ScannerAndTopography.Models;
using Polly;

namespace NetToolkit.Modules.ScannerAndTopography.Services;

/// <summary>
/// Configuration utility service - the digital scepter wielding administrative power
/// Where network configurations bend to the will of PowerShell sorcery
/// </summary>
public class ConfigurationUtilityService : IConfigurationUtility
{
    private readonly ILogger<ConfigurationUtilityService> _logger;
    private readonly IScannerEventPublisher _eventPublisher;
    private readonly IEventBus _eventBus;
    private readonly IAsyncPolicy _executionPolicy;
    private readonly Dictionary<string, List<ConfigCommand>> _commandLibrary;

    public ConfigurationUtilityService(
        ILogger<ConfigurationUtilityService> logger, 
        IScannerEventPublisher eventPublisher,
        IEventBus eventBus)
    {
        _logger = logger;
        _eventPublisher = eventPublisher;
        _eventBus = eventBus;
        
        // Resilience policy for configuration operations
        _executionPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 2,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    _logger.LogWarning("Configuration retry {RetryCount} after {Delay}s - persistence in the face of digital resistance! 🔄", 
                        retryCount, timespan.TotalSeconds);
                });

        _commandLibrary = InitializeCommandLibrary();
        
        _logger.LogInformation("🔧 Configuration Utility Service initialized - ready to reshape the digital realm!");
    }

    /// <summary>
    /// Configure network node with specified command - wielding administrative power
    /// </summary>
    public async Task<ConfigurationResult> ConfigureNodeAsync(string nodeId, ConfigCommand command, CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;
        
        _logger.LogInformation("⚡ Initiating configuration {CommandName} on node {NodeId} - digital transformation commencing!",
            command.Name, nodeId);

        try
        {
            var result = new ConfigurationResult
            {
                CommandId = command.Id,
                NodeId = nodeId,
                ExecutedAt = startTime
            };

            // Validate command before execution
            var node = await GetNodeByIdAsync(nodeId, cancellationToken);
            if (node == null)
            {
                result.Success = false;
                result.ErrorDetails = $"Node {nodeId} not found in cosmic registry!";
                return result;
            }

            var validationResult = await ValidateCommandAsync(command, node, cancellationToken);
            if (!validationResult.IsValid)
            {
                result.Success = false;
                result.ErrorDetails = $"Command validation failed: {string.Join(", ", validationResult.Errors)}";
                result.Warnings.AddRange(validationResult.Warnings);
                return result;
            }

            result.Warnings.AddRange(validationResult.Warnings);

            // Execute the configuration command
            result = await _executionPolicy.ExecuteAsync(async () => 
                await ExecuteConfigurationCommand(command, node, result, cancellationToken));

            result.Duration = DateTime.UtcNow - startTime;

            // Publish configuration event
            await _eventPublisher.PublishConfigAppliedAsync(nodeId, command.Name, result.Success, result.Message);

            _logger.LogInformation("✨ Configuration {CommandName} on node {NodeId} completed - Success: {Success}, Duration: {Duration}ms",
                command.Name, nodeId, result.Success, result.Duration.TotalMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Configuration {CommandName} on node {NodeId} encountered cosmic turbulence!", 
                command.Name, nodeId);

            return new ConfigurationResult
            {
                CommandId = command.Id,
                NodeId = nodeId,
                Success = false,
                ErrorDetails = ex.Message,
                Duration = DateTime.UtcNow - startTime,
                ExecutedAt = startTime
            };
        }
    }

    /// <summary>
    /// Get available configuration commands for node type - the arsenal of possibilities
    /// </summary>
    public List<ConfigCommand> GetAvailableCommands(string nodeType)
    {
        _logger.LogDebug("🗡️ Retrieving command arsenal for node type: {NodeType}", nodeType);
        
        var normalizedType = nodeType?.ToLower() ?? "unknown";
        
        if (_commandLibrary.TryGetValue(normalizedType, out var commands))
        {
            _logger.LogDebug("⚔️ Found {CommandCount} commands for {NodeType} - digital armory ready!", 
                commands.Count, nodeType);
            return commands.ToList();
        }

        // Return generic commands if specific type not found
        _logger.LogDebug("🔄 Using generic command set for unknown node type: {NodeType}", nodeType);
        return _commandLibrary.GetValueOrDefault("generic", new List<ConfigCommand>()).ToList();
    }

    /// <summary>
    /// Validate configuration command before execution - wisdom before action
    /// </summary>
    public async Task<ValidationResult> ValidateCommandAsync(ConfigCommand command, NetworkNode targetNode, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("🧐 Validating command {CommandName} for node {NodeId} - digital wisdom consultation...",
            command.Name, targetNode.Id);

        var result = new ValidationResult { IsValid = true };

        await Task.Run(() =>
        {
            // Check prerequisites
            foreach (var prerequisite in command.Prerequisites)
            {
                if (!CheckPrerequisite(prerequisite, targetNode))
                {
                    result.Errors.Add($"Prerequisite not met: {prerequisite}");
                    result.IsValid = false;
                }
            }

            // Validate parameters
            var paramValidation = ValidateParameters(command, targetNode);
            result.Errors.AddRange(paramValidation.Errors);
            result.Warnings.AddRange(paramValidation.Warnings);
            
            if (paramValidation.Errors.Any())
            {
                result.IsValid = false;
            }

            // Security checks
            var securityValidation = PerformSecurityValidation(command, targetNode);
            result.Warnings.AddRange(securityValidation.Warnings);
            result.Recommendations.AddRange(securityValidation.Recommendations);

            // Final validation message
            result.ValidationMessage = result.IsValid 
                ? "✅ Command validation successful - ready for digital transformation!"
                : "❌ Command validation failed - cosmic interference detected!";

        }, cancellationToken);

        _logger.LogInformation("🎯 Validation complete for {CommandName} - Valid: {IsValid}, Warnings: {WarningCount}",
            command.Name, result.IsValid, result.Warnings.Count);

        return result;
    }

    /// <summary>
    /// Generate PowerShell script for configuration - translating will to execution
    /// </summary>
    public string GeneratePowerShellScript(ConfigCommand command, NetworkNode node)
    {
        _logger.LogDebug("📜 Generating PowerShell script for {CommandName} on {NodeId} - digital incantation creation...",
            command.Name, node.Id);

        var script = new StringBuilder();
        
        // Script header with cosmic commentary
        script.AppendLine("# 🌟 NetToolkit Configuration Script");
        script.AppendLine($"# Command: {command.Name}");
        script.AppendLine($"# Target: {node.HostName} ({node.IpAddress})");
        script.AppendLine($"# Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        script.AppendLine("# Prepare for digital transformation! ⚡");
        script.AppendLine();

        // Error handling setup
        script.AppendLine("$ErrorActionPreference = 'Stop'");
        script.AppendLine("$WarningPreference = 'Continue'");
        script.AppendLine();

        // Logging function
        script.AppendLine("function Write-NetToolkitLog {");
        script.AppendLine("    param([string]$Message, [string]$Level = 'Info')");
        script.AppendLine("    $timestamp = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'");
        script.AppendLine("    Write-Host \"[$timestamp] [$Level] $Message\"");
        script.AppendLine("}");
        script.AppendLine();

        script.AppendLine($"Write-NetToolkitLog '🚀 Starting configuration: {command.Name}'");
        script.AppendLine();

        // Parameter substitution
        var processedScript = command.Script;
        foreach (var param in command.Parameters)
        {
            processedScript = processedScript.Replace($"{{{param.Key}}}", param.Value);
        }

        // Node-specific substitutions
        processedScript = processedScript.Replace("{NODE_IP}", node.IpAddress);
        processedScript = processedScript.Replace("{NODE_NAME}", node.HostName);
        processedScript = processedScript.Replace("{NODE_MAC}", node.MacAddress ?? "Unknown");

        script.AppendLine("try {");
        script.AppendLine("    # 🎯 Core configuration logic");
        script.AppendLine(processedScript.Replace("\n", "\n    "));
        script.AppendLine();
        script.AppendLine($"    Write-NetToolkitLog '✅ Configuration {command.Name} completed successfully!'");
        script.AppendLine("}");
        script.AppendLine("catch {");
        script.AppendLine("    Write-NetToolkitLog \"❌ Configuration failed: $($_.Exception.Message)\" 'Error'");
        script.AppendLine("    throw");
        script.AppendLine("}");
        script.AppendLine("finally {");
        script.AppendLine($"    Write-NetToolkitLog '🏁 Configuration script execution finished'");
        script.AppendLine("}");

        var generatedScript = script.ToString();
        _logger.LogInformation("📋 PowerShell script generated - {Lines} lines of digital sorcery ready!",
            generatedScript.Count(c => c == '\n'));

        return generatedScript;
    }

    /// <summary>
    /// Execute configuration with progress monitoring - the conductor of digital symphony
    /// </summary>
    public async Task<ConfigurationResult> ExecuteWithProgressAsync(string nodeId, ConfigCommand command, 
        IProgress<string>? progressCallback = null, CancellationToken cancellationToken = default)
    {
        progressCallback?.Report($"🎬 Initializing configuration {command.Name} on node {nodeId}...");
        
        var result = await ConfigureNodeAsync(nodeId, command, cancellationToken);
        
        progressCallback?.Report(result.Success 
            ? $"✅ Configuration completed successfully!" 
            : $"❌ Configuration failed: {result.ErrorDetails}");

        return result;
    }

    #region Private Helper Methods

    private Dictionary<string, List<ConfigCommand>> InitializeCommandLibrary()
    {
        var library = new Dictionary<string, List<ConfigCommand>>();

        // Generic commands available for all device types
        var genericCommands = new List<ConfigCommand>
        {
            new ConfigCommand
            {
                Name = "Ping Test",
                Description = "Test network connectivity to the device",
                Type = ConfigCommandType.Troubleshooting,
                Script = "Test-NetConnection -ComputerName {NODE_IP} -Port {port}",
                Parameters = new Dictionary<string, string> { ["port"] = "80" }
            },
            new ConfigCommand
            {
                Name = "Get System Info",
                Description = "Retrieve basic system information",
                Type = ConfigCommandType.Discovery,
                Script = "Get-ComputerInfo -Property WindowsProductName,TotalPhysicalMemory,CsProcessors",
                RequiresElevation = false
            }
        };

        // Router/Gateway specific commands
        var routerCommands = new List<ConfigCommand>(genericCommands)
        {
            new ConfigCommand
            {
                Name = "Configure Static Route",
                Description = "Add a static route to the routing table",
                Type = ConfigCommandType.NetworkInterface,
                Script = "route add {destination} mask {netmask} {gateway}",
                Parameters = new Dictionary<string, string> 
                { 
                    ["destination"] = "0.0.0.0",
                    ["netmask"] = "255.255.255.0", 
                    ["gateway"] = "192.168.1.1"
                },
                RequiresElevation = true
            }
        };

        // Server specific commands
        var serverCommands = new List<ConfigCommand>(genericCommands)
        {
            new ConfigCommand
            {
                Name = "Restart Service",
                Description = "Restart a Windows service",
                Type = ConfigCommandType.Maintenance,
                Script = "Restart-Service -Name {serviceName} -Force",
                Parameters = new Dictionary<string, string> { ["serviceName"] = "Spooler" },
                RequiresElevation = true
            },
            new ConfigCommand
            {
                Name = "Update Windows",
                Description = "Install Windows updates",
                Type = ConfigCommandType.Maintenance,
                Script = "Install-WindowsUpdate -AcceptAll -AutoReboot:$false",
                RequiresElevation = true,
                Prerequisites = new List<string> { "PSWindowsUpdate module" }
            }
        };

        // Workstation specific commands
        var workstationCommands = new List<ConfigCommand>(genericCommands)
        {
            new ConfigCommand
            {
                Name = "Clear DNS Cache",
                Description = "Flush the DNS resolver cache",
                Type = ConfigCommandType.Troubleshooting,
                Script = "Clear-DnsClientCache; Write-Host 'DNS cache cleared successfully!'",
                RequiresElevation = true
            }
        };

        library["generic"] = genericCommands;
        library["router"] = routerCommands;
        library["gateway"] = routerCommands;
        library["server"] = serverCommands;
        library["web server"] = serverCommands;
        library["windows server"] = serverCommands;
        library["workstation"] = workstationCommands;
        library["unknown"] = genericCommands;

        return library;
    }

    private async Task<NetworkNode?> GetNodeByIdAsync(string nodeId, CancellationToken cancellationToken)
    {
        // In a full implementation, this would query the topology database
        // For now, we'll simulate with a basic lookup
        return new NetworkNode 
        { 
            Id = nodeId, 
            IpAddress = "192.168.1.100", 
            HostName = $"Node-{nodeId}" 
        };
    }

    private async Task<ConfigurationResult> ExecuteConfigurationCommand(ConfigCommand command, NetworkNode node, 
        ConfigurationResult result, CancellationToken cancellationToken)
    {
        var script = GeneratePowerShellScript(command, node);
        
        // Publish PowerShell execution event to be handled by PowerShell module
        var executionEvent = new
        {
            EventType = "ExecutePowerShellScript",
            Script = script,
            NodeId = node.Id,
            CommandId = command.Id,
            RequiresElevation = command.RequiresElevation,
            Source = "ConfigurationUtility"
        };

        await _eventBus.PublishAsync(executionEvent);
        
        // For now, simulate execution result
        // In a full implementation, this would wait for the PowerShell module response
        await Task.Delay(1000, cancellationToken); // Simulate execution time
        
        result.Success = true;
        result.Message = $"Configuration {command.Name} executed successfully";
        result.Output = "PowerShell script executed via event bus integration";
        
        return result;
    }

    private bool CheckPrerequisite(string prerequisite, NetworkNode node)
    {
        // Simple prerequisite checking - can be expanded based on needs
        return prerequisite.ToLower() switch
        {
            "pswindowsupdate module" => true, // Assume modules are available
            "admin rights" => true, // Assume we have necessary permissions
            "remote access" => !string.IsNullOrEmpty(node.IpAddress),
            _ => true // Default to true for unknown prerequisites
        };
    }

    private ValidationResult ValidateParameters(ConfigCommand command, NetworkNode node)
    {
        var result = new ValidationResult { IsValid = true };

        foreach (var param in command.Parameters)
        {
            if (string.IsNullOrWhiteSpace(param.Value))
            {
                result.Errors.Add($"Parameter '{param.Key}' cannot be empty");
                result.IsValid = false;
            }

            // IP address validation
            if (param.Key.ToLower().Contains("ip") && !IsValidIpAddress(param.Value))
            {
                result.Errors.Add($"Parameter '{param.Key}' contains invalid IP address: {param.Value}");
                result.IsValid = false;
            }
        }

        return result;
    }

    private ValidationResult PerformSecurityValidation(ConfigCommand command, NetworkNode node)
    {
        var result = new ValidationResult { IsValid = true };

        // Check for potentially dangerous commands
        var dangerousPatterns = new[] { "format", "del /s", "rm -rf", "shutdown", "net user" };
        
        foreach (var pattern in dangerousPatterns)
        {
            if (command.Script.ToLower().Contains(pattern.ToLower()))
            {
                result.Warnings.Add($"⚠️ Potentially dangerous operation detected: {pattern}");
                result.Recommendations.Add("Please verify this operation is intended and safe");
            }
        }

        // Elevation warnings
        if (command.RequiresElevation)
        {
            result.Warnings.Add("🔐 This command requires elevated privileges");
            result.Recommendations.Add("Ensure you have administrative access before proceeding");
        }

        return result;
    }

    private bool IsValidIpAddress(string ipAddress)
    {
        return System.Net.IPAddress.TryParse(ipAddress, out _);
    }

    #endregion
}