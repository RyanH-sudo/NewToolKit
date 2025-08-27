using Microsoft.Extensions.Logging;
using NetToolkit.Modules.AiOrb.Interfaces;
using NetToolkit.Modules.AiOrb.Models;
using Newtonsoft.Json;
using Polly;
using Polly.Extensions.Http;
using System.Net.Http.Headers;
using System.Text;

namespace NetToolkit.Modules.AiOrb.Services;

/// <summary>
/// AI client service for communication with OpenAI and compatible APIs
/// The cosmic bridge between NetToolkit and AI consciousness
/// </summary>
public class AiClientService
{
    private readonly ILogger<AiClientService> _logger;
    private readonly IConfigManager _configManager;
    private readonly HttpClient _httpClient;
    private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy;
    private readonly Dictionary<string, object> _responseCache;
    private readonly SemaphoreSlim _rateLimitSemaphore;
    
    private ApiConfig? _currentConfig;
    private DateTime _lastConfigUpdate = DateTime.MinValue;
    private int _requestCount = 0;
    private DateTime _rateLimitWindow = DateTime.UtcNow;

    public AiClientService(ILogger<AiClientService> logger, IConfigManager configManager)
    {
        _logger = logger;
        _configManager = configManager;
        _responseCache = new Dictionary<string, object>();
        _rateLimitSemaphore = new SemaphoreSlim(1, 1);
        
        // Configure HTTP client with reasonable defaults
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };
        
