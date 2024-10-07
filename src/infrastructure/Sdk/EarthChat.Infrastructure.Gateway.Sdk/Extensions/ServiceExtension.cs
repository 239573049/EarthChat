using EarthChat.Infrastructure.Gateway.Sdk.Internal;
using EarthChat.Infrastructure.Gateway.Sdk.Internal.Client;
using EarthChat.Infrastructure.Gateway.Sdk.Options;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace EarthChat.Infrastructure.Gateway.Sdk;

public static class ServiceExtensions
{
    public static IServiceCollection AddGatewayService(this IServiceCollection services,
        Action<GatewayServiceOptions> configure)
    {
        services.Configure(configure);
        services.AddHostedService<GatewayRegistrationHostedService>();
        services.TryAddSingleton<IGatewayClient, GatewayClient>();
        services.AddHttpClient(Const.HttpClientName, (provider, client) =>
        {
            var options = provider.GetRequiredService<IOptions<GatewayServiceOptions>>().Value;
            client.BaseAddress = new Uri(options.Address);

            client.DefaultRequestHeaders.Add("Gateway-Key", options.Token);
            client.DefaultRequestHeaders.UserAgent.ParseAdd(options.Service);
            
        });

        return services;
    }
}