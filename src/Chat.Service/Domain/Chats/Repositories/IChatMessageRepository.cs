using Chat.Service.Domain.Chats.Aggregates;

namespace Chat.Service.Domain.Chats.Repositories;

public interface IChatMessageRepository : IBaseRepository<ChatMessage, Guid>
{
    Task<List<ChatMessage>> GetListAsync(Guid queryGroupId, int page = 1, int pageSize = 20);

    /// <summary>
    /// 创建并直接写入到数据库
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Task CreateAsync(ChatMessage message);

    /// <summary>
    /// 更新消息的撤回状态
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <param name="countermand"></param>
    /// <returns></returns>
    Task<bool> UpdateCountermand(Guid id, Guid userId, bool countermand);
}