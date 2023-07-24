using System.ComponentModel.DataAnnotations.Schema;
using Chat.Contracts.Chats;
using Chat.Service.Domain.Users.Aggregates;

namespace Chat.Service.Domain.Chats.Aggregates;

public class ChatMessage : AuditAggregateRoot<Guid, Guid>
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

    [NotMapped] public virtual User User { get; set; }

    /// <summary>
    /// 扩展参数
    /// </summary>
    public Dictionary<string, string> Extends { get; set; }

    protected ChatMessage()
    {
        Extends = new Dictionary<string, string>();
    }

    public ChatMessage(Guid id) : base(id)
    {
        Extends = new Dictionary<string, string>();
    }
}