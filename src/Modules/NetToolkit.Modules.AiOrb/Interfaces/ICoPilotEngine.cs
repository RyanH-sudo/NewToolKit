using NetToolkit.Modules.AiOrb.Models;

namespace NetToolkit.Modules.AiOrb.Interfaces;

/// <summary>
/// CLI Co-Pilot engine for intelligent terminal assistance and code generation
/// Your AI companion for command-line mastery and automation
/// </summary>
public interface ICoPilotEngine
{
    /// <summary>
    /// Parse terminal output to identify errors, warnings, or opportunities for assistance
    /// </summary>
    /// <param name="output">Raw terminal output text</param>
    /// <param name="context">Context about the command that generated this output</param>
    /// <returns>Parsed information about the terminal output</returns>
    Task<TerminalParseResult> ParseOutputAsync(string output, CliContext context);

    /// <summary>
    /// Generate code suggestions based on terminal output analysis
    /// </summary>
    /// <param name="parseResult">Results from parsing terminal output</param>
    /// <returns>AI-generated code suggestions and fixes</returns>
    Task<CodeSuggestion> GenerateSuggestionAsync(TerminalParseResult parseResult);

    /// <summary>
    /// Execute suggested code with user confirmation
    /// </summary>
    /// <param name="suggestion">Code suggestion to execute</param>
    /// <param name="confirmationCallback">Callback for user confirmation</param>
    /// <returns>Execution result</returns>
    Task<ExecutionResult> ExecuteSuggestionAsync(CodeSuggestion suggestion, Func<string, Task<bool>> confirmationCallback);

    /// <summary>
    /// Analyze command history to provide proactive suggestions
    /// </summary>
    /// <param name="commandHistory">Recent command history</param>
    /// <returns>Proactive suggestions for workflow improvement</returns>
    Task<IEnumerable<ProactiveSuggestion>> AnalyzeWorkflowAsync(IEnumerable<string> commandHistory);

    /// <summary>
    /// Generate script templates for common network engineering tasks
    /// </summary>
    /// <param name="taskType">Type of networking task</param>
    /// <param name="parameters">Task-specific parameters</param>
    /// <returns>Generated script template</returns>
    Task<ScriptTemplate> GenerateScriptTemplateAsync(NetworkTaskType taskType, Dictionary<string, object> parameters);

    /// <summary>
    /// Explain complex commands or error messages in simple terms
    /// </summary>
    /// <param name="command">Command or error message to explain</param>
    /// <returns>Human-readable explanation with learning context</returns>
    Task<string> ExplainCommandAsync(string command);
}