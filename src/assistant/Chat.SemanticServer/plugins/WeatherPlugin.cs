using System.ComponentModel;
using System.Text.Json;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

namespace Chat.SemanticServer.plugins.ChatPlugin;

/// <summary>
/// 获取天气插件
/// </summary>
public class WeatherPlugin
{
    private static List<AdCode>? _codes;

    static WeatherPlugin()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "adcode.json");
        if (File.Exists(path))
        {
            var str = File.ReadAllText(path);
            _codes = JsonSerializer.Deserialize<List<AdCode>>(str);
        }

        _codes ??= new List<AdCode>();
    }

    private readonly IHttpClientFactory _httpClientFactory;

    public WeatherPlugin(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [SKFunction, Description("获取天气")]
    [SKParameter("input", "入参")]
    public async Task<string> GetWeather(SKContext context)
    {
        var value = _codes.FirstOrDefault(x => x.name.StartsWith(context.Result));
        if (value == null)
        {
            return "请先描述指定城市！";
        }
        var http = _httpClientFactory.CreateClient(nameof(WeatherPlugin));
        var result =await http.GetAsync(
            "https://restapi.amap.com/v3/weather/weatherInfo?key=2e92f9d6c58e20fcf2ba7e71978ecd16&extensions=base&output=JSON&city="+value.adcode);

        if (result.IsSuccessStatusCode)
        {
            return await result.Content.ReadAsStringAsync();
        }

        return "获取天气失败了！";
    }
}

public class AdCode
{
    public string name { get; set; }

    public string adcode { get; set; }

    public string citycode { get; set; }
}