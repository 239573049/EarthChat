using EarthChat.Infrastructure.Gateway.Node;
using EarthChat.Infrastructure.Gateway.Options;
using Microsoft.Extensions.Options;
using Yarp.ReverseProxy.Configuration;

namespace EarthChat.Infrastructure.Gateway.Gateway;

/// <summary>
/// 网关内存
/// </summary>
/// <param name="nodeClientManager"></param>
/// <param name="inMemoryConfigProvider"></param>
/// <param name="options"></param>
public class GatewayMemory(
    NodeClientManager nodeClientManager,
    InMemoryConfigProvider inMemoryConfigProvider,
    IOptions<GatewayOptions> options)
{
    private readonly GatewayOptions _options = options.Value;

    private readonly string _prefix = options.Value.ServicePrefix.TrimStart('/').TrimEnd('/');

    /// <summary>
    /// 刷新路由
    /// </summary>
    public void RefreshRoute()
    {
        var routes = new List<RouteConfig>();
        var clusters = new List<ClusterConfig>();

        foreach (var node in nodeClientManager.GetAllWithService())
        {
            var clusterId = Guid.NewGuid().ToString("N");
            var routeId = Guid.NewGuid().ToString("N");
            string path;
            if (string.IsNullOrEmpty(_prefix))
            {
                path = $"/{node.key}/{{**catch-all}}";
            }
            else
            {
                path = $"/{_prefix}/{node.key}/{{**catch-all}}";
            }

            var route = new RouteConfig
            {
                RouteId = routeId,
                ClusterId = clusterId,
                Match = new RouteMatch
                {
                    Path = path
                }
            };

            var destinations = node.clients.Where(x =>
                    !string.IsNullOrEmpty(x.Address) && x.Stats is NodeClientStats.Healthy or NodeClientStats.Exception)
                .Select(x =>
                    new DestinationConfig
                    {
                        Address = x.Address,
                        Health = x.HealthCheck
                    }).ToDictionary((x => Guid.NewGuid().ToString("N")));

            var cluster = new ClusterConfig
            {
                ClusterId = clusterId,
                Destinations = destinations
            };

            routes.Add(route);

            clusters.Add(cluster);
        }

        inMemoryConfigProvider.Update(routes, clusters);
    }
}