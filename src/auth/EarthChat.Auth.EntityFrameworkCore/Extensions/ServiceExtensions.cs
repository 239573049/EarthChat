using EarthChat.Auth.Domains;
using EarthChat.Auth.EntityFrameworkCore.Users.Repositories;
using EarthChat.EntityFrameworkCore.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EarthChat.Auth.EntityFrameworkCore;

public static class ServiceExtensions
{
    /// <summary>
    /// 添加 Auth 数据库上下文
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEarthChatDbContext<AuthDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("Default"))
                .EnableDetailedErrors();

            options.UseOpenIddict();
        });

        services.AddIdentity<EarthUser, IdentityRole>()
            .AddEntityFrameworkStores<AuthDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IEarthUserRepository, EarthUserRepository>();

        return services;
    }
}