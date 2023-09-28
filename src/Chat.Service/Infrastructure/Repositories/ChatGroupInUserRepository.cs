using Chat.Service.Domain.Chats.Aggregates;
using Chat.Service.Domain.Chats.Repositories;
using Chat.Service.Domain.Users.Aggregates;
using Masa.Contrib.Ddd.Domain.Repository.EFCore;

namespace Chat.Service.Infrastructure.Repositories;

public class ChatGroupInUserRepository : BaseRepository<ChatDbContext, ChatGroupInUser, Guid>, IChatGroupInUserRepository
{
    public ChatGroupInUserRepository(ChatDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public Task<IEnumerable<ChatGroup>> GetUserChatGroupAsync(Guid userId)
    {
        var query =
            Context.ChatGroupInUsers
                .Where(x => x.UserId == userId)
                .Include(x => x.ChatGroup)
                .Select(x => x.ChatGroup);

        return Task.FromResult(query.AsEnumerable());
    }

    public async Task<List<User>> GetGroupInUserAsync(Guid groupId)
    {
        var query = Context.ChatGroupInUsers
            .Where(x => x.ChatGroupId == groupId)
            .Include(x => x.User)
            .Select(x=>x.User);

        return await query.ToListAsync();

    }
}