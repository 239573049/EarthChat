namespace Chat.Service.Domain.Users.Aggregates;

public class User : AuditAggregateRoot<Guid, Guid>
{
    protected User()
    {
    }

    public User(Guid id) : base(id)
    {
    }

    public User(string account, string password, string avatar, string name) : this()
    {
        Id = Guid.NewGuid();
        Account = account;
        Password = password;
        Avatar = avatar;
        Name = name;
    }

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

    public string? GiteeId { get; set; }

    public string? GithubId { get; set; }
}