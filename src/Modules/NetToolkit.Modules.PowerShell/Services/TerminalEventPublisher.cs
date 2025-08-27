using NetToolkit.Modules.PowerShell.Interfaces;
using NetToolkit.Core.Interfaces;

namespace NetToolkit.Modules.PowerShell.Services;

/// <summary>
/// Terminal Event Publisher - Broadcasting the gospel of PowerShell across the digital realm!
/// The town crier of terminal activities, spreading news of victories and defeats with witty flair.
/// </summary>
public class TerminalEventPublisher : ITerminalEventPublisher
{
    private readonly IEventBus _eventBus;
    private readonly ILoggerWrapper _logger;
    
    public TerminalEventPublisher(IEventBus eventBus, ILoggerWrapper logger)
    {
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _logger.LogInfo("Terminal Event Publisher initialized - The digital town crier is ready! 📢");
    }
    
    /// <summary>
    /// Publish script execution started event - let the world know we're cooking
    /// </summary>
    public async Task PublishScriptStartedAsync(ScriptExecutionStartedEvent eventData)
    {
        try
        {
            await _eventBus.PublishAsync(eventData);
            
            _logger.LogInfo("🚀 Script execution started: '{ScriptName}' - {RiskLevel} risk level", 
                           eventData.ScriptName, 
                           GetRiskEmoji(eventData.RiskLevel));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish script started event - The town crier lost their voice!");
        }
    }
    
    /// <summary>
    /// Publish script execution completed event - victory or defeat, we tell all
    /// </summary>
    public async Task PublishScriptCompletedAsync(ScriptExecutionCompletedEvent eventData)
    {
        try
        {
            await _eventBus.PublishAsync(eventData);
            
            var statusEmoji = eventData.Success ? "✅" : "❌";
            var wittyMessage = eventData.GetWittyStatusMessage();
            
            _logger.LogInfo("{StatusEmoji} Script execution completed: '{ScriptName}' in {Duration}ms - {WittyMessage}", 
                           statusEmoji,
                           eventData.ScriptName, 
                           eventData.Duration.TotalMilliseconds,
                           wittyMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish script completed event - The victory horn is broken!");
        }
    }
    
    /// <summary>
    /// Publish output chunk received - real-time streaming for the impatient
    /// </summary>
    public async Task PublishOutputReceivedAsync(OutputReceivedEvent eventData)
    {
        try
        {
            await _eventBus.PublishAsync(eventData);
            
            // Only log significant outputs to avoid spam
            if (eventData.OutputType == PowerShellOutputType.Error || 
                eventData.OutputType == PowerShellOutputType.Warning ||
                (eventData.OutputType == PowerShellOutputType.Information && eventData.Content.Length > 50))
            {
                var typeEmoji = GetOutputTypeEmoji(eventData.OutputType);
                _logger.LogDebug("{TypeEmoji} Output received ({Type}): {Content}", 
                               typeEmoji, 
                               eventData.OutputType, 
                               TruncateContent(eventData.Content, 100));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish output received event - The message was lost in the digital void!");
        }
    }
    
    /// <summary>
    /// Publish error occurred event - when things go boom, everyone should know
    /// </summary>
    public async Task PublishErrorOccurredAsync(ErrorOccurredEvent eventData)
    {
        try
        {
            await _eventBus.PublishAsync(eventData);
            
            var severityEmoji = GetSeverityEmoji(eventData.Severity);
            var wittyMessage = eventData.GetWittyErrorMessage();
            
            _logger.LogError("{SeverityEmoji} Error occurred ({Severity}): {WittyMessage} - Details: {ErrorMessage}", 
                           severityEmoji,
                           eventData.Severity,
                           wittyMessage,
                           TruncateContent(eventData.ErrorMessage, 200));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish error event - Error reporting has errored! The irony is thick!");
        }
    }
    
    /// <summary>
    /// Publish SSH mode changed event - for when we switch dimensions
    /// </summary>
    public async Task PublishSshModeChangedAsync(SshModeChangedEvent eventData)
    {
        try
        {
            await _eventBus.PublishAsync(eventData);
            
            var statusEmoji = eventData.ConnectionSuccessful ? "🌐" : "🚫";
            var action = eventData.Enabled ? "enabled" : "disabled";
            var location = eventData.Enabled ? $" to {eventData.RemoteHost}:{eventData.RemotePort}" : " - returned to local realm";
            
            _logger.LogInfo("{StatusEmoji} SSH mode {Action}{Location} - {Status}", 
                           statusEmoji,
                           action,
                           location,
                           eventData.ConnectionSuccessful ? "Portal established!" : "Dimensional instability detected!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish SSH mode changed event - The dimensional broadcaster is offline!");
        }
    }
    
    /// <summary>
    /// Publish template generated event - when magic happens
    /// </summary>
    public async Task PublishTemplateGeneratedAsync(TemplateGeneratedEvent eventData)
    {
        try
        {
            await _eventBus.PublishAsync(eventData);
            
            _logger.LogInfo("⚗️ Template generated: '{TemplateName}' in {GenerationTime}ms - Digital alchemy complete! Risk: {RiskLevel}", 
                           eventData.TemplateName,
                           eventData.GenerationTime.TotalMilliseconds,
                           GetRiskEmoji(eventData.RiskLevel));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish template generated event - The alchemical broadcaster is broken!");
        }
    }
    
    /// <summary>
    /// Get emoji for risk level - because visual indicators are better than text
    /// </summary>
    private static string GetRiskEmoji(TemplateRisk riskLevel)
    {
        return riskLevel switch
        {
            TemplateRisk.Low => "🟢 Low",
            TemplateRisk.Medium => "🟡 Medium", 
            TemplateRisk.High => "🟠 High",
            TemplateRisk.Critical => "🔴 Critical",
            _ => "⚪ Unknown"
        };
    }
    
    /// <summary>
    /// Get emoji for output type - adding visual flair to terminal streams
    /// </summary>
    private static string GetOutputTypeEmoji(PowerShellOutputType outputType)
    {
        return outputType switch
        {
            PowerShellOutputType.Information => "ℹ️",
            PowerShellOutputType.Warning => "⚠️",
            PowerShellOutputType.Error => "❌",
            PowerShellOutputType.Debug => "🐛",
            PowerShellOutputType.Verbose => "📝",
            PowerShellOutputType.Progress => "⏳",
            PowerShellOutputType.Command => "⚡",
            _ => "📄"
        };
    }
    
    /// <summary>
    /// Get emoji for error severity - because even errors deserve proper categorization
    /// </summary>
    private static string GetSeverityEmoji(ErrorSeverity severity)
    {
        return severity switch
        {
            ErrorSeverity.Info => "💡",
            ErrorSeverity.Warning => "⚠️",
            ErrorSeverity.Error => "❌",
            ErrorSeverity.Critical => "💥",
            ErrorSeverity.Fatal => "☠️",
            _ => "❓"
        };
    }
    
    /// <summary>
    /// Truncate content for logging - because sometimes less is more
    /// </summary>
    private static string TruncateContent(string content, int maxLength)
    {
        if (string.IsNullOrEmpty(content) || content.Length <= maxLength)
            return content;
        
        return content[..(maxLength - 3)] + "...";
    }
}