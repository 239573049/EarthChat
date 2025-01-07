using EarthChat.Core.Contract;
using EarthChat.Jwt.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EarthChat.Jwt.Extensions;

public static class ServiceExtensions
{
	/// <summary>
	/// 添加Jwt访问
	/// </summary>
	/// <param name="services"></param>
	/// <param name="configuration"></param>
	/// <returns></returns>
	public static IServiceCollection WithJwt(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddHttpContextAccessor();
		services.AddSingleton<IJwtContext, JwtContext>();
		services.AddScoped<IUserContext, UserContext>();

		var option = configuration.GetSection(JwtOptions.Name);

		var jwtOption = option.Get<JwtOptions>();

		services.Configure<JwtOptions>(option);

		services.AddAuthorization()
			.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = jwtOption!.Issuer,
					ValidAudience = jwtOption.Audience,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOption.Secret))
				};
			});

		return services;
	}
}