using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Chat;

[Dependency(ReplaceServices = true)]
public class ChatBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "Chat";
}
