namespace Chat.Service.Application.Chats.Queries;

/// <summary>
/// 获取指定群组的所有成员
/// </summary>
/// <param name="GroupId"></param>
/// <param name="Page"></param>
/// <param name="PageSize"></param>
public record GetGroupInUserQuery(Guid GroupId,int Page,int PageSize,Guid[] UserIds) : Query<List<UserDto>>
{
    public override List<UserDto> Result { get; set; }
}