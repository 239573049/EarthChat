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
            _codes = JsonSerializer.Deserialize<List<AdCode>>(File.ReadAllBytes(path));
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
    public string GetWeather(SKContext context)
    {
        var http = _httpClientFactory.CreateClient(nameof(WeatherPlugin));
        http.GetAsync(
            "https://restapi.amap.com/v3/weather/weatherInfo?key=2e92f9d6c58e20fcf2ba7e71978ecd16&extensions=base&output=JSON&city=");


        return "";
    }
}

public class AdCode
{
    public string name { get; set; }

    public string adcode { get; set; }

    public string citycode { get; set; }
}