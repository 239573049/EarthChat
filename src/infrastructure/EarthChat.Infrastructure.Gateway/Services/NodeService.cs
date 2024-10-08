using EarthChat.Core.Models;
using EarthChat.Infrastructure.Gateway.Node;
using EarthChat.Infrastructure.Gateway.ServiceInspection;

namespace EarthChat.Infrastructure.Gateway.Services;

public class NodeService
{
    private readonly NodeClientManager _nodeClientManager;
    private readonly ILogger<NodeService> _logger;

    public NodeService(NodeClientManager nodeClientManager, ILogger<NodeService> logger)
    {
        _nodeClientManager = nodeClientManager;
        _logger = logger;
    }

    /// <summary>
    /// 注册节点
    /// </summary>
    /// <param name="nodeClient"></param>
    /// <returns></returns>
    public async Task<ResultDto<string>> RegisterAsync(NodeClient nodeClient)
    {
        nodeClient.Key = Guid.NewGuid().ToString("N");
        _nodeClientManager.Add(nodeClient);

        _logger.LogInformation("节点注册成功 key:{key} Service:{service} Ip:{ip} Port:{port} HealthCheck:{healthCheck}",
            nodeClient.Key, nodeClient.Service, nodeClient.Ip, nodeClient.Port, nodeClient.HealthCheck);

        return await Task.FromResult(ResultDto<string>.SuccessResult(nodeClient.Key));
    }

    /// <summary>
    /// 如果节点不存在则注册
    /// </summary>
    /// <param name="nodeClient"></param>
    /// <returns></returns>
    public async Task<ResultDto<bool>> InspectAsync(NodeClient nodeClient)
    {
        if (!_nodeClientManager.Contains(nodeClient.Key))
        {
            _nodeClientManager.Add(nodeClient);
        }

        return await Task.FromResult(ResultDto<bool>.SuccessResult(true));
    }

    /// <summary>
    /// 注销节点
    /// </summary>
    /// <param name="key"></param>
    public async Task DeregisterAsync(string key)
    {
        // _nodeClientManager.Remove(key);
        // await Task.CompletedTask;
    }
}

public static class NodeExtensions
{
    public static IServiceCollection AddNodeService(this IServiceCollection services)
    {
        services.AddSingleton<NodeService>();
        services.AddHostedService<ServiceInspectionBackgroundTask>();

        return services;
    }

    public static IEndpointRouteBuilder MapNodeService(this IEndpointRouteBuilder endpoints)
    {
        var node = endpoints.MapGroup("/api/v1/node")
            .WithDisplayName("节点服务")
            .WithTags("节点服务")
            .WithDescription("节点服务 API");

        node.MapPost("register",
            async (NodeService nodeService, NodeClient nodeClient) => await nodeService.RegisterAsync(nodeClient));

        node.MapPost("inspect",
            async (NodeService nodeService, NodeClient nodeClient) => await nodeService.InspectAsync(nodeClient));

        node.MapDelete("deregister",
            async (NodeService nodeService, string key) => await nodeService.DeregisterAsync(key));

        return endpoints;
    }
}