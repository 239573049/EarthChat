using Chat.Contracts.Chats;

namespace Chat.Service.Application.Chats.Commands;

/// <summary>
/// 创建聊天消息
/// </summary>
/// <param name="Dto"></param>
public record CreateChatMessageCommand(CreateChatMessageDto Dto) : Command;