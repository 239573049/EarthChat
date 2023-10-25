# 获取用户所在城市

## 如何实现获取所在城市

实现我们用户是通过web进入Chat项目，但是呢用户并没有给我们GPS定位，所以我们并没有办法直接获取到用户的详细位置，但是我们现在这个需求并不需要很详细，比如只获取用户所在城市（获取用户详细地址属于隐私信息！）。

下面我们来讲解一下获取的流程

![Alt text](./img/IpOwnership.png)

上面是实现流程图

下面将使用代码讲解。

首先构造注入`IHttpContextAccessor`,在构造注入之前先确保有添加注入过`builder.Services.AddHttpContextAccessor();`

```csharp

    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserQueryHandler(IHttpContextAccessor httpContextAccessor,....)
    {
        _httpContextAccessor = httpContextAccessor;
    }

```

注入的`IHttpContextAccessor`用于获取用户的ip

```csharp
 var ip = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.MapToIPv4().ToString();
        if ((_httpContextAccessor?.HttpContext?.Request?.Headers)?.TryGetValue("X-Forwarded-For", out var header) == true)
        {
            ip = header.ToString();
        }
```

有人可能有疑惑了为什么要在请求头中搜索`X-Forwarded-For`,这里解释一下，实现如果你的服务是正常部署的，并没有并没有存在代理的情况下，直接使用` var ip = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.MapToIPv4().ToString();`可以拿到用户的ip，但是如果通过nginx代理的服务如果使用的话将获取到局域网的地址，
这时候就需要使用nginx配置一下参数,这个参数会将链接的ip添加到请求头中，参数名称则是`X-Forwarded-For`,所以上面的c#代码则会在获取一次请求头中是否存在`X-Forwarded-For`,如果存在则优先使用`X-Forwarded-For`的值。


```conf
proxy_set_header   Host $http_host;
proxy_set_header X-Forwarded-Proto $scheme;
proxy_set_header        X-Real-IP       $remote_addr;
proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
```

得到ip以后就可以通过第三方api得到ip的位置，也有离线包，但是离线包一般都很大，所以直接使用第三方的api，

下面将讲解使用HttpClient访问第三方api得到地址,`whois.pconline.com.cn`太平洋IP查询是一个免费的api。

`json=true`表示返回json格式的参数，只需要将ip传递然后就可以得到想要的参数，要注意的是使用HttpClient的时候请不要直接new，可能存在socket消耗完的问题，可以注入`_httpClientFactory`,这样就可以重复利用HttpClient。

```csharp

var http = new HttpClient();
var result = await http.GetFromJsonAsync<GetObtainingIPHomeDto>(
"https://whois.pconline.com.cn/ipJson.jsp?json=true&ip=" +ip);
public class GetObtainingIPHomeDto
{
    /// <summary>
    /// 当前公网ip
    /// </summary>
    public string ip { get; set; }

    /// <summary>
    /// 省
    /// </summary>
    public string pro { get; set; }

    /// <summary>
    /// 省 code
    /// </summary>
    public string proCode { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    public string city { get; set; }

    /// <summary>
    /// 城市代码
    /// </summary>
    public string cityCode { get; set; }
}

```
