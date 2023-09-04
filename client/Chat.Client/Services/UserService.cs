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
}