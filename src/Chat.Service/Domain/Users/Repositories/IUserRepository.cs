using Chat.Service.Domain.Users.Aggregates;
using Masa.BuildingBlocks.Ddd.Domain.Repositories;

namespace Chat.Service.Domain.Users.Repositories;

public interface IUserRepository : IBaseRepository<User, Guid>
{
}