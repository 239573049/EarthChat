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
    .WriteTo.Async(c => c.Console(), blockWhenFull: true)
    .CreateLogger();

Log.Logger.Information("Starting DbMigrator...");

Host.CreateDefaultBuilder(args)
    .AddAppSettingsSecretsJson()
    .ConfigureLogging(((context, builder) =>
    {
        builder.ClearProviders();
        
        builder.AddSerilog(Log.Logger);
    }))
    .ConfigureServices(((context, services) =>
    {
        services.AddAuth(context.Configuration);

        services.AddHostedService<DbMigratorHostedService>();

        services.AddHttpContextAccessor();
    }))
    .AddDataSeedContributorService()
    .Build()
    .Run();
    
Log.Logger.Information("DbMigrator started.");