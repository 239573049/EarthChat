using Volo.Abp.Modularity;

namespace Chat;

[DependsOn(
    typeof(ChatApplicationModule),
    typeof(ChatDomainTestModule)
    )]
public class ChatApplicationTestModule : AbpModule
{

}
