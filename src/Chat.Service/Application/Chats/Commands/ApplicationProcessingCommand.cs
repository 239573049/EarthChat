using Chat.Contracts.Chats;

namespace Chat.Service.Application.Chats.Commands;

public record ApplicationProcessingCommand(Guid Id, FriendState State) : Command;