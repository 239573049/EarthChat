using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Chat.Blazor;

[Dependency(ReplaceServices = true)]
public class ChatBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "Chat";
}
