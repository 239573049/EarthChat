namespace EarthChat.Infrastructure.Gateway.Sdk.Options;

public class GatewayServiceOptions
{
    /// <summary>
    /// 服务名称
    /// /服务名称/服务api路由
    /// </summary>
    public string Service { get; set; }

    /// <summary>
    /// 网关服务地址
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    /// 服务IP
    /// </summary>
    public string Ip { get; set; }

    /// <summary>
    /// 服务端口
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// 健康检查地址
    /// </summary>
    public string HealthCheck { get; set; } = "/health";

    /// <summary>
    /// 服务访问token
    /// </summary>
    public string Token { get; set; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Service))
        {
            throw new ArgumentNullException(nameof(Service));
        }

        if (string.IsNullOrWhiteSpace(Address))
        {
            throw new ArgumentNullException(nameof(Address));
        }

        if (string.IsNullOrWhiteSpace(Token))
        {
            throw new ArgumentNullException(nameof(Token));
        }

        if (string.IsNullOrWhiteSpace(Ip))
        {
            throw new ArgumentNullException(nameof(Ip));
        }

        if (Port <= 0)
        {
            throw new ArgumentNullException(nameof(Port));
        }
    }
}