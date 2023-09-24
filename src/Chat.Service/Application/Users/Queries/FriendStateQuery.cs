namespace Chat.Service.Application.Users.Queries;

/// <summary>
/// 查询是否为好友
/// </summary>
/// <param name="friendId"></param>
public record FriendStateQuery(Guid friendId):Query<bool>
{
    public override bool Result { get; set; }
}