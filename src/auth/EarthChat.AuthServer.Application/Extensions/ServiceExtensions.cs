using EarthChat.Mapster.Extensions;
using Lazy.Captcha.Core.Generator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EarthChat.AuthServer.Application.Extensions;

public static class ServiceExtensions
{
    /// <summary>
    /// 添加认证应用服务，提供授权核心业务
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection WithAuthApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCaptcha(options =>
        {
            options.CaptchaType = CaptchaType.ARITHMETIC;

            options.ExpirySeconds = 300;
            options.IgnoreCase = true;
            options.StoreageKeyPrefix = "EarthChat:Auth:Captcha";
        });

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = "captcha:";
        });

        services.WithMapster();


        return services;
    }
}