using EarthChat.AuthServer.Application.Contract.Verification.Dto;

namespace EarthChat.AuthServer.Application.Contract.Verification;

public interface IVerificationService
{
    /// <summary>
    /// 获取验证码
    /// </summary>
    /// <param name="type">
    /// 验证码类型 
    /// </param>
    /// <returns></returns>
    Task<VerificationDto> GetAsync(string type);
}