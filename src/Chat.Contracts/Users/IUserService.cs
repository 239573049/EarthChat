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

    /// <summary>
    /// 更新用户资料
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<ResultDto> UpdateAsync(UpdateUserDto dto);

    /// <summary>
    /// 通过ids回去用户信息。
    /// </summary>
    /// <param name="userIds"></param>
    /// <returns></returns>
    Task<IReadOnlyList<UserDto>> ListAsync(List<Guid> userIds);

    /// <summary>
    /// 获取好友状态
    /// </summary>
    /// <param name="friendId"></param>
    /// <returns></returns>
    Task<ResultDto<bool>> FriendStateAsync(Guid friendId);

    /// <summary>
    /// 好友申请
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task FriendRegistrationAsync(FriendRegistrationInput input);
}