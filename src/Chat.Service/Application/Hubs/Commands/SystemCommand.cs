using Chat.Contracts.Hubs;

namespace Chat.Service.Application.Hubs.Commands;

/// <summary>
/// 发送系统通知
/// </summary>
/// <param name="Notification"></param>
public record SystemCommand(Notification Notification,Guid[] ids,bool group) : Command;