using EarthChat.Auth.EntityFrameworkCore;
using EarthChat.DbMigrator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Volo.Abp", LogEventLevel.Warning)
#if DEBUG
    .MinimumLevel.Override("Raccoon", LogEventLevel.Debug)
#else
                .MinimumLevel.Override("Raccoon", LogEventLevel.Information)
#endif
    .Enrich.FromLogContext()
    .WriteTo.Async(c => c.File("Logs/logs.txt"))
    .WriteTo.Async(c => c.Console())
    .CreateLogger();


Host.CreateDefaultBuilder(args)
    .AddAppSettingsSecretsJson()
    .ConfigureLogging((context, logging) => logging.ClearProviders())
    .ConfigureServices(((context, services) =>
    {
        services.AddAuth(context.Configuration);

        services.AddHostedService<DbMigratorHostedService>();

        services.AddHttpContextAccessor();
    }))
    .Build()
    .Run();