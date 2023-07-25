using Chat.Service.Application.Chats.Commands;
using Chat.Service.Domain.Chats.Aggregates;
using Chat.Service.Domain.Chats.Repositories;

namespace Chat.Service.Application.Chats;

public class CommandHandler
{
    private readonly IChatMessageRepository _chatMessageRepository;
    private readonly IUserContext _userContext;

    public CommandHandler(IChatMessageRepository chatMessageRepository, IUserContext userContext)
    {
        _chatMessageRepository = chatMessageRepository;
        _userContext = userContext;
    }

    [EventHandler]
    public async Task CreateAsync(CreateChatMessageCommand command)
    {
        var chatMessage = new ChatMessage(command.Dto.Id)
        {
            Content = command.Dto.Content,
            Extends = command.Dto.Extends ?? new Dictionary<string, string>(),
            Type = command.Dto.Type,

            UserId = _userContext.GetUserId<Guid>()
        };
        await _chatMessageRepository.AddAsync(chatMessage);
    }
}