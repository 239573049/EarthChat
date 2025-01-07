using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using EarthChat.AuthServer.Application.Contract.Authorize;
using EarthChat.AuthServer.Application.Contract.Authorize.Dto;
using EarthChat.AuthServer.Application.Contract.Authorize.Input;
using EarthChat.AuthServer.Domain.Events;
using EarthChat.AuthServer.Domain.Users;
using EarthChat.BuildingBlocks.Event;
using EarthChat.Core.Contract;
using EarthChat.Core.Exceptions;
using EarthChat.Domain;
using Gnarly.Data;
using Lazy.Captcha.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace TokenAI.Site.Application.Authorize;

public class AuthorizeService(
    IHttpContextAccessor httpContextAccessor,
    IRepository<User> userRepository,
    IRepository<UserOAuth> userOAuthRepository,
    IEventBus<CreateUserEto> eventBus,
    IEventBus<LoginEto> loginEventBus,
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    IJwtContext jwtContext,
    ICaptcha captcha)
    : IAuthorizeService, IScopeDependency
{
    public async Task<string> TokenAsync(TokenInput input)
    {
        var httpContext = httpContextAccessor.HttpContext;

        if (!captcha.Validate(input.CaptchaKey, input.CaptchaCode))
        {
            // 发布登录事件
            await loginEventBus.PublishAsync(new LoginEto(null, input.Username, null, DateTime.Now,
                httpContext.Request.Headers["Origin"].ToString(), GetClientIp(),
                httpContext.Request.Headers["User-Agent"].ToString(), "Password", true, "验证码错误"));
            throw new BusinessException("验证码错误!");
        }

        var user = await userRepository.FirstOrDefaultAsync(x => x.Username == input.Username);

        if (user == null)
        {
            // 发布登录事件
            await loginEventBus.PublishAsync(new LoginEto(user?.Id, input.Username, null, DateTime.Now,
                httpContext.Request.Headers["Origin"].ToString(), GetClientIp(),
                httpContext.Request.Headers["User-Agent"].ToString(), "Password", true, "用户不存在"));
            throw new BusinessException("用户不存在");
        }

        if (!user.VerifyPassword(input.Password))
        {
            // 发布登录事件
            await loginEventBus.PublishAsync(new LoginEto(user.Id, user.Username, null, DateTime.Now,
                httpContext.Request.Headers["Origin"].ToString(), GetClientIp(),
                httpContext.Request.Headers["User-Agent"].ToString(), "Password", true, "密码错误"));
            throw new BusinessException("密码错误");
        }

        var userDit = new Dictionary<string, string>();

        user.PasswordHash = null;
        user.PasswordSalt = null;

        userDit.Add("UserInfo", JsonSerializer.Serialize(user, JsonSerializerOptions.Web));

        var token = jwtContext.CreateToken(userDit, user.Id, user.Roles.ToArray());

        // 发布登录事件
        await loginEventBus.PublishAsync(new LoginEto(user.Id, user.Username, token, DateTime.Now,
            httpContext.Request.Headers["Origin"].ToString(), GetClientIp(),
            httpContext.Request.Headers["User-Agent"].ToString(), "Password", true, null));

        return token;
    }

    private string GetClientIp()
    {
        var httpContext = httpContextAccessor.HttpContext;
        string ip = string.Empty;
        // 如果请求头中有代理服务器的ip地址，则取第一个ip地址
        if (httpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            ip = httpContext.Request.Headers["X-Forwarded-For"].ToString().Split(',')[0];
        }
        else
        {
            ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
        }

        return ip;
    }

    public async Task<string> OAuthTokenAsync(string type, string code, string state, string? redirectUri = null)
    {
        var client = httpClientFactory.CreateClient(nameof(AuthorizeService));

        OAuthUserDto? userDto = null;
        var httpContext = httpContextAccessor.HttpContext;

        // 这里需要处理第三方登录的逻辑
        if (type.Equals("github"))
        {
            // 处理github登录
            var clientId = configuration["OAuth:Github:ClientId"];
            var clientSecret = configuration["OAuth:Github:ClientSecret"];

            var response =
                await client.PostAsync(
                    $"https://github.com/login/oauth/access_token?code={code}&client_id={clientId}&client_secret={clientSecret}",
                    null);

            var result = await response.Content.ReadFromJsonAsync<OAuthTokenDto>();
            if (result is null)
            {
                // 发布登录事件
                await loginEventBus.PublishAsync(new LoginEto(null, null, null, DateTime.Now,
                    httpContext.Request.Headers["Origin"].ToString(), GetClientIp(),
                    httpContext.Request.Headers["User-Agent"].ToString(), "OAuth", true, "Github授权失败"));
                throw new Exception("Github授权失败");
            }

            var request = new HttpRequestMessage(HttpMethod.Get,
                $"https://api.github.com/user")
            {
                Headers =
                {
                    Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken)
                }
            };

            var responseMessage = await client.SendAsync(request);

            userDto = await responseMessage.Content.ReadFromJsonAsync<OAuthUserDto>();
        }
        else if (type.Equals("gitee"))
        {
            // 处理github登录
            var clientId = configuration["OAuth:Gitee:ClientId"];
            var clientSecret = configuration["OAuth:Gitee:ClientSecret"];

            var response =
                await client.PostAsync(
                    $"https://gitee.com/oauth/token?grant_type=authorization_code&redirect_uri={redirectUri}&response_type=code&code={code}&client_id={clientId}&client_secret={clientSecret}",
                    null);

            var result = await response.Content.ReadFromJsonAsync<OAuthTokenDto>();
            if (result?.AccessToken is null)
            {
                // 发布登录事件
                await loginEventBus.PublishAsync(new LoginEto(null, null, null, DateTime.Now,
                    httpContext.Request.Headers["Origin"].ToString(), GetClientIp(),
                    httpContext.Request.Headers["User-Agent"].ToString(), "OAuth", true, "Gitee授权失败"));
                throw new Exception("Gitee授权失败");
            }


            var request = new HttpRequestMessage(HttpMethod.Get,
                $"https://gitee.com/api/v5/user?access_token=" + result.AccessToken);

            var responseMessage = await client.SendAsync(request);

            userDto = await responseMessage.Content.ReadFromJsonAsync<OAuthUserDto>();
        }

        // 获取是否存在当前渠道
        var oauth = await userOAuthRepository.FirstOrDefaultAsync(x =>
            x.Provider == type && x.ProviderUserId == userDto.Id.ToString());

        User user;

        if (oauth == null)
        {
            // 如果邮箱是空则随机生成
            if (string.IsNullOrEmpty(userDto.Email))
            {
                userDto.Email = "oauth_" + userDto.Id + "@token-ai.cn";
            }


            // 创建一个新的用户
            user = new User((userDto?.Name ?? userDto?.Id.ToString()), userDto.Email);

            user.SetPassword("Aa123456.");
            user.Avatar = userDto.AvatarUrl ?? "🦜";
            user.Roles.Add(RoleConstant.User);
            user.UpdateLastLogin();

            oauth = new UserOAuth()
            {
                AccessToken = string.Empty,
                Provider = type,
                Id = Guid.NewGuid(),
                UserId = user.Id,
                ProviderUserId = userDto.Id.ToString(),
                RefreshToken = string.Empty,
                UpdateTime = DateTime.Now,
                CreateTime = DateTime.Now,
                AccessTokenExpiresAt = DateTimeOffset.Now.AddDays(1),
            };

            await userOAuthRepository.InsertAsync(oauth);

            user = await userRepository.InsertAsync(user);

            await userRepository.SaveChangesAsync();

            await eventBus.PublishAsync(new CreateUserEto(user.Id));
        }
        else
        {
            user = await userRepository.FirstOrDefaultAsync(x => x.Id == oauth.UserId);
        }


        var userDit = new Dictionary<string, string>();

        user.PasswordHash = null;
        user.PasswordSalt = null;

        userDit.Add("UserInfo", JsonSerializer.Serialize(user, JsonSerializerOptions.Web));

        var token = jwtContext.CreateToken(userDit, user.Id, user.Roles.ToArray());


        // 发布登录事件
        await loginEventBus.PublishAsync(new LoginEto(user.Id, user.Username, token, DateTime.Now,
            httpContext.Request.Headers["Origin"].ToString(), GetClientIp(),
            httpContext.Request.Headers["User-Agent"].ToString(), "OAuth", true, null));

        return token;
    }
}