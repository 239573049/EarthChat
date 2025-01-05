using EarthChat.Rabbit.Handler;
using Microsoft.Extensions.Options;

namespace EarthChat.Rabbit;

public static class ServiceExtensions
{
    /// <summary>
    /// 注册rabbit Handler
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddRabbitBoot(this IServiceCollection services, Action<RabbitOptions> options)
    {
        services.AddRabbitClient<RabbitClientBus>(options);

        services.AddHostedService<RabbitBoot>();
        services.AddScoped<IRabbitEventBus, RabbitEventBus>();

        return services;
    }

    /// <summary>
    /// 注册rabbit客户端
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddRabbitClient<T>(this IServiceCollection services, Action<RabbitOptions> options)
        where T : RabbitClient
    {
        var name = typeof(T).Name;
        services.Configure(name, options);

        services.AddSingleton<T>(sp =>
        {
            var log = sp.GetRequiredService<ILogger<T>>();
            var opt = sp.GetRequiredService<IOptionsMonitor<RabbitOptions>>();
            return ActivatorUtilities.CreateInstance<T>(sp, log, opt.Get(name));
        });
        services.AddSingleton<RabbitClient>(sp => sp.GetService<T>());

        return services;
    }
}