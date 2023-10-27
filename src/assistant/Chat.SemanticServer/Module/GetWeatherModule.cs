namespace Chat.SemanticServer.Module;

public class GetWeatherModule
{
    public Results[] results { get; set; }
}

public class Results
{
    public Location location { get; set; }
    public Now now { get; set; }
    public string last_update { get; set; }
}

public class Location
{
    public string id { get; set; }
    public string name { get; set; }
    public string country { get; set; }
    public string path { get; set; }
    public string timezone { get; set; }
    public string timezone_offset { get; set; }
}

public class Now
{
    public string text { get; set; }
    public string code { get; set; }
    public string temperature { get; set; }
}

