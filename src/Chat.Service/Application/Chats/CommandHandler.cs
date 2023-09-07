using System.Diagnostics;
using Chat.Contracts.Chats;
using Chat.Contracts.Hubs;
using Chat.Service.Application.Chats.Commands;
using Chat.Service.Application.Hubs.Commands;
using Chat.Service.Domain.Chats.Aggregates;
using Chat.Service.Domain.Chats.Repositories;
using Chat.Service.Domain.Users.Aggregates;
using Chat.Service.Domain.Users.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Service.Application.Chats;

public class CommandHandler
{
    private readonly IChatMessageRepository _chatMessageRepository;
    private readonly IChatGroupRepository _chatGroupRepository;
    private readonly IChatGroupInUserRepository _chatGroupInUserRepository;
    private readonly IUserContext _userContext;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IFriendRepository _friendRepository;
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly ILogger<CommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventBus _eventBus;
    private readonly IFriendRequestRepository _friendRequestRepository;

    public CommandHandler(IChatMessageRepository chatMessageRepository, IUserContext userContext,
        IHttpClientFactory httpClientFactory, IHubContext<ChatHub> hubContext, IEventBus eventBus,
        ILogger<CommandHandler> logger, IChatGroupRepository chatGroupRepository, IUnitOfWork unitOfWork,
        IChatGroupInUserRepository chatGroupInUserRepository, IFriendRepository friendRepository,
        IFriendRequestRepository friendRequestRepository)
    {
        _chatMessageRepository = chatMessageRepository;
        _userContext = userContext;
        _httpClientFactory = httpClientFactory;
        _hubContext = hubContext;
        _eventBus = eventBus;
        _logger = logger;
        _chatGroupRepository = chatGroupRepository;
        _unitOfWork = unitOfWork;
        _chatGroupInUserRepository = chatGroupInUserRepository;
        _friendRepository = friendRepository;
        _friendRequestRepository = friendRequestRepository;
    }

