using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using Chat.Contracts;
using Chat.Contracts.Chats;
using Chat.Contracts.Eto.Chat;
using Chat.Contracts.Eto.Semantic;
using Chat.SemanticServer.Module;
using Chat.SemanticServer.Options;
using Chat.SemanticServer.plugins;
using FreeRedis;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI.ChatCompletion;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using Microsoft.SemanticKernel.Skills.Core;

namespace Chat.SemanticServer.Services;

/// <summary>
/// 智能助手服务
/// </summary>
public class IntelligentAssistantHandle
{
    private readonly IKernel _kernel;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly RedisClient _redisClient;
    private readonly ILogger<IntelligentAssistantHandle> _logger;
    private readonly OpenAIChatCompletion _chatCompletion;
    private readonly IConfiguration _configuration;
    private readonly IDictionary<string, ISKFunction> intentPlugin;
    private readonly IDictionary<string, ISKFunction> travelPlugin;
    private readonly IDictionary<string, ISKFunction> chatPlugin;
    private readonly IDictionary<string, ISKFunction> getWeather;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="kernel"></param>
    /// <param name="redisClient"></param>
    /// <param name="logger"></param>
    /// <param name="httpClientFactory"></param>
    /// <param name="chatCompletion"></param>
    /// <param name="configuration"></param>
    public IntelligentAssistantHandle(IKernel kernel, RedisClient redisClient,
        ILogger<IntelligentAssistantHandle> logger, IHttpClientFactory httpClientFactory,
        OpenAIChatCompletion chatCompletion, IConfiguration configuration)
    {
        _kernel = kernel;
        _redisClient = redisClient;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _chatCompletion = chatCompletion;
        _configuration = configuration;

        _redisClient.Subscribe(nameof(IntelligentAssistantEto),
            ((s, o) =>
            {
                try
                {
                    HandleAsync(JsonSerializer.Deserialize<IntelligentAssistantEto>(o as string));
                }
                catch (Exception e)
                {
                    _logger.LogError("{e}", e);
                }
            }));



        //对话摘要  SK.Skills.Core 核心技能
        _kernel.ImportSkill(new ConversationSummarySkill(_kernel), "ConversationSummarySkill");

        var pluginsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "plugins");

        intentPlugin = _kernel
            .ImportSemanticSkillFromDirectory(pluginsDirectory, "BasePlugin");
        travelPlugin = _kernel
            .ImportSemanticSkillFromDirectory(pluginsDirectory, "Travel");

        chatPlugin = _kernel
            .ImportSemanticSkillFromDirectory(pluginsDirectory, "ChatPlugin");

