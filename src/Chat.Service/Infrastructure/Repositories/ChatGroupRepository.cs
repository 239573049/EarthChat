using Chat.Service.Domain.Chats.Aggregates;
using Chat.Service.Domain.Chats.Repositories;

namespace Chat.Service.Infrastructure.Repositories;

public class ChatGroupRepository : BaseRepository<ChatDbContext, ChatGroup, Guid>, IChatGroupRepository
{
    public ChatGroupRepository(ChatDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public async Task UpdateNewMessage(Guid id, string newMessage)
    {
        await Context.Database.ExecuteSqlInterpolatedAsync($"UPDATE  \"ChatGroups\" SET \"NewMessage\"={newMessage} where \"Id\"={id}");
    }
}