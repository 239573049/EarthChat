using EarthChat.AuthServer.Domain.Users;
using EarthChat.AuthServer.EntityFrameworkCore.Extensions;
using EarthChat.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EarthChat.AuthServer.EntityFrameworkCore;

public class AuthDbContext(DbContextOptions<AuthDbContext> options, IServiceProvider serviceProvider)
    : MasterDbContext<AuthDbContext>(options, serviceProvider)
{
    public DbSet<User> Users { get; set; }

    public DbSet<UserOAuth> UserOAuths { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureAuth();
    }
}