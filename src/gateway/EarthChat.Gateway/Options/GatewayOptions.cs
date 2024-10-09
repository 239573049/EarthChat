namespace EarthChat.Infrastructure.Gateway.Options;

/// <summary>
///     网关配置
/// </summary>
public class GatewayOptions
{
    /// <summary>
    ///     服务注册token
    /// </summary>
    public string Token { get; set; } = null!;

    /// <summary>
    ///     服务前缀
    /// </summary>
    public string ServicePrefix { get; set; } = null!;
}