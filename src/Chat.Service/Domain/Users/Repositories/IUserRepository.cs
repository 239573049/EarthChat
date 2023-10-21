using Chat.Service.Domain.Users.Aggregates;
using Masa.BuildingBlocks.Ddd.Domain.Repositories;

namespace Chat.Service.Domain.Users.Repositories;

public interface IUserRepository : IBaseRepository<User, Guid>
{
    /// <summary>
    /// 更新ip和归属地
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="ip"></param>
    /// <param name="location"></param>
    /// <returns></returns>
    Task UpdateLocationAsync(Guid userId, string ip, string location);
}