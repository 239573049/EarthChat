# 智能服务天气插件

## 介绍

本文将介绍SK使用天气插件原理，目前项目当中使用的天气插件是[心知天气](https://www.seniverse.com/)，心知天气提供了免费API和开发者套餐，下面的智能开发将使用开发者套餐，由于免费api提供参数较少，请尽量购买开发者套餐，以便更好的体验。

## 注册心知天气

打开浏览器进入`https://www.seniverse.com/`,然后注册账号，进入控制台`https://www.seniverse.com/dashboard`，然后打开`产品管理`->`添加产品`，购买开发者套餐（99一年并且需要先认证），免费套餐也可以使用。

然后选择购买的套餐，将私钥复制，然后打开项目`Chat.SemanticServer`,打开`appsettings.json`，将添加`SeniverseKey`，吧复制的私钥填写上去，

## 天气解析

当sk解析出我们的意图为`Weather`,我们会进入天气的第一步，解析用户的prompt，将提取出来城市和需要获取的时间（支持`昨天`,`今天`,`明天`）,解析完成以后将得到,为什么会得到一下json？这是因为利用了ChatGPT的能力将用户的prompt的信息提取出来了，类似于利用AI进行分析解析然后得到定义的结果，如果并不是我们想要的结果则可能是用户的prompt本身就是有问题的。

```json
{
    "city":"",
    "time":""
}
```

得到上面的json的数据以后会执行`GetWeather`SK函数，这个是在使用之前就import了，所以可以使用`GetFunction`获取`WeatherPlugin`插件下面的`GetWeather`函数。然后执行，下面我们看看`GetWeather`函数的实现

```csharp
MathFunction = _kernel.Skills.GetFunction("WeatherPlugin", "GetWeather");
result = await _kernel.RunAsync(newValue, MathFunction);
```

`WeatherPlugin.cs`

```csharp
/// <summary>
/// 获取天气插件
/// </summary>
public class WeatherPlugin
{
    private readonly IHttpClientFactory _httpClientFactory;

    private readonly IConfiguration _configuration;

    public WeatherPlugin(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [SKFunction, Description("获取天气")]
    [SKParameter("input", "入参")]
    public async Task<string> GetWeather(SKContext context)
    {
        var weatherInput = JsonSerializer.Deserialize<WeatherInput>(context.Result);

        string url = $"https://api.seniverse.com/v3/weather/$time.json?key={_configuration["SeniverseKey"]}&location={weatherInput.city}&language=zh-Hans&unit=c";

        if (weatherInput.time.IsNullOrWhiteSpace() || weatherInput.time == "今天")
        {
            url = url.Replace("$time", "now");
        }
        else if (weatherInput.time == "明天")
        {
            url = url.Replace("$time", "hourly");

        }
        else if (weatherInput.time == "昨天")
        {
            url = url.Replace("$time", "hourly_history");
        }
        else
        {
            url = url.Replace("$time", "now");
        }
        var http = _httpClientFactory.CreateClient(nameof(WeatherPlugin));
        var result = await http.GetAsync(url);

        if (result.IsSuccessStatusCode)
        {
            return await result.Content.ReadAsStringAsync();
        }
        return string.Empty;
    }
}

public class WeatherInput
{
    public string city { get; set; }
    public string time { get; set; }
}


public class WeatherDto
{
    public Result[] results { get; set; }
}

public class Result
{
    public Location location { get; set; }
    public List<Hourly_History> hourly_history { get; set; } = new();
}

public class Location
{
    public string name { get; set; }
}

public class Hourly_History
{
    public string text { get; set; }

    public string temperature { get; set; }


    public string humidity { get; set; }
    public string visibility { get; set; }
    public string wind_direction { get; set; }

    public string wind_speed { get; set; }
    public string wind_scale { get; set; }

    public string dew_point { get; set; }

}

```

我们可以看到`GetWeather`则是我们获取的sk函数，在这里会得到传递的json参数然后序列化成`WeatherInput`对象，然后得到我们的对象，在通过心知天气得到我们城市的天气，然后在解析传递的时间，如果时间为空则是今天，或时间定义错误也是获取当天，然后得到结果直接返回当前函数。

```csharp

if (!result.Result.IsNullOrWhiteSpace())
{
    var weatherInput = JsonSerializer.Deserialize<WeatherInput>(newValue);

    if (result.Result.IsNullOrEmpty())
    {
        await SendMessage("获取天气失败了！", item.RevertId, item.Id);
        return;
    }

    var json = JsonSerializer.Deserialize<WeatherDto>(result.Result);

    var hourly = json.results.FirstOrDefault().hourly_history;

    if (hourly.Count > 1)
    {
        var first = hourly.FirstOrDefault();
        var lastOrDefault = hourly.LastOrDefault();

        hourly.Clear();

        hourly.AddRange(new[] { first, lastOrDefault });

    }

    newValue = (await _kernel.RunAsync(new ContextVariables
    {
        ["input"] = JsonSerializer.Serialize(hourly),
        ["date"] = weatherInput.time,
        ["city"] = json.results.FirstOrDefault().location.name,

    }, chatPlugin["ConstituteWeather"])).Result;

    await SendMessage(newValue, item.RevertId, item.Id);
    return;
}

```

在上面的代码中我们会对于数据进行先处理，然后创建一个`ContextVariables`,里面的参数对应一下定义的`prompt`,AI则会自动根据json参数进行解析并且返回一段优化的结果。

```tex
{{$input}}
{{$date}}
{{$city}}

上面是天气参数和获取的时间还有所在城市，请解析并且提供一句
```

下面是回复的一个简单案例

```tex
问：ai 深圳昨天天气怎么样？
答：根据获取的天气参数，深圳昨天的天气情况为晴天，温度在25°C左右，湿度在80%左右。需要注意的是，昨天的天气较为炎热，建议适当防晒和补水。
```

这样就通过SK+AI大模型进行我们的智能天气服务了！