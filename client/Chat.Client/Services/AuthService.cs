using System.Threading.Tasks;
using Chat.Contracts.Core;
using Chat.Contracts.Users;

namespace Chat.Client.Services;

public class AuthService : IAuthService
{
    public Task<ResultDto<string>> CreateAsync(string account, string password)
    {
        throw new NotImplementedException();
    }

    public Task<ResultDto<string>> GithubAuthAsync(string accessToken)
    {
        throw new NotImplementedException();
    }

    public Task<ResultDto<string>> GiteeAuthAsync(string accessToken)
    {
        throw new NotImplementedException();
    }

    public Task<ResultDto<string>> LoginAsync(string account, string password)
    {
        throw new NotImplementedException();
    }
}