using AutoMapper;
using Chat.Chat;
using Chat.Chats.Dtos;

namespace Chat.MapperProfile;

public class ChatProfile : Profile
{
    public ChatProfile()
    {
        CreateMap<ChatGroupDto, ChatGroup>()
            .ReverseMap();


    }
}