        // Configure retry policy with exponential backoff
        _retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .Or<HttpRequestException>()
            .Or<TaskCanceledException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    _logger.LogWarning("AI API request retry {RetryCount} after {Delay}ms - Orb persistence activated! 🔄", 
                        retryCount, timespan.TotalMilliseconds);
                });
        
        _logger.LogInformation("AI client service initialized - Cosmic communication channels established! 🌌");
    }

    /// <summary>
    /// Send chat request to AI API with intelligent retry and caching
    /// </summary>
    public async Task<string> ChatAsync(string prompt, ChatContext? context = null)
    {
        try
        {
            await EnsureConfigurationAsync();
            
            if (_currentConfig == null)
            {
                return "Orb consciousness not configured - please set up your API keys first! 🔑";
            }

            // Check rate limiting
            await EnforceRateLimitAsync();
            
            // Generate cache key
            var cacheKey = GenerateCacheKey(prompt, context);
            
            // Check cache first if enabled
            if (_currentConfig.EnableCaching && _responseCache.ContainsKey(cacheKey))
            {
                _logger.LogDebug("Cache hit for prompt - Orb remembers this wisdom! 💾");
                return _responseCache[cacheKey].ToString() ?? string.Empty;
            }

            // Prepare the request
            var request = await PrepareChatRequestAsync(prompt, context);
            
            // Execute request with retry policy
            var response = await _retryPolicy.ExecuteAsync(async () => 
            {
                var httpResponse = await _httpClient.PostAsync(
                    $"{_currentConfig.BaseUrl.TrimEnd('/')}/v1/chat/completions",
                    new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
                );
                
                if (!httpResponse.IsSuccessStatusCode)
                {
                    var errorContent = await httpResponse.Content.ReadAsStringAsync();
                    _logger.LogWarning("AI API request failed with status {StatusCode} - Cosmic interference detected! ⚡ Error: {Error}", 
                        httpResponse.StatusCode, errorContent);
                }
                
                return httpResponse;
            });

            // Process response
            var responseContent = await response.Content.ReadAsStringAsync();
            var aiResponse = JsonConvert.DeserializeObject<OpenAiChatResponse>(responseContent);
            
            if (aiResponse?.Choices == null || !aiResponse.Choices.Any())
            {
                return "Orb received empty response from cosmic API - try again with different cosmic alignment! 🌙";
            }

            var result = aiResponse.Choices.First().Message.Content;
            
            // Cache the response if enabled
            if (_currentConfig.EnableCaching && !string.IsNullOrWhiteSpace(result))
            {
                _responseCache[cacheKey] = result;
                
                // Prevent cache from growing too large
                if (_responseCache.Count > 100)
                {
                    var oldestKey = _responseCache.Keys.First();
                    _responseCache.Remove(oldestKey);
                }
            }

            _logger.LogInformation("AI chat completed successfully - Orb wisdom delivered! ✨ Tokens: {Tokens}", 
                aiResponse.Usage?.TotalTokens ?? 0);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI chat request failed - Orb temporarily speechless! 😶 Prompt: {Prompt}", 
                prompt.Length > 50 ? prompt.Substring(0, 50) + "..." : prompt);
            return $"Orb encountered cosmic turbulence: {ex.Message}. Try again when the digital stars align! ⭐";
        }
    }

    /// <summary>
    /// Generate code analysis and suggestions based on terminal output
    /// </summary>
    public async Task<CodeSuggestion> AnalyzeCodeAsync(string code, CliContext? context = null)
    {
        try
        {
            var analysisPrompt = BuildCodeAnalysisPrompt(code, context);
            var response = await ChatAsync(analysisPrompt);
            
            return ParseCodeSuggestionFromResponse(response, code);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Code analysis failed - Orb coding circuits overloaded! 💻");
            return new CodeSuggestion
            {
                Title = "Analysis Failed",
                Description = $"Orb could not analyze the code: {ex.Message}",
                Type = SuggestionType.Fix,
                ConfidenceLevel = 0
            };
        }
    }

    /// <summary>
    /// Analyze text for networking insights
    /// </summary>
    public async Task<List<NetworkingInsight>> AnalyzeNetworkingContentAsync(string content)
    {
        try
        {
            var analysisPrompt = BuildNetworkingAnalysisPrompt(content);
            var response = await ChatAsync(analysisPrompt);
            
            return ParseNetworkingInsightsFromResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Networking content analysis failed - Orb network sensors confused! 🌐");
            return new List<NetworkingInsight>
            {
                new()
                {
                    Title = "Analysis Error",
                    Description = $"Could not analyze networking content: {ex.Message}",
                    Category = "Error",
                    Priority = 1
                }
            };
        }
    }

    /// <summary>
    /// Test API connection and get basic information
    /// </summary>
    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            await EnsureConfigurationAsync();
            
            if (_currentConfig == null)
            {
                return false;
            }

            var testPrompt = "Respond with 'Connection successful' if you can read this message.";
            var response = await ChatAsync(testPrompt);
            
            var isConnected = !string.IsNullOrWhiteSpace(response) && 
                             !response.Contains("not configured") && 
                             !response.Contains("cosmic turbulence");
            
            _logger.LogInformation("API connection test completed - Cosmic link status: {Status} 📡", 
                isConnected ? "ACTIVE" : "FAILED");
            
            return isConnected;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API connection test failed - Orb communication disrupted! 📵");
            return false;
        }
    }

    /// <summary>
    /// Ensure current configuration is loaded and up-to-date
    /// </summary>
    private async Task EnsureConfigurationAsync()
    {
        if (_currentConfig == null || DateTime.UtcNow - _lastConfigUpdate > TimeSpan.FromMinutes(5))
        {
            _currentConfig = await _configManager.LoadConfigAsync();
            _lastConfigUpdate = DateTime.UtcNow;
            
            // Update HTTP client headers
            _httpClient.DefaultRequestHeaders.Clear();
            
            if (!string.IsNullOrWhiteSpace(_currentConfig.ApiKey))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", _currentConfig.ApiKey);
            }

            foreach (var header in _currentConfig.CustomHeaders)
            {
                _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            _httpClient.Timeout = TimeSpan.FromSeconds(_currentConfig.TimeoutSeconds);
            
            _logger.LogDebug("Configuration refreshed - Orb cosmic settings updated! ⚙️");
        }
    }

    /// <summary>
    /// Enforce rate limiting based on configuration
    /// </summary>
    private async Task EnforceRateLimitAsync()
    {
        if (_currentConfig == null) return;

        await _rateLimitSemaphore.WaitAsync();
        try
        {
            var now = DateTime.UtcNow;
            
            // Reset counter if we're in a new minute window
            if (now - _rateLimitWindow >= TimeSpan.FromMinutes(1))
            {
                _requestCount = 0;
                _rateLimitWindow = now;
            }

            // Check if we've exceeded the rate limit
            if (_requestCount >= _currentConfig.RateLimitPerMinute)
            {
                var waitTime = TimeSpan.FromMinutes(1) - (now - _rateLimitWindow);
                _logger.LogWarning("Rate limit reached - Orb taking a brief meditation break for {WaitTime}ms 🧘‍♂️", 
                    waitTime.TotalMilliseconds);
                await Task.Delay(waitTime);
                
                // Reset after waiting
                _requestCount = 0;
                _rateLimitWindow = DateTime.UtcNow;
            }

            _requestCount++;
        }
        finally
        {
            _rateLimitSemaphore.Release();
        }
    }

    /// <summary>
    /// Generate cache key for request caching
    /// </summary>
    private static string GenerateCacheKey(string prompt, ChatContext? context)
    {
        var keyData = $"{prompt}|{context?.UserRole}|{context?.CurrentModule}";
        var keyBytes = Encoding.UTF8.GetBytes(keyData);
        var hashBytes = System.Security.Cryptography.SHA256.HashData(keyBytes);
        return Convert.ToHexString(hashBytes);
    }

    /// <summary>
    /// Prepare chat request payload for OpenAI API
    /// </summary>
    private async Task<object> PrepareChatRequestAsync(string prompt, ChatContext? context)
    {
        var messages = new List<object>
        {
            new { role = "system", content = GetSystemPrompt(context) }
        };

        // Add conversation history if available
        if (context?.ConversationHistory != null && context.ConversationHistory.Any())
        {
            foreach (var msg in context.ConversationHistory.TakeLast(10)) // Limit history to last 10 messages
            {
                messages.Add(new { role = msg.Role, content = msg.Content });
            }
        }

        // Add current user message
        messages.Add(new { role = "user", content = prompt });

        var request = new
        {
            model = _currentConfig!.Model,
            messages = messages,
            max_tokens = _currentConfig.MaxTokens,
            temperature = _currentConfig.Temperature,
            top_p = _currentConfig.ProviderSpecificSettings.GetValueOrDefault("top_p", 1.0),
            frequency_penalty = _currentConfig.ProviderSpecificSettings.GetValueOrDefault("frequency_penalty", 0.0),
            presence_penalty = _currentConfig.ProviderSpecificSettings.GetValueOrDefault("presence_penalty", 0.0)
        };

        return request;
    }

    /// <summary>
    /// Get system prompt based on context
    /// </summary>
    private static string GetSystemPrompt(ChatContext? context)
    {
        var basePrompt = """
            You are Claude, the AI assistant integrated into NetToolkit - a premium network engineering toolkit. 
            You are witty, helpful, and have deep expertise in networking, security, and system administration.
            Provide practical, actionable advice with a touch of humor. Always explain complex concepts clearly.
            Your responses should be professional but engaging, using occasional cosmic and network-themed metaphors.
            """;

        if (context?.UserRole == "NetworkEngineer")
        {
            basePrompt += " The user is an experienced network engineer, so you can use technical terminology appropriately.";
        }

        if (!string.IsNullOrEmpty(context?.CurrentModule))
        {
            basePrompt += $" The user is currently working with the {context.CurrentModule} module.";
        }

        return basePrompt;
    }

    /// <summary>
    /// Build code analysis prompt
    /// </summary>
    private static string BuildCodeAnalysisPrompt(string code, CliContext? context)
    {
        var prompt = new StringBuilder();
        prompt.AppendLine("Analyze the following code/output and provide suggestions for improvement, fixes, or alternatives:");
        prompt.AppendLine();
        prompt.AppendLine("```");
        prompt.AppendLine(code);
        prompt.AppendLine("```");
        
        if (context != null)
        {
            prompt.AppendLine();
            prompt.AppendLine($"Context: Shell: {context.Shell}, Directory: {context.CurrentDirectory}");
            if (context.ExitCode != 0)
            {
                prompt.AppendLine($"Exit Code: {context.ExitCode} (indicates an error occurred)");
            }
        }

        prompt.AppendLine();
        prompt.AppendLine("Please provide specific, actionable suggestions in a clear, structured format.");
        
        return prompt.ToString();
    }

    /// <summary>
    /// Build networking analysis prompt
    /// </summary>
    private static string BuildNetworkingAnalysisPrompt(string content)
    {
        return $"""
            Analyze the following networking-related content and provide insights:

            {content}

            Please identify:
            1. Key networking concepts mentioned
            2. Potential issues or concerns
            3. Optimization opportunities
            4. Educational insights for network engineers
            5. Action items or recommendations

            Format your response as structured insights with clear categories and priorities.
            """;
    }

    /// <summary>
    /// Parse code suggestion from AI response
    /// </summary>
    private static CodeSuggestion ParseCodeSuggestionFromResponse(string response, string originalCode)
    {
        // This would typically use more sophisticated parsing
        // For now, we'll create a basic suggestion structure
        return new CodeSuggestion
        {
            Title = "AI Analysis Result",
            Description = response,
            Code = ExtractCodeFromResponse(response) ?? originalCode,
            Language = DetectLanguage(originalCode),
            Type = DetectSuggestionType(response),
            ConfidenceLevel = 75, // Default confidence
            ExpectedOutcome = "Improved code based on AI analysis"
        };
    }

    /// <summary>
    /// Parse networking insights from AI response
    /// </summary>
    private static List<NetworkingInsight> ParseNetworkingInsightsFromResponse(string response)
    {
        // This would typically use more sophisticated parsing
        var insights = new List<NetworkingInsight>
        {
            new()
            {
                Title = "AI Analysis",
                Description = response,
                Category = "General",
                Priority = 2
            }
        };

        return insights;
    }

    /// <summary>
    /// Extract code blocks from AI response
    /// </summary>
    private static string? ExtractCodeFromResponse(string response)
    {
        // Look for code blocks in markdown format
        var codeBlockStart = response.IndexOf("```", StringComparison.Ordinal);
        if (codeBlockStart == -1) return null;

        var codeStart = response.IndexOf('\n', codeBlockStart) + 1;
        var codeBlockEnd = response.IndexOf("```", codeStart, StringComparison.Ordinal);
        
        if (codeBlockEnd == -1) return null;

        return response.Substring(codeStart, codeBlockEnd - codeStart).Trim();
    }

    /// <summary>
    /// Detect programming language from code
    /// </summary>
    private static string DetectLanguage(string code)
    {
        if (code.Contains("Import-Module") || code.Contains("Get-") || code.Contains("Set-"))
            return "powershell";
        if (code.Contains("#!/bin/bash") || code.Contains("sudo ") || code.Contains("grep "))
            return "bash";
        if (code.Contains("using ") || code.Contains("namespace "))
            return "csharp";
        
        return "text";
    }

    /// <summary>
    /// Detect suggestion type from AI response
    /// </summary>
    private static SuggestionType DetectSuggestionType(string response)
    {
        var lowerResponse = response.ToLowerInvariant();
        
        if (lowerResponse.Contains("fix") || lowerResponse.Contains("error") || lowerResponse.Contains("bug"))
            return SuggestionType.Fix;
        if (lowerResponse.Contains("optimize") || lowerResponse.Contains("improve") || lowerResponse.Contains("performance"))
            return SuggestionType.Optimization;
        if (lowerResponse.Contains("automate") || lowerResponse.Contains("script"))
            return SuggestionType.Automation;
        
        return SuggestionType.Enhancement;
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
        _rateLimitSemaphore?.Dispose();
        _logger.LogInformation("AI client service disposed - Orb consciousness archived! 💤");
    }
}

/// <summary>
/// OpenAI API response structure
/// </summary>
internal class OpenAiChatResponse
{
    public List<OpenAiChoice> Choices { get; set; } = new();
    public OpenAiUsage? Usage { get; set; }
}

internal class OpenAiChoice
{
    public OpenAiMessage Message { get; set; } = new();
    public string FinishReason { get; set; } = string.Empty;
}

internal class OpenAiMessage
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

internal class OpenAiUsage
{
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public int TotalTokens { get; set; }
}