using Chat.Contracts.Chats;

namespace Chat.Service.Application.Chats.Commands;

public record CreateChatMessageCommand(CreateChatMessageDto Dto) : Command;