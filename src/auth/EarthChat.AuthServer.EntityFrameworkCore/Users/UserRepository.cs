using EarthChat.AuthServer.Domain.Users;
using EarthChat.EntityFrameworkCore;
using Gnarly.Data;

namespace EarthChat.AuthServer.EntityFrameworkCore.Users;

[Registration(typeof(IUserRepository<AuthDbContext>))]
public class UserRepository(AuthDbContext dbContext)
    : Repository<AuthDbContext, User>(dbContext), IUserRepository<AuthDbContext>, IScopeDependency
{
}