    [EventHandler]
    public async Task CreateAsync(CreateChatMessageCommand command)
    {
        var chatMessage = new ChatMessage(command.Dto.Id, DateTime.Now)
        {
            Content = command.Dto.Content,
            Extends = command.Dto.Extends ?? new Dictionary<string, string>(),
            Type = command.Dto.Type,
            ChatGroupId = command.Dto.ChatGroupId,
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

            if (value.IsNullOrWhiteSpace())
            {
                return;
            }
            
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

                if (command.group)
                {
                    var message = new ChatMessageDto
                    {
                        Content = content,
                        Type = ChatType.Text,
                        UserId = Guid.Empty,
                        CreationTime = DateTime.Now,
                        GroupId = command.id,
                        Id = Guid.NewGuid(),
                        User = new GetUserDto
                        {
                            Avatar = "https://blog-simple.oss-cn-shenzhen.aliyuncs.com/ai.png",
                            Id = Guid.Empty,
                            Name = "聊天机器人",
                        }
                    };

                    await _hubContext.Clients.Group(command.id.ToString("N")).SendAsync("ReceiveMessage", command.id,message);
                }
                else
                {
                    
                }

                // 创建助手的消息
                var chatMessage = new ChatMessage(Guid.NewGuid(), DateTime.Now)
                {
                    Content = content,
                    Extends = new Dictionary<string, string>(),
                    Type = ChatType.Text,
                    ChatGroupId = command.id
                };

                await _chatMessageRepository.AddAsync(chatMessage);
            }
            catch (Exception e)
            {
                _logger.LogError("智能助手出现异常 {e}", e);
                
                var message = new ChatMessageDto
                {
                    Content = "机器人出错了，请联系管理员检查！",
                    Type = ChatType.Text,
                    UserId = Guid.Empty,
                    CreationTime = DateTime.Now,
                    GroupId = command.id,
                    Id = Guid.NewGuid(),
                    User = new GetUserDto
                    {
                        Avatar = "https://blog-simple.oss-cn-shenzhen.aliyuncs.com/ai.png",
                        Id = Guid.Empty,
                        Name = "聊天机器人",
                    }
                };
                
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", JsonSerializer.Serialize(message, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));
            }
        }
    }

    [EventHandler]
    public async Task CreateGroupAsync(CreateGroupCommand command)
    {
        if (await _chatGroupRepository.GetCountAsync(x => x.Creator == _userContext.GetUserId<Guid>()) > 10)
            throw new UserFriendlyException("最多只能创建10个群组");

        var chatGroup = new ChatGroup(Guid.NewGuid())
        {
            Avatar = command.Dto.Avatar,

            Description = command.Dto.Description,
            Name = command.Dto.Name,
        };

        await _chatGroupRepository.AddAsync(chatGroup);

        var chatGroupInUser = new ChatGroupInUser()
        {
            ChatGroupId = chatGroup.Id,
            UserId = _userContext.GetUserId<Guid>()
        };

        await _chatGroupInUserRepository.AddAsync(chatGroupInUser);

        // 新建群聊的时候想要将当前链接加入群聊。
        await _hubContext.Groups.AddToGroupAsync(command.connections, chatGroup.Id.ToString("N"));
    }

    [EventHandler]
    public async Task InvitationGroupAsync(InvitationGroupCommand command)
    {
        if (await _chatGroupInUserRepository.GetCountAsync(x =>
                x.ChatGroupId == command.id && x.UserId == _userContext.GetUserId<Guid>()) > 0)
        {
            throw new UserFriendlyException("您已经加入群聊");
        }

        if (await _chatGroupRepository.GetCountAsync(x => x.Id == command.id) <= 0)
        {
            throw new UserFriendlyException("群聊不存在");
        }

        await _chatGroupInUserRepository.AddAsync(new ChatGroupInUser()
        {
            ChatGroupId = command.id,
            UserId = _userContext.GetUserId<Guid>()
        });
    }

    [EventHandler]
    public async Task ApplyForFriendAsync(ApplyForFriendCommand command)
    {
        if (await _friendRepository.GetCountAsync(x => x.FriendId == command.Dto.BeAppliedForId) > 0)
        {
            throw new UserFriendlyException("已经存在好友关系");
        }

        if (await _friendRequestRepository.GetCountAsync(x =>
                x.RequestId == _userContext.GetUserId<Guid>() && x.State == FriendState.ApplyFor) > 0)
        {
            throw new UserFriendlyException("已经存在申请");
        }

        var value = new FriendRequest()
        {
            ApplicationDate = DateTime.Now,
            RequestId = _userContext.GetUserId<Guid>(),
            BeAppliedForId = command.Dto.BeAppliedForId,
            Description = command.Dto.Description,
            State = FriendState.ApplyFor
        };

        await _friendRequestRepository.AddAsync(value);

        var systemCommand = new SystemCommand(new Notification()
        {
            createdTime = DateTime.Now,
            type = NotificationType.FriendRequest,
            content = "发起新的好友申请",
        }, new[] { command.Dto.BeAppliedForId }, false);
        await _eventBus.PublishAsync(systemCommand);
    }

    [EventHandler]
    public async Task ApplicationProcessingAsync(ApplicationProcessingCommand command)
    {
        var value = await _friendRequestRepository.FindAsync(x => x.Id == command.Id);

        if (value?.State == FriendState.ApplyFor)
        {
            if (command.State == FriendState.Consent)
            {
                await _friendRepository.AddAsync(new Friend()
                {
                    SelfId = _userContext.GetUserId<Guid>(),
                    FriendId = value.RequestId
                });

                await _friendRepository.AddAsync(new Friend()
                {
                    SelfId = value.RequestId,
                    FriendId = _userContext.GetUserId<Guid>()
                });

                var systemCommand = new SystemCommand(new Notification()
                {
                    createdTime = DateTime.Now,
                    type = NotificationType.FriendRequest,
                    content = "同意了好友申请",
                }, new[] { value.RequestId }, false);
                await _eventBus.PublishAsync(systemCommand);
            }

            value.State = command.State;

            await _friendRequestRepository.UpdateAsync(value);
        }
    }
}