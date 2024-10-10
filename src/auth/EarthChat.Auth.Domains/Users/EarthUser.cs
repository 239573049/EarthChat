using Microsoft.AspNetCore.Identity;

namespace EarthChat.Auth.Domains;

public class EarthUser : IdentityUser
{
    /// <summary>
    /// Github唯一标识
    /// </summary>
    public string? GithubId { get; set; }
}