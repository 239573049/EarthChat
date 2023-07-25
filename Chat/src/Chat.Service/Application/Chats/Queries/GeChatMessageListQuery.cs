using Chat.Contracts.Chats;

namespace Chat.Service.Application.Chats.Queries;

public record GeChatMessageListQuery(int page, int pageSize) : Query<PaginatedListBase<ChatMessageDto>>
{
    public override PaginatedListBase<ChatMessageDto> Result { get; set; }
}