using Chat.Contracts.Chats;
using Chat.Contracts.Eto.Chat;
using Chat.Service.Application.Chats.Queries;
using Chat.Service.Domain.Chats.Aggregates;
using Chat.Service.Domain.Chats.Repositories;
using Chat.Service.Infrastructure.Helper;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Service.Application.Chats;

public class ChatMessageHandle  : IDisposable
{
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IEventBus _eventBus;
    private readonly RedisClient _redisClient;
    private readonly IServiceScope _serviceScope;
    private readonly IChatMessageRepository _chatMessageRepository;

    public ChatMessageHandle(IHubContext<ChatHub> hubContext,RedisClient redisClient, IServiceProvider serviceProvider)
    {
        _serviceScope = serviceProvider.CreateScope();
        _eventBus = _serviceScope.ServiceProvider.GetService<IEventBus>();
        _chatMessageRepository = _serviceScope.ServiceProvider.GetService<IChatMessageRepository>();
        _hubContext = hubContext;
        _redisClient = redisClient;
        _redisClient.Subscribe(nameof(ChatMessageEto), (async (s, o) =>
        {
            if (o is string str)
            {
                await HandleAsync(JsonSerializer.Deserialize<ChatMessageEto>(str));
            }
        }));
    }

    public async Task HandleAsync(ChatMessageEto eto)
    {
        if (eto.RevertId != null && eto.RevertId != Guid.Empty && eto.Revert == null)
        {
            var messageQuery = new GetMessageQuery((Guid)eto.RevertId);
            await _eventBus.PublishAsync(messageQuery);
            eto.Revert = messageQuery.Result;
        }


        await _hubContext.Clients.Group(eto.GroupId.ToString("N"))
            .SendAsync("ReceiveMessage", eto.Id, eto);

        if (eto is ChatMessageDto dto)
        {
            await _chatMessageRepository.CreateAsync(new ChatMessage(dto.Id, dto.CreationTime)
            {
                Countermand = dto.Countermand,
                ChatGroupId = dto.GroupId,
                Content = dto.Content,
                RevertId = eto.RevertId,
                Type = eto.Type,
                UserId = eto.UserId
            });
        }
    }

    public void Dispose()
    {
        _serviceScope.Dispose();
    }
}