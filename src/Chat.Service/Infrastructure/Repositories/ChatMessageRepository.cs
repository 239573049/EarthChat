using Chat.Service.Domain.Chats.Aggregates;
using Chat.Service.Domain.Chats.Repositories;
using Masa.BuildingBlocks.Data.UoW;
using Masa.Contrib.Ddd.Domain.Repository.EFCore;

namespace Chat.Service.Infrastructure.Repositories;

public class ChatMessageRepository : Repository<ChatDbContext, ChatMessage, Guid>, IChatMessageRepository
{
    public ChatMessageRepository(ChatDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public async Task<List<ChatMessage>> GetListAsync(int page = 1, int pageSize = 20)
    {
        // 查询ChatMessage指定数量
        return await Context.ChatMessages
            .OrderByDescending(x => x.CreationTime)
            .Include(x => x.User)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}