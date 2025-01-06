using EarthChat.Domain.Internal;

namespace EarthChat.AuthServer.Domain.Users;

public class UserOAuth : AuditEntity<Guid, Guid?>
{
    /// <summary>
    /// 获取或设置用户的唯一标识符
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 获取或设置 OAuth 提供者的名称（如 Google, Facebook, etc.）
    /// </summary>
    public string Provider { get; set; }

    /// <summary>
    /// 获取或设置 OAuth 提供者分配给用户的唯一标识符
    /// </summary>
    public string ProviderUserId { get; set; }

    /// <summary>
    /// 获取或设置用户的访问令牌
    /// </summary>
    public string AccessToken { get; set; }

    /// <summary>
    /// 获取或设置访问令牌的过期时间
    /// </summary>
    public DateTimeOffset AccessTokenExpiresAt { get; set; }

    /// <summary>
    /// 获取或设置刷新令牌（如果适用）
    /// </summary>
    public string RefreshToken { get; set; }
}