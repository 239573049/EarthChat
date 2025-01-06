using EarthChat.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EarthChat.AuthServer.EntityFrameworkCore.Extensions;

public static class ServiceExtensions
{
    /// <summary>
    /// 添加 AuthDbContext 数据库访问服务。
    /// </summary>
    /// <param name="services"></param>
    /// <param name="optionsAction"></param>
    /// <returns></returns>
    public static IServiceCollection WithAuthDbAccess(this IServiceCollection services,
        Action<DbContextOptionsBuilder> optionsAction)
    {
        services.WithDbAccess<AuthDbContext>(optionsAction);
        return services;
    }
}