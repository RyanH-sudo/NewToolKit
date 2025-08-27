using Microsoft.Extensions.Logging;
using NetToolkit.Core.Interfaces;
using NetToolkit.Modules.AiOrb.Events;
using NetToolkit.Modules.AiOrb.Interfaces;
using NetToolkit.Modules.AiOrb.Models;
using System.Diagnostics;

namespace NetToolkit.Modules.AiOrb.Services;

/// <summary>
/// Main AI Orb service that orchestrates all AI functionality
/// The cosmic intelligence hub of NetToolkit's assistance capabilities
/// </summary>
public class AiOrbService : IAiOrbService, IOrbEventPublisher
{
    private readonly ILogger<AiOrbService> _logger;
    private readonly IConfigManager _configManager;
    private readonly AiClientService _aiClient;
    private readonly IOcrProcessor _ocrProcessor;
    private readonly ICoPilotEngine _coPilotEngine;
    private readonly IEventBus _eventBus;
    
    private ApiConfig? _currentConfig;
    private bool _isInitialized;
    private readonly object _initializationLock = new();

    public AiOrbService(
        ILogger<AiOrbService> logger,
        IConfigManager configManager,
        AiClientService aiClient,
        IOcrProcessor ocrProcessor,
        ICoPilotEngine coPilotEngine,
        IEventBus eventBus)
    {
        _logger = logger;
        _configManager = configManager;
        _aiClient = aiClient;
        _ocrProcessor = ocrProcessor;
        _coPilotEngine = coPilotEngine;
        _eventBus = eventBus;

        _logger.LogInformation("AI Orb service initialized - Cosmic consciousness awakening! 🌟");
    }

    /// <summary>
    /// Configure the AI API settings for the orb's consciousness
    /// </summary>
    public async Task ConfigureApiAsync(ApiConfig config)
    {
        try
        {
            var oldConfig = _currentConfig;
            
            // Validate and save configuration
            var isValid = await _configManager.SaveConfigAsync(config);
            if (!isValid)
            {
                throw new InvalidOperationException("Configuration validation failed - Orb wisdom rejected! ❌");
            }

            _currentConfig = config;

            // Initialize components if configuration is complete
            if (!string.IsNullOrWhiteSpace(config.ApiKey))
            {
                await EnsureInitializedAsync();
            }

            // Publish configuration update event
            await PublishConfigUpdatedAsync(oldConfig ?? new ApiConfig(), config);

            _logger.LogInformation("API configuration updated successfully - Orb consciousness reconfigured! ⚙️");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API configuration failed - Orb configuration circuits confused! 🤖");
            throw;
        }
    }

