using Chat.Localization;
using Volo.Abp.AspNetCore.Components;

namespace Chat.Blazor;

public abstract class ChatComponentBase : AbpComponentBase
{
    protected ChatComponentBase()
    {
        LocalizationResource = typeof(ChatResource);
    }
}
