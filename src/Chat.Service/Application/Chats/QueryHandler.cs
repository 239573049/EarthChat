using AutoMapper;
using Chat.Contracts.Chats;
using Chat.Service.Application.Chats.Queries;
using Chat.Service.Domain.Chats.Repositories;
using Chat.Service.Domain.Users.Aggregates;

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

        foreach (var message in list.Where(message => message.UserId == Guid.Empty))
        {
            message.User = new User(Guid.Empty)
            {
                Account = string.Empty,
                Avatar = "https://blog-simple.oss-cn-shenzhen.aliyuncs.com/ai.png",
                Name = "聊天机器人",
            };
        }

        query.Result = new PaginatedListBase<ChatMessageDto>
        {
            Result = _mapper.Map<List<ChatMessageDto>>(list.OrderBy(x => x.CreationTime)),
            Total = await _chatMessageRepository.GetCountAsync()
        };
    }
}