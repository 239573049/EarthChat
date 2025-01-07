using EarthChat.AuthServer.EntityFrameworkCore.Extensions;
using EarthChat.Core.Contract;
using EarthChat.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EarthChat.Migrations.EntityFrameworkCore;

public class MigrationDbContext(DbContextOptions<MigrationDbContext> options, IServiceProvider serviceProvider)
    : MasterDbContext<MigrationDbContext>(options, serviceProvider)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureAuth();
        
    }
}