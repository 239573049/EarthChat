using System.Security.Cryptography;
using System.Text;
using EarthChat.Domain.Internal;

namespace EarthChat.AuthServer.Domain.Users;

public class User : AuditEntity<Guid, Guid?>
{
    public string Avatar { get; set; }

    /// <summary>
    /// 用户的用户名，必须唯一。
    /// </summary>
    public string Username { get; private set; }

    /// <summary>
    /// 用户的电子邮件地址，必须唯一。
    /// </summary>
    public string Email { get; private set; }

    /// <summary>
    /// 个人签名
    /// </summary>
    public string? Signature { get; set; }

    /// <summary>
    /// 用户的密码哈希值。
    /// </summary>
    public string PasswordHash { get; set; }

    /// <summary>
    /// 用户密码的盐值。
    /// </summary>
    public string PasswordSalt { get; set; }

    /// <summary>
    /// 用户的创建日期。
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// 用户的最后登录日期。
    /// </summary>
    public DateTime? LastLoginAt { get; private set; }

    /// <summary>
    /// 用户的角色列表。
    /// </summary>
    public List<string> Roles { get; private set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 用户构造函数，初始化用户的基本信息。
    /// </summary>
    /// <param name="username">用户名</param>
    /// <param name="email">电子邮件地址</param>
    public User(string username, string email)
    {
        Id = Guid.NewGuid();
        Username = username;
        Email = email;
        CreatedAt = DateTime.UtcNow;
        Roles = new List<string>();
    }

    /// <summary>
    /// 设置用户的密码，生成哈希和盐。
    /// </summary>
    /// <param name="password">用户的明文密码</param>
    public void SetPassword(string password)
    {
        PasswordSalt = GenerateSalt();
        PasswordHash = HashPassword(password, PasswordSalt);
    }

    /// <summary>
    /// 验证用户输入的密码是否正确。
    /// </summary>
    /// <param name="password">用户输入的明文密码</param>
    /// <returns>如果密码正确则返回true，否则返回false</returns>
    public bool VerifyPassword(string password)
    {
        var hash = HashPassword(password, PasswordSalt);
        return hash == PasswordHash;
    }

    /// <summary>
    /// 生成盐值的方法（示例）。
    /// </summary>
    /// <returns>生成的盐值</returns>
    private string GenerateSalt()
    {
        // 实际应用中应使用安全的随机数生成器生成盐值
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }

    /// <summary>
    /// 哈希密码的方法（示例）。
    /// </summary>
    /// <param name="password">明文密码</param>
    /// <param name="salt">盐值</param>
    /// <returns>哈希后的密码</returns>
    private string HashPassword(string password, string salt)
    {
        // 使用md5哈希算法生成密码哈希
        using var md5 = MD5.Create();
        var inputBytes = Encoding.UTF8.GetBytes(password + salt);

        var hashBytes = md5.ComputeHash(inputBytes);

        var sb = new StringBuilder();

        foreach (var t in hashBytes)
        {
            sb.Append(t.ToString("X2"));
        }

        return sb.ToString();
    }

    /// <summary>
    /// 更新用户的最后登录时间。
    /// </summary>
    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }
}