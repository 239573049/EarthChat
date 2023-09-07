using AutoMapper;
using Chat.Service.Domain.Users.Aggregates;

namespace Chat.Service.Application.MappProfile;

public class EmojiProfile : Profile
{
    public EmojiProfile()
    {
        CreateMap<EmojiDto, Emoji>()
            .ReverseMap();
    }
}