using Chat.Service.Domain.Chats.Aggregates;
using Masa.BuildingBlocks.Ddd.Domain.Repositories;

namespace Chat.Service.Domain.Chats.Repositories;

public interface IChatMessageRepository : IRepository<ChatMessage, Guid>
{
    Task<List<ChatMessage>> GetListAsync(Guid queryGroupId, int page = 1, int pageSize = 20);
}