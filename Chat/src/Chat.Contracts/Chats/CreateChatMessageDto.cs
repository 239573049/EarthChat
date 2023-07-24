using Chat.Contracts.Users;

namespace Chat.Contracts.Chats;

public class CreateChatMessageDto
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

    /// <summary>
    /// 扩展参数
    /// </summary>
    public Dictionary<string, string> Extends { get; set; }
    
    public GetUserDto User { get; set; }
}