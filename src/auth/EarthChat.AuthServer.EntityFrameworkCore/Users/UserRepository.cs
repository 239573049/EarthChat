using EarthChat.AuthServer.Domain.Users;
using EarthChat.EntityFrameworkCore;
using Gnarly.Data;

namespace EarthChat.AuthServer.EntityFrameworkCore.Users;

[Registration(typeof(IUserRepository))]
public class UserRepository(MasterDbContext<AuthDbContext> dbContext)
    : Repository<AuthDbContext, User>(dbContext), IUserRepository, IScopeDependency
{
}