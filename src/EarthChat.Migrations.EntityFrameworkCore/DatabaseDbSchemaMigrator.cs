using EarthChat.AuthServer.EntityFrameworkCore;
using Gnarly.Data;
using Microsoft.EntityFrameworkCore;

namespace EarthChat.Migrations.EntityFrameworkCore;

public class DatabaseDbSchemaMigrator(MigrationDbContext dbContext) : IScopeDependency
{
    public async Task MigrateAsync()
    {
        await dbContext.Database.MigrateAsync();
    }
}