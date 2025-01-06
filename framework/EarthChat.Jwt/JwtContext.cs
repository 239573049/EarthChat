using EarthChat.Core.Contract;
using EarthChat.Jwt.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EarthChat.Jwt;

public class JwtContext(IOptions<JwtOptions> options) : IJwtContext
{
	public string CreateToken(Dictionary<string, string> dist, Guid userId, string[] roles)
	{
		var claims = new List<Claim>
		{
			new(ClaimTypes.Role, string.Join(',', roles)),
			new(ClaimTypes.Sid, userId.ToString())
		};

		foreach (var item in dist)
		{
			claims.Add(new Claim(item.Key, item.Value));
		}

		// 2. 从 appsettings.json 中读取SecretKey
		var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.Secret));

		// 3. 选择加密算法
		var algorithm = SecurityAlgorithms.HmacSha256;

		// 4. 生成Credentials
		var signingCredentials = new SigningCredentials(secretKey, algorithm);

		// 5. 根据以上，生成token
		var jwtSecurityToken = new JwtSecurityToken(
			options.Value.Issuer, //Issuer
			options.Value.Audience, //Audience
			claims, //Claims,
			DateTime.Now, //notBefore
			DateTime.Now.AddDays(options.Value.ExpireDay), //expires
			signingCredentials //Credentials
		);

		// 6. 将token变为string
		var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

		return token;
	}
}