using Chat.Contracts.Users;

namespace Chat.Contracts.Chats;

public class ChatMessageDto
{
    /// <summary>
    /// 内容
    /// </summary>
    public string Cotnent { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    public ChatType Type { get; set; }

    /// <summary>
    /// id
    /// </summary>
    public Guid UserId { get; set; }

    public GetUserDto User { get; set; }
}