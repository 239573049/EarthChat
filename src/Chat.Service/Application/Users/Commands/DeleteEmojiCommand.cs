namespace Chat.Service.Application.Users.Commands;

/// <summary>
/// 删除表情包
/// </summary>
/// <param name="Id"></param>
public record DeleteEmojiCommand(Guid Id) : Command;