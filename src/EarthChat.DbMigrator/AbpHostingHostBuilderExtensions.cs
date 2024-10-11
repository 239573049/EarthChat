using EarthChat.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EarthChat.DbMigrator;

public static class AbpHostingHostBuilderExtensions
{
    public const string AppSettingsSecretJsonPath = "appsettings.secrets.json";

    public static IHostBuilder AddAppSettingsSecretsJson(
        this IHostBuilder hostBuilder,
        bool optional = true,
        bool reloadOnChange = true,
        string path = "appsettings.secrets.json")
    {
        return hostBuilder.ConfigureAppConfiguration(
            (Action<HostBuilderContext, IConfigurationBuilder>)((_, builder) =>
                builder.AddJsonFile(path, optional, reloadOnChange)));
    }

    public static IHostBuilder AddDataSeedContributorService(this IHostBuilder hostBuilder)
    {
        return hostBuilder.ConfigureServices(((context, services) =>
        {
            // 扫描程序集中的IDataSeedContributor实现类，执行数据初始化,命名规范：xxx.Domains
            var assembly = AppDomain.CurrentDomain.GetAssemblies()
                .ToArray();

            var dataSeedContributors = assembly.SelectMany(x => x.GetTypes())
                .Where(x => x is { IsClass: true, IsAbstract: false } && x.GetInterfaces().Contains(typeof(IDataSeedContributor)));

            foreach (var dataSeedContributor in dataSeedContributors)
            {
                services.AddScoped(typeof(IDataSeedContributor), dataSeedContributor);
            }
        }));
    }
}