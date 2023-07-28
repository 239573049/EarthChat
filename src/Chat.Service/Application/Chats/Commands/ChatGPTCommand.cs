namespace Chat.Service.Application.Chats.Commands;

public record ChatGPTCommand(string value,string connectionId) : Command;