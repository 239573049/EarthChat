using EarthChat.Domain.Internal;

namespace EarthChat.AuthServer.Domain.LoginLogs;

public class LoginLog : Entity<long>
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// 登录令牌
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// 登录时间
    /// </summary>
    public DateTime LoginTime { get; set; }

    /// <summary>
    /// 登录来源
    /// </summary>
    public string? Origin { get; set; }

    /// <summary>
    /// 登录客户端IP
    /// </summary>
    public string? Ip { get; set; }

    /// <summary>
    /// 登录设备信息
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// 登录类型
    /// </summary>
    public string LoginType { get; set; }

    /// <summary>
    /// 登录是否成功
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; set; }
}