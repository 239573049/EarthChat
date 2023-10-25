using MessagePack;

namespace Chat.Contracts.Users;

[MessagePackObject]
public class GetUserDto
{
    [Key("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// 账号
    /// </summary>
    [Key("account")]
    public string Account { get; set; }

    /// <summary>
    /// 头像
    /// </summary>
    [Key("avatar")]
    public string Avatar { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    [Key("name")]
    public string Name { get; set; }

    /// <summary>
    /// 发送所在位置
    /// </summary>
    public string? Location { get; set; }

}