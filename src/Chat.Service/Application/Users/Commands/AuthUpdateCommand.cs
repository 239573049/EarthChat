namespace Chat.Service.Application.Users.Commands;

/// <summary>
/// 登录时存在更新用户信息的需求，所以使用Command
/// </summary>
/// <param name="name"></param>
/// <param name="avatar"></param>
public record AuthUpdateCommand(Guid userId,string name,string avatar) : Command;