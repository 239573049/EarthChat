using System.Diagnostics;
using System.Threading.Channels;
using Chat.Contracts.Chats;
using Chat.Contracts.Eto.Semantic;
using Chat.Service.Application.Chats.Queries;
using Chat.Service.Domain.Chats.Aggregates;
using Chat.Service.Domain.Chats.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Service.Services;

/// <summary>
/// 后台任务。
/// </summary>
public class BackgroundTaskService : ISingletonDependency, IDisposable
{
    /// <summary>
    /// 消息管道。
    /// </summary>
    private readonly Channel<AssistantDto> _channel;

    private readonly IServiceProvider _serviceProvider;

    private readonly IServiceScope _serviceScope;

    private readonly IHubContext<ChatHub> _hubContext;

    private readonly CancellationTokenSource _cancellationTokenSource;

    private readonly IHttpClientFactory _httpClientFactory;

    private readonly ILogger<BackgroundTaskService> _logger;

    private readonly IChatMessageRepository _chatMessageRepository;

    private readonly RedisClient _redisClient;

    private readonly IEventBus _eventBus;

    public BackgroundTaskService(IServiceProvider serviceProvider, IHttpClientFactory httpClientFactory,
        ILogger<BackgroundTaskService> logger, RedisClient redisClient)
    {
        _serviceProvider = serviceProvider;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _redisClient = redisClient;
        // 请注意由于仓储使用的是作用域，所以需要在这个单例的服务中创建一个作用域。
        _serviceScope = serviceProvider.CreateScope();
        _chatMessageRepository = _serviceScope.ServiceProvider.GetRequiredService<IChatMessageRepository>();
        _hubContext = _serviceScope.ServiceProvider.GetRequiredService<IHubContext<ChatHub>>();
        _eventBus = _serviceScope.ServiceProvider.GetRequiredService<IEventBus>();
        // 使用管道的话会控制ChatGPT的数量，相当于当前系统只运行一个服务。
        _channel = Channel.CreateBounded<AssistantDto>(5000);
        _cancellationTokenSource = new CancellationTokenSource();
        Task.Run(Start);
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

    private async Task Start()
    {
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            var item = await _channel.Reader.ReadAsync(_cancellationTokenSource.Token);
            if (item != null)
            {
                // 触发聊天机器人
                if (!item.Value.StartsWith("ai") || item.Value.Length <= 3) continue;

                // 判断是否需要获取当前服务的状态

                string value = item.Value[2..].Trim();

                if (value.IsNullOrWhiteSpace())
                {
                    continue;
                }

                await _redisClient.PublishAsync(nameof(IntelligentAssistantEto), JsonSerializer.Serialize(
                    new IntelligentAssistantEto()
                    {
                        Group = item.Group,
                        Id = item.Id,
                        RevertId = item.RevertId,
                        UserId = item.UserId,
                        Value = item.Value
                    }));

                continue;

                try
                {
                    string content;
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

                                await _hubContext.Clients.Group(item.Id.ToString("N"))
                                    .SendAsync("ReceiveMessage", item.Id, messageLimit);

                                // 创建助手的消息
                                var chatMessageLimit = new ChatMessage(Guid.NewGuid(), messageLimit.CreationTime)
                                {
                                    Content = messageLimit.Content,
                                    Type = ChatType.Text,
                                    RevertId = item.RevertId,
                                    ChatGroupId = item.Id
                                };

                                await _chatMessageRepository.CreateAsync(chatMessageLimit);
                                return;
                            }
                        }

                        var httpClient = _httpClientFactory.CreateClient(Constant.ChatGPT);
                        var response = await httpClient.PostAsJsonAsync("v1/chat/completions", new
                        {
                            model = "gpt-3.5-turbo",
                            temperature = 0,
                            stream = false,
                            max_tokens = 1000,
                            messages = new[]
                            {
                                new
                                {
                                    role = "system",
                                    content =
                                        "从现在开始你是EarthChat的智能助手，不管别人怎么问你的名称你只需要回复你是EarthChat的智能助手。"
                                },
                                new
                                {
                                    role = "user",
                                    content = value
                                }
                            }
                        });

                        // 如果状态码不对则需要异常
                        if (response.IsSuccessStatusCode)
                        {
                            var result = await response.Content.ReadFromJsonAsync<GetChatGPTDto>();
                            content = result?.choices?.FirstOrDefault()?.message?.content ?? "聊天机器人出错了";

                            // 当消息成功则扣除额度
                            if (await _redisClient.ExistsAsync(key))
                            {
                                await _redisClient.IncrByAsync(key, 1);
                            }
                            else
                            {
                                // 获取当前时间
                                DateTime now = DateTime.Now;

                                // 获取当天的23:59:59
                                DateTime endOfDay = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);

                                await _redisClient.IncrByAsync(key, 1);
                                await _redisClient.ExpireAsync(key, endOfDay - now);
                            }
                        }
                        else
                        {
                            content = "聊天机器人出错了，请联系管理员！";
                        }
                    }

                    var message = new ChatMessageDto
                    {
                        Content = content,
                        Type = ChatType.Text,
                        UserId = Guid.Empty,
                        RevertId = item.RevertId,
                        CreationTime = DateTime.Now,
                        GroupId = item.Id,
                        Id = Guid.NewGuid()
                    };

                    var messageQuery = new GetMessageQuery((Guid)message.RevertId);
                    await _eventBus.PublishAsync(messageQuery);
                    message.Revert = messageQuery.Result;
                    if (item.Group)
                    {
                        await _hubContext.Clients.Group(item.Id.ToString("N"))
                            .SendAsync("ReceiveMessage", item.Id, message);
                    }
                    else
                    {
                        await _hubContext.Clients.Group(item.Id.ToString("N"))
                            .SendAsync("ReceiveMessage", item.Id, message);
                    }

                    // 创建助手的消息
                    var chatMessage = new ChatMessage(Guid.NewGuid(), DateTime.Now)
                    {
                        Content = content,
                        Type = ChatType.Text,
                        RevertId = item.RevertId,
                        ChatGroupId = item.Id
                    };

                    await _chatMessageRepository.CreateAsync(chatMessage);
                }
                catch (Exception e)
                {
                    _logger.LogError("智能助手出现异常 {e}", e);
                }
            }
        }
    }

    /// <summary>
    /// 写入到管道。
    /// </summary>
    /// <param name="dto"></param>
    public async Task WriteAsync(AssistantDto dto)
    {
        await _channel.Writer.WriteAsync(dto);
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _serviceScope.Dispose();
    }
}