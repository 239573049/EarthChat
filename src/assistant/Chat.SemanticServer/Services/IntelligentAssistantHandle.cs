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

    public IntelligentAssistantHandle(IKernel kernel, RedisClient redisClient,
        ILogger<IntelligentAssistantHandle> logger, IHttpClientFactory httpClientFactory,
        OpenAIChatCompletion chatCompletion)
    {
        _kernel = kernel;
        _redisClient = redisClient;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _chatCompletion = chatCompletion;

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
        使用方法：
            输入 "ai help" 获取帮助信息
            输入 "ai status" 检查服务器状态
            输入 "ai 提问信息" 提问并获取回答
        示例：
            输入 "ai help"，将显示命令行帮助手册
            输入 "ai status"，将检查服务器状态并返回结果
            输入 "ai 你好吗？"，将提问并获取回答
        注意事项：
            确保在命令行中正确输入命令和参数
            请确保服务器正常运行以获取准确的状态信息和回答
        """;

    private const string WeatherTemplate =
        """
        {province}的天气{weather}，平均温度{temperature_float},风向{winddirection},湿度{humidity};
        """;

    /// <inheritdoc />
    public async Task HandleAsync(IntelligentAssistantEto item)
    {
        string value = item.Value;

        if (value.IsNullOrWhiteSpace())
        {
            return;
        }

        try
        {
            if (item.Value.StartsWith("ai help") || item.Value.StartsWith("ai -h"))
            {
                await SendMessage(CommandTemplate, item.RevertId, item.Id);
                return;
            }

            if (item.Value.StartsWith("ai status"))
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
                        UserId = Guid.Empty,
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

            //对话摘要  SK.Skills.Core 核心技能
            _kernel.ImportSkill(new ConversationSummarySkill(_kernel), "ConversationSummarySkill");

            var pluginsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "plugins");

            var intentPlugin = _kernel
                .ImportSemanticSkillFromDirectory(pluginsDirectory, "BasePlugin");
            var travelPlugin = _kernel
                .ImportSemanticSkillFromDirectory(pluginsDirectory, "Travel");

            var chatPlugin = _kernel
                .ImportSemanticSkillFromDirectory(pluginsDirectory, "ChatPlugin");

            var getWeather = _kernel.ImportSkill(new WeatherPlugin(_httpClientFactory), "WeatherPlugin");

            var getIntentVariables = new ContextVariables
            {
                ["input"] = value,
                ["options"] = "Weather,Attractions,Delicacy,Traffic" //给GPT的意图，通过Prompt限定选用这些里面的
            };
            string intent = (await _kernel.RunAsync(getIntentVariables, intentPlugin["GetIntent"])).Result.Trim();
            ISKFunction MathFunction = null;
            SKContext? result = null;

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
                    if (result.Result.IsNullOrEmpty())
                    {
                        await SendMessage("获取天气失败了！", item.RevertId, item.Id);
                        return;
                    }

                    var weather = JsonSerializer.Deserialize<GetWeatherModule>(result.Result);
                    var live = weather?.lives.FirstOrDefault();
                    await SendMessage(WeatherTemplate
                        .Replace("{province}", live!.city)
                        .Replace("{weather}", live?.weather)
                        .Replace("{temperature_float}", live?.temperature_float)
                        .Replace("{winddirection}", live?.winddirection)
                        .Replace("{humidity}", live.humidity), item.RevertId, item.Id);
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