using Chat.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;

namespace Chat;

[DependsOn(
    typeof(ChatDomainModule),
    typeof(AbpAccountApplicationModule),
    typeof(ChatApplicationContractsModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule)
    )]
public class ChatApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<ChatApplicationModule>();
        });
        ConfigureMinIo(context);
    }

    private static void ConfigureMinIo(ServiceConfigurationContext context)
    {
        context.Services.AddSingleton(async services =>
        {
            var options = services.GetService<IOptions<MinIOOptions>>();
            var minio = new MinioClient()
               .WithEndpoint(options.Value.Endpoint)
               .WithCredentials(options.Value.AccessKey, options.Value.SecretKey)
               .WithSSL(options.Value.Secure)
               .Build();

            var getListBucketsTask = await minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(options.Value.Bucket)).ConfigureAwait(false);

            if (!getListBucketsTask)
            {
                await minio.MakeBucketAsync(new MakeBucketArgs().WithBucket(options.Value.Bucket));
            }
            return minio;
        });
    }
}
