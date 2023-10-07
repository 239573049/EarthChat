using Chat.Contracts.Chats;
using Chat.Service.Domain.Chats.Aggregates;
using Chat.Service.Domain.Users.Aggregates;

namespace Chat.Service.Infrastructure.Repositories.Views;

public class ChatMessageView : AuditAggregateRoot<Guid, Guid>
{
    public ChatMessageView(Guid id, DateTime creationTime) : base(id)
    {
        CreationTime = creationTime;
    }

    /// <summary>
    /// 内容
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    public ChatType Type { get; set; }

    /// <summary>
    ///  Id
    /// </summary>
    public Guid? UserId { get; set; }

    public Guid ChatGroupId { get; set; }

    /// <summary>
    /// 回复上条内容的Id
    /// </summary>
    public Guid? RevertId { get; set; }

    /// <summary>
    /// 是否撤回
    /// </summary>
    public bool Countermand { get; set; }

    public  User User { get; set; }

    public ChatMessage? Revert { get; set; }
}