using Chat.Contracts.Core;

namespace Chat.Contracts.Users;

public interface IUserService
{
    Task<ResultDto<GetUserDto>> GetAsync();

    /// <summary>
    /// 创建新用户
    /// </summary>
    /// <returns></returns>
    Task<ResultDto> CreateAsync(CreateUserDto dto);
}