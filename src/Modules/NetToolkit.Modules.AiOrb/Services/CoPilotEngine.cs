using Microsoft.Extensions.Logging;
using NetToolkit.Modules.AiOrb.Interfaces;
using NetToolkit.Modules.AiOrb.Models;
using System.Diagnostics;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;

namespace NetToolkit.Modules.AiOrb.Services;

/// <summary>
/// CLI Co-Pilot engine for intelligent terminal assistance and code generation
/// Your AI companion for command-line mastery and automation excellence
/// </summary>
public class CoPilotEngine : ICoPilotEngine
{
    private readonly ILogger<CoPilotEngine> _logger;
    private readonly AiClientService _aiClient;
    private readonly Dictionary<string, List<string>> _commandHistory;
    private readonly Dictionary<string, ScriptTemplate> _scriptTemplates;
    private readonly SemaphoreSlim _executionLock;

    // Common error patterns for different shells and tools
    private static readonly Dictionary<string, Regex[]> ErrorPatterns = new()
    {
        ["powershell"] = new[]
        {
            new Regex(@".*: (.+) is not recognized as the name of a cmdlet.*", RegexOptions.IgnoreCase),
            new Regex(@"Cannot bind parameter '(.+)'", RegexOptions.IgnoreCase),
            new Regex(@"Access to the path '(.+)' is denied", RegexOptions.IgnoreCase),
            new Regex(@"Exception calling ""(.+)"" with ""(.+)"" argument", RegexOptions.IgnoreCase),
            new Regex(@"The term '(.+)' is not recognized", RegexOptions.IgnoreCase)
        },
        ["bash"] = new[]
        {
            new Regex(@"(.+): command not found", RegexOptions.IgnoreCase),
            new Regex(@"(.+): Permission denied", RegexOptions.IgnoreCase),
            new Regex(@"(.+): No such file or directory", RegexOptions.IgnoreCase),
            new Regex(@"sudo: (.+): command not found", RegexOptions.IgnoreCase)
        },
        ["cmd"] = new[]
        {
            new Regex(@"'(.+)' is not recognized as an internal or external command", RegexOptions.IgnoreCase),
            new Regex(@"The system cannot find the file specified", RegexOptions.IgnoreCase),
            new Regex(@"Access is denied", RegexOptions.IgnoreCase)
        }
    };

    public CoPilotEngine(ILogger<CoPilotEngine> logger, AiClientService aiClient)
    {
        _logger = logger;
        _aiClient = aiClient;
        _commandHistory = new Dictionary<string, List<string>>();
        _scriptTemplates = new Dictionary<string, ScriptTemplate>();
        _executionLock = new SemaphoreSlim(1, 1);
        
        InitializeScriptTemplates();
        
        _logger.LogInformation("CLI Co-Pilot engine initialized - Orb command mastery activated! ⚡");
    }

