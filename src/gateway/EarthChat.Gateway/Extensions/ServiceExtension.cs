using EarthChat.Infrastructure.Gateway.Options;
using EarthChat.Infrastructure.Gateway.Tunnels;
using Yarp.ReverseProxy.Forwarder;

// ReSharper disable All

namespace EarthChat.Infrastructure.Gateway;

public static class ServiceExtensions
{
    public static IServiceCollection AddGateway(this IServiceCollection services)
    {

        services.AddSingleton<YarpMemoryService>();

        services.AddSingleton<NodeMiddleware>();
        services.AddSingleton<TunnelMiddleware>();
        services.AddSingleton<GatewayClientManager>();
        services.AddSingleton<GatewayClientStateChannel>();
        services.AddSingleton<HttpTunnelFactory>();
        services.AddSingleton<TunnelClientFactory>();

        services.AddSingleton<IForwarderHttpClientFactory>((s) => s.GetRequiredService<TunnelClientFactory>());     
        
        return services;
    }

    public static IServiceCollection AddGateway(this IServiceCollection services, IConfiguration configure)
    {
        services.Configure<GatewayOptions>(configure.GetSection("Gateway"));

        services.AddGateway();

        return services;
    }

    public static WebApplication UseGatewayMiddleware(this WebApplication app)
    {
        app.MapReverseProxy();
        
        app.Map("/gateway/node",(builder =>
        {
            // 注意这里的顺序，先添加NodeMiddleware再添加TunnelMiddleware
            builder.UseMiddleware<NodeMiddleware>();
            builder.UseMiddleware<TunnelMiddleware>();
        }));
        
        return app;
    }
}