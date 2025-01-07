using EarthChat.AuthServer.Application.Contract.Verification;
using EarthChat.AuthServer.Application.Contract.Verification.Dto;
using Gnarly.Data;
using Lazy.Captcha.Core;

namespace TokenAI.Site.Application.Verification;

/// <summary>
/// 验证码服务实现
/// </summary>
/// <param name="captcha"></param>
public class VerificationService(ICaptcha captcha) : IVerificationService, ITransientDependency
{
    /// <summary>
    /// 获取验证码, 返回验证码图片base64
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public Task<VerificationDto> GetAsync(string type)
    {
        var uuid = type + ":" + Guid.NewGuid().ToString("N");

        var code = captcha.Generate(uuid, 240);

        return Task.FromResult(new VerificationDto
        {
            Key = uuid,
            Code = "data:image/png;base64," + code.Base64
        });
    }
}