using System.Collections.Concurrent;
using System.Diagnostics;
using EarthChat.Gateway.Sdk.Data;

namespace EarthChat.Infrastructure.Gateway.Tunnels;


public sealed partial class HttpTunnelFactory(ILogger<HttpTunnel> logger)
{
    private readonly ConcurrentDictionary<Guid, TaskCompletionSource<HttpTunnel>> httpTunnelCompletionSources =
        new();

    /// <summary>
    /// 创建HttpTunnel
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<HttpTunnel> CreateHttpTunnelAsync(GatewayClientConnection connection,
        CancellationToken cancellationToken)
    {
        var tunnelId = Guid.NewGuid();
        var httpTunnelSource = new TaskCompletionSource<HttpTunnel>();
        if (this.httpTunnelCompletionSources.TryAdd(tunnelId, httpTunnelSource) == false)
        {
            throw new SystemException($"系统中已存在{tunnelId}的tunnelId");
        }

        try
        {
            var stopwatch = Stopwatch.StartNew();
            logger.LogInformation("[{clientId}] 请求创建隧道{tunnelId}", connection.ClientId, tunnelId);
            await connection.CreateHttpTunnelAsync(tunnelId, cancellationToken);
            var httpTunnel = await httpTunnelSource.Task.WaitAsync(cancellationToken);

            var httpTunnelCount = connection.IncrementHttpTunnelCount();
            httpTunnel.BindConnection(connection);

            stopwatch.Stop();
            logger.LogInformation("[{clientId}] 创建隧道{tunnelId}成功，协议：{protocol}，耗时：{elapsed}，隧道数：{httpTunnelCount}",
                connection.ClientId, tunnelId, httpTunnel.Protocol, stopwatch.Elapsed, httpTunnelCount);
            return httpTunnel;
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("[{clientId}] 创建隧道{tunnelId}失败：远程端操作超时", connection.ClientId, tunnelId);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "[{clientId}] 创建隧道{tunnelId}失败", connection.ClientId, tunnelId);
            throw;
        }
        finally
        {
            this.httpTunnelCompletionSources.TryRemove(tunnelId, out _);
        }
    }

    public bool Contains(Guid tunnelId)
    {
        return this.httpTunnelCompletionSources.ContainsKey(tunnelId);
    }

    public bool SetResult(HttpTunnel httpTunnel)
    {
        return this.httpTunnelCompletionSources.TryRemove(httpTunnel.Id, out var source) &&
               source.TrySetResult(httpTunnel);
    }
}