using Chat.Contracts.Chats;
using Chat.Service.Domain.Users.Aggregates;

namespace Chat.Service.Domain.Chats.Aggregates;

public class ChatMessage : AuditAggregateRoot<Guid, Guid>
{
    protected ChatMessage()
    {
    }
    
    public ChatMessage(Guid id,DateTime creationTime) : base(id)
    {
        CreationTime = creationTime;
    }

    /// <summary>
    ///     内容
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    ///     类型
    /// </summary>
    public ChatType Type { get; set; }

    /// <summary>
    ///     Id
    /// </summary>
    public Guid? UserId { get; set; }

    public Guid ChatGroupId { get; set; }
    
    public virtual User User { get; set; }

    protected override DateTime GetCurrentTime()
    {
        return DateTime.Now;
    }
}