using EarthChat.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EarthChat.AuthServer.EntityFrameworkCore.Extensions;

public static class ServiceExtensions
{
    /// <summary>
    /// 添加 AuthDbContext 数据库访问服务。
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection WithAuthDbAccess(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.WithDbAccess<AuthDbContext>((builder =>
        {
            builder.UseNpgsql(configuration.GetConnectionString("Default"));
#if DEBUG
            builder.EnableSensitiveDataLogging();
            builder.EnableDetailedErrors();
#endif
        }));
        return services;
    }
}