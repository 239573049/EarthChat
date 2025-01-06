using System.Net;
using EarthChat.Gateway.Sdk.Data;
using Microsoft.AspNetCore.Http.Features;

namespace EarthChat.Infrastructure.Gateway.Tunnels;

public sealed partial class GatewayClient
{
    private volatile bool _disposed;
    public readonly GatewayClientConnection Connection;
    private readonly HttpTunnelFactory _httpTunnelFactory;
    private readonly HttpContext _httpContext;
    private readonly Lazy<HttpMessageInvoker> _httpClientLazy;

    public string Id => this.Connection.ClientId;
    
    public string NodeName => this.Connection.NodeName;

    public TransportProtocol Protocol => this._httpContext.Features.GetRequiredFeature<IGatewayFeature>().Protocol;

    public int HttpTunnelCount => this.Connection.HttpTunnelCount;

    public IPEndPoint? RemoteEndpoint
    {
        get
        {
            var connection = this._httpContext.Connection;
            return connection.RemoteIpAddress == null
                ? null
                : new IPEndPoint(connection.RemoteIpAddress, connection.RemotePort);
        }
    }

    public DateTimeOffset CreationTime { get; } = DateTimeOffset.Now;


    public GatewayClient(
        GatewayClientConnection connection,
        HttpTunnelFactory httpTunnelFactory,
        HttpContext httpContext)
    {
        this.Connection = connection;
        this._httpTunnelFactory = httpTunnelFactory;
        this._httpContext = httpContext;
    }

    public async ValueTask DisposeAsync()
    {
        if (this._disposed == false)
        {
            this._disposed = true;

            if (this._httpClientLazy?.IsValueCreated == true)
            {
                this._httpClientLazy.Value.Dispose();
            }

            await this.Connection.DisposeAsync();
        }
    }

    public override string ToString()
    {
        return this.Id;
    }
}