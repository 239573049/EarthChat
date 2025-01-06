using EarthChat.Gateway.Sdk.Data;

namespace EarthChat.Infrastructure.Gateway.Tunnels;

/// <summary>
/// http隧道
/// </summary>
public sealed partial class HttpTunnel(Stream inner, Guid tunnelId, TransportProtocol protocol, ILogger logger)
    : DelegatingStream(inner)
{
    private GatewayClientConnection? connection;
    private readonly long _tickCout = Environment.TickCount64;
    private readonly TaskCompletionSource _closeTaskCompletionSource = new();

    /// <summary>
    /// 等待HttpClient对其关闭
    /// </summary>
    public Task Closed => _closeTaskCompletionSource.Task;

    /// <summary>
    /// 隧道标识
    /// </summary>
    public Guid Id { get; } = tunnelId;

    /// <summary>
    /// 传输协议
    /// </summary>
    public TransportProtocol Protocol { get; } = protocol;

    public void BindConnection(GatewayClientConnection connection)
    {
        this.connection = connection;
    }

    public override ValueTask DisposeAsync()
    {
        this.SetClosedResult();
        return this.Inner.DisposeAsync();
    }

    protected override void Dispose(bool disposing)
    {
        this.SetClosedResult();
        this.Inner.Dispose();
    }

    private void SetClosedResult()
    {
        if (this._closeTaskCompletionSource.TrySetResult())
        {
            var httpTunnelCount = this.connection?.DecrementHttpTunnelCount();
            var lifeTime = TimeSpan.FromMilliseconds(Environment.TickCount64 - this._tickCout);
            logger.LogInformation("[{clientId}] 隧道{tunnelId}关闭，协议：{protocol}，生命周期：{lifeTime}，隧道数：{httpTunnelCount}",
                this.connection?.ClientId, this.Protocol, this.Id, lifeTime, httpTunnelCount);
        }
    }

    public override string ToString()
    {
        return this.Id.ToString();
    }
}