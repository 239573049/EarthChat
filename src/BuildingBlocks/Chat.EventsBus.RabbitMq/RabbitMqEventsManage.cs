using System.Reflection;
using System.Text.Json;
using Chat.EventsBus.Contract;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Chat.EventsBus.RabbitMq;

public class RabbitMqEventsManage<TEto> where TEto : class
{
    private readonly IServiceProvider _serviceProvider;
    private readonly RabbitMqFactory _rabbitMqFactory;
    private readonly EventsBusOptions _eventsBusOptions;
    private readonly ILogger<RabbitMqEventsManage<TEto>> _logger;

    public RabbitMqEventsManage(IServiceProvider serviceProvider, RabbitMqFactory rabbitMqFactory,
        EventsBusOptions eventsBusOptions, ILogger<RabbitMqEventsManage<TEto>> logger)
    {
        _serviceProvider = serviceProvider;
        _rabbitMqFactory = rabbitMqFactory;
        _eventsBusOptions = eventsBusOptions;
        _logger = logger;
        _ = Task.Run(Start);
    }

    private void Start()
    {
        var channel = _rabbitMqFactory.CreateRabbitMq();
        var eventBus = typeof(TEto).GetCustomAttribute<EventsBusAttribute>();
        var name = eventBus?.Name ?? typeof(TEto).Name;
        channel.QueueDeclare(name, false, false, false, null);
        var consumer = new EventingBasicConsumer(channel); //消费者
        channel.BasicConsume(name, true, consumer); //消费消息
        consumer.Received += async (model, ea) =>
        {
            for (var i = 0; i < _eventsBusOptions.Retry; i++)
            {
                var bytes = ea.Body.ToArray();
                try
                {
                    // 这样就可以实现多个订阅
                    var events = _serviceProvider.GetServices<IEventsBusHandle<TEto>>();

                    var result = JsonSerializer.Deserialize<BaseEto<TEto>>(bytes);

                    _logger.LogInformation("消费消息 message id:{0} ", result?.Id);

                    foreach (var handle in events)
                    {
                        await handle?.HandleAsync(result?.Data);
                    }

                    break;
                }
                catch (Exception e)
                {
                    _logger.LogError($"消费异常，进行重试，当前次数：{i + 1}; {e}");
                }
            }
        };
    }
}