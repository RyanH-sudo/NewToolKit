using NetToolkit.Modules.AiOrb.Models;

namespace NetToolkit.Modules.AiOrb.Interfaces;

/// <summary>
/// The omniscient AI Orb service interface - your cosmic guide to network engineering enlightenment
/// Provides chat assistance, OCR analysis, and CLI co-piloting capabilities
/// </summary>
public interface IAiOrbService
{
    /// <summary>
    /// Configure the AI API settings for the orb's consciousness
    /// </summary>
    /// <param name="config">API configuration containing keys and settings</param>
    /// <returns>Task representing the configuration operation</returns>
    Task ConfigureApiAsync(ApiConfig config);

    /// <summary>
    /// Engage in enlightened conversation with the AI orb
    /// </summary>
    /// <param name="prompt">Your query or request for cosmic wisdom</param>
    /// <param name="context">Optional context for more informed responses</param>
    /// <returns>AI-generated response infused with networking knowledge</returns>
    Task<string> ChatAsync(string prompt, ChatContext? context = null);

    /// <summary>
    /// Analyze screenshots using OCR and AI for insights and suggestions
    /// </summary>
    /// <param name="screenshot">Raw image data from screenshot capture</param>
    /// <param name="analysisType">Type of analysis to perform on the image</param>
    /// <returns>OCR text extraction and AI-generated analysis</returns>
    Task<OcrAnalysisResult> OcrAnalyzeAsync(byte[] screenshot, OcrAnalysisType analysisType = OcrAnalysisType.General);

    /// <summary>
    /// Co-pilot CLI assistance - analyze terminal output and suggest fixes/improvements
    /// </summary>
    /// <param name="terminalOutput">Raw terminal output needing analysis</param>
    /// <param name="context">Additional context about the command or operation</param>
    /// <returns>Code suggestions and remediation steps</returns>
    Task<CodeSuggestion> CoPilotCliAsync(string terminalOutput, CliContext? context = null);

    /// <summary>
    /// Get the current configuration status of the AI orb
    /// </summary>
    /// <returns>Current configuration state and health status</returns>
    Task<OrbStatus> GetStatusAsync();

    /// <summary>
    /// Test the AI API connectivity and response
    /// </summary>
    /// <returns>API health check result</returns>
    Task<bool> TestApiConnectionAsync();
}

/// <summary>
/// Extended interface for orb event publishing capabilities
/// </summary>
public interface IOrbEventPublisher
{
    /// <summary>
    /// Publish orb activation event when the orb becomes active
    /// </summary>
    Task PublishOrbActivatedAsync(string userId, DateTime activatedAt);

    /// <summary>
    /// Publish chat response event with conversation details
    /// </summary>
    Task PublishChatResponseAsync(string prompt, string response, TimeSpan responseTime);

    /// <summary>
    /// Publish OCR analysis result event
    /// </summary>
    Task PublishOcrResultAsync(OcrAnalysisResult result);

    /// <summary>
    /// Publish CLI co-pilot suggestion event
    /// </summary>
    Task PublishCoPilotSuggestionAsync(CodeSuggestion suggestion);

    /// <summary>
    /// Publish configuration update event
    /// </summary>
    Task PublishConfigUpdatedAsync(ApiConfig oldConfig, ApiConfig newConfig);
}