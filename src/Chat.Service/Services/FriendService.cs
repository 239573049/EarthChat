using Chat.Service.Application.Users.Commands;
using Microsoft.AspNetCore.Authorization;

namespace Chat.Service.Services;

public class FriendService : BaseService<FriendService>, IFriendService
{
    [Authorize]
    public async Task<ResultDto<bool>> FriendStateAsync(Guid friendId)
    {
        var query = new FriendStateQuery(friendId);

        await PublishAsync(query);

        return new ResultDto<bool>(query.Result);
    }

    [Authorize]
    public async Task<ResultDto> FriendRegistrationAsync(FriendRegistrationInput input)
    {
        var command = new FriendRegistrationCommand(input);

        await PublishAsync(command);

        return new ResultDto();
    }

    public Task<ResultDto<List<FriendRequestDto>>> GetListAsync(int page, int pageSize)
    {
        throw new NotImplementedException();
    }
}