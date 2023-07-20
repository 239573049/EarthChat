using Chat.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Chat.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class ChatController : AbpControllerBase
{
    protected ChatController()
    {
        LocalizationResource = typeof(ChatResource);
    }
}
