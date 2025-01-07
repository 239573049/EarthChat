using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace EarthChat.Serilog.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection WithSerilog(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        services.AddLogging(loggingBuilder => { loggingBuilder.AddSerilog(dispose: true); });

        return services;
    }
}