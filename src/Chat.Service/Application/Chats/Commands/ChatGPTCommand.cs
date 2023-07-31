namespace Chat.Service.Application.Chats.Commands;

/// <summary>
/// 智能聊天
/// </summary>
/// <param name="value"></param>
/// <param name="connectionId"></param>
public record ChatGPTCommand(string value,string connectionId) : Command;