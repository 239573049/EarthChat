using System.Linq.Expressions;
using Chat.Service.Domain.Chats.Aggregates;

namespace Chat.Service.Domain.Chats.Repositories;

public interface IChatGroupInUserRepository : IRepository<ChatGroupInUser,Guid>
{
    Task<IEnumerable<ChatGroup>> GetUserChatGroupAsync(Guid userId);
}