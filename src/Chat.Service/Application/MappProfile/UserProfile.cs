using AutoMapper;
using Chat.Contracts.Chats;
using Chat.Service.Domain.Chats.Aggregates;
using Chat.Service.Domain.Users.Aggregates;

namespace Chat.Service.Application.MappProfile;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserDto, User>()
            .ReverseMap();

        CreateMap<GetUserDto, User>();

        CreateMap<User, GetUserDto>();
        CreateMap<ChatGroupInUser, ChatGroupInUserDto>();

        CreateMap<FriendRequestDto, FriendRequest>()
            .ReverseMap();
    }
}