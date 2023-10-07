using Chat.Contracts.Users;
using MessagePack;

namespace Chat.Contracts.Chats;

[MessagePackObject]
public class ChatMessageDto
{
    [Key("id")]
    public Guid Id { get; set; }

    /// <summary>
    ///     内容
    /// </summary>
    [Key("content")]
    public string Content { get; set; }

    /// <summary>
    ///     类型
    /// </summary>
    [Key("type")]
    public ChatType Type { get; set; }

    /// <summary>
    ///     id
    /// </summary>
    [Key("userId")]
    public Guid UserId { get; set; }

    [IgnoreMember]
    public GetUserDto User { get; set; }

    /// <summary>
    /// 回复上条内容的Id
    /// </summary>
    [Key("revertId")]
    public Guid? RevertId { get; set; }

    /// <summary>
    /// 是否撤回
    /// </summary>
    [Key("countermand")]
    public bool Countermand { get; set; }

    /// <summary>
    /// 发送指定链接id
    /// </summary>
    [Key("groupId")]
    public Guid GroupId { get; set; }

    [Key("revert")]
    public ChatMessageDto? Revert { get; set; }

    [Key("creationTime")]
    public DateTime CreationTime { get; set; }
}