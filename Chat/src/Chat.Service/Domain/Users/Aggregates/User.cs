namespace Chat.Service.Domain.Users.Aggregates;

public class User : AuditAggregateRoot<Guid, Guid>
{
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

    public Dictionary<string, string> Extends { get; set; }

    protected User()
    {
        Extends = new Dictionary<string, string>();
    }

    public User(Guid id) : base(id)
    {
        Extends = new Dictionary<string, string>();
    }

    public User(string account, string password, string avatar, string name) : this()
    {
        Id = Guid.NewGuid();
        Account = account;
        Password = password;
        Avatar = avatar;
        Name = name;
    }
}