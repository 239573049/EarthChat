using Chat.Service.Application.Users.Commands;
using Microsoft.AspNetCore.Authorization;

namespace Chat.Service.Services;

public class UserService : BaseService<UserService>, IUserService
{
    [Authorize]
    public async Task<ResultDto<GetUserDto>> GetAsync()
    {
        var user = GetRequiredService<IUserContext>();
        var query = new GetUserQuery(user.GetUserId<Guid>());
        await _eventBus.PublishAsync(query);

        // 未获取到用户消息则直接401
        if (query.Result == null)
        {
            throw new UnauthorizedAccessException();
        }

        return query.Result.CreateResult();
    }

    public async Task<ResultDto> CreateAsync(CreateUserDto dto)
    {
        dto.Avatar = "/favicon.png";
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

    public async Task<IReadOnlyList<UserDto>> ListAsync([FromBody]List<Guid> userIds)
    {
        var query = new GetUserListQuery(userIds);
        await PublishAsync(query);

        if (userIds.Any(x => x == Guid.Empty))
        {
            query.Result.Add(new UserDto()
            {
                Id = Guid.Empty,
                Account = "chat_ai",
                Avatar = "https://blog-simple.oss-cn-shenzhen.aliyuncs.com/ai.png",
                Name = "聊天机器人",
            });
        }
        
        return query.Result;
    }
}