namespace EarthChat.Gateway.Sdk.Options;

public class GatewayServiceOptions
{
    /// <summary>
    ///     服务名称
    ///     /服务名称/服务api路由
    /// </summary>
    public string Service { get; set; }

    /// <summary>
    ///     网关服务地址
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    ///     健康检查地址
    /// </summary>
    public string HealthCheck { get; set; } = "/health";

    /// <summary>
    ///     服务访问token
    /// </summary>
    public string Token { get; set; }

    public string Prefix { get; set; } = "/api";

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Service)) throw new ArgumentNullException(nameof(Service));

        if (string.IsNullOrWhiteSpace(Address)) throw new ArgumentNullException(nameof(Address));

        if (string.IsNullOrWhiteSpace(Token)) throw new ArgumentNullException(nameof(Token));
    }
}