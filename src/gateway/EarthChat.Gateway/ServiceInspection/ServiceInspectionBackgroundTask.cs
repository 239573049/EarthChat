using EarthChat.Infrastructure.Gateway.Gateway;
using EarthChat.Infrastructure.Gateway.Node;

namespace EarthChat.Infrastructure.Gateway.ServiceInspection;

/// <summary>
/// 服务监控检查
/// </summary>
/// <param name="logger"></param>
/// <param name="nodeClientManager"></param>
/// <param name="serviceProvider"></param>
public sealed class ServiceInspectionBackgroundTask(
    ILogger<ServiceInspectionBackgroundTask> logger,
    NodeClientManager nodeClientManager,
    GatewayMemory gatewayMemory,
    IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await InspectService();
                await Task.Delay(10000, stoppingToken);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "服务检查失败");
        }
    }

    private async Task InspectService()
    {
        try
        {
            foreach (var client in nodeClientManager.GetAll())
            {
                var scope = serviceProvider.CreateAsyncScope();

                var tasks = client.Where(x => !string.IsNullOrEmpty(x.HealthCheck))
                    .Select(c => CheckService(c, scope.ServiceProvider));

                await Task.WhenAll(tasks);

                await scope.DisposeAsync();
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "服务检查失败");
        }
    }

    /// <summary>
    ///     检查服务是否可用
    ///     如果不可用则移除
    /// </summary>
    /// <param name="client"></param>
    /// <param name="serviceProvider"></param>
    public async Task CheckService(NodeClient client, IServiceProvider serviceProvider)
    {
        var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        var logger = serviceProvider.GetRequiredService<ILogger<ServiceInspectionBackgroundTask>>();
        var nodeClientManager = serviceProvider.GetRequiredService<NodeClientManager>();

        var clientName = client.Service.ToLower();
        var clientKey = client.Key.ToLower();

        var clientService = httpClientFactory.CreateClient(clientName);

        var retryCount = 0;
        const int maxRetryAttempts = 3;

        while (retryCount < maxRetryAttempts)
        {
            try
            {
                var url = new Uri(client.Address + client.HealthCheck);
                var response = await clientService.GetAsync(url, CancellationToken.None);

                if (response.IsSuccessStatusCode)
                    logger.LogInformation("服务检查成功 {clientName} {clientKey}", clientName, clientKey);
                else
                    logger.LogInformation("服务检查失败 {clientName} {clientKey}", clientName, clientKey);

                nodeClientManager.Update(client.Service, client.Key, response.IsSuccessStatusCode
                    ? NodeClientStats.Healthy
                    : NodeClientStats.Exception);

                return; // 成功后退出循环
            }
            catch (Exception e)
            {
                logger.LogError(e, "服务检查失败 {clientName} {clientKey}", clientName, clientKey);
            }

            retryCount++;
            if (retryCount < maxRetryAttempts) await Task.Delay(1000); // 等待一秒后重试
        }

        // 如果client上次的状态更新时间超过1分钟，则移除
        if (client.Stats == NodeClientStats.Unhealthy && client.LastUpdateTime.AddSeconds(30) < DateTime.Now)
        {
            logger.LogError("服务检查失败超过最大重试次数，移除客户端 {clientName} {clientKey}", clientName, clientKey);
            nodeClientManager.Remove(client.Service, client.Key);
        }
        else
        {
            logger.LogError("服务检查失败超过最大重试次数，更新客户端状态 {clientName} {clientKey}", clientName, clientKey);
            nodeClientManager.Update(client.Service, client.Key, NodeClientStats.Unhealthy);
        }

        gatewayMemory.RefreshRoute();
    }
}