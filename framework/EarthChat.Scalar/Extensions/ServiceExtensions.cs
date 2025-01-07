using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EarthChat.Scalar.Extensions
{
	public static class ServiceExtensions
	{
		public static IServiceCollection WithScalar(this IServiceCollection services)
		{
			services.AddOpenApi();

			return services;
		}

		public static IEndpointRouteBuilder UseScalar(this IEndpointRouteBuilder builder, string title)
		{
			builder.MapOpenApi();

			builder.MapScalarApiReference((options =>
			{
				options.Title = title;
				options.Authentication = new ScalarAuthenticationOptions()
				{
					PreferredSecurityScheme = "Bearer",
				};
			}));

			return builder;
		}
	}
}
