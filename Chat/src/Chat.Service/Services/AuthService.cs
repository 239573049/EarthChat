using System.Net.Http.Headers;
using Chat.Contracts.Core;
using Chat.Contracts.Users;
using Chat.Service.Application.Users.Commands;
using Chat.Service.Application.Users.Queries;
using Chat.Service.Infrastructure.Helper;
using Chat.Service.Options;
using Microsoft.Extensions.Options;

namespace Chat.Service.Services;

public class AuthService : BaseService<AuthService>
{
    public async Task<ResultDto<string>> CreateAsync(string account, string password)
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

    public async Task<ResultDto<string>> GithubAuthAsync(string accessToken)
    {
        using var http = new HttpClient();
        var github = GetOptions<GithubOptions>();
        var response =
            await http.GetAsync(
                $"https://api.github.com/user?access_token={accessToken}&client_id={github?.ClientId}&client_secret={github?.ClientSecrets}");
        var result = await response.Content.ReadFromJsonAsync<GitTokenDto>();
        if (result is null)
        {
            throw new Exception("Github授权失败");
        }

        http.DefaultRequestHeaders.Add("User-Agent", "Chat");
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.access_token);
        var githubUser = await http.GetFromJsonAsync<GithubUserDto>("https://api.github.com/user");
        if (githubUser is null)
        {
            throw new Exception("Github授权失败");
        }

        var query = new AuthTypeQuery("Github", githubUser.id.ToString());
        await _eventBus.PublishAsync(query);

        if (query.Result is null)
        {
            var command = new CreateUserCommand(new CreateUserDto()
            {
                Account = "guest_" + StringHelper.RandomString(8),
                Avatar = githubUser.avatar_url,
                Name = githubUser.name,
                Extends = new Dictionary<string, string> { { "Github", githubUser.id.ToString() } }
            });

            await _eventBus.PublishAsync(command);
            query.Result = command.Result;
        }

        var claims = JwtHelper.GetClaimsIdentity(query.Result);
        var jwt = GetService<IOptions<JwtOptions>>()?.Value;

        var token = JwtHelper.GeneratorAccessToken(claims, jwt);

        return new ResultDto<string>
        {
            Code = "200",
            Message = "登录成功",
            Data = token
        };
    }


    public async Task<ResultDto<string>> GiteeAuthAsync(string accessToken,
        string redirect_uri = "http://localhost:3000/login?type=gitee")
    {
        using var http = new HttpClient();
        var gitee = GetOptions<GiteeOptions>();
        var response =
            await http.GetAsync(
                $"https://gitee.com/oauth/token?grant_type=authorization_code&redirect_uri={redirect_uri}&code={accessToken}&client_id={gitee.ClientId}&client_secret={gitee.ClientSecrets}");
        var result = await response.Content.ReadFromJsonAsync<GitTokenDto>();
        if (result is null)
        {
            throw new Exception("Gitee授权失败");
        }

        var githubUser =
            await http.GetFromJsonAsync<GiteeDto>("https://gitee.com/api/v5/user?access_token=" + result.access_token);
        if (githubUser is null)
        {
            throw new Exception("Gitee授权失败");
        }

        var query = new AuthTypeQuery("Gitee", githubUser.id.ToString());
        await _eventBus.PublishAsync(query);

        if (query.Result is null)
        {
            var command = new CreateUserCommand(new CreateUserDto()
            {
                Account = "guest_" + StringHelper.RandomString(8),
                Avatar = githubUser.avatar_url,
                Name = githubUser.name,
                Extends = new Dictionary<string, string> { { "Gitee", githubUser.id.ToString() } }
            });

            await _eventBus.PublishAsync(command);
            query.Result = command.Result;
        }

        var claims = JwtHelper.GetClaimsIdentity(query.Result);
        var jwt = GetService<IOptions<JwtOptions>>()?.Value;

        var token = JwtHelper.GeneratorAccessToken(claims, jwt);

        return new ResultDto<string>
        {
            Code = "200",
            Message = "登录成功",
            Data = token
        };
    }
}