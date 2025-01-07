namespace EarthChat.AuthServer.Domain.Events;

/// <summary>
/// 创建用户事件
/// </summary>
/// <param name="UserId"></param>
public record CreateUserEto(Guid UserId);