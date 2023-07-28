using System.Diagnostics;
using Chat.Contracts.Chats;
using Chat.Service.Application.Chats.Commands;
using Chat.Service.Domain.Chats.Aggregates;
using Chat.Service.Domain.Chats.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Service.Application.Chats;

public class CommandHandler
{
    private readonly IChatMessageRepository _chatMessageRepository;
    private readonly IUserContext _userContext;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly ILogger<CommandHandler> _logger;
    private readonly IEventBus _eventBus;

    public CommandHandler(IChatMessageRepository chatMessageRepository, IUserContext userContext,
        IHttpClientFactory httpClientFactory, IHubContext<ChatHub> hubContext, IEventBus eventBus, ILogger<CommandHandler> logger)
    {
        _chatMessageRepository = chatMessageRepository;
        _userContext = userContext;
        _httpClientFactory = httpClientFactory;
        _hubContext = hubContext;
        _eventBus = eventBus;
        _logger = logger;
    }

    [EventHandler]
    public async Task CreateAsync(CreateChatMessageCommand command)
    {
        var chatMessage = new ChatMessage(command.Dto.Id, DateTime.Now)
        {
            Content = command.Dto.Content,
            Extends = command.Dto.Extends ?? new Dictionary<string, string>(),
            Type = command.Dto.Type,
            UserId = _userContext.GetUserId<Guid>()
        };
        await _chatMessageRepository.AddAsync(chatMessage);
    }

    [EventHandler]
    public async Task ChatGPTAsync(ChatGPTCommand command)
    {
        // 触发聊天机器人
        if (command.value.StartsWith("ai"))
        {
            // 判断是否需要获取当前服务的状态

            string value = command.value[2..].Trim();
            try
            {
                string content = string.Empty;
                if (command.value.StartsWith("ai status"))
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


                await _hubContext.Clients.All.SendAsync("ReceiveMessage", JsonSerializer.Serialize(new ChatMessageDto
                {
                    Content = content,
                    Type = ChatType.Text,
                    User = new GetUserDto
                    {
                        Id = Guid.Empty,
                        Name = "聊天机器人",
                        Avatar = "https://blog-simple.oss-cn-shenzhen.aliyuncs.com/ai.png"
                    },
                    CreationTime = DateTime.Now
                }, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));

                var chatMessage = new ChatMessage(Guid.NewGuid(), DateTime.Now)
                {
                    Content = content,
                    Extends = new Dictionary<string, string>(),
                    Type = ChatType.Text,
                    UserId = Guid.Empty
                };

                await _chatMessageRepository.AddAsync(chatMessage);
            }
            catch (Exception e)
            {
                _logger.LogError("智能助手出现异常", e);
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", JsonSerializer.Serialize(new ChatMessageDto
                {
                    Content = "机器人出错了，请联系管理员检查！",
                    Type = ChatType.Text,
                    User = new GetUserDto
                    {
                        Id = Guid.Empty,
                        Name = "聊天机器人",
                        Avatar = "https://blog-simple.oss-cn-shenzhen.aliyuncs.com/ai.png"
                    },
                    CreationTime = DateTime.Now
                }, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));
            }
        }
    }
}