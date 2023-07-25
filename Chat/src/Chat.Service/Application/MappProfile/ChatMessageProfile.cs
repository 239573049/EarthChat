using AutoMapper;
using Chat.Contracts.Chats;
using Chat.Service.Domain.Chats.Aggregates;

namespace Chat.Service.Application.MappProfile;

public class ChatMessageProfile : Profile
{
    public ChatMessageProfile()
    {
        CreateMap<ChatMessageDto, ChatMessage>();
        CreateMap<ChatMessage, ChatMessageDto>();
    }
}