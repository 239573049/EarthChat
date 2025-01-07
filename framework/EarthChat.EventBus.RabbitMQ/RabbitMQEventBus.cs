using EarthChat.BuildingBlocks.Event;
using EarthChat.Rabbit;

namespace Token.RabbitMQEvent;

public class RabbitMqEventBus<TEvent>(RabbitClient rabbitClient, IHandlerSerializer handlerSerializer)
    : IEventBus<TEvent> where TEvent : class
{
    public async ValueTask PublishAsync(TEvent eventEvent)
    {
        ArgumentNullException.ThrowIfNull(eventEvent);

        // 分布式事件总线发布事件，由于事件总线是基于RabbitMQ实现的，所以需要将事件序列化为字节数组，并且将TEvent的FullName作为事件类型包装，消费者会解析这个类型，然后反序列化为TEvent
        var eto = new EventEto(eventEvent.GetType().FullName, handlerSerializer.Serialize(eventEvent));

        await rabbitClient.PublishAsync("EarthChat:EventBus:exchange", "EarthChat:EventBus:key",
            handlerSerializer.Serialize(eto));
    }
}