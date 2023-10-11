namespace Chat.SemanticServer.Module;

/// <summary>
/// 高德天气Api返回模型
/// </summary>
public class GetWeatherModule
{
    public string status { get; set; }
    public string count { get; set; }
    public string info { get; set; }
    public string infocode { get; set; }
    public Lives[] lives { get; set; }
}

public class Lives
{
    public string province { get; set; }
    public string city { get; set; }
    public string adcode { get; set; }
    public string weather { get; set; }
    public string temperature { get; set; }
    public string winddirection { get; set; }
    public string windpower { get; set; }
    public string humidity { get; set; }
    public string reporttime { get; set; }
    public string temperature_float { get; set; }
    public string humidity_float { get; set; }
}