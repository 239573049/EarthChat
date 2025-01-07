using EarthChat.AuthServer.Domain.Events;
using EarthChat.AuthServer.Domain.LoginLogs;
using EarthChat.BuildingBlocks.Event;
using Gnarly.Data;
using MapsterMapper;

namespace EarthChat.AuthServer.Application.EventHandlers;

/// <summary>
/// 登录事件处理器
/// </summary>
/// <param name="loginLogRepository"></param>
/// <param name="mapper"></param>
[Registration(typeof(IEventHandler<LoginEto>))]
public sealed class LoginHandler(ILoginLogRepository loginLogRepository, IMapper mapper)
    : IEventHandler<LoginEto>, IScopeDependency
{
    public async Task HandleAsync(LoginEto @event)
    {
        var entity = mapper.Map<LoginLog>(@event);

        await loginLogRepository.InsertAsync(entity);

        await loginLogRepository.SaveChangesAsync();
    }
}