        getWeather = _kernel.ImportSkill(new WeatherPlugin(_httpClientFactory, _configuration), "WeatherPlugin");

    }

    /// <summary>
    /// 指令模板
    /// </summary>
    private const string CommandTemplate =
        """
        命令行帮助手册：
            ai help - 获取帮助信息
            ai status - 检查服务器状态
            ai 提问信息 - 提问并获取回答
            ai 询问天气  - 提问并且得到天气信息
        使用方法：
            输入 "ai help" 获取帮助信息
            输入 "ai status" 检查服务器状态
            输入 "ai 提问信息" 提问并获取回答
            输入 "ai 需要知道的城市" 提问并且得到天气信息
        示例：
            输入 "ai help"，将显示命令行帮助手册
            输入 "ai status"，将检查服务器状态并返回结果
            输入 "ai 你好吗？"，将提问并获取回答
            输入 "ai 深圳天气怎么样" 提问并且得到天气信息
        注意事项：
            确保在命令行中正确输入命令和参数
            请确保服务器正常运行以获取准确的状态信息和回答
        """;

    public async Task HandleAsync(IntelligentAssistantEto item)
    {
        string value = item.Value.ToLower();

        if (value.IsNullOrWhiteSpace())
        {
            return;
        }

        try
        {
            if (value.StartsWith("ai help") || value.StartsWith("ai -h"))
            {
                await SendMessage(CommandTemplate, item.RevertId, item.Id);
                return;
            }

            if (value.StartsWith("ai status"))
            {
                // 获获取当前进程的内存占用，和cpu占用
                var process = Process.GetCurrentProcess();
                var content =
                    $"当前内存占用：{process.WorkingSet64 / 1024 / 1024}MB;{Environment.NewLine}{Environment.OSVersion}";

                await SendMessage(content, item.RevertId, item.Id);
                return;
            }

            string key = "Background:ChatGPT:" + item.UserId.ToString("N");

            // 限制用户发送消息频率
            if (await _redisClient.ExistsAsync(key))
            {
                var count = await _redisClient.GetAsync<int>(key);

                // 限制用户的智能助手使用限制
                if (count > 20)
                {
                    var messageLimit = new ChatMessageEto
                    {
                        Content = "您今天的额度已经用完！",
                        Type = ChatType.Text,
                        UserId = Constant.Group.AssistantId,
                        CreationTime = DateTime.Now,
                        RevertId = item.RevertId,
                        GroupId = item.Id,
                        Id = Guid.NewGuid(),
                    };

                    await SendMessage(messageLimit.Content, item.RevertId, item.Id);
                    await _redisClient.PublishAsync(nameof(ChatMessageEto), JsonSerializer.Serialize(messageLimit));

                    return;
                }
            }

            var getIntentVariables = new ContextVariables
            {
                ["input"] = value,
                ["options"] = "Weather,Attractions,Delicacy,Traffic" //给GPT的意图，通过Prompt限定选用这些里面的
            };
            string intent = (await _kernel.RunAsync(getIntentVariables, intentPlugin["GetIntent"])).Result.Trim();
            ISKFunction? MathFunction;
            SKContext? result;

            //获取意图后动态调用Fun
            if (intent is "Attractions" or "Delicacy" or "Traffic")
            {
                MathFunction = _kernel.Skills.GetFunction("Travel", intent);
                result = await _kernel.RunAsync(value, MathFunction);
            }
            else if (intent is "Weather")
            {
                var newValue = (await _kernel.RunAsync(new ContextVariables
                {
                    ["input"] = value
                }, chatPlugin["Weather"])).Result;
                MathFunction = _kernel.Skills.GetFunction("WeatherPlugin", "GetWeather");
                result = await _kernel.RunAsync(newValue, MathFunction);

                if (!result.Result.IsNullOrWhiteSpace())
                {

                    var weatherInput = JsonSerializer.Deserialize<WeatherInput>(newValue);

                    if (result.Result.IsNullOrEmpty())
                    {
                        await SendMessage("获取天气失败了！", item.RevertId, item.Id);
                        return;
                    }

                    var weatherResult = JsonSerializer.Deserialize<WeatherDto>(result.Result).results.FirstOrDefault();

                    var hourlys = new List<Hourly_History>();

                    if (weatherResult.hourly_history.Count != 0)
                    {
                        var first = weatherResult.hourly_history.FirstOrDefault();
                        var lastOrDefault = weatherResult.hourly_history.LastOrDefault();

                        hourlys.AddRange(new[] { first, lastOrDefault });
                    }
                    else if (weatherResult.hourly.Count != 0)
                    {
                        var first = weatherResult.hourly.FirstOrDefault();
                        var lastOrDefault = weatherResult.hourly.LastOrDefault();

                        hourlys.AddRange(new[] { first, lastOrDefault });
                    }
                    else if (weatherResult.now != null)
                    {

                        hourlys.AddRange(new[] { weatherResult.now });
                    }
                    else
                    {

                        await SendMessage("天气解析错误了：" + result.Result, item.RevertId, item.Id);
                        return;
                    }

                    newValue = (await _kernel.RunAsync(new ContextVariables
                    {
                        ["input"] = JsonSerializer.Serialize(hourlys),
                        ["date"] = weatherInput?.time,
                        ["city"] = weatherResult.location.name,

                    }, chatPlugin["ConstituteWeather"])).Result;

                    await SendMessage(newValue, item.RevertId, item.Id);
                    return;
                }
            }
            else
            {
                var chatHistory = _chatCompletion.CreateNewChat();
                chatHistory.AddUserMessage(value);
                var reply = await _chatCompletion.GenerateMessageAsync(chatHistory);

                await SendMessage(reply, item.RevertId, item.Id);
                return;
            }


            await SendMessage(result.Result, item.RevertId, item.Id);
        }
        catch (Exception e)
        {
            _logger.LogError("智能助手出现异常 {e}", e);

            await SendMessage("智能助手出现异常！", item.RevertId, item.Id);
        }
    }

    private async Task SendMessage(string content, Guid revertId, Guid id)
    {
        var messageLimit = new ChatMessageEto
        {
            Content = content,
            Type = ChatType.Text,
            UserId = Constant.Group.AssistantId,
            CreationTime = DateTime.Now,
            RevertId = revertId,
            GroupId = id,
            Id = Guid.NewGuid(),
        };

        await _redisClient.PublishAsync(nameof(ChatMessageEto), JsonSerializer.Serialize(messageLimit));
    }
}