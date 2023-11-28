using Chat.Service.Domain.Users.Aggregates;

namespace Chat.Service.Domain.Users.Repositories;

public interface IFriendRequestRepository : IBaseRepository<FriendRequest,Guid>
{
    Task<List<FriendRequest>> GetListAsync(Guid userId, int page, int pageSize);

    Task<int> GetCountAsync(Guid userId);

}