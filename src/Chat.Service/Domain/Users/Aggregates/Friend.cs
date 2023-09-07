namespace Chat.Service.Domain.Users.Aggregates;

/// <summary>
/// 好友表
/// </summary>
public class Friend : Entity<Guid>
{
    /// <summary>
    /// 好友Id
    /// </summary>
    public Guid FriendId { get; set; }

    /// <summary>
    /// 本人Id
    /// </summary>
    public Guid SelfId { get; set; }

    /// <summary>
    /// 好友备注
    /// </summary>
    public string Remark { get; set; }
}