using Chat.Service.Domain.Users.Aggregates;

namespace Chat.Service.Domain.Chats.Aggregates;

public class ChatGroupInUser : Entity<Guid>
{
    public Guid UserId { get; set; }
    
    public Guid ChatGroupId { get; set; }
    
    public virtual ChatGroup ChatGroup { get; set; }

    public virtual User User { get; set; }   
}