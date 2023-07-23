using Chat.Contracts.Core;
using Chat.Service.Application.Users.Queries;
using Chat.Service.Infrastructure.Helper;
using Chat.Service.Options;
using Microsoft.Extensions.Options;

namespace Chat.Service.Services;

public class AuthService : ChatService<AuthService>
{
    public async Task<ResultDto<string>> AuthAsync(string account, string password)
    {
        var query = new AuthQuery(account, password);
        await _eventBus.PublishAsync(query);

        var jwt = GetService<IOptions<JwtOptions>>()?.Value;

        var claims = JwtHelper.GetClaimsIdentity(query.Result);

        var token = JwtHelper.GeneratorAccessToken(claims, jwt);

        return new ResultDto<string>
        {
            Code = "200",
            Message = "登录成功",
            Data = token
        };
    }
}