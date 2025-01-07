using EarthChat.Migration;
using EarthChat.Migrations.EntityFrameworkCore.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
#if DEBUG
    .MinimumLevel.Override("Chat", LogEventLevel.Debug)
#else
                .MinimumLevel.Override("Chat", LogEventLevel.Information)
#endif
    .Enrich.FromLogContext()
    .WriteTo.Async(c => c.File("logs/logs.txt"))
    .WriteTo.Async(c => c.Console())
    .CreateLogger();

await Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(((_, builder) =>
    {
        builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    }))
    .ConfigureLogging((_, logging) =>
    {
        logging.ClearProviders();
        logging.AddSerilog(Log.Logger);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<DbMigratorHostedService>();

        services.AddAutoGnarly();

        services.WithMigrationDbAccess(context.Configuration);
    })
    .RunConsoleAsync();