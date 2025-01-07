using System.ComponentModel;
using EarthChat.AuthServer.Application.Contract.Verification;
using Microsoft.AspNetCore.Mvc;

namespace EarthChat.AuthServer.Host.Apis;

public static class VerificationApis
{
    public static IEndpointRouteBuilder MapVerificationApis(this IEndpointRouteBuilder endpoints)
    {
        var verification = endpoints.MapGroup("/api/verification")
            .WithTags("验证码服务");

        verification.MapGet(string.Empty,
            [EndpointSummary("获取验证码")]
            async (IVerificationService verificationService, 
                [FromQuery]
                [Description("验证码类型")]
                string type) => await verificationService.GetAsync(type));

        return endpoints;
    }
}