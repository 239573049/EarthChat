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

    public async Task<List<ChatMessage>> GetListAsync(Guid queryGroupId, int page = 1, int pageSize = 20)
    {
        var query =
            from messages in Context.ChatMessages
            where messages.ChatGroupId == queryGroupId
            orderby messages.CreationTime descending
            select new ChatMessage(messages.Id, messages.CreationTime)
            {
                Content = messages.Content,
                Type = messages.Type,
                UserId = messages.UserId ?? Guid.Empty
            };

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task CreateAsync(ChatMessage message)
    {
        await Context.ChatMessages.AddAsync(message); 
        await Context.SaveChangesAsync();
    }
}