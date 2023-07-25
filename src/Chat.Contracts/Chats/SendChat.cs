namespace Chat.Contracts.Chats;

public class SendChat
{
    /// <summary>
    ///     内容
    /// </summary>
    public string Cotnent { get; set; }

    /// <summary>
    ///     类型
    /// </summary>
    public ChatType Type { get; set; }
}