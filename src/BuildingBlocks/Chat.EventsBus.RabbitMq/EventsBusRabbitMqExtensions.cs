using Chat.EventsBus.Contract;
using Chat.EventsBus.RabbitMq;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class EventsBusRabbitMQExtensions
{
    public static IServiceCollection AddEventsBusRabbitMq(this IServiceCollection services,
        IConfiguration configuration, Action<EventsBusOptions>? options)
    {
        var eventsBusOptions = new EventsBusOptions();
        options?.Invoke(eventsBusOptions);
        
        services.AddSingleton(eventsBusOptions);
        services.AddSingleton<RabbitMqFactory>();
        services.AddSingleton(typeof(RabbitMqEventsManage<>));
        services.AddSingleton<IEventBus, RabbitMqEventBus>();
        services.Configure<RabbitOptions>(configuration.GetSection(nameof(RabbitOptions)));

        return services;
    }

    public static IServiceCollection AddEventsBusRabbitMq(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddEventsBusRabbitMq(configuration, null);
        return services;
    }
}