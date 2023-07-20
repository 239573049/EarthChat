using Chat.Localization;
using Volo.Abp.Application.Services;

namespace Chat;

/* Inherit your application services from this class.
 */
public abstract class ChatAppService : ApplicationService
{
    protected ChatAppService()
    {
        LocalizationResource = typeof(ChatResource);
    }
}
