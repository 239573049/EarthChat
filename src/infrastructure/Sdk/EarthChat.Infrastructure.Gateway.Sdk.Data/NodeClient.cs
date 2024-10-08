namespace EarthChat.Infrastructure.Gateway.Node;

/// <summary>
/// 节点客户端
/// </summary>
public sealed class NodeClient
{
    /// <summary>
    /// 客户端唯一标识
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// 客户端服务名称
    /// </summary>
    public string Service { get; set; }

    /// <summary>
    /// 客户端服务访问token
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// 服务IP
    /// </summary>
    public string Ip { get; set; }

    /// <summary>
    /// 服务端口
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// 请求地址
    /// </summary>
    public string Address => $"http://{Ip}:{Port}";

    /// <summary>
    /// 健康检查地址
    /// </summary>
    public string? HealthCheck { get; set; }

    /// <summary>
    /// 当前服务状态
    /// </summary>
    public NodeClientStats Stats { get; set; } = NodeClientStats.Healthy;
}