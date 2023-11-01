using Chat.Service.Domain.Chats.Aggregates;
using Chat.Service.Domain.Chats.Repositories;
using Chat.Service.Domain.Users.Aggregates;
using Masa.Contrib.Ddd.Domain.Repository.EFCore;

namespace Chat.Service.Infrastructure.Repositories;

public class ChatGroupInUserRepository : BaseRepository<ChatDbContext, ChatGroupInUser, Guid>,
    IChatGroupInUserRepository
{
    public ChatGroupInUserRepository(ChatDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public async Task<List<ChatGroup>> GetUserChatGroupAsync(Guid userId)
    {
        var query = 
            from cu in Context.ChatGroupInUsers
            join cg in Context.ChatGroups on cu.ChatGroupId equals cg.Id
            where cu.UserId == userId
            select cg;

        return await query.ToListAsync();
    }

    public async Task<List<User>> GetGroupInUserAsync(Guid groupId, int page, int pageSize, Guid[] queryUserIds)
    {
        var query = Context.ChatGroupInUsers
            .Where(x => x.ChatGroupId == groupId)
            .Include(x => x.User)
            .Select(x => x.User)
            .OrderByDescending(x => queryUserIds.Contains(x.Id) ? 1 : 0)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        return await query.ToListAsync();
    }
}