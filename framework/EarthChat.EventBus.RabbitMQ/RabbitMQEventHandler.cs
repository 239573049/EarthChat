using System.Collections.Concurrent;
using EarthChat.BuildingBlocks.Event;
using EarthChat.Rabbit;
using EarthChat.Rabbit.Handler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;

namespace Token.RabbitMQEvent;

public class RabbitMQEventHandler(ILogger<RabbitMQEventHandler> logger, IHandlerSerializer handlerSerializer)
    : IRabbitHandler
{
    private readonly ConcurrentDictionary<string, Type> _types = new();

    public bool Enable(ConsumeOptions options)
    {
        return true;
    }

    public async Task Handle(IServiceProvider sp, BasicDeliverEventArgs args, ConsumeOptions options)
    {
        var eto = handlerSerializer.Deserialize<EventEto>(args.Body);

        var type = _types.GetOrAdd(eto.FullName, ((s) =>
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => x.GetType(eto.FullName) != null)
                .Select(x => x).FirstOrDefault();

            var type = assembly?.GetType(eto.FullName);

            return type;
        }));

        if (type == null)
        {
            logger.LogWarning($"Event type {eto.FullName} not found");
            return;
        }

        var @event = handlerSerializer.Deserialize(eto.Data, type);

        var handlerType = typeof(IEventHandler<>).MakeGenericType(type);

        var handler = sp.GetRequiredService(handlerType);

        // Handler method
        var method = handlerType.GetMethod("HandleAsync");

        logger.LogInformation($"Receive event {type.Name}");

        await (Task)method.Invoke(handler, [@event]);
    }
}