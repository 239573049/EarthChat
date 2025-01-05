using EarthChat.BuildingBlocks.Event;
using EarthChat.Rabbit;
using EarthChat.Rabbit.Handler;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Token.RabbitMQEvent;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加RabbitMQ事件总线
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddRabbitMqEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        // 是否启用RabbitMQ
        var connection = configuration["RabbitMQ:ConnectionString"];
        if (string.IsNullOrWhiteSpace(connection))
        {
            return services;
        }

        services
            .AddSingleton(typeof(IEventBus<>), typeof(RabbitMqEventBus<>));

        services.AddScoped<IRabbitHandler, RabbitMQEventHandler>();

        // 设置消费并行度
        ushort.TryParse(configuration["RabbitMQ:FetchCount"] ?? "10", out var fetchCount);

        services.AddRabbitBoot((options =>
        {
            options.ConnectionString = connection;
            options.Consumes =
            [
                new ConsumeOptions
                {
                    AutoAck = false,
                    FetchCount = fetchCount,
                    Queue = "EarthChat:EventBus",
                    Declaration = (declaration =>
                    {
                        declaration.QueueDeclareAsync("EarthChat:EventBus", true);
                        declaration.ExchangeDeclareAsync("EarthChat:EventBus:exchange", ExchangeType.Direct, true);
                        declaration.QueueBindAsync("EarthChat:EventBus", "EarthChat:EventBus:exchange", "EarthChat:EventBus:key");
                    })
                }
            ];
        }));

        return services;
    }

    public static IServiceCollection AddRabbitMqJsonSerializer(this IServiceCollection services)
    {
        services.AddSingleton<IHandlerSerializer, JsonHandlerSerializer>();

        return services;
    }
}