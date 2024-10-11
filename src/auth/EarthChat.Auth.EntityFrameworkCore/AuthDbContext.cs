using EarthChat.Auth.Domains;
using EarthChat.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EarthChat.Auth.EntityFrameworkCore;

public class AuthDbContext(
    DbContextOptions<AuthDbContext> options,
    IServiceProvider serviceProvider)
    : EarthDbContext<AuthDbContext>(options, serviceProvider)
{
    public DbSet<EarthUser> EarthUsers { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureEntities();
    }
}