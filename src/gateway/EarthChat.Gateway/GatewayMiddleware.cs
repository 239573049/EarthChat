using EarthChat.Infrastructure.Gateway.Node;
using EarthChat.Infrastructure.Gateway.Options;
using Microsoft.Extensions.Options;
using Yarp.ReverseProxy.Forwarder;

namespace EarthChat.Infrastructure.Gateway;

/// <summary>
///     网关核心中间件
/// </summary>
/// <param name="nodeClientManager"></param>
/// <param name="httpForwarder"></param>
public sealed class GatewayMiddleware(
    NodeClientManager nodeClientManager,
    IHttpForwarder httpForwarder,
    IOptions<GatewayOptions> options) : IMiddleware
{
    private readonly ForwarderRequestConfig _forwarderRequestConfig = new()
    {
        ActivityTimeout = TimeSpan.FromSeconds(100)
    };

    private readonly HttpMessageInvoker _httpMessageInvoker = new(new SocketsHttpHandler
    {
        UseProxy = true,
        EnableMultipleHttp2Connections = true,
        PooledConnectionIdleTimeout = TimeSpan.FromSeconds(60),
        ConnectTimeout = TimeSpan.FromSeconds(60)
    });

    private readonly string _servicePrefix = options.Value.ServicePrefix.TrimEnd('/').TrimStart('/');

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // 去掉前缀
        if (!string.IsNullOrEmpty(_servicePrefix) && context.Request.Path.StartsWithSegments($"/{_servicePrefix}"))
            context.Request.Path = context.Request.Path.Value?.Substring(_servicePrefix.Length + 1);
        // 截取路由第一个
        var service = context.Request.Path.Value?.Split('/').Skip(1).FirstOrDefault();
        if (string.IsNullOrWhiteSpace(service))
        {
            await next(context);
            return;
        }

        // 获取节点
        var nodeClient = nodeClientManager.Get(service)
            .Where(x => x.Stats is NodeClientStats.Healthy or NodeClientStats.Exception)
            .MinBy(x => Guid.NewGuid());

        if (nodeClient == null)
        {
            await next(context);
            return;
        }

        // 增加请求头
        context.Request.Headers["Gateway-Node-Id"] = nodeClient.Key;

        // 删除路由第一个
        context.Request.Path = context.Request.Path.Value?[(service.Length + 1)..];

        // 转发请求
        await httpForwarder.SendAsync(context, nodeClient.Address, _httpMessageInvoker, _forwarderRequestConfig)
            .ConfigureAwait(false);
    }
}