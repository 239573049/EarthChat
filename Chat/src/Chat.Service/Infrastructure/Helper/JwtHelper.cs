using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Chat.Contracts.Users;
using Chat.Service.Options;
using Masa.Contrib.Authentication.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Chat.Service.Infrastructure.Helper;

public class JwtHelper
{
    /// <summary>
    /// 生成token
    /// </summary>
    /// <param name="claimsIdentity"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string GeneratorAccessToken(ClaimsIdentity claimsIdentity, JwtOptions options)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(options.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claimsIdentity,
            Expires = DateTime.UtcNow.AddHours(options.EffectiveHours),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public static ClaimsIdentity GetClaimsIdentity(UserDto user)
    {
        return new ClaimsIdentity(new Claim[]
        {
            new(ClaimType.DEFAULT_USER_ID, user.Id.ToString()),
            new("avatar", user.Avatar),
            new(ClaimType.DEFAULT_USER_NAME, user.Name),
        });
    }
}