using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.System.Text.Json;

namespace EarthChat.Redis.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddRedis(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<ISerializer, SystemTextJsonSerializer>();

        services.AddStackExchangeRedisExtensions<SystemTextJsonSerializer>(new RedisConfiguration()
        {
            ConnectionString = configuration.GetConnectionString("Redis"),
            Database = 0,
            ConnectTimeout = 10000,
            ConnectRetry = 3,
        });

        return services;
    }
}