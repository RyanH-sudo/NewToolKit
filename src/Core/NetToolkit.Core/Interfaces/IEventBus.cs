namespace NetToolkit.Core.Interfaces;

public interface IEventBus
{
    Task PublishAsync<T>(T eventData) where T : class;
    void Subscribe<T>(Func<T, Task> handler) where T : class;
    void Unsubscribe<T>(Func<T, Task> handler) where T : class;
}