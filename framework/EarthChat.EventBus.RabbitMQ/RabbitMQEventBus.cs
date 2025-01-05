using EarthChat.BuildingBlocks.Event;
using EarthChat.Rabbit;

namespace Token.RabbitMQEvent;

public class RabbitMqEventBus<TEvent>(RabbitClient rabbitClient, IHandlerSerializer handlerSerializer)
    : IEventBus<TEvent> where TEvent : class
{
    public async ValueTask PublishAsync(TEvent eventEvent)
    {
        ArgumentNullException.ThrowIfNull(eventEvent);

        var eto = new EventEto(eventEvent.GetType().FullName, handlerSerializer.Serialize(eventEvent));

        await rabbitClient.PublishAsync("EarthChat:EventBus:exchange", "EarthChat:EventBus:key",
            handlerSerializer.Serialize(eto));
    }
}