namespace Chat.Service.Domain.Chats.Aggregates;

public class Friend: AuditAggregateRoot<Guid, Guid>
{
    /// <summary>
    /// 好友Id
    /// </summary>
    public Guid FriendId { get; set; }
}