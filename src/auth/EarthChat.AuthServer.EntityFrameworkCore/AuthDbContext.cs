using EarthChat.AuthServer.Domain.Users;
using EarthChat.Core.Contract;
using EarthChat.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EarthChat.AuthServer.EntityFrameworkCore;

public class AuthDbContext(DbContextOptions<AuthDbContext> options, IUserContext userContext)
    : MasterDbContext<AuthDbContext>(options, userContext)
{
    public DbSet<User> Users { get; set; }

    public DbSet<UserOAuth> UserOAuths { get; set; }
}