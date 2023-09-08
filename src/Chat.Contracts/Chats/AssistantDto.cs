namespace Chat.Contracts.Chats;

public class AssistantDto
{
    /// <summary>
    /// 是否群组。
    /// </summary>
    public bool Group { get; set; }
    
    /// <summary>
    /// 内容
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// 发送人Id
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 链接Id
    /// </summary>
    public Guid Id { get; set; }
}