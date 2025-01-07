using EarthChat.AuthServer.EntityFrameworkCore.Extensions;
using EarthChat.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EarthChat.Migrations.EntityFrameworkCore.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection WithMigrationDbAccess(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<MigrationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));
        
        services.WithAuthDbAccess(configuration);

        return services;
    }
}