using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using EarthChat.Gateway.Sdk.Data;
using EarthChat.Gateway.Sdk.Options;

namespace EarthChat.Gateway.Sdk.Tunnels;

public class TunnelServer(HttpClient httpClient)
{
    private static readonly HttpMessageInvoker HttpClient = new(CreateDefaultHttpHandler(), true);

    private static readonly string NodeId = Guid.NewGuid().ToString();

    private static IPEndPoint? _ipEndPoint;

    public static void SetUri(IPEndPoint? uri)
    {
        _ipEndPoint = uri;
    }

    public static SocketsHttpHandler CreateDefaultHttpHandler()
    {
        return new SocketsHttpHandler
        {
            // 允许多个http2连接
            EnableMultipleHttp2Connections = true,
            // 设置连接超时时间
            ConnectTimeout = TimeSpan.FromHours(1),
            KeepAlivePingTimeout = TimeSpan.FromSeconds(60),
            MaxConnectionsPerServer = int.MaxValue,
            SslOptions = new SslClientAuthenticationOptions
            {
                // 由于我们没有证书，所以我们需要设置为true
                RemoteCertificateValidationCallback = (_, _, _, _) => true,
            },
        };
    }

    public async Task<Stream> CreateTargetTunnelAsync(CancellationToken cancellationToken)
    {
        var socket = new Socket(SocketType.Stream, ProtocolType.Tcp) { NoDelay = true };

        try
        {
            using var linkedTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(new CancellationToken(), cancellationToken);

            await socket.ConnectAsync(_ipEndPoint!, linkedTokenSource.Token);
            return new NetworkStream(socket);
        }
        catch (Exception ex)
        {
            Console.WriteLine("创建targetTunnel隧道失败。");
            Console.WriteLine(ex);
            socket.Dispose();
            throw;
        }
    }

    private async Task<Stream> HttpConnectServerCoreAsync(string server, string clientId,
        GatewayServiceOptions? clients,
        Guid? tunnelId,
        CancellationToken cancellationToken)
    {
        return await this.Http20ConnectServerAsync(server, clientId, clients, tunnelId, cancellationToken);
    }

    public async Task<GatewayConnection> CreateServerConnectionAsync(string server, string clientId,
        GatewayServiceOptions ports,
        CancellationToken cancellationToken)
    {
        var stream = await HttpConnectServerCoreAsync(server, clientId, ports, null, cancellationToken);
        var safeWriteStream = new SafeWriteStream(stream);
        return new GatewayConnection(safeWriteStream, TimeSpan.FromSeconds(120));
    }

    public async Task<Stream> ConnectServerAsync(string server, Guid? tunnelId, CancellationToken cancellationToken)
    {
        return await this.HttpConnectServerCoreAsync(server, null, null, tunnelId, cancellationToken);
    }

    /// <summary>
    /// 创建到服务器的通道
    /// </summary>
    /// <param name="server"></param>
    /// <param name="tunnelId"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="OperationCanceledException"></exception>
    /// <returns></returns>
    public async Task<Stream> CreateServerTunnelAsync(string server, Guid tunnelId,
        CancellationToken cancellationToken)
    {
        var stream = await this.ConnectServerAsync(server, tunnelId, cancellationToken);
        return new ForceFlushStream(stream);
    }

    /// <summary>
    /// 创建http2连接
    /// </summary>
    /// <param name="server"></param>
    /// <param name="nodeName"></param>
    /// <param name="clients"></param>
    /// <param name="tunnelId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<Stream> Http20ConnectServerAsync(string server, string? nodeName, GatewayServiceOptions? clients,
        Guid? tunnelId,
        CancellationToken cancellationToken)
    {
        Uri serverUri;
        if (tunnelId == null)
        {
            serverUri = new Uri(
                $"{server.TrimEnd('/')}/Gateway/node?nodeId={NodeId}&nodeName={nodeName}&token={clients?.Token}");
        }
        else
        {
            serverUri = new Uri($"{server.TrimEnd('/')}/Gateway/node/Server?tunnelId=" + tunnelId);
        }

        // 这里我们使用Connect方法，因为我们需要建立一个双工流, 这样我们就可以进行双工通信了。
        var request = new HttpRequestMessage(HttpMethod.Connect, serverUri);
        // 如果设置了Connect，那么我们需要设置Protocol
        request.Headers.Protocol = GatewayConstant.Protocol;
        // 我们需要设置http2的版本
        request.Version = HttpVersion.Version20;

        // 我们需要确保我们的请求是http2的
        request.VersionPolicy = HttpVersionPolicy.RequestVersionExact;

        // 设置一下超时时间，这样我们就可以在超时的时候取消连接了。
        using var timeoutTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));
        using var linkedTokenSource =
            CancellationTokenSource.CreateLinkedTokenSource(timeoutTokenSource.Token, cancellationToken);
        // 发送请求，然后等待响应
        var httpResponse = await HttpClient.SendAsync(request, linkedTokenSource.Token);

        // 返回h2的流，用于传输数据
        return await httpResponse.Content.ReadAsStreamAsync(linkedTokenSource.Token);
    }
}