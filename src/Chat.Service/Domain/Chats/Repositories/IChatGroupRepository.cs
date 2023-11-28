using Chat.Service.Domain.Chats.Aggregates;

namespace Chat.Service.Domain.Chats.Repositories;

public interface IChatGroupRepository : IBaseRepository<ChatGroup,Guid>
{
    /// <summary>
    /// 更新最新消息
    /// </summary>
    /// <param name="id"></param>
    /// <param name="newMessage"></param>
    /// <returns></returns>
    Task UpdateNewMessage(Guid id, string newMessage);
}