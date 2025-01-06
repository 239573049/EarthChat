using EarthChat.Gateway.Sdk.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EarthChat.Gateway.Sdk.Tunnels;

public class GatewayHostedService(IOptions<GatewayServiceOptions> options, ILogger<TunnelClient> logger,TunnelServer tunnelServer)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var serverClient = new TunnelClient(
                tunnelServer,
                options.Value.Address,
                options.Value.Service, logger);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await serverClient.TransportCoreAsync(options.Value, stoppingToken);
                }
                catch (Exception e)
                {
                    logger.LogError("出现异常，准备重新连接" + e);
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}