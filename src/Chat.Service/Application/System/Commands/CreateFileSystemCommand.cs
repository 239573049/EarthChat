namespace Chat.Service.Application.System.Commands;

public record CreateFileSystemCommand(string FileName, string FullName,string uri, long Size) : Command;