    /// <summary>
    /// Engage in enlightened conversation with the AI orb
    /// </summary>
    public async Task<string> ChatAsync(string prompt, ChatContext? context = null)
    {
        try
        {
            await EnsureInitializedAsync();

            if (!await IsConfiguredAsync())
            {
                return "Orb consciousness not configured - please set up your API keys first! 🔑";
            }

            var startTime = DateTime.UtcNow;

            // Enhance context with NetToolkit-specific information
            var enhancedContext = EnhanceChatContext(context ?? new ChatContext());

            // Generate response using AI client
            var response = await _aiClient.ChatAsync(prompt, enhancedContext);

            // Calculate response time
            var responseTime = DateTime.UtcNow - startTime;

            // Publish chat response event
            await PublishChatResponseAsync(prompt, response, responseTime);

            _logger.LogInformation("Chat completed successfully - Orb wisdom delivered! 💬 Response time: {ResponseTime}ms", 
                responseTime.TotalMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Chat request failed - Orb temporarily speechless! 😶");
            return $"Orb encountered cosmic interference: {ex.Message}. Please try again when the digital stars align! ⭐";
        }
    }

    /// <summary>
    /// Analyze screenshots using OCR and AI for insights and suggestions
    /// </summary>
    public async Task<OcrAnalysisResult> OcrAnalyzeAsync(byte[] screenshot, OcrAnalysisType analysisType = OcrAnalysisType.General)
    {
        try
        {
            await EnsureInitializedAsync();

            var startTime = DateTime.UtcNow;

            // Extract text using OCR
            var ocrResult = await _ocrProcessor.ExtractTextAsync(screenshot);
            
            if (string.IsNullOrWhiteSpace(ocrResult.Text))
            {
                var emptyResult = new OcrAnalysisResult
                {
                    ExtractedText = "No text detected in image",
                    ConfidenceScore = 0,
                    AiAnalysis = "Orb could not detect any text in the provided image. Try capturing a clearer screenshot! 📷",
                    ProcessingTime = DateTime.UtcNow - startTime
                };

                await PublishOcrResultAsync(emptyResult);
                return emptyResult;
            }

            // Analyze extracted text with AI
            var textAnalysis = await _ocrProcessor.AnalyzeExtractedTextAsync(ocrResult.Text, analysisType);

            // Generate comprehensive AI analysis
            var aiAnalysis = await GenerateAIAnalysisForOcr(ocrResult.Text, analysisType, textAnalysis);

            var result = new OcrAnalysisResult
            {
                ExtractedText = ocrResult.Text,
                ConfidenceScore = ocrResult.Confidence,
                AiAnalysis = aiAnalysis,
                Insights = textAnalysis.Insights,
                Suggestions = textAnalysis.Suggestions,
                ProcessingTime = DateTime.UtcNow - startTime,
                TechnicalData = new Dictionary<string, object>
                {
                    ["ocrLanguage"] = ocrResult.Language,
                    ["regionCount"] = ocrResult.Regions.Count,
                    ["analysisType"] = analysisType.ToString(),
                    ["wordCount"] = ocrResult.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length
                }
            };

            // Publish OCR analysis event
            await PublishOcrResultAsync(result);

            _logger.LogInformation("OCR analysis completed - Orb vision decoded {CharCount} characters! 👁️ Confidence: {Confidence}%", 
                result.ExtractedText.Length, Math.Round(result.ConfidenceScore * 100, 1));

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OCR analysis failed - Orb optical circuits overloaded! ⚡");
            
            var errorResult = new OcrAnalysisResult
            {
                ExtractedText = "OCR analysis failed",
                ConfidenceScore = 0,
                AiAnalysis = $"Orb encountered an error during analysis: {ex.Message}",
                ProcessingTime = DateTime.UtcNow - startTime
            };

            await PublishOcrResultAsync(errorResult);
            return errorResult;
        }
    }

    /// <summary>
    /// Co-pilot CLI assistance - analyze terminal output and suggest fixes/improvements
    /// </summary>
    public async Task<CodeSuggestion> CoPilotCliAsync(string terminalOutput, CliContext? context = null)
    {
        try
        {
            await EnsureInitializedAsync();

            var enhancedContext = context ?? new CliContext();

            // Parse terminal output
            var parseResult = await _coPilotEngine.ParseOutputAsync(terminalOutput, enhancedContext);

            // Generate suggestions based on analysis
            var suggestion = await _coPilotEngine.GenerateSuggestionAsync(parseResult);

            // Enhance suggestion with orb-specific metadata
            suggestion.Parameters["orbProcessedAt"] = DateTime.UtcNow;
            suggestion.Parameters["orbConfidence"] = suggestion.ConfidenceLevel;
            suggestion.Parameters["sourceTerminalOutput"] = terminalOutput.Length > 500 
                ? terminalOutput.Substring(0, 500) + "..." 
                : terminalOutput;

            // Publish co-pilot suggestion event
            await PublishCoPilotSuggestionAsync(suggestion);

            _logger.LogInformation("CLI co-pilot analysis completed - Orb coding wisdom generated! 💻 Type: {SuggestionType}", 
                suggestion.Type);

            return suggestion;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CLI co-pilot analysis failed - Orb coding circuits confused! 🤖");
            
            return new CodeSuggestion
            {
                Title = "Co-Pilot Analysis Failed",
                Description = $"Orb encountered an error analyzing the terminal output: {ex.Message}",
                Code = "# Analysis failed - please try again with different input",
                Type = SuggestionType.Fix,
                ConfidenceLevel = 0,
                Parameters = new Dictionary<string, object>
                {
                    ["error"] = ex.Message,
                    ["failedAt"] = DateTime.UtcNow
                }
            };
        }
    }

    /// <summary>
    /// Get the current configuration status of the AI orb
    /// </summary>
    public async Task<OrbStatus> GetStatusAsync()
    {
        try
        {
            var isConfigured = await IsConfiguredAsync();
            var apiConnected = isConfigured && await TestApiConnectionAsync();
            
            return new OrbStatus
            {
                IsConfigured = isConfigured,
                ApiConnected = apiConnected,
                OcrReady = _isInitialized,
                OverlayActive = false, // Will be updated by overlay handler
                CurrentMode = DetermineCurrentMode(),
                LastActivity = DateTime.UtcNow,
                HealthMetrics = await GetHealthMetricsAsync(),
                ActiveFeatures = GetActiveFeatures()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Status check failed - Orb self-diagnostics confused! 🔍");
            
            return new OrbStatus
            {
                IsConfigured = false,
                ApiConnected = false,
                OcrReady = false,
                CurrentMode = $"Error: {ex.Message}",
                LastActivity = DateTime.UtcNow
            };
        }
    }

    /// <summary>
    /// Test the AI API connectivity and response
    /// </summary>
    public async Task<bool> TestApiConnectionAsync()
    {
        try
        {
            await EnsureInitializedAsync();
            
            if (!await IsConfiguredAsync())
            {
                return false;
            }

            var testResult = await _aiClient.TestConnectionAsync();
            
            _logger.LogInformation("API connection test completed - Cosmic link status: {Status} 🌐", 
                testResult ? "CONNECTED" : "DISCONNECTED");
            
            return testResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API connection test failed - Orb communication disrupted! 📡");
            return false;
        }
    }

    // Event publishing methods

    public async Task PublishOrbActivatedAsync(string userId, DateTime activatedAt)
    {
        try
        {
            var activationEvent = new OrbActivatedEvent
            {
                UserId = userId,
                ActivatedAt = activatedAt,
                Configuration = _currentConfig ?? new ApiConfig(),
                AvailableFeatures = GetActiveFeatures(),
                ActivationReason = "UserRequest"
            };

            await _eventBus.PublishAsync(activationEvent);
            
            _logger.LogInformation("Orb activation event published - Cosmic awakening announced! 🌟");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish orb activation event - Cosmic communication disrupted! 📡");
        }
    }

    public async Task PublishChatResponseAsync(string prompt, string response, TimeSpan responseTime)
    {
        try
        {
            var chatEvent = new ChatResponseEvent
            {
                Prompt = prompt.Length > 200 ? prompt.Substring(0, 200) + "..." : prompt,
                Response = response.Length > 500 ? response.Substring(0, 500) + "..." : response,
                ResponseTime = responseTime,
                TokensUsed = EstimateTokenUsage(prompt, response),
                ConfidenceScore = 0.85, // Default confidence score
                Keywords = ExtractKeywords(prompt + " " + response)
            };

            await _eventBus.PublishAsync(chatEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish chat response event - Orb communication glitch! 💬");
        }
    }

    public async Task PublishOcrResultAsync(OcrAnalysisResult result)
    {
        try
        {
            var ocrEvent = new OcrAnalysisCompletedEvent
            {
                Result = result,
                AnalysisType = OcrAnalysisType.General, // Default, should be passed in
                CompletedAt = DateTime.UtcNow,
                Metadata = new Dictionary<string, object>
                {
                    ["processingTimeMs"] = result.ProcessingTime.TotalMilliseconds,
                    ["textLength"] = result.ExtractedText.Length,
                    ["insightCount"] = result.Insights.Count
                }
            };

            await _eventBus.PublishAsync(ocrEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish OCR result event - Orb vision relay malfunction! 👁️");
        }
    }

    public async Task PublishCoPilotSuggestionAsync(CodeSuggestion suggestion)
    {
        try
        {
            var coPilotEvent = new CoPilotSuggestionEvent
            {
                Suggestion = suggestion,
                SourceModule = "AiOrb",
                GeneratedAt = DateTime.UtcNow,
                Context = new CliContext() // Should be passed from calling method
            };

            await _eventBus.PublishAsync(coPilotEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish co-pilot suggestion event - Orb coding wisdom transmission failed! 💻");
        }
    }

    public async Task PublishConfigUpdatedAsync(ApiConfig oldConfig, ApiConfig newConfig)
    {
        try
        {
            var configEvent = new ConfigurationUpdatedEvent
            {
                OldConfiguration = oldConfig,
                NewConfiguration = newConfig,
                UpdatedBy = "User",
                UpdatedAt = DateTime.UtcNow,
                ChangedFields = GetChangedFields(oldConfig, newConfig),
                UpdateReason = "Manual configuration update"
            };

            await _eventBus.PublishAsync(configEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish configuration update event - Orb memory sync failed! ⚙️");
        }
    }

    // Private helper methods

    private async Task EnsureInitializedAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        lock (_initializationLock)
        {
            if (_isInitialized)
            {
                return;
            }

            try
            {
                // Initialize OCR processor
                Task.Run(async () => await _ocrProcessor.InitializeAsync());

                // Load current configuration
                _currentConfig = Task.Run(async () => await _configManager.LoadConfigAsync()).Result;

                _isInitialized = true;
                
                _logger.LogInformation("AI Orb service initialization completed - Cosmic systems online! 🚀");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI Orb service initialization failed - Cosmic startup error! ⚠️");
                throw;
            }
        }
    }

    private async Task<bool> IsConfiguredAsync()
    {
        try
        {
            return await _configManager.IsConfiguredAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Configuration check failed - Orb status unknown! ❓");
            return false;
        }
    }

    private ChatContext EnhanceChatContext(ChatContext context)
    {
        // Add NetToolkit-specific context
        context.EnvironmentInfo["netToolkitVersion"] = "1.0";
        context.EnvironmentInfo["orbActivated"] = DateTime.UtcNow;
        context.EnvironmentInfo["availableModules"] = GetActiveFeatures();

        return context;
    }

    private async Task<string> GenerateAIAnalysisForOcr(string extractedText, OcrAnalysisType analysisType, TextAnalysisResult textAnalysis)
    {
        var analysisPrompt = $"""
            Analyze the following text extracted from a screenshot using OCR:

            Text: {extractedText}
            
            Analysis Type: {analysisType}
            
            Current Insights: {string.Join(", ", textAnalysis.Insights.Select(i => i.Title))}

            Provide a comprehensive analysis including:
            1. What this content appears to be
            2. Key networking or technical concepts identified
            3. Potential issues or concerns
            4. Actionable recommendations
            5. Educational insights for network engineers

            Keep the analysis practical and focused on actionable intelligence.
            """;

        return await _aiClient.ChatAsync(analysisPrompt);
    }

    private string DetermineCurrentMode()
    {
        if (!_isInitialized)
            return "Initializing";
        
        if (_currentConfig == null || string.IsNullOrEmpty(_currentConfig.ApiKey))
            return "Configuration Required";

        return "Active";
    }

    private async Task<Dictionary<string, object>> GetHealthMetricsAsync()
    {
        var metrics = new Dictionary<string, object>
        {
            ["lastConfigUpdate"] = _currentConfig != null ? DateTime.UtcNow : DateTime.MinValue,
            ["initializationStatus"] = _isInitialized,
            ["memoryUsage"] = GC.GetTotalMemory(false),
            ["uptime"] = DateTime.UtcNow - Process.GetCurrentProcess().StartTime
        };

        try
        {
            metrics["apiConnectivity"] = await TestApiConnectionAsync();
        }
        catch
        {
            metrics["apiConnectivity"] = false;
        }

        return metrics;
    }

    private List<string> GetActiveFeatures()
    {
        var features = new List<string> { "Chat", "OCR Analysis", "CLI Co-Pilot" };

        if (_isInitialized)
        {
            features.Add("Fully Initialized");
        }

        if (_currentConfig != null && !string.IsNullOrEmpty(_currentConfig.ApiKey))
        {
            features.Add("AI Connected");
        }

        return features;
    }

    private static int EstimateTokenUsage(string prompt, string response)
    {
        // Rough estimation: ~4 characters per token
        return (prompt.Length + response.Length) / 4;
    }

    private static List<string> ExtractKeywords(string text)
    {
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                       .Select(w => w.Trim().ToLowerInvariant())
                       .Where(w => w.Length > 4)
                       .GroupBy(w => w)
                       .Where(g => g.Count() > 1)
                       .Select(g => g.Key)
                       .Take(10);

        return words.ToList();
    }

    private static List<string> GetChangedFields(ApiConfig oldConfig, ApiConfig newConfig)
    {
        var changes = new List<string>();

        if (oldConfig.ApiKey != newConfig.ApiKey)
            changes.Add("ApiKey");
        
        if (oldConfig.BaseUrl != newConfig.BaseUrl)
            changes.Add("BaseUrl");
        
        if (oldConfig.Model != newConfig.Model)
            changes.Add("Model");
        
        if (oldConfig.MaxTokens != newConfig.MaxTokens)
            changes.Add("MaxTokens");
        
        if (Math.Abs(oldConfig.Temperature - newConfig.Temperature) > 0.01)
            changes.Add("Temperature");

        return changes;
    }
}