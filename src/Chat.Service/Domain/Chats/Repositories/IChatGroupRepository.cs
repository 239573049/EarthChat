using Chat.Service.Domain.Chats.Aggregates;

namespace Chat.Service.Domain.Chats.Repositories;

public interface IChatGroupRepository : IRepository<ChatGroup,Guid>
{
    
}