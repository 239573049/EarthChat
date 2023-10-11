using System.Reflection;
using System.Text.Json;
using Chat.EventsBus.Contract;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chat.EventsBus.RabbitMq;

public class RabbitMqEventBus : IEventBus
{
    private readonly IServiceProvider _serviceProvider;
    private readonly RabbitMqFactory _rabbitMqFactory;
    private readonly ILogger<RabbitMqEventBus> _logger;

    public RabbitMqEventBus(IServiceProvider serviceProvider, RabbitMqFactory rabbitMqFactory,
        ILogger<RabbitMqEventBus> logger)
    {
        _serviceProvider = serviceProvider;
        _rabbitMqFactory = rabbitMqFactory;
        _logger = logger;
    }

    public async Task PushAsync<TEto>(TEto eto) where TEto : class
    {
        //创建一个通道
        //这里Rabbit的玩法就是一个通道channel下包含多个队列Queue
        using var channel = _rabbitMqFactory.CreateRabbitMq();

        // 获取Eto中的EventsBusAttribute特性，获取名称，如果没有默认使用类名称
        var eventBus = typeof(TEto).GetCustomAttribute<EventsBusAttribute>();
        var name = eventBus?.Name ?? typeof(TEto).Name;

        // 使用获取的名称创建一个通道
        channel.QueueDeclare(name, false, false, false, null);
        var properties = channel.CreateBasicProperties();
        properties.DeliveryMode = 1;

        var value = new BaseEto<TEto>()
        {
            Id = Guid.NewGuid().ToString("N"),
            Data = eto,
            Extend = new Dictionary<string, string> { { nameof(EventsBusAttribute), name } }
        };

        _logger.LogInformation("RabbitMq create message id:{0} name:{1} ", value.Id, name);

        // 将数据序列号，然后发布
        channel.BasicPublish("", name, false, properties, JsonSerializer.SerializeToUtf8Bytes(value)); //生产消息

        await Task.CompletedTask;
    }
}