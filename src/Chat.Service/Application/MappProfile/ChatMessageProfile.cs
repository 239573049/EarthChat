﻿using AutoMapper;
using Chat.Contracts.Chats;
using Chat.Service.Domain.Chats.Aggregates;
using Chat.Service.Infrastructure.Repositories.Views;

namespace Chat.Service.Application.MappProfile;

public class ChatMessageProfile : Profile
{
    public ChatMessageProfile()
    {
        CreateMap<ChatMessageDto, ChatMessage>();
        CreateMap<ChatMessage, ChatMessageDto>()
            .ForMember(x => x.CreationTime, x => x.MapFrom(x => x.CreationTime.AddHours(-8)));

        CreateMap<ChatGroup,ChatGroupDto>();

        CreateMap<ChatGroupInUserDto, ChatGroupInUser>();

        CreateMap<ChatMessageView,ChatMessageDto>();
    }
}