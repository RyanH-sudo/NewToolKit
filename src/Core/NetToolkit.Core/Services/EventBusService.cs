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

    public async Task PublishAsync<T>(T eventData, CancellationToken cancellationToken = default) where T : class
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
            tasks.Add(ExecuteHandlerSafely(handler, eventData, cancellationToken));
        }

        await Task.WhenAll(tasks);
    }

    public async Task SubscribeAsync<T>(Func<T, Task> handler) where T : class
    {
        Subscribe(handler);
        await Task.CompletedTask;
    }

    public async Task SubscribeAsync<T>(string eventName, Func<T, Task> handler) where T : class
    {
        // INFERRED: String-based event subscription using composite key
        var eventKey = $"{eventName}:{typeof(T).Name}";
        var eventType = typeof((string, Type));
        
        _handlers.AddOrUpdate(
            eventType,
            new ConcurrentBag<object> { (eventKey, handler) },
            (key, existing) =>
            {
                existing.Add((eventKey, handler));
                return existing;
            });
        
        _logger.LogDebug("Subscribed handler for event name {EventName} of type {EventType}", eventName, typeof(T).Name);
        await Task.CompletedTask;
    }

    public async Task UnsubscribeAsync<T>() where T : class
    {
        var eventType = typeof(T);
        
        if (_handlers.TryRemove(eventType, out var handlers))
        {
            _logger.LogDebug("Unsubscribed all handlers for event type {EventType}", eventType.Name);
        }
        
        await Task.CompletedTask;
    }

    public async Task UnsubscribeAsync<T>(string eventName, Func<T, Task> handler) where T : class
    {
        // INFERRED: String-based event unsubscription using composite key
        var eventKey = $"{eventName}:{typeof(T).Name}";
        var eventType = typeof((string, Type));
        
        if (_handlers.TryGetValue(eventType, out var handlers))
        {
            var filteredHandlers = handlers
                .Cast<(string, object)>()
                .Where(h => !(h.Item1.Equals(eventKey) && ReferenceEquals(h.Item2, handler)))
                .Cast<object>()
                .ToArray();
            
            _handlers[eventType] = new ConcurrentBag<object>(filteredHandlers);
            _logger.LogDebug("Unsubscribed handler for event name {EventName} of type {EventType}", eventName, typeof(T).Name);
        }
        
        await Task.CompletedTask;
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

    private async Task ExecuteHandlerSafely<T>(Func<T, Task> handler, T eventData, CancellationToken cancellationToken = default) where T : class
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