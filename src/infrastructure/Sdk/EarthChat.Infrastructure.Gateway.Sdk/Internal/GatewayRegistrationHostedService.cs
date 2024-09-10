using EarthChat.Infrastructure.Gateway.Sdk.Internal.Client;

namespace EarthChat.Infrastructure.Gateway.Sdk.Internal;

public class GatewayRegistrationHostedService : IHostedService
{
    private readonly IGatewayClient _gatewayClient;

    public GatewayRegistrationHostedService(IGatewayClient gatewayClient)
    {
        _gatewayClient = gatewayClient;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return _gatewayClient.RegisterAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _gatewayClient.ServiceDeregisterAsync(cancellationToken);
    }
}