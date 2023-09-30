using Chat.Service.Domain.Chats.Aggregates;
using Chat.Service.Domain.Users.Aggregates;
using Chat.Service.Domain.Users.Repositories;

namespace Chat.Service.Infrastructure.Repositories;

public class FriendRepository : BaseRepository<ChatDbContext, Friend, Guid>, IFriendRepository
{
    public FriendRepository(ChatDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public async Task<List<ChatGroup>> GetUserInFriendAsync(Guid userId)
    {
        var query =
            (from friend in Context.Friends
                join user in Context.Users on friend.FriendId equals user.Id
                where friend.SelfId == userId
                select new ChatGroup(friend.GroupId,user.Id)
                {
                    Avatar = user.Avatar,
                    Default = false,
                    Name = user.Name
                });

        return await query.ToListAsync();
    }
}