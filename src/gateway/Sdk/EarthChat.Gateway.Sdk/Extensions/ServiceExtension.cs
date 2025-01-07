using EarthChat.Gateway.Sdk.Options;
using EarthChat.Gateway.Sdk.Tunnels;

namespace EarthChat.Gateway.Sdk.Extensions;

public static class ServiceExtension
{
	public static IServiceCollection AddGatewayNode(this IServiceCollection services, IConfiguration configuration)
	{
		services.Configure<GatewayServiceOptions>(configuration.GetSection("Gateway"));
		services.AddHostedService<GatewayHostedService>();
		services.AddSingleton<TunnelServer>();
		return services;
	}

	public static IWebHostBuilder WithGatewayNode(this IWebHostBuilder builder)
	{
		builder.ConfigureKestrel(((context, options) =>
		{
			options.ConfigureEndpointDefaults((listenOptions =>
			{
				TunnelServer.SetUri(listenOptions.IPEndPoint);
			}));
		}));

		builder.ConfigureServices(((hostBuilder, services) => { services.AddGatewayNode(hostBuilder.Configuration); }));

		return builder;
	}
}