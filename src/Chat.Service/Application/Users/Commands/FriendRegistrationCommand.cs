namespace Chat.Service.Application.Users.Commands;

/// <summary>
/// 添加好友
/// </summary>
/// <param name="Input"></param>
public record FriendRegistrationCommand(FriendRegistrationInput Input) : Command;