using System.Diagnostics;
using System.Threading.Channels;
using Chat.Contracts.Chats;
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
    
    public BackgroundTaskService(IServiceProvider serviceProvider, IHttpClientFactory httpClientFactory, ILogger<BackgroundTaskService> logger)
    {
        _serviceProvider = serviceProvider;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        
        // 请注意由于仓储使用的是作用域，所以需要在这个单例的服务中创建一个作用域。
        _serviceScope = serviceProvider.CreateScope();
        _chatMessageRepository = _serviceScope.ServiceProvider.GetRequiredService<IChatMessageRepository>();
        _hubContext = _serviceScope.ServiceProvider.GetRequiredService<IHubContext<ChatHub>>();
        
        // 使用管道的话会控制ChatGPT的数量，相当于当前系统只运行一个服务。
        _channel = Channel.CreateBounded<AssistantDto>(5000);
        _cancellationTokenSource = new CancellationTokenSource();
        Task.Factory.StartNew(Start);
    }

    private async Task Start()
    {
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            var item = await _channel.Reader.ReadAsync(_cancellationTokenSource.Token);
            if (item != null)
            {
                // 触发聊天机器人
                if (item.Value.StartsWith("ai"))
                {
                    // 判断是否需要获取当前服务的状态

                    string value = item.Value[2..].Trim();

                    if (value.IsNullOrWhiteSpace())
                    {
                        return;
                    }

                    try
                    {
                        string content = string.Empty;
                        if (item.Value.StartsWith("ai status"))
                        {
                            // 获取当前进程的内存占用，和cpu占用
                            var process = Process.GetCurrentProcess();
                            content =
                                $"当前内存占用：{process.WorkingSet64 / 1024 / 1024}MB";
                        }
                        else
                        {
                            var httpClient = _httpClientFactory.CreateClient(Constant.ChatGPT);
                            var response = await httpClient.PostAsJsonAsync("v1/chat/completions", new
                            {
                                model = "gpt-3.5-turbo",
                                temperature = 0.5,
                                stream = false,
                                max_tokens = 1000,
                                messages = new[]
                                {
                                    new
                                    {
                                        role = "user",
                                        content = value
                                    }
                                }
                            });

                            var result = await response.Content.ReadFromJsonAsync<GetChatGPTDto>();
                            content = result?.choices.FirstOrDefault()?.message.content ?? "聊天机器人出错了";
                        }

                        if (item.Group)
                        {
                            var message = new ChatMessageDto
                            {
                                Content = content,
                                Type = ChatType.Text,
                                UserId = Guid.Empty,
                                CreationTime = DateTime.Now,
                                GroupId = item.Id,
                                Id = Guid.NewGuid(),
                                User = new GetUserDto
                                {
                                    Avatar = "https://blog-simple.oss-cn-shenzhen.aliyuncs.com/ai.png",
                                    Id = Guid.Empty,
                                    Name = "聊天机器人",
                                }
                            };

                            await _hubContext.Clients.Group(item.Id.ToString("N"))
                                .SendAsync("ReceiveMessage", item.Id, message);
                        }
                        else
                        {
                            // TODO: 好友发送处理；
                        }

                        // 创建助手的消息
                        var chatMessage = new ChatMessage(Guid.NewGuid(), DateTime.Now)
                        {
                            Content = content,
                            Type = ChatType.Text,
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