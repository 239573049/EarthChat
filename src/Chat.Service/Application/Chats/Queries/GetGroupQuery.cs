using Chat.Contracts.Chats;

namespace Chat.Service.Application.Chats.Queries;

public record GetGroupQuery(Guid id):Query<ChatGroupDto>
{
    public override ChatGroupDto Result { get; set; }
}