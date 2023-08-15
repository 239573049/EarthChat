using Chat.Contracts.Core;

namespace Chat.Contracts.Users;

public interface IAuthService
{
    Task<ResultDto<string>> CreateAsync(string account, string password);
    
    Task<ResultDto<string>> GithubAuthAsync(string accessToken);
    
    Task<ResultDto<string>> GiteeAuthAsync(string accessToken);
    
    Task<ResultDto<string>> LoginAsync(string account, string password);
}