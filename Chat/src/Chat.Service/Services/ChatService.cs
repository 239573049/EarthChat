namespace Chat.Service.Services;

public abstract class ChatService<T> : ServiceBase where T : class
{
    protected ILogger<T> _logger => GetRequiredService<ILogger<T>>();
    
    protected IEventBus _eventBus => GetRequiredService<IEventBus>();
    
    protected async Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent
    {
        await _eventBus.PublishAsync(@event);
    }
    
}