using EarthChat.Identity.Data;
using EarthChat.Identity.Options;
using Microsoft.Extensions.DependencyInjection;

namespace EarthChat.Identity.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services.AddScoped<IUserContext, UserContext>();

        return services;
    }

    public static IServiceCollection AddIdentityOptions(this IServiceCollection services,
        Action<EarthIdentityOptions> configure)
    {
        services.Configure(configure);

        services.AddIdentity();

        return services;
    }
}