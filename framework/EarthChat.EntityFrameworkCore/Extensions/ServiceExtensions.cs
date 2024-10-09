using EarthChat.EntityFrameworkCore.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EarthChat.EntityFrameworkCore.Extensions;

public static class ServiceExtensions
{
    /// <summary>
    /// 添加 EarthChat 数据库上下文
    /// </summary>
    public static IServiceCollection AddEarthChatDbContext<TDbContext>(this IServiceCollection services,
        Action<DbContextOptionsBuilder>? optionsAction = null,
        Action<EarthDbContextOptions>? configureOptions = null,
        ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
        ServiceLifetime optionsLifetime = ServiceLifetime.Scoped
    ) where TDbContext : EarthDbContext<TDbContext>
    {
        var v =
            optionsAction == null
                ? (Action<EarthDbContextOptions>)(b => { })
                : (b) => configureOptions?.Invoke(b);

        services.Configure(v);

        services.AddDbContext<TDbContext>(optionsAction, contextLifetime, optionsLifetime);

        return services;
    }
}