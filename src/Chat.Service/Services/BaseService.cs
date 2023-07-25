using Microsoft.Extensions.Options;

namespace Chat.Service.Services;

public abstract class BaseService<T> : ServiceBase where T : class
{
    protected ILogger<T> _logger
        => GetRequiredService<ILogger<T>>();

    protected IEventBus _eventBus
        => GetRequiredService<IEventBus>();

    protected TOptions? GetOptions<TOptions>() where TOptions : class
    {
        return GetService<IOptions<TOptions>>()?.Value;
    }

    protected async Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent
    {
        await _eventBus.PublishAsync(@event);
    }
}