namespace Chat.Service.Domain.Chats.Aggregates;

public class ChatGroup: AuditAggregateRoot<Guid, Guid>
{
    public string Name { get; set; }
    
    public string Avatar { get; set; }
    
    public string Description { get; set; }

    public bool Default { get; set; }

    protected override DateTime GetCurrentTime()
    {
        return DateTime.Now;
    }
    
    protected ChatGroup()
    {
    }

    public ChatGroup(Guid id) : base(id)
    {
    }

    public ChatGroup(Guid id,Guid creator) : base(id)
    {
        Creator = creator;
    }
}