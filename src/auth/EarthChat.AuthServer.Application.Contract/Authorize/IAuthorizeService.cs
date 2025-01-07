using EarthChat.AuthServer.Application.Contract.Authorize.Input;

namespace EarthChat.AuthServer.Application.Contract.Authorize;

public interface IAuthorizeService
{
    /// <summary>
    /// token授权码
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<string> TokenAsync(TokenInput input);

    /// <summary>
    /// 第三方平台登录
    /// </summary>
    /// <param name="type"></param>
    /// <param name="code"></param>
    /// <param name="state"></param>
    /// <param name="redirectUri"></param>
    /// <returns></returns>
    Task<string> OAuthTokenAsync(string type, string code, string state, string? redirectUri = null);
}