using EarthChat.Infrastructure.Gateway.Gateway;
using EarthChat.Infrastructure.Gateway.Node;
using EarthChat.Infrastructure.Gateway.Options;

namespace EarthChat.Infrastructure.Gateway;

public static class ServiceExtensions
{
    public static IServiceCollection AddGateway(this IServiceCollection services)
    {
        services.AddSingleton<GatewayMemory>();
        services.AddHttpClient()
            .ConfigureHttpClientDefaults(builder =>
            {
                builder.ConfigureHttpClient((provider, client) =>
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "EarthChat-Gateway");

                    client.Timeout = TimeSpan.FromSeconds(10);
                });
            });
        services.AddSingleton<NodeClientManager>();

        return services;
    }

    public static IServiceCollection AddGateway(this IServiceCollection services, IConfiguration configure)
    {
        services.Configure<GatewayOptions>(configure.GetSection("Gateway"));

        services.AddGateway();

        return services;
    }

    public static IEndpointRouteBuilder UseGatewayMiddleware(this IEndpointRouteBuilder app)
    {
        app.MapReverseProxy();
        return app;
    }
}