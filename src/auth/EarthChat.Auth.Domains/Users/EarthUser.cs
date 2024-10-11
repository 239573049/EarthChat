using System.Text.RegularExpressions;
using EarthChat.Core.System.Extensions;
using Microsoft.AspNetCore.Identity;

namespace EarthChat.Auth.Domains;

public class EarthUser : IdentityUser
{
    private string _userName = null!;

    public override string? UserName
    {
        get => _userName;
        set
        {
            _userName = value ?? throw new ArgumentNullException(nameof(value));

            // 用户名必须是字母和数字的组合，使用正则表达式判断
            if(Regex.IsMatch(value, @"[^a-zA-Z0-9]"))
            {
                throw new ArgumentException("用户名必须是字母和数字的组合", nameof(value));
            }
            
            // 用户名长度不能小于3位
            if (value.Length < 3)
            {
                throw new ArgumentException("用户名长度不能小于3位", nameof(value));
            }

            // 用户名长度不能大于20位
            if (value.Length > 20)
            {
                throw new ArgumentException("用户名长度不能大于20位", nameof(value));
            }

            // 用户名不能包含特殊字符
            if (value.Any(char.IsPunctuation))
            {
                throw new ArgumentException("用户名不能包含特殊字符", nameof(value));
            }

            // 用户名不能包含空格
            if (value.Any(char.IsWhiteSpace))
            {
                throw new ArgumentException("用户名不能包含空格", nameof(value));
            }

            NormalizedUserName = value.ToUpperInvariant();
        }
    }

    public override string? NormalizedUserName { get; set; }

    /// <summary>
    /// Github唯一标识
    /// </summary>
    public string? GithubId { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; private set; }

    public void SetGithubId(string? githubId)
    {
        GithubId = githubId;
    }

    public void SetPassword(string? password)
    {
        if (password.IsNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(password));
        }

        // 密码必须满足复杂度要求
        if (password.Length < 6)
        {
            throw new ArgumentException("密码长度不能小于6位", nameof(password));
        }

        if (password.Length > 20)
        {
            throw new ArgumentException("密码长度不能大于20位", nameof(password));
        }

        // 密码必须是字母和数字和特殊字符的组合
        if (!password.Any(char.IsDigit) || !password.Any(char.IsLetter) || !password.Any(char.IsPunctuation))
        {
            throw new ArgumentException("密码必须是字母和数字和特殊字符的组合", nameof(password));
        }

        Password = password;
        PasswordHash = password.ToMd5();
        SecurityStamp = Guid.NewGuid().ToString("N");
    }
}