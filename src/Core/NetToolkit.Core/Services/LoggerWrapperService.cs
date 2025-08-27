using Microsoft.Extensions.Logging;
using NetToolkit.Core.Interfaces;

namespace NetToolkit.Core.Services;

public class LoggerWrapperService : ILoggerWrapper
{
    private readonly ILogger<LoggerWrapperService> _logger;

    public LoggerWrapperService(ILogger<LoggerWrapperService> logger)
    {
        _logger = logger;
    }

    public void LogDebug(string message, params object[] args)
    {
        _logger.LogDebug(message, args);
    }

    public void LogInfo(string message, params object[] args)
    {
        _logger.LogInformation(message, args);
    }

    public void LogWarn(string message, params object[] args)
    {
        _logger.LogWarning(message, args);
    }

    public void LogError(string message, params object[] args)
    {
        _logger.LogError(message, args);
    }

    public void LogError(Exception exception, string message, params object[] args)
    {
        _logger.LogError(exception, message, args);
    }
}