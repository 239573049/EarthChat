namespace EarthChat.Identity.Abstractions;

public interface IUserContext
{
    string? CurrentUserId { get; }

    /// <summary>
    /// 当前用户的角色列表
    /// </summary>
    IReadOnlyList<string> CurrentUserRoles { get; }

    /// <summary>
    /// 用户实体
    /// </summary>
    T? User<T>() where T : class;
    
    /// <summary>
    /// 当前租户
    /// </summary>
    string? CurrentTenantId { get; }
    
    /// <summary>
    /// 是否授权
    /// </summary>
    bool IsAuthenticated { get; }
}