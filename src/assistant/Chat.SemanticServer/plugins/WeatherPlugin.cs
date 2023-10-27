using System.ComponentModel;
using System.Net.WebSockets;
using System.Text.Json;
using Chat.SemanticServer.Module;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

namespace Chat.SemanticServer.plugins;

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

        if (weatherInput.time.IsNullOrWhiteSpace() || weatherInput.time == "今天")
        {
            var url =
                $"https://api.seniverse.com/v3/weather/now.json?key={_configuration["SeniverseKey"]}&location={weatherInput.city}&language=zh-Hans&unit=c";
            var http = _httpClientFactory.CreateClient(nameof(WeatherPlugin));
            var result = await http.GetAsync(url);

            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadAsStringAsync();
            }

        }
        else if (weatherInput.time == "明天")
        {
            var url = $"https://api.seniverse.com/v3/weather/hourly.json?key={_configuration["SeniverseKey"]}&location={weatherInput.city}&language=zh-Hans&unit=c&start=0&hours=24";
            var http = _httpClientFactory.CreateClient(nameof(WeatherPlugin));
            var result = await http.GetAsync(url);

            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadAsStringAsync();
            }
        }
        else if (weatherInput.time == "昨天")
        {
            var url = $"https://api.seniverse.com/v3/weather/hourly_history.json?key={_configuration["SeniverseKey"]}&location={weatherInput.city}&language=zh-Hans&unit=c&start=0&hours=24";
            var http = _httpClientFactory.CreateClient(nameof(WeatherPlugin));
            var result = await http.GetAsync(url);

            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadAsStringAsync();
            }
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
