namespace Chat.Service.Application.Chats.Commands;

/// <summary>
/// 撤回消息事件
/// </summary>
/// <param name="Id">消息id</param>
public record CountermandCommand(Guid Id) : Command;