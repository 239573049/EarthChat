namespace Chat.Service.Services;

public abstract class BaseService<T> : ServiceBase where T : class
{
    protected ILogger<T> Logger
        => GetRequiredService<ILogger<T>>();

    protected IUserContext UserContext
        => GetRequiredService<IUserContext>();
    
    protected TOptions? GetOptions<TOptions>() where TOptions : class
    {
        return GetService<IOptions<TOptions>>()?.Value;
    }

    protected async Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent
    {
        await GetRequiredService<IEventBus>().PublishAsync(@event);
    }
}