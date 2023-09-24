using System.Collections.Generic;
using System.Threading.Tasks;
using Chat.Contracts.Core;
using Chat.Contracts.Users;

namespace Chat.Client.Services;

public class UserService : IUserService
{
    public Task<ResultDto<GetUserDto>> GetAsync()
    {
        return Caller.GetAsync<ResultDto<GetUserDto>>("Users");
    }

    public Task<ResultDto> CreateAsync(CreateUserDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<ResultDto> UpdateAsync(UpdateUserDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<UserDto>> ListAsync(List<Guid> userIds)
    {
        throw new NotImplementedException();
    }

    public Task<ResultDto<bool>> FriendStateAsync(Guid friendId)
    {
        throw new NotImplementedException();
    }

    public Task FriendRegistrationAsync(FriendRegistrationInput input)
    {
        throw new NotImplementedException();
    }
}