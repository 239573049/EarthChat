using AutoMapper;
using Chat.Contracts.Chats;
using Chat.Service.Application.Chats.Queries;
using Chat.Service.Domain.Chats.Repositories;

namespace Chat.Service.Application.Chats;

public class QueryHandler
{
    private readonly IChatMessageRepository _chatMessageRepository;
    private readonly IMapper _mapper;

    public QueryHandler(IChatMessageRepository chatMessageRepository, IMapper mapper)
    {
        _chatMessageRepository = chatMessageRepository;
        _mapper = mapper;
    }

    [EventHandler]
    public async Task GetListAsync(GeChatMessageListQuery query)
    {
        var list = await _chatMessageRepository.GetListAsync(query.page, query.pageSize);
        query.Result = new PaginatedListBase<ChatMessageDto>()
        {
            Result = _mapper.Map<List<ChatMessageDto>>(list),
            Total = await _chatMessageRepository.GetCountAsync()
        };
    }
}