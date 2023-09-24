namespace Chat.Service.Services;

public class FriendService : BaseService<FriendService>, IFriendService
{
    public Task<bool> FriendState(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task FriendRegistration(FriendRegistrationInput input)
    {
        throw new NotImplementedException();
    }
}