using System.Diagnostics;
using System.Runtime.InteropServices;
using EarthChat.Domain.Data;
using EarthChat.Migrations.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EarthChat.Migration;

public class DbMigratorHostedService(
    ILogger<DbMigratorHostedService> logger,
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        {
            logger.LogInformation("迁移文件开始");

            var initialMigrationAdded = AddInitialMigrationIfNotExist();

            if (initialMigrationAdded)
            {
                hostApplicationLifetime.StopApplication();
                return;
            }

            await using var scope = serviceProvider.CreateAsyncScope();

            var migrator = scope.ServiceProvider.GetServices<IDbSchemaMigrator>();

            var databaseDbSchemaMigrators = scope.ServiceProvider.GetService<DatabaseDbSchemaMigrator>();

            await databaseDbSchemaMigrators!.MigrateAsync();

            foreach (var dbSchemaMigrator in migrator)
            {
                await dbSchemaMigrator.MigrateAsync();

                logger.LogInformation($"迁移文件{dbSchemaMigrator.GetType().Name}完成");
            }

            logger.LogInformation("迁移文件结束");
        }


        hostApplicationLifetime.StopApplication();
    }


    private bool AddInitialMigrationIfNotExist()
    {
        try
        {
            if (!DbMigrationsProjectExists())
            {
                return false;
            }
        }
        catch (Exception)
        {
            return false;
        }

        try
        {
            if (!MigrationsFolderExists())
            {
                AddInitialMigration();
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception e)
        {
            logger.LogWarning("Couldn't determinate if any migrations exist : " + e.Message);
            return false;
        }
    }

    private void AddInitialMigration()
    {
        logger.LogInformation("创建初始化迁移文件...");

        string argumentPrefix;
        string fileName;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            argumentPrefix = "-c";
            fileName = "/bin/bash";
        }
        else
        {
            argumentPrefix = "/C";
            fileName = "cmd.exe";
        }

        var procStartInfo = new ProcessStartInfo(fileName,
            $"{argumentPrefix} \"dotnet ef migrations add Initial -p {GetEntityFrameworkCoreProjectFolderPath()} -s {GetEntityFrameworkCoreProjectFolderPath(true)}\" --context MigrationDbContext");

        try
        {
            var process = Process.Start(procStartInfo);

            process!.WaitForExit();
        }
        catch (Exception)
        {
            throw new Exception("无法创建初始化迁移文件！");
        }
    }

    private bool MigrationsFolderExists()
    {
        var dbMigrationsProjectFolder = GetEntityFrameworkCoreProjectFolderPath();
        return dbMigrationsProjectFolder != null &&
               Directory.Exists(Path.Combine(dbMigrationsProjectFolder, "Migrations"));
    }

    private string? GetSolutionDirectoryPath()
    {
        var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (currentDirectory != null && Directory.GetParent(currentDirectory.FullName) != null)
        {
            currentDirectory = Directory.GetParent(currentDirectory.FullName);

            if (currentDirectory != null &&
                Directory.GetFiles(currentDirectory.FullName).FirstOrDefault(f => f.EndsWith(".sln")) != null)
            {
                return currentDirectory.FullName;
            }
        }

        return null;
    }

    private bool DbMigrationsProjectExists()
    {
        var dbMigrationsProjectFolder = GetEntityFrameworkCoreProjectFolderPath();

        return dbMigrationsProjectFolder != null;
    }

    private string? GetEntityFrameworkCoreProjectFolderPath(bool isStartup = false)
    {
        var slnDirectoryPath = GetSolutionDirectoryPath();

        if (slnDirectoryPath == null)
        {
            throw new Exception("未找到解决方案文件夹！");
        }

        var srcDirectoryPath = Path.Combine(slnDirectoryPath, "src");

        if (isStartup)
        {
            return Directory.GetDirectories(srcDirectoryPath)
                .FirstOrDefault(d => d.EndsWith(".Migration"));
        }

        return Directory.GetDirectories(srcDirectoryPath)
            .FirstOrDefault(d => d.EndsWith(".Migrations.EntityFrameworkCore"));
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}