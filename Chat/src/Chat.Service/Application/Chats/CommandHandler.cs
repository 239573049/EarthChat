using Chat.Service.Application.Chats.Commands;
using Chat.Service.Domain.Chats.Aggregates;
using Chat.Service.Domain.Chats.Repositories;
using Masa.BuildingBlocks.Authentication.Identity;

namespace Chat.Service.Application.Chats;

public class CommandHandler
{
    private readonly IUserContext _userContext;
    private readonly IChatMessageRepository _chatMessageRepository;

    public CommandHandler(IChatMessageRepository chatMessageRepository, IUserContext userContext)
    {
        _chatMessageRepository = chatMessageRepository;
        _userContext = userContext;
    }

    [EventHandler]
    public async Task CreateAsync(CreateChatMessageCommand command)
    {
        var chatMessage = new ChatMessage(Guid.NewGuid())
        {
            Cotnent = command.Dto.Cotnent,
            Extends = command.Dto.Extends,
            Type = command.Dto.Type,
            UserId = _userContext.GetUserId<Guid>()
        };
        await _chatMessageRepository.AddAsync(chatMessage);
    }
}