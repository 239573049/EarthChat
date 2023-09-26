using Chat.Service.Domain.Chats.Aggregates;
using Chat.Service.Domain.Chats.Repositories;
using Chat.Service.Domain.Users.Aggregates;
using Chat.Service.Domain.Users.Repositories;
using Masa.Contrib.Ddd.Domain.Repository.EFCore;

namespace Chat.Service.Infrastructure.Repositories;

public class FriendRequestRepository : Repository<ChatDbContext, FriendRequest>, IFriendRequestRepository
{
    public FriendRequestRepository(ChatDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public Task<List<FriendRequest>> GetListAsync(Guid userId, int page, int pageSize)
    {
        var query = CreateQuery(userId);

        return query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public Task<int> GetCountAsync(Guid userId)
    {
        var query = CreateQuery(userId);

        return query.CountAsync();
    }

    private IQueryable<FriendRequest> CreateQuery(Guid userId)
    {
        var query = Context.FriendRequests.Where(x => x.RequestId == userId)
            .OrderBy(x => x.CreationTime);

        return query;
    }
}