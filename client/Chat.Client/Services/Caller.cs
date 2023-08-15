using System.Net.Http;
using System.Net.Http.Headers;

namespace Chat.Client.Services;

public static class Caller
{
    private static HttpClient HttpClient { get; }

    private const string BaseUrl = "http://localhost:5218/api/v1/";

    static Caller()
    {
        HttpClient = MainAppHelper.GetRequiredService<IHttpClientFactory>().CreateClient("ServiceBase");
        HttpClient.Timeout = TimeSpan.FromSeconds(30);
        HttpClient.BaseAddress = new Uri(BaseUrl);
    }
    
    public static void SetToken(string token)
    {
        HttpClient.DefaultRequestHeaders.Remove("Authorization");
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
    
    public static HttpClient GetHttpClient()
    {
        return HttpClient;
    }
}