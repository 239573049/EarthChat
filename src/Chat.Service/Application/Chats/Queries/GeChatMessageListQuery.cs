using Chat.Contracts.Chats;

namespace Chat.Service.Application.Chats.Queries;

/// <summary>
/// 获取聊天消息列表
/// </summary>
/// <param name="page"></param>
/// <param name="pageSize"></param>
public record GeChatMessageListQuery(Guid groupId,int page, int pageSize) : Query<PaginatedListBase<ChatMessageDto>>
{
    public override PaginatedListBase<ChatMessageDto> Result { get; set; }
}