using Chat.Service.Domain.Users.Aggregates;
using Chat.Service.Domain.Users.Repositories;

namespace Chat.Service.Infrastructure.Repositories;

public class UserRepository : BaseRepository<ChatDbContext, User, Guid>, IUserRepository
{
    public UserRepository(ChatDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public async Task UpdateLocationAsync(Guid userId, string Ip, string location)
    {
        await Context.Database.ExecuteSqlInterpolatedAsync(
            $"update \"Users\" set \"Ip\"={Ip},\"Location\"={location} where \"Id\"={userId};");
    }
}