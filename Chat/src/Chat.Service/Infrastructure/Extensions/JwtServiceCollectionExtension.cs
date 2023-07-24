using System.Text;
using Chat.Service.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Chat.Service.Infrastructure.Extensions;

public static class JwtServiceCollectionExtension
{
    /// <summary>
    /// 注册JWT Bearer认证服务的静态扩展方法
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options">JWT授权的配置项</param>
    public static IServiceCollection AddJwtBearerAuthentication(this IServiceCollection services, JwtOptions options)
    {
        //使用应用密钥得到一个加密密钥字节数组
        var key = Encoding.ASCII.GetBytes(options.Secret);
        services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie(cfg => cfg.SlidingExpiration = true)
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var path = context.HttpContext.Request.Path;
                        if (!path.StartsWithSegments("/chatHub")) return Task.CompletedTask;

                        var accessToken = context.Request.Query["access_token"];

                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            // 从查询字符串中读取令牌
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }
}