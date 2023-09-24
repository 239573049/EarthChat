namespace Chat.Contracts.Users;

public interface IFriendService
{
    Task<bool> FriendState(Guid id);

    /// <summary>
    /// 添加好友
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task FriendRegistration(FriendRegistrationInput input);


}