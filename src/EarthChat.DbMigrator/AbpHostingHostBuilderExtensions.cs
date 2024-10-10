using Microsoft.Extensions.Configuration;
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
}