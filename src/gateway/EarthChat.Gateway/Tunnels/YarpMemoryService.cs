using Yarp.ReverseProxy.Configuration;

namespace EarthChat.Infrastructure.Gateway.Tunnels;

public class YarpMemoryService(InMemoryConfigProvider inMemoryConfigProvider, GatewayClientManager gatewayClientManager)
{
    /// <summary>
    /// 更新路由缓存
    /// </summary>
    /// <returns></returns>
    public void UpdateRoutes()
    {
        var (routes, clusters) = GetRoutes();
        inMemoryConfigProvider.Update(routes, clusters);
    }

    /// <summary>
    /// 获取路由 集群
    /// </summary>
    /// <returns></returns>
    private (IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters) GetRoutes()
    {
        var routes = new List<RouteConfig>();
        var clusters = new List<ClusterConfig>();

        foreach (var (host, clients) in gatewayClientManager.ToArray())
        {
            var route = new RouteConfig
            {
                RouteId = host,
                ClusterId = host,
                Match = new RouteMatch
                {
                    Path = "/api/" + host + "/{**catch-all}",
                },
            };
            var destinations = new Dictionary<string, DestinationConfig>();
            var clusterConfig = new ClusterConfig
            {
                ClusterId = host,
                Destinations = destinations
            };

            // 获取prefix 
            var prefix = clients.FirstOrDefault().Value.Connection.Prefix;

            // 不需要在这里负载均衡
            destinations.Add(Guid.NewGuid().ToString("N"), new DestinationConfig
            {
                Address = $"http://{host}{prefix}",
            });

            clusters.Add(clusterConfig);

            routes.Add(route);
        }

        return (routes, clusters);
    }
}