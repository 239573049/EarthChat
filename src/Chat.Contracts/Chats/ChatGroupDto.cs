namespace Chat.Contracts.Chats;

public class ChatGroupDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Avatar { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 是否群组
    /// </summary>
    public bool Group { get; set; }

    public bool Default { get; set; }

    /// <summary>
    /// 群组创建者
    /// </summary>
    public Guid Creator { get; set; }

    public DateTime CreationTime { get; set; }
}