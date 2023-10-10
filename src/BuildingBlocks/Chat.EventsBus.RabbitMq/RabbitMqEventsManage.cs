using System.Reflection;
using System.Text.Json;
using Chat.EventsBus.Contract;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Chat.EventsBus.RabbitMq;


public class RabbitMqEventsManage<TEto> where TEto : class
{
    private readonly IServiceProvider _serviceProvider;
    private readonly RabbitMqFactory _rabbitMqFactory;
    private readonly EventsBusOptions _eventsBusOptions;

    public RabbitMqEventsManage(IServiceProvider serviceProvider, RabbitMqFactory rabbitMqFactory, EventsBusOptions eventsBusOptions)
    {
        _serviceProvider = serviceProvider;
        _rabbitMqFactory = rabbitMqFactory;
        _eventsBusOptions = eventsBusOptions;
        _ = Task.Run(Start);
    }

    private void Start()
    {
        var channel = _rabbitMqFactory.CreateRabbitMQ();
        var eventBus = typeof(TEto).GetCustomAttribute<EventsBusAttribute>();
        var name = eventBus?.Name ?? typeof(TEto).Name;
        channel.QueueDeclare(name, false, false, false, null);
        var consumer = new EventingBasicConsumer(channel); //消费者
        channel.BasicConsume(name, true, consumer); //消费消息
        consumer.Received += async (model, ea) =>
        {
            var bytes = ea.Body.ToArray();
            try
            {
                // 这样就可以实现多个订阅
                var events = _serviceProvider.GetServices<IEventsBusHandle<TEto>>();
                foreach (var handle in events)
                {
                    await handle?.HandleAsync(JsonSerializer.Deserialize<TEto>(bytes));
                }
            }
            catch (Exception e)
            {
                // EventsBusOptions.ReceiveExceptionEvent?.Invoke(_serviceProvider, e, bytes);
            }
        };
    }
}