    /// <summary>
    /// Parse terminal output to identify errors, warnings, or opportunities for assistance
    /// </summary>
    public async Task<TerminalParseResult> ParseOutputAsync(string output, CliContext context)
    {
        try
        {
            var result = new TerminalParseResult();
            
            if (string.IsNullOrWhiteSpace(output))
            {
                return result;
            }

            var shell = context.Shell.ToLowerInvariant();
            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                             .Select(l => l.Trim())
                             .ToList();

            // Analyze exit code
            result.HasErrors = context.ExitCode != 0;

            // Parse errors and warnings
            await ParseErrorsAndWarningsAsync(result, lines, shell);
            
            // Identify success indicators
            ParseSuccessIndicators(result, lines, shell);
            
            // Extract structured data
            await ExtractStructuredDataAsync(result, lines, context);
            
            // Identify improvement opportunities
            await IdentifyImprovementOpportunitiesAsync(result, context);

            _logger.LogInformation("Terminal output parsed - Orb analysis complete! 🔍 Errors: {ErrorCount}, Warnings: {WarningCount}", 
                result.ErrorMessages.Count, result.WarningMessages.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Terminal output parsing failed - Orb parser circuits overloaded! ⚡");
            return new TerminalParseResult
            {
                HasErrors = true,
                ErrorMessages = new List<string> { $"Parsing failed: {ex.Message}" }
            };
        }
    }

    /// <summary>
    /// Generate code suggestions based on terminal output analysis
    /// </summary>
    public async Task<CodeSuggestion> GenerateSuggestionAsync(TerminalParseResult parseResult)
    {
        try
        {
            // Build context for AI suggestion generation
            var contextBuilder = new StringBuilder();
            contextBuilder.AppendLine("Terminal Analysis Results:");
            
            if (parseResult.HasErrors)
            {
                contextBuilder.AppendLine("ERRORS DETECTED:");
                foreach (var error in parseResult.ErrorMessages)
                {
                    contextBuilder.AppendLine($"- {error}");
                }
            }

            if (parseResult.HasWarnings)
            {
                contextBuilder.AppendLine("WARNINGS:");
                foreach (var warning in parseResult.WarningMessages)
                {
                    contextBuilder.AppendLine($"- {warning}");
                }
            }

            if (parseResult.Opportunities.Any())
            {
                contextBuilder.AppendLine("IMPROVEMENT OPPORTUNITIES:");
                foreach (var opportunity in parseResult.Opportunities)
                {
                    contextBuilder.AppendLine($"- {opportunity.Title}: {opportunity.Description}");
                }
            }

            contextBuilder.AppendLine();
            contextBuilder.AppendLine("Please provide a specific, actionable code suggestion to address these issues.");
            contextBuilder.AppendLine("Include the actual commands or code to run, with explanations.");

            // Generate AI suggestion
            var suggestion = await _aiClient.AnalyzeCodeAsync(contextBuilder.ToString());
            
            // Enhance suggestion with co-pilot specific information
            EnhanceSuggestionWithCoPilotData(suggestion, parseResult);

            _logger.LogInformation("Code suggestion generated - Orb co-pilot wisdom delivered! 🤖 Type: {Type}", 
                suggestion.Type);

            return suggestion;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Code suggestion generation failed - Orb co-pilot circuits confused! 🤔");
            return new CodeSuggestion
            {
                Title = "Suggestion Generation Failed",
                Description = $"Co-pilot encountered an error: {ex.Message}",
                Type = SuggestionType.Fix,
                ConfidenceLevel = 0
            };
        }
    }

    /// <summary>
    /// Execute suggested code with user confirmation
    /// </summary>
    public async Task<ExecutionResult> ExecuteSuggestionAsync(CodeSuggestion suggestion, Func<string, Task<bool>> confirmationCallback)
    {
        try
        {
            await _executionLock.WaitAsync();

            // Request user confirmation
            var confirmationMessage = $"Orb Co-Pilot suggests executing:\n\n{suggestion.Code}\n\nDescription: {suggestion.Description}\n\nProceed?";
            var confirmed = await confirmationCallback(confirmationMessage);

            if (!confirmed)
            {
                return new ExecutionResult
                {
                    Success = false,
                    Output = "Execution cancelled by user - Orb respects your cosmic judgment! 🙏",
                    ExecutionTime = TimeSpan.Zero
                };
            }

            // Execute the suggestion
            var startTime = DateTime.UtcNow;
            var result = await ExecuteCodeAsync(suggestion);
            result.ExecutionTime = DateTime.UtcNow - startTime;

            _logger.LogInformation("Code suggestion executed - Orb co-pilot action completed! ✅ Success: {Success}", 
                result.Success);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Code suggestion execution failed - Orb co-pilot malfunction! 🚨");
            return new ExecutionResult
            {
                Success = false,
                ErrorOutput = $"Execution failed: {ex.Message}",
                ExecutionTime = TimeSpan.Zero
            };
        }
        finally
        {
            _executionLock.Release();
        }
    }

    /// <summary>
    /// Analyze command history to provide proactive suggestions
    /// </summary>
    public async Task<IEnumerable<ProactiveSuggestion>> AnalyzeWorkflowAsync(IEnumerable<string> commandHistory)
    {
        try
        {
            var suggestions = new List<ProactiveSuggestion>();
            var commands = commandHistory.ToList();

            if (!commands.Any())
            {
                return suggestions;
            }

            // Analyze command patterns
            var patterns = AnalyzeCommandPatterns(commands);
            
            // Generate suggestions based on patterns
            foreach (var pattern in patterns)
            {
                var suggestion = await GenerateProactiveSuggestionForPattern(pattern, commands);
                if (suggestion != null)
                {
                    suggestions.Add(suggestion);
                }
            }

            // Add general workflow improvements
            suggestions.AddRange(await GenerateGeneralWorkflowSuggestions(commands));

            _logger.LogInformation("Workflow analysis completed - Orb identified {SuggestionCount} optimization opportunities! 🎯", 
                suggestions.Count);

            return suggestions.OrderByDescending(s => s.Priority).Take(5); // Return top 5 suggestions
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Workflow analysis failed - Orb pattern recognition circuits overloaded! 🔄");
            return new List<ProactiveSuggestion>();
        }
    }

    /// <summary>
    /// Generate script templates for common network engineering tasks
    /// </summary>
    public async Task<ScriptTemplate> GenerateScriptTemplateAsync(NetworkTaskType taskType, Dictionary<string, object> parameters)
    {
        try
        {
            var templateKey = $"{taskType}_{string.Join("_", parameters.Keys.OrderBy(k => k))}";
            
            if (_scriptTemplates.ContainsKey(templateKey))
            {
                var cachedTemplate = _scriptTemplates[templateKey];
                return PersonalizeTemplate(cachedTemplate, parameters);
            }

            // Generate new template using AI
            var template = await GenerateAIScriptTemplate(taskType, parameters);
            
            // Cache the template for future use
            _scriptTemplates[templateKey] = template;

            _logger.LogInformation("Script template generated - Orb automation blueprint created! 📜 Type: {TaskType}", 
                taskType);

            return PersonalizeTemplate(template, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Script template generation failed - Orb automation circuits jammed! 🛠️");
            return new ScriptTemplate
            {
                Name = "Template Generation Failed",
                Description = $"Failed to generate template: {ex.Message}",
                Language = "text",
                Template = "# Template generation failed - please try again with different parameters"
            };
        }
    }

    /// <summary>
    /// Explain complex commands or error messages in simple terms
    /// </summary>
    public async Task<string> ExplainCommandAsync(string command)
    {
        try
        {
            var explanationPrompt = $"""
                Please explain this command or error message in simple, clear terms suitable for a network engineer:

                {command}

                Provide:
                1. What this command/message means
                2. What it's trying to do (if it's a command)
                3. Common reasons for this error (if it's an error)
                4. Suggested next steps
                5. Related concepts to learn

                Keep the explanation practical and actionable.
                """;

            var explanation = await _aiClient.ChatAsync(explanationPrompt);
            
            _logger.LogInformation("Command explanation generated - Orb teaching mode activated! 🎓 Length: {Length}", 
                explanation.Length);

            return explanation;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Command explanation failed - Orb professor circuits confused! 👨‍🏫");
            return $"Orb could not explain this command due to a technical issue: {ex.Message}. " +
                   "Try breaking down the command into smaller parts or consulting documentation.";
        }
    }

    /// <summary>
    /// Parse errors and warnings from terminal output
    /// </summary>
    private async Task ParseErrorsAndWarningsAsync(TerminalParseResult result, List<string> lines, string shell)
    {
        if (!ErrorPatterns.ContainsKey(shell))
        {
            // Generic error detection for unknown shells
            foreach (var line in lines)
            {
                var lowerLine = line.ToLowerInvariant();
                if (lowerLine.Contains("error") || lowerLine.Contains("fail") || lowerLine.Contains("exception"))
                {
                    result.ErrorMessages.Add(line);
                    result.HasErrors = true;
                }
                else if (lowerLine.Contains("warning") || lowerLine.Contains("warn"))
                {
                    result.WarningMessages.Add(line);
                    result.HasWarnings = true;
                }
            }
            return;
        }

        var patterns = ErrorPatterns[shell];
        
        foreach (var line in lines)
        {
            foreach (var pattern in patterns)
            {
                if (pattern.IsMatch(line))
                {
                    result.ErrorMessages.Add(line);
                    result.HasErrors = true;
                    break;
                }
            }

            // Check for warnings
            if (line.ToLowerInvariant().Contains("warning"))
            {
                result.WarningMessages.Add(line);
                result.HasWarnings = true;
            }
        }
    }

    /// <summary>
    /// Parse success indicators from output
    /// </summary>
    private static void ParseSuccessIndicators(TerminalParseResult result, List<string> lines, string shell)
    {
        var successKeywords = new[] { "success", "completed", "done", "finished", "ok", "connected", "started" };
        
        foreach (var line in lines)
        {
            var lowerLine = line.ToLowerInvariant();
            if (successKeywords.Any(keyword => lowerLine.Contains(keyword)))
            {
                result.SuccessIndicators.Add(line);
            }
        }
    }

    /// <summary>
    /// Extract structured data from terminal output
    /// </summary>
    private async Task ExtractStructuredDataAsync(TerminalParseResult result, List<string> lines, CliContext context)
    {
        // Extract IP addresses
        var ipPattern = new Regex(@"\b(?:[0-9]{1,3}\.){3}[0-9]{1,3}\b");
        foreach (var line in lines)
        {
            var matches = ipPattern.Matches(line);
            foreach (Match match in matches)
            {
                result.ParsedData[$"ip_address_{result.ParsedData.Count}"] = match.Value;
            }
        }

        // Extract file paths
        var pathPattern = new Regex(@"(?:[A-Za-z]:\\|\/)(?:[^\\\/\r\n]+[\\\/])*[^\\\/\r\n]*");
        foreach (var line in lines)
        {
            var matches = pathPattern.Matches(line);
            foreach (Match match in matches)
            {
                result.ParsedData[$"file_path_{result.ParsedData.Count}"] = match.Value;
            }
        }

        // Extract URLs
        var urlPattern = new Regex(@"https?:\/\/[^\s]+");
        foreach (var line in lines)
        {
            var matches = urlPattern.Matches(line);
            foreach (Match match in matches)
            {
                result.ParsedData[$"url_{result.ParsedData.Count}"] = match.Value;
            }
        }

        // Set command type based on last command
        if (!string.IsNullOrEmpty(context.LastCommand))
        {
            result.CommandType = DetermineCommandType(context.LastCommand);
        }
    }

    /// <summary>
    /// Identify improvement opportunities
    /// </summary>
    private async Task IdentifyImprovementOpportunitiesAsync(TerminalParseResult result, CliContext context)
    {
        var opportunities = new List<ImprovementOpportunity>();

        // Check for inefficient command usage
        if (context.CommandHistory.Count >= 3)
        {
            var recentCommands = context.CommandHistory.TakeLast(3).ToList();
            if (recentCommands.All(cmd => cmd.StartsWith("cd ")) || 
                recentCommands.Count(cmd => cmd.Contains("ls") || cmd.Contains("dir")) > 1)
            {
                opportunities.Add(new ImprovementOpportunity
                {
                    Title = "Navigation Efficiency",
                    Description = "Multiple directory navigation commands detected",
                    Category = "Workflow",
                    ImpactScore = 2,
                    SuggestedAction = "Consider using tab completion or bookmarking frequently used paths"
                });
            }
        }

        // Check for missing error handling
        if (result.HasErrors && context.LastCommand.Contains("powershell") && !context.LastCommand.Contains("try"))
        {
            opportunities.Add(new ImprovementOpportunity
            {
                Title = "Error Handling",
                Description = "Command failed without error handling",
                Category = "Reliability",
                ImpactScore = 3,
                SuggestedAction = "Add try-catch blocks for better error management"
            });
        }

        // Check for security concerns
        if (context.LastCommand.ToLowerInvariant().Contains("password") && 
            !context.LastCommand.Contains("SecureString"))
        {
            opportunities.Add(new ImprovementOpportunity
            {
                Title = "Security Enhancement",
                Description = "Potential password handling detected",
                Category = "Security",
                ImpactScore = 4,
                SuggestedAction = "Use SecureString or credential management for sensitive data"
            });
        }

        result.Opportunities = opportunities;
    }

    /// <summary>
    /// Enhance suggestion with co-pilot specific data
    /// </summary>
    private static void EnhanceSuggestionWithCoPilotData(CodeSuggestion suggestion, TerminalParseResult parseResult)
    {
        // Add risk factors based on parse results
        if (parseResult.HasErrors)
        {
            suggestion.RiskFactors.Add("Previous command resulted in errors");
        }

        // Add prerequisites based on command type
        if (parseResult.CommandType.Contains("Network"))
        {
            suggestion.Prerequisites.Add("Network connectivity");
            suggestion.Prerequisites.Add("Appropriate permissions");
        }

        // Adjust confidence based on parse quality
        if (parseResult.ParsedData.Any())
        {
            suggestion.ConfidenceLevel = Math.Min(95, suggestion.ConfidenceLevel + 10);
        }
    }

    /// <summary>
    /// Execute code suggestion
    /// </summary>
    private async Task<ExecutionResult> ExecuteCodeAsync(CodeSuggestion suggestion)
    {
        try
        {
            if (suggestion.Language.ToLowerInvariant() == "powershell")
            {
                return await ExecutePowerShellAsync(suggestion.Code);
            }
            else if (suggestion.Language.ToLowerInvariant() == "bash")
            {
                return await ExecuteBashAsync(suggestion.Code);
            }
            else
            {
                return new ExecutionResult
                {
                    Success = false,
                    ErrorOutput = $"Unsupported language: {suggestion.Language}"
                };
            }
        }
        catch (Exception ex)
        {
            return new ExecutionResult
            {
                Success = false,
                ErrorOutput = $"Execution failed: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Execute PowerShell code
    /// </summary>
    private async Task<ExecutionResult> ExecutePowerShellAsync(string code)
    {
        using var powerShell = PowerShell.Create();
        
        try
        {
            powerShell.AddScript(code);
            var results = await Task.Run(() => powerShell.Invoke());
            
            var output = new StringBuilder();
            foreach (var result in results)
            {
                output.AppendLine(result.ToString());
            }

            var errors = new StringBuilder();
            foreach (var error in powerShell.Streams.Error)
            {
                errors.AppendLine(error.ToString());
            }

            return new ExecutionResult
            {
                Success = !powerShell.HadErrors,
                Output = output.ToString(),
                ErrorOutput = errors.ToString(),
                ExitCode = powerShell.HadErrors ? 1 : 0
            };
        }
        catch (Exception ex)
        {
            return new ExecutionResult
            {
                Success = false,
                ErrorOutput = ex.Message,
                ExitCode = 1
            };
        }
    }

    /// <summary>
    /// Execute Bash command (Windows Subsystem for Linux or external bash)
    /// </summary>
    private async Task<ExecutionResult> ExecuteBashAsync(string code)
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = "bash",
            Arguments = $"-c \"{code.Replace("\"", "\\\"")}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        try
        {
            using var process = Process.Start(processInfo);
            if (process == null)
            {
                return new ExecutionResult
                {
                    Success = false,
                    ErrorOutput = "Failed to start bash process"
                };
            }

            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();
            
            await process.WaitForExitAsync();
            
            var output = await outputTask;
            var errorOutput = await errorTask;

            return new ExecutionResult
            {
                Success = process.ExitCode == 0,
                Output = output,
                ErrorOutput = errorOutput,
                ExitCode = process.ExitCode
            };
        }
        catch (Exception ex)
        {
            return new ExecutionResult
            {
                Success = false,
                ErrorOutput = ex.Message,
                ExitCode = 1
            };
        }
    }

    /// <summary>
    /// Analyze command patterns for workflow optimization
    /// </summary>
    private static List<string> AnalyzeCommandPatterns(List<string> commands)
    {
        var patterns = new List<string>();
        
        // Look for repeated command sequences
        for (int i = 0; i < commands.Count - 2; i++)
        {
            var sequence = string.Join(" -> ", commands.Skip(i).Take(3));
            if (commands.Skip(i + 1).Any(cmd => commands.Skip(commands.IndexOf(cmd)).Take(3)
                .SequenceEqual(commands.Skip(i).Take(3))))
            {
                patterns.Add($"Repeated sequence: {sequence}");
            }
        }

        return patterns.Distinct().ToList();
    }

    /// <summary>
    /// Generate proactive suggestions based on patterns
    /// </summary>
    private async Task<ProactiveSuggestion?> GenerateProactiveSuggestionForPattern(string pattern, List<string> commands)
    {
        if (pattern.Contains("Repeated sequence"))
        {
            return new ProactiveSuggestion
            {
                Title = "Workflow Automation Opportunity",
                Description = $"Detected repeated command pattern: {pattern}",
                Category = "Automation",
                Priority = 3,
                Benefits = new List<string> { "Reduce manual work", "Increase consistency", "Save time" },
                CodeSuggestion = new CodeSuggestion
                {
                    Title = "Automation Script",
                    Description = "Create a script to automate this repeated workflow",
                    Code = "# Automation script will be generated based on your specific pattern",
                    Type = SuggestionType.Automation
                }
            };
        }

        return null;
    }

    /// <summary>
    /// Generate general workflow suggestions
    /// </summary>
    private async Task<List<ProactiveSuggestion>> GenerateGeneralWorkflowSuggestions(List<string> commands)
    {
        var suggestions = new List<ProactiveSuggestion>();
        
        // Check for lack of aliases
        if (commands.Count(cmd => cmd.Length > 20) > commands.Count * 0.3)
        {
            suggestions.Add(new ProactiveSuggestion
            {
                Title = "Command Aliases",
                Description = "Many long commands detected - consider creating aliases",
                Category = "Efficiency",
                Priority = 2,
                Benefits = new List<string> { "Faster command execution", "Reduced typing errors" }
            });
        }

        return suggestions;
    }

    /// <summary>
    /// Generate AI-powered script template
    /// </summary>
    private async Task<ScriptTemplate> GenerateAIScriptTemplate(NetworkTaskType taskType, Dictionary<string, object> parameters)
    {
        var prompt = $"""
            Generate a PowerShell script template for the following network task:
            Task Type: {taskType}
            Parameters: {string.Join(", ", parameters.Select(kvp => $"{kvp.Key}={kvp.Value}"))}

            The script should:
            1. Include proper error handling
            2. Have clear parameter definitions
            3. Include helpful comments
            4. Follow PowerShell best practices
            5. Be modular and reusable

            Provide the script template with parameter placeholders.
            """;

        var scriptContent = await _aiClient.ChatAsync(prompt);
        
        return new ScriptTemplate
        {
            Name = $"{taskType} Template",
            Description = $"AI-generated template for {taskType} tasks",
            Language = "powershell",
            Template = scriptContent,
            Category = taskType.ToString()
        };
    }

    /// <summary>
    /// Personalize template with user parameters
    /// </summary>
    private static ScriptTemplate PersonalizeTemplate(ScriptTemplate template, Dictionary<string, object> parameters)
    {
        var personalizedTemplate = template.Template;
        
        foreach (var param in parameters)
        {
            personalizedTemplate = personalizedTemplate.Replace($"{{{param.Key}}}", param.Value.ToString());
        }

        return new ScriptTemplate
        {
            Name = template.Name,
            Description = template.Description,
            Language = template.Language,
            Template = personalizedTemplate,
            Parameters = template.Parameters,
            RequiredModules = template.RequiredModules,
            Category = template.Category
        };
    }

    /// <summary>
    /// Determine command type from command text
    /// </summary>
    private static string DetermineCommandType(string command)
    {
        var lowerCommand = command.ToLowerInvariant();
        
        if (lowerCommand.Contains("ping") || lowerCommand.Contains("tracert") || lowerCommand.Contains("nslookup"))
            return "Network Diagnostic";
        
        if (lowerCommand.Contains("get-") || lowerCommand.Contains("set-") || lowerCommand.Contains("new-"))
            return "PowerShell Management";
        
        if (lowerCommand.Contains("ssh") || lowerCommand.Contains("scp"))
            return "Remote Connection";
        
        if (lowerCommand.Contains("netstat") || lowerCommand.Contains("ipconfig") || lowerCommand.Contains("ifconfig"))
            return "Network Configuration";
        
        return "General Command";
    }

    /// <summary>
    /// Initialize built-in script templates
    /// </summary>
    private void InitializeScriptTemplates()
    {
        // Network Scan Template
        _scriptTemplates["NetworkScan"] = new ScriptTemplate
        {
            Name = "Network Scanner",
            Description = "Comprehensive network scanning and discovery",
            Language = "powershell",
            Category = "NetworkScan",
            Template = """
                # Network Scanner Template
                param(
                    [Parameter(Mandatory=$true)]
                    [string]$NetworkRange,
                    [int]$Timeout = 1000
                )
                
                Write-Output "Scanning network range: $NetworkRange"
                # Template will be expanded by AI
                """,
            Parameters = new List<ScriptParameter>
            {
                new() { Name = "NetworkRange", Description = "IP range to scan", Type = "string", Required = true },
                new() { Name = "Timeout", Description = "Ping timeout in ms", Type = "int", DefaultValue = "1000" }
            }
        };

        _logger.LogDebug("Script templates initialized - Orb automation library loaded! 📚");
    }
}