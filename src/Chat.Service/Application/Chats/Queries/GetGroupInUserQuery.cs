using Chat.Contracts.Chats;

namespace Chat.Service.Application.Chats.Queries;

/// <summary>
/// 获取指定群组的所有成员
/// </summary>
/// <param name="groupId"></param>
public record GetGroupInUserQuery(Guid groupId) : Query<List<ChatGroupInUserDto>>
{
    public override List<ChatGroupInUserDto> Result { get; set; }
}