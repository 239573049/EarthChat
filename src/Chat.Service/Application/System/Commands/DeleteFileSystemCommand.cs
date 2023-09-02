namespace Chat.Service.Application.System.Commands;

/// <summary>
/// 根据Uri删除文件
/// </summary>
/// <param name="Uri"></param>
public record DeleteFileSystemCommand(string Uri): Command;