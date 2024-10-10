using EarthChat.Auth.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EarthChat.DbMigrator;

public class DbMigratorHostedService : IHostedService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public DbMigratorHostedService(IConfiguration configuration, IHostApplicationLifetime hostApplicationLifetime, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _hostApplicationLifetime = hostApplicationLifetime;
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        
        var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        
        await dbContext.Database.MigrateAsync(cancellationToken);
        
        _hostApplicationLifetime.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}