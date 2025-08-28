namespace NetToolkit.Core.Interfaces;

public interface IEventBus
{
    Task PublishAsync<T>(T eventData, CancellationToken cancellationToken = default) where T : class;
    Task SubscribeAsync<T>(Func<T, Task> handler) where T : class;
    Task SubscribeAsync<T>(string eventName, Func<T, Task> handler) where T : class; // INFERRED: String-based event subscription
    Task UnsubscribeAsync<T>() where T : class;
    Task UnsubscribeAsync<T>(string eventName, Func<T, Task> handler) where T : class; // INFERRED: String-based event unsubscription
    void Subscribe<T>(Func<T, Task> handler) where T : class;
    void Unsubscribe<T>(Func<T, Task> handler) where T : class;
}