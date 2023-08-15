using Chat.Contracts.Core;

namespace Chat.Contracts.Users;

public interface IUserService
{
    Task<ResultDto<GetUserDto>> GetAsync();
}