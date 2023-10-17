using Chat.Service.Application.Users.Commands;
using Microsoft.AspNetCore.Authorization;

namespace Chat.Service.Services;

public class UserService : BaseService<UserService>, IUserService
{
    [Authorize]
    public async Task<ResultDto<GetUserDto>> GetAsync()
    {
        var user = GetRequiredService<IUserContext>();
        var userId = user.GetUserId<Guid>();

        // 未获取到用户id则直接401
        if (userId == Guid.Empty)
        {
            throw new UnauthorizedAccessException();
        }

        var query = new GetUserQuery(userId);
        await PublishAsync(query);

        // 未获取到用户消息则直接401
        if (query.Result == null)
        {
            throw new UnauthorizedAccessException();
        }

        return query.Result.CreateResult();
    }

    public async Task<ResultDto> CreateAsync(CreateUserDto dto)
    {
        dto.Avatar = Constant.User.DefaultAvatar;
        var command = new CreateUserCommand(dto);
        await PublishAsync(command);

        return new ResultDto();
    }

    [Authorize]
    public async Task<ResultDto> UpdateAsync(UpdateUserDto dto)
    {
        var command = new UpdateUserCommand(dto);
        await PublishAsync(command);

        return new ResultDto();
    }

    [Authorize]
    public async Task<List<UserDto>> ListAsync([FromBody] List<Guid> userIds)
    {
        var query = new GetUserListQuery(userIds);
        await PublishAsync(query);

        return query.Result;
    }
}