using Chat.Service.Domain.Users.Aggregates;
using Chat.Service.Domain.Users.Repositories;

namespace Chat.Service.Infrastructure.Repositories;

public class UserRepository : BaseRepository<ChatDbContext, User, Guid>, IUserRepository
{
    public UserRepository(ChatDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public async Task UpdateLocationAsync(Guid userId, string ip, string location)
    {
        await Context.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE \"Users\" SET \"Ip\"={ip},Location={location} where \"Id\"={userId};");
    }
}