namespace EarthChat.Infrastructure.Gateway.Tunnels;

public record GatewayClientState()
{
    /// <summary>
    /// 客户端实例
    /// </summary>
    public required GatewayClient Client { get; init; }

    /// <summary>
    /// 是否为连接状态
    /// </summary>
    public required bool IsConnected { get; init; }
}