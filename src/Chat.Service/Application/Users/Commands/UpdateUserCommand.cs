namespace Chat.Service.Application.Users.Commands;

/// <summary>
/// 更新用户事件
/// </summary>
/// <param name="Dto"></param>
public record UpdateUserCommand(UpdateUserDto Dto) : Command;