namespace EarthChat.Gateway.Sdk.Internal.Client;

public interface IGatewayClient
{
    /// <summary>
    ///     服务注册
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RegisterAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     服务注销
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task ServiceDeregisterAsync(CancellationToken cancellationToken);
}