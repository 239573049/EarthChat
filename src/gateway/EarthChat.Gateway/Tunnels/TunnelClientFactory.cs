using System.Collections.Concurrent;
using System.Net.Sockets;
using Yarp.ReverseProxy.Forwarder;

namespace EarthChat.Infrastructure.Gateway.Tunnels;

/// <summary>
/// The factory that YARP will use the create outbound connections by host name.
/// </summary>
internal sealed class TunnelClientFactory(GatewayClientManager gatewayClientManager, HttpTunnelFactory httpTunnelFactory)
    : ForwarderHttpClientFactory
{
    /// <summary>
    /// 随机获取一个GatewayClient
    /// </summary>
    /// <param name="streams"></param>
    private static GatewayClient GetGatewayClient(ConcurrentDictionary<string, GatewayClient> streams)
    {
        var index = new Random().Next(0, streams.Count);

        return streams.ElementAt(index).Value;
    }

    /// <summary>
    /// yarp的网关客户端工厂，当yarp需要创建一个新的请求时，会调用此方法。
    /// </summary>
    /// <param name="context"></param>
    /// <param name="handler"></param>
    protected override void ConfigureHandler(ForwarderHttpClientContext context, SocketsHttpHandler handler)
    {
        base.ConfigureHandler(context, handler);

        var previous = handler.ConnectCallback ?? DefaultConnectCallback;

        static async ValueTask<Stream> DefaultConnectCallback(SocketsHttpConnectionContext context,
            CancellationToken cancellationToken)
        {
            var socket = new Socket(SocketType.Stream, ProtocolType.Tcp)
            {
                NoDelay = true
            };
            try
            {
                await socket.ConnectAsync(context.DnsEndPoint, cancellationToken);
                return new NetworkStream(socket, ownsSocket: true);
            }
            catch
            {
                socket.Dispose();
                throw;
            }
        }


        handler.ConnectCallback = async (context, cancellationToken) =>
        {
            if (gatewayClientManager.TryGetValue(context.DnsEndPoint.Host.ToLower(), out var clients))
            {
                // 通过算法获取一个client
                var client = GetGatewayClient(clients);

                var agentTunnel =
                    await httpTunnelFactory.CreateHttpTunnelAsync(client.Connection, cancellationToken);

                return agentTunnel;
            }

            return await previous(context, cancellationToken);
        };

        handler.MaxAutomaticRedirections = 3;
        handler.EnableMultipleHttp2Connections = true;
    }
}