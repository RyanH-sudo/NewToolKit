using System.Collections.Concurrent;
using NetToolkit.Core.Interfaces;

namespace NetToolkit.Core.Services;

public class EventBusService : IEventBus
{
    private readonly ConcurrentDictionary<Type, ConcurrentBag<object>> _handlers = new();
    private readonly ILoggerWrapper _logger;

    public EventBusService(ILoggerWrapper logger)
    {
        _logger = logger;
    }

    public async Task PublishAsync<T>(T eventData) where T : class
    {
        var eventType = typeof(T);
        
        if (!_handlers.TryGetValue(eventType, out var handlers))
        {
            _logger.LogDebug("No handlers found for event type {EventType}", eventType.Name);
            return;
        }

        var tasks = new List<Task>();
        
        foreach (var handler in handlers.OfType<Func<T, Task>>())
        {
            tasks.Add(ExecuteHandlerSafely(handler, eventData));
        }

        await Task.WhenAll(tasks);
    }

    public void Subscribe<T>(Func<T, Task> handler) where T : class
    {
        var eventType = typeof(T);
        _handlers.AddOrUpdate(
            eventType,
            new ConcurrentBag<object> { handler },
            (key, existing) =>
            {
                existing.Add(handler);
                return existing;
            });
        
        _logger.LogDebug("Subscribed handler for event type {EventType}", eventType.Name);
    }

    public void Unsubscribe<T>(Func<T, Task> handler) where T : class
    {
        var eventType = typeof(T);
        
        if (_handlers.TryGetValue(eventType, out var handlers))
        {
            var newHandlers = handlers.Where(h => !ReferenceEquals(h, handler)).ToArray();
            _handlers[eventType] = new ConcurrentBag<object>(newHandlers);
            _logger.LogDebug("Unsubscribed handler for event type {EventType}", eventType.Name);
        }
    }

    private async Task ExecuteHandlerSafely<T>(Func<T, Task> handler, T eventData) where T : class
    {
        try
        {
            await handler(eventData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing event handler for {EventType}", typeof(T).Name);
        }
    }
}