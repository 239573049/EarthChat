namespace Chat.Contracts.Users;

public class UserDto
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// 账号
    /// </summary>
    public string Account { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// 头像
    /// </summary>
    public string Avatar { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    public string Name { get; set; }

    public Dictionary<string,string> Extends { get; set; }
}