using EarthChat.Gateway.Sdk.Data;

namespace EarthChat.Infrastructure.Gateway.Tunnels;

public class NodeMiddleware(
    HttpTunnelFactory httpTunnelFactory,
    ILogger<NodeMiddleware> logger,
    YarpMemoryService yarpMemoryService,
    GatewayClientManager clientManager) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var feature = new GatewayFeature(context);
        if (IsAllowProtocol(feature.Protocol))
        {
            context.Features.Set<IGatewayFeature>(feature);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
            return;
        }

        var nodeName = context.Request.Query["nodeName"].ToString();
        var nodeId = context.Request.Query["nodeId"].ToString();

        if (string.IsNullOrEmpty(nodeName))
        {
            await next(context);
            return;
        }

        var token = context.Request.Query["token"].ToString();
        var prefix = context.Request.Query["prefix"].ToString();

        var key = $"{nodeName}:{nodeId}:{token}";

        try
        {
            // 创建连接
            var stream = await feature.AcceptAsSafeWriteStreamAsync();

            var connection = new GatewayClientConnection(key, nodeName, prefix, stream, new ConnectionConfig(), logger);

            var disconnected = false;

            await using var client = new GatewayClient(connection, httpTunnelFactory, context);
            if (await clientManager.AddAsync(client, default))
            {
                // 每当有新的客户端连接时，更新路由表
                yarpMemoryService.UpdateRoutes();

                await connection.WaitForCloseAsync();

                disconnected = await clientManager.RemoveAsync(client, default);

                // 每当客户端端口则更新
                yarpMemoryService.UpdateRoutes();
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to create client connection.");
        }
        finally
        {
        }
    }

    private static bool IsAllowProtocol(TransportProtocol protocol)
    {
        return protocol is TransportProtocol.Http11 or TransportProtocol.Http2;
    }
}