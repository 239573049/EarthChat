namespace Chat.Service.Application.Users.Commands;

public record UpdateLocationCommand(Guid UserId,string Ip) : Command;