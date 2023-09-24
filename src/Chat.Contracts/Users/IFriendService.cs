using Chat.Contracts.Core;

namespace Chat.Contracts.Users;

public interface IFriendService
{
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