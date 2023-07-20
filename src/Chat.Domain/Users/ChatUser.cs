using Volo.Abp.Identity;

namespace Chat.Users;

public class ChatUser : IdentityUser
{
    public string GiteeId { get; set; }

    public string GitHubId { get; set; }
}