namespace EarthChat.Infrastructure.Gateway.Node;

/// <summary>
///     节点状态
/// </summary>
public enum NodeClientStats : byte
{
    /// <summary>
    ///     不健康
    /// </summary>
    Unhealthy = 1,

    /// <summary>
    ///     健康
    /// </summary>
    Healthy = 2,

    /// <summary>
    ///     存在但异常状态
    /// </summary>
    Exception = 3
}