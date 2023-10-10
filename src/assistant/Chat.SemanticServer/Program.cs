using System.Text.Json;
using Chat.SemanticServer.Options;
using Chat.SemanticServer.Services;
using FreeRedis;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.GetSection("OpenAI").Get<OpenAIOptions>();

builder.Services.AddSingleton<IntelligentAssistantHandle>();

builder.Services.AddEventsBusRabbitMq(builder.Configuration);

builder.Services.AddHttpClient("ChatGPT", (services, c) =>
{
    c.DefaultRequestHeaders.Add("X-Token", "token");
    c.DefaultRequestHeaders.Add("Accept", "application/json");
    c.DefaultRequestHeaders.Add("User-Agent", "Chat");
});

builder.Services.AddTransient<IKernel>((services) =>
{
    var httpClientFactory = services.GetRequiredService<IHttpClientFactory>();
    return Kernel.Builder
        .WithOpenAIChatCompletionService(
            OpenAIOptions.Model,
            OpenAIOptions.Endpoint,
            OpenAIOptions.Key, httpClient: httpClientFactory.CreateClient("ChatGPT"))
        .Build();
}).AddSingleton(_ =>
{
    var client = new RedisClient(builder.Configuration["ConnectionStrings:Redis"]);
    client.Serialize = o => JsonSerializer.Serialize(o);
    client.Deserialize = (s, t) => JsonSerializer.Deserialize(s, t);
    return client;
});


var app = builder.Services.AddServices(builder, options =>
{
    options.MapHttpMethodsForUnmatched = new[] { "Post" }; //当请求类型匹配失败后，默认映射为Post请求 (当前项目范围内，除非范围配置单独指定)
});

app.Services.GetServices<IntelligentAssistantHandle>();

app.Run();