using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace EarthChat.Mapster.Extensions;

public static class ServiceExtensions
{
    /// <summary>
    /// 添加Mapster映射，提供对象映射服务
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection WithMapster(this IServiceCollection services)
    {
        services.AddMapster();
        return services;
    }
}