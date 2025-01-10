using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EarthChat.FileStorage.Application.Extensions;

public static class ServiceExtensions
{
    /// <summary>
    /// 添加 FileStorageDbContext 数据库访问服务。
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection WithFileStorageDbAccess(this IServiceCollection services,
        IConfiguration configuration)
    {
        return services;
    }
}