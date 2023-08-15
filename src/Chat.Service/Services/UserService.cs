using Chat.Service.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Chat.Service.Services;

public class UserService : BaseService<UserService>,IUserService
{
    [Authorize]
    public async Task<ResultDto<GetUserDto>> GetAsync()
    {
        var user = GetRequiredService<IUserContext>();
        var query = new GetUserQuery(user.GetUserId<Guid>());
        await _eventBus.PublishAsync(query);
        return query.Result.CreateResult();
    }
}