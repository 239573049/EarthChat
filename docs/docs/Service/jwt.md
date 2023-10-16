# 如何使用 JWT（JSON Web Token）

## 什么是JWT（JSON Web Token）

JWT（JSON Web Token）是一种用于两个系统之间安全传输信息的简短、自包含的标准。它通过使用JSON对象来描述信息并可以被加密，从而保证信息的完整性。JWT在很多场景中都是非常有用的，例如：在客户端和服务器之间进行认证或信息交换。

JWT 由三部分组成，分别由点 (.) 分隔：

1. **头部 (Header)**: 通常包含两部分：token 类型和所使用的算法（例如：RSA、HMAC 或 SHA256）。

   ```json
   {
     "alg": "HS256",
     "typ": "JWT"
   }
   ```

2. **载荷 (Payload)**: 载荷包含声明，这些声明是关于用户的信息，以及其他的元数据。载荷中有一些预定义的声明，如：
   - **iss** (Issuer)：发布者
   - **exp** (Expiration Time)：过期时间
   - **sub** (Subject)：主题
   - **aud** (Audience)：受众
   除了预定义的声明，你还可以添加其他自定义的声明。
3. **签名 (Signature)**: 为了创建签名，你需要采用在头部定义的算法，对前两部分进行加密，并加上一个密钥。这确保了 token 的完整性和验证性。例如，如果你要使用 HMAC SHA256 算法，签名的生成方式为：
   ```HMACSHA256(base64UrlEncode(header) + "." + base64UrlEncode(payload), secret)```

将这三个部分用点 (.) 连接起来，就得到了完整的 JWT。例如：
```HS256(base64UrlEncode(header).base64UrlEncode(payload).Signature)```

主要应用场景：

- **身份验证 (Authentication)**：一旦用户登录，每次请求时，都会将 JWT 发送给服务器进行验证，从而免去多次查询数据库的麻烦。
- **信息交换 (Information Exchange)**：两个系统之间可以通过 JWT 安全、轻量地交换信息。由于 JWT 可以加密，所以信息的完整性是可以保证的。

需要注意的是，虽然 JWT 可以加密，但通常的使用中，载荷部分（即第二部分）是简单的 Base64Url 编码，并不是加密的。如果你要将敏感信息放入 JWT，必须确保 JWT 本身是加密的或使用加密技术来保护这些信息。

## 在Asp.NET Core中使用JWT（JSON Web Token）

:::caution 注意

当前使用的.NET 6以上版本
:::

1. **创建实体模型**

```csharp
public class JwtOptions
{
    /// <summary>
    ///     秘钥
    /// </summary>
    public string Secret { get; set; }

    /// <summary>
    ///     有效时间（小时）
    /// </summary>
    public int EffectiveHours { get; set; }
}
```

2. **在appsettings.json中添加响应配置**

```json
{
  "Jwt": {
    "Secret": "a4lfhweiou4fbiwj6213ewf",
    "EffectiveHours": 720
  }
}
```

3. **安装Nuget包**

安装以下`Nuget`包

```xml
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="7.0.9" />
```

4. **注入绑定Options**

```csharp
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtOptions>(jwtSection);
```

使用方式

```csharp
builder.Services.AddJwtBearerAuthentication(jwtSection.Get<JwtOptions>())
```

创建JWT的扩展方法
在`AddJwtBearer`中的`x.Events`JWT的事件，在这里需要处理以下`SignalR`的Token，由于`SignalR`默认的Token是存放在地址当中，但是配置的校验token的位置在请求头中的`Authorization`,所以我们需要处理一下Token，如果当前地址是`/chatHub`对应配置的`SignalR`的地址则获取`query`参数中的`access_token`的值。

```csharp

    /// <summary>
    ///     注册JWT Bearer认证服务的静态扩展方法
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options">JWT授权的配置项</param>
    public static IServiceCollection AddJwtBearerAuthentication(this IServiceCollection services, JwtOptions options)
    {
        //使用应用密钥得到一个加密密钥字节数组
        var key = Encoding.ASCII.GetBytes(options.Secret);

        services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie(cfg => cfg.SlidingExpiration = true)
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var path = context.HttpContext.Request.Path;
                        if (!path.StartsWithSegments("/chatHub")) return Task.CompletedTask;

                        var accessToken = context.Request.Query["access_token"];

                        if (!string.IsNullOrEmpty(accessToken))
                            // 从查询字符串中读取令牌
                            context.Token = accessToken;

                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }
```

5. **添加鉴权中间件**

请注意添加顺序

```csharp
app.UseAuthentication();
app.UseAuthorization()
```

6. **如何生成token**

在这里封装了一个生成工具，首先我们会使用`GetClaimsIdentity`方法给你UserDto生成`ClaimsIdentity`对象，并且使用注入的方式得到`JwtOptions`,将其传递到`GeneratorAccessToken`方法，返回的token就是我们需要的。

```csharp
public class JwtHelper
{
    /// <summary>
    ///     生成token
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
            new(ClaimType.DEFAULT_USER_NAME, user.Name)
        });
    }
}
```
