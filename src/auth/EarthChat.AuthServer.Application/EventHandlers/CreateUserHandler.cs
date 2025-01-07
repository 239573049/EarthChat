using EarthChat.AuthServer.Domain.Events;
using EarthChat.BuildingBlocks.Event;

namespace EarthChat.AuthServer.Application.EventHandlers;

/// <summary>
/// 创建用户事件处理器
/// </summary>
public class CreateUserHandler : IEventHandler<CreateUserEto>
{
    public async Task HandleAsync(CreateUserEto @event)
    {
        // 创建用户
        await Task.CompletedTask;
    }
}