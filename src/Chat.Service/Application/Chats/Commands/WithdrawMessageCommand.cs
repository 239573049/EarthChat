namespace Chat.Service.Application.Chats.Commands;

public record WithdrawMessageCommand(Guid Id) : Command;