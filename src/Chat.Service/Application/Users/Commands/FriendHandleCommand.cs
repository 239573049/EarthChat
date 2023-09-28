using Chat.Contracts.Chats;

namespace Chat.Service.Application.Users.Commands;

/// <summary>
/// 拒绝或同意申请
/// </summary>
/// <param name="Id"></param>
/// <param name="State"></param>
public record FriendHandleCommand(Guid Id,FriendState State):Command;