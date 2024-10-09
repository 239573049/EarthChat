using EarthChat.Gateway.Sdk.Internal;
using EarthChat.Gateway.Sdk.Internal.Client;
using EarthChat.Gateway.Sdk.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace EarthChat.Gateway.Sdk;

public static class ServiceExtensions
{
    public static IServiceCollection AddGatewayService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GatewayServiceOptions>(configuration.GetSection("Gateway"));
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