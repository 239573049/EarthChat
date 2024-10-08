using EarthChat.Infrastructure.Gateway.Sdk.Internal.Client;

namespace EarthChat.Infrastructure.Gateway.Sdk.Internal;

public class GatewayRegistrationHostedService : BackgroundService
{
    private readonly IGatewayClient _gatewayClient;

    public GatewayRegistrationHostedService(IGatewayClient gatewayClient)
    {
        _gatewayClient = gatewayClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _gatewayClient.RegisterAsync(stoppingToken);
    }
}