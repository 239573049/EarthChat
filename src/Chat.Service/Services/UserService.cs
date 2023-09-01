using Chat.Service.Application.Users.Commands;
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

    public async Task<ResultDto> CreateAsync(CreateUserDto dto)
    {
        var command = new CreateUserCommand(dto);
        await PublishAsync(command);

        return new ResultDto();
    }
}