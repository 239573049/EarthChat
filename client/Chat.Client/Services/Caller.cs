using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Chat.Client.Services;

public static class Caller
{
    private static HttpClient HttpClient { get; }

    private const string BaseUrl = "https://chat.tokengo.top/api/v1/";

    static Caller()
    {
        HttpClient = MainAppHelper.GetRequiredService<IHttpClientFactory>().CreateClient("ServiceBase");
        HttpClient.Timeout = TimeSpan.FromSeconds(30);
        HttpClient.BaseAddress = new Uri(BaseUrl);
    }

    public static void SetToken(string token)
    {
        SetHeader("Authorization", "Bearer " + token);
    }

    public static HttpClient GetHttpClient()
    {
        return HttpClient;
    }

    public static async Task<T> PostAsync<T>(string url, object? data = null) 
    {
        var response = await HttpClient.PostAsJsonAsync(url, data);
        return await response.Content.ReadFromJsonAsync<T>();
    }
    
    public static async Task<T> GetAsync<T>(string url)
    {
        var response = await HttpClient.GetAsync(url);
        return await response.Content.ReadFromJsonAsync<T>();
    }
    
    public static async Task<T> PutAsync<T>(string url, object? data = null) 
    {
        var response = await HttpClient.PutAsJsonAsync(url, data);
        return await response.Content.ReadFromJsonAsync<T>();
    }
    
    public static async Task<T> DeleteAsync<T>(string url)
    {
        var response = await HttpClient.DeleteAsync(url);
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public static void SetHeader(string key, string value)
    {
        HttpClient.DefaultRequestHeaders.Remove(key);
        HttpClient.DefaultRequestHeaders.Add(key, value);
    }
}