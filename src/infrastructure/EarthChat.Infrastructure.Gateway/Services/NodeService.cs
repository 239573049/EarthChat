using EarthChat.Core.Models;
using EarthChat.Infrastructure.Gateway.Node;

namespace EarthChat.Infrastructure.Gateway.Services;

public class NodeService
{
    private readonly NodeClientManager _nodeClientManager;

    public NodeService(NodeClientManager nodeClientManager)
    {
        _nodeClientManager = nodeClientManager;
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

        return await Task.FromResult(ResultDto<string>.SuccessResult(nodeClient.Key));
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

        node.MapDelete("deregister",
            async (NodeService nodeService, string key) => await nodeService.DeregisterAsync(key));

        return endpoints;
    }
}