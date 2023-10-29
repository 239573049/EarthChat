using Chat.Contracts.Chats;

namespace Chat.Service.Application.Chats.Queries;

/// <summary>
/// 获取指定用户所在群组
/// </summary>
public record GetUserGroupQuery(Guid userId, bool? group) : Query<IReadOnlyList<ChatGroupDto>>
{
    public override IReadOnlyList<ChatGroupDto> Result { get; set; }
}