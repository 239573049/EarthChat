using Masa.BuildingBlocks.Ddd.Domain.Entities.Auditing;
namespace Chat.Service.Domain.Users.Aggregates;

public class User : AuditAggregateRoot<int, int>
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

    public Dictionary<string,string> Extends { get; set; }

    protected User()
    {
        Extends = new Dictionary<string, string>();
    }

    public User(int id) : base(id)
    {
        Extends = new Dictionary<string, string>();
    }
}