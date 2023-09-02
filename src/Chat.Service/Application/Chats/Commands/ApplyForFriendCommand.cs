using Chat.Contracts.Chats;

namespace Chat.Service.Application.Chats.Commands;

public record ApplyForFriendCommand(ApplyForFriendDto Dto) : Command;