using AutoMapper;
using Chat.Contracts.Users;
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

    }
}