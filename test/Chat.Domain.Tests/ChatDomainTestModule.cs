using Chat.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Chat;

[DependsOn(
    typeof(ChatEntityFrameworkCoreTestModule)
    )]
public class ChatDomainTestModule : AbpModule
{

}
