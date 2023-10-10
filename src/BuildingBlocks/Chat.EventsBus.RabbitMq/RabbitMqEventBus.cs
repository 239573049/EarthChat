using System.Reflection;
using System.Text.Json;
using Chat.EventsBus.Contract;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.EventsBus.RabbitMq;

public class RabbitMqEventBus: IEventBus
{
    private readonly IServiceProvider _serviceProvider;
    private readonly RabbitMqFactory _rabbitMqFactory;

    public RabbitMqEventBus(IServiceProvider serviceProvider, RabbitMqFactory rabbitMqFactory)
    {
        _serviceProvider = serviceProvider;
        _rabbitMqFactory = rabbitMqFactory;
    }

    public async Task PushAsync<TEto>(TEto eto) where TEto : class
    {

        //创建一个通道
        //这里Rabbit的玩法就是一个通道channel下包含多个队列Queue
        using var channel = _rabbitMqFactory.CreateRabbitMQ();
        
        // 获取Eto中的EventsBusAttribute特性，获取名称，如果没有默认使用类名称
        var eventBus = typeof(TEto).GetCustomAttribute<EventsBusAttribute>();
        var name = eventBus?.Name ?? typeof(TEto).Name;
        
        // 使用获取的名称创建一个通道
        channel.QueueDeclare(name, false, false, false, null);
        var properties = channel.CreateBasicProperties();
        properties.DeliveryMode = 1;
        // 将数据序列号，然后发布
        channel.BasicPublish("", name, false, properties, JsonSerializer.SerializeToUtf8Bytes(eto)); //生产消息
        // 让其注入启动管理服务，RabbitMQEventsManage需要手动激活，由于RabbitMQEventsManage是单例，只有第一次激活才有效，
        var eventsManage = _serviceProvider.GetService<RabbitMqEventsManage<TEto>>();
        
        await Task.CompletedTask;
    }
}