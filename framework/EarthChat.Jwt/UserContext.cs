using EarthChat.Core.Contract;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace EarthChat.Jwt;

public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public Guid CurrentUserId
    {
        get
        {
            var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Sid)?.Value;

            return string.IsNullOrEmpty(userId) ? Guid.Empty : Guid.Parse(userId);
        }
    }

    public string CurrentUserName
    {
        get { return httpContextAccessor.HttpContext?.User.Identity?.Name; }
    }

    public bool IsAuthenticated => httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

    public string[] Roles
    {
        get
        {
            var roles = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;

            return string.IsNullOrEmpty(roles) ? [] : roles.Split(',');
        }
    }
}