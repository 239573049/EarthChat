using Chat.Contracts.Chats;

namespace Chat.Service.Application.Chats.Queries;

/// <summary>
/// 通过id得到Message
/// </summary>
/// <param name="Id"></param>
public record GetMessageQuery(Guid Id):Query<ChatMessageDto>
{
    public override ChatMessageDto Result { get; set; }
}