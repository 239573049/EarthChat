namespace Chat.Service.Application.Users.Commands;

/// <summary>
/// 添加一个表情包
/// </summary>
/// <param name="path"></param>
public record AddEmojiCommand(string path) : Command;