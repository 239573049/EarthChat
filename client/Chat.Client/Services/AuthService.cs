using System.Threading.Tasks;
using Chat.Contracts.Core;
using Chat.Contracts.Users;

namespace Chat.Client.Services;

public class AuthService : IAuthService
{
    public async Task<ResultDto<string>> CreateAsync(string account, string password)
    {
        return await Caller.PostAsync<ResultDto<string>>($"Auths?account={account}&password={password}",null);
    }

    public Task<ResultDto<string>> GithubAuthAsync(string accessToken)
    {
        throw new NotImplementedException();
    }

    public Task<ResultDto<string>> GiteeAuthAsync(string accessToken)
    {
        throw new NotImplementedException();
    }

}