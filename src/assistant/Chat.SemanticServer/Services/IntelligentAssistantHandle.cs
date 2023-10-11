using System.Diagnostics;
using System.Text.Json;
using Chat.Contracts.Chats;
using Chat.Contracts.Eto.Semantic;
using Chat.EventsBus.Contract;
using Chat.SemanticServer.Module;
using Chat.SemanticServer.plugins;
using Chat.SemanticServer.plugins.ChatPlugin;
using FreeRedis;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using Microsoft.SemanticKernel.Skills.Core;

namespace Chat.SemanticServer.Services;

/// <summary>
/// 智能助手服务
/// </summary>
public class IntelligentAssistantHandle : IEventsBusHandle<IntelligentAssistantEto>
{
    private readonly IKernel _kernel;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly RedisClient _redisClient;
    private readonly ILogger<IntelligentAssistantHandle> _logger;

    public IntelligentAssistantHandle(IKernel kernel, RedisClient redisClient,
        ILogger<IntelligentAssistantHandle> logger, IHttpClientFactory httpClientFactory)
    {
        _kernel = kernel;
        _redisClient = redisClient;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
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
        当前{province}的天气{weather}，平均温度{temperature_float},风向{winddirection},湿度{humidity};
        """;
    
    /// <inheritdoc />
    public async Task HandleAsync(IntelligentAssistantEto item)
    {
        string value = item.Value;

        if (value.IsNullOrWhiteSpace())
        {
            return;
        }

        string content = null;
        try
        {
            if (item.Value.StartsWith("ai help") || item.Value.StartsWith("ai -h"))
            {
                content = CommandTemplate;
            }
            else if (item.Value.StartsWith("ai status"))
            {
                // 获获取当前进程的内存占用，和cpu占用
                var process = Process.GetCurrentProcess();
                content =
                    $"当前内存占用：{process.WorkingSet64 / 1024 / 1024}MB";
            }
            else
            {
                string key = "Background:ChatGPT:" + item.UserId.ToString("N");

                // 限制用户发送消息频率
                if (await _redisClient.ExistsAsync(key))
                {
                    var count = await _redisClient.GetAsync<int>(key);

                    // 限制用户的智能助手使用限制
                    if (count > 20)
                    {
                        var messageLimit = new ChatMessageDto
                        {
                            Content = "您今天的额度已经用完！",
                            Type = ChatType.Text,
                            UserId = Guid.Empty,
                            CreationTime = DateTime.Now,
                            RevertId = item.RevertId,
                            GroupId = item.Id,
                            Id = Guid.NewGuid(),
                        };

                        // await _hubContext.Clients.Group(item.Id.ToString("N"))
                        //     .SendAsync("ReceiveMessage", item.Id, messageLimit);
                        //
                        // // 创建助手的消息
                        // var chatMessageLimit = new ChatMessage(Guid.NewGuid(), messageLimit.CreationTime)
                        // {
                        //     Content = messageLimit.Content,
                        //     Type = ChatType.Text,
                        //     RevertId = item.RevertId,
                        //     ChatGroupId = item.Id
                        // };
                        //
                        // await _chatMessageRepository.CreateAsync(chatMessageLimit);
                        return;
                    }
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
                ["options"] = "Weather" //给GPT的意图，通过Prompt限定选用这些里面的
            };
            string intent = (await _kernel.RunAsync(getIntentVariables, intentPlugin["GetIntent"])).Result.Trim();
            ISKFunction MathFunction = null;
            SKContext result = null;

            //获取意图后动态调用Fun
            if (intent is "Attractions" or "Delicacy" or "Traffic" or "Weather")
            {
                MathFunction = _kernel.Skills.GetFunction("Travel", intent);
                result = await _kernel.RunAsync(value, MathFunction);
            }
            else if (intent is "Weather")
            {
                value = (await _kernel.RunAsync(new ContextVariables
                {
                    ["input"] = value
                }, chatPlugin["Weather"])).Result;
                MathFunction = _kernel.Skills.GetFunction("WeatherPlugin", "GetWeather");
                result = await _kernel.RunAsync(value, MathFunction);
            }
            else
            {
                result = await _kernel.RunAsync(value);
            }

            var message = new ChatMessageDto
            {
                Content = result.Result,
                Type = ChatType.Text,
                UserId = Guid.Empty,
                RevertId = item.RevertId,
                CreationTime = DateTime.Now,
                GroupId = item.Id,
                Id = Guid.NewGuid()
            };

            if (item.Group)
            {
                // await _hubContext.Clients.Group(item.Id.ToString("N"))
                //     .SendAsync("ReceiveMessage", item.Id, message);
            }
            else
            {
                // await _hubContext.Clients.Group(item.Id.ToString("N"))
                //     .SendAsync("ReceiveMessage", item.Id, message);
            }

            // 创建助手的消息
            // var chatMessage = new ChatMessage(Guid.NewGuid(), DateTime.Now)
            // {
            //     Content = content,
            //     Type = ChatType.Text,
            //     RevertId = item.RevertId,
            //     ChatGroupId = item.Id
            // };
            //
            // await _chatMessageRepository.CreateAsync(chatMessage);
        }
        catch (Exception e)
        {
            _logger.LogError("智能助手出现异常 {e}", e);
        }
    }

    public async Task<string> SKHandle(string value)
    {
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
            ["options"] = "Weather,Attractions,Delicacy,Traffic,博客园" //给GPT的意图，通过Prompt限定选用这些里面的
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
                var weather = JsonSerializer.Deserialize<GetWeatherModule>(result.Result);
                var live = weather?.lives.FirstOrDefault();
                return WeatherTemplate
                    .Replace("{province}", live!.city)
                    .Replace("{weather}",live?.weather)
                    .Replace("{temperature_float}",live?.temperature_float)
                    .Replace("{winddirection}",live?.winddirection)
                    .Replace("{humidity}",live.humidity);
            }
        }
        else
        {
            result = await _kernel.RunAsync(value);
        }

        return result.Result;
    }
}