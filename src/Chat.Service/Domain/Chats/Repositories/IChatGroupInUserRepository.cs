using Chat.Service.Domain.Chats.Aggregates;
using Chat.Service.Domain.Users.Aggregates;

namespace Chat.Service.Domain.Chats.Repositories;

public interface IChatGroupInUserRepository : IBaseRepository<ChatGroupInUser,Guid>
{
    Task<List<ChatGroup>> GetUserChatGroupAsync(Guid userId);

    /// <summary>
    /// 获取群组中的用户。
    /// </summary>
    /// <param name="groupId"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <param name="queryUserIds"></param>
    /// <returns></returns>
    Task<List<User>> GetGroupInUserAsync(Guid groupId, int page, int pageSize, Guid[] queryUserIds);

}