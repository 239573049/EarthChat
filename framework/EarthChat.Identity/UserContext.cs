using System.Security.Claims;
using System.Text.Json;
using EarthChat.Identity.Abstractions;
using EarthChat.Identity.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace EarthChat.Identity;

/// <summary>
/// 用户上下文
/// </summary>
/// <param name="contextAccessor"></param>
/// <param name="options"></param>
public class UserContext(IHttpContextAccessor contextAccessor, IOptions<EarthIdentityOptions> options) : IUserContext
{
    private HttpContext Context => contextAccessor.HttpContext;
    private EarthIdentityOptions Options => options.Value;

    private string? _currentUserId;

    public string? CurrentUserId
    {
        get
        {
            if (string.IsNullOrEmpty(_currentUserId))
            {
                _currentUserId = Context.User.Claims.FirstOrDefault(c => c.Type == Options.IdentityUserId)?.Value;
            }

            return _currentUserId;
        }
    }

    private IReadOnlyList<string> _currentUserRoles = new List<string>();

    public IReadOnlyList<string> CurrentUserRoles
    {
        get
        {
            if (_currentUserId == null)
            {
                _currentUserRoles = Context.User.Claims.Where(c => c.Type == Options.IdentityRoleId)
                    .Select(c => c.Value).ToList();
            }

            return _currentUserRoles;
        }
    }

    public T? User<T>() where T : class
    {
        var user = Context.User.Claims.FirstOrDefault(c => c.Type == Options.IdentityUserClaimId)?.Value;

        return user == null ? null : JsonSerializer.Deserialize<T>(user);
    }

    public string? CurrentTenantId =>
        Context.Request.Headers.TryGetValue(Options.TenantId, out var tenantId) ? tenantId : tenantId;

    public bool IsAuthenticated => Context.User.Identity?.IsAuthenticated ?? false;
}