using Chat.Service.Domain.Chats.Aggregates;
using Chat.Service.Domain.Chats.Repositories;
using Chat.Service.Infrastructure.Repositories.Views;
using Masa.BuildingBlocks.Data.UoW;
using Masa.Contrib.Ddd.Domain.Repository.EFCore;

namespace Chat.Service.Infrastructure.Repositories;

public class ChatMessageRepository : BaseRepository<ChatDbContext, ChatMessage, Guid>, IChatMessageRepository
{
    public ChatMessageRepository(ChatDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public async Task<List<ChatMessageView>> GetListAsync(Guid queryGroupId, int page = 1, int pageSize = 20)
    {
        var query =
            from messages in Context.ChatMessages
            where messages.ChatGroupId == queryGroupId
            join chatMessage in Context.ChatMessages on messages.RevertId equals chatMessage.Id into chatMessages
            from revert in chatMessages.DefaultIfEmpty() 
            orderby messages.CreationTime descending
            select new ChatMessageView(messages.Id, messages.CreationTime)
            {
                Content = messages.Content,
                Type = messages.Type,
                Countermand = messages.Countermand,
                RevertId = messages.RevertId,
                Revert = revert,
                ChatGroupId = messages.ChatGroupId,
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

    public async Task<bool> UpdateCountermand(Guid id, Guid userId, bool countermand)
    {
        return (await Context.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE \"ChatMessages\" SET \"Countermand\"={countermand} where \"Id\"={id} and \"Creator\" = {userId};"))>0;
    }
}