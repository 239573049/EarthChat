using Chat.Service.Domain.Chats.Aggregates;
using Chat.Service.Domain.Users.Aggregates;

namespace Chat.Service.Domain.Users.Repositories;

public interface IFriendRepository : IBaseRepository<Friend,Guid>
{
    Task<List<ChatGroup>> GetUserInFriendAsync(Guid userId);

    /// <summary>
    /// 更新最新消息
    /// </summary>
    /// <param name="id"></param>
    /// <param name="newMessage"></param>
    /// <returns></returns>
    Task UpdateNewMessage(Guid id, string newMessage);
}