namespace Chat.Contracts.Chats;

public class Notification
{
    /// <summary>
    /// 内容
    /// </summary>
    public string content { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    public NotificationType type { get; set; }

    /// <summary>
    /// 发布时间
    /// </summary>
    public DateTime createdTime { get; set; }
    
}