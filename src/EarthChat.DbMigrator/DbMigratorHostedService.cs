using EarthChat.Auth.EntityFrameworkCore;
using EarthChat.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EarthChat.DbMigrator;

public class DbMigratorHostedService : IHostedService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DbMigratorHostedService> _logger;

    public DbMigratorHostedService(IConfiguration configuration, IHostApplicationLifetime hostApplicationLifetime,
        IServiceProvider serviceProvider, ILogger<DbMigratorHostedService> logger)
    {
        _configuration = configuration;
        _hostApplicationLifetime = hostApplicationLifetime;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("DbMigrator started.");

        await using var scope = _serviceProvider.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

        await dbContext.Database.MigrateAsync(cancellationToken);

        _logger.LogInformation("Migrated database.");

        // 扫描程序集中的IDataSeedContributor实现类，执行数据初始化
        var dataSeedContributors = scope.ServiceProvider.GetServices<IDataSeedContributor>();

        var dic = _configuration.GetSection("DataSeed").Get<Dictionary<string, string>>();

        var context = new DataSeedContext();

        foreach (var (key, value) in dic)
        {
            context[key] = value;

            _logger.LogInformation($"DataSeedContext: {key} = {value}");
        }

        _logger.LogInformation("Start seeding data...");

        _logger.LogInformation("DataSeedContributors: " +
                               string.Join(", ", dataSeedContributors.Select(x => x.GetType().Name)));

        foreach (var dataSeedContributor in dataSeedContributors)
        {
            await dataSeedContributor.SeedAsync(context);
        }

        // 提交事务
        await dbContext.SaveChangesAsync(cancellationToken);

        _hostApplicationLifetime.StopApplication();

        _logger.LogInformation("DbMigrator completed.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}