using MessagePack;

namespace Chat.Contracts.Hubs;

/// <summary>
/// 通知
/// </summary>
public class Notification
{
    /// <summary>
    /// 内容
    /// </summary>
    [Key("content")]
    public string content { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    [Key("type")]
    public NotificationType type { get; set; }

    /// <summary>
    /// 发布时间
    /// </summary>
    public DateTime createdTime { get; set; }

    /// <summary>
    /// 内容
    /// </summary>
    public object data { get; set; }

}