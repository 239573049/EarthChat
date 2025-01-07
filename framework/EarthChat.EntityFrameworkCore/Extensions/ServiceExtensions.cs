using EarthChat.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EarthChat.EntityFrameworkCore.Extensions;

public static class ServiceExtensions
{
    /// <summary>
    /// 添加数据库访问
    /// </summary>
    /// <param name="services"></param>
    /// <param name="optionsAction"></param>
    /// <typeparam name="TDbContext"></typeparam>
    /// <returns></returns>
    public static IServiceCollection WithDbAccess<TDbContext>(this IServiceCollection services,
        Action<DbContextOptionsBuilder> optionsAction)
        where TDbContext : MasterDbContext<TDbContext>
    {
        services.AddDbContext<TDbContext>(optionsAction);

        services.AddScoped<IUnitOfWork, UnitOfWork<TDbContext>>();

        services.AddScoped(typeof(IRepository<,>),typeof(Repository<,>));

        return services;
    }
}