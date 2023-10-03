using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.Contracts.Core;
using Chat.Contracts.Users;
using LiteDB;

namespace Chat.Client.Services;

public class UserService : IUserService
{
    private readonly LiteDatabase _liteDatabase;

    public UserService(LiteDatabase liteDatabase)
    {
        _liteDatabase = liteDatabase;
    }

    public Task<ResultDto<GetUserDto>> GetAsync()
    {
        return Caller.GetAsync<ResultDto<GetUserDto>>("Users");
    }

    public Task<ResultDto> CreateAsync(CreateUserDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<ResultDto> UpdateAsync(UpdateUserDto dto)
    {
        throw new NotImplementedException();
    }

    public async Task<List<UserDto>> ListAsync(List<Guid> userIds)
    {
        var users = _liteDatabase.GetCollection<UserDto>();
        var usersDto = users.Find(x => userIds.Contains(x.Id)).ToList();
        var notUsesIds = userIds.Where(x => usersDto.All(y => y.Id != x));

        if (notUsesIds.Count() > 0 )
        {
            var result = await Caller.PostAsync<IReadOnlyList<UserDto>>("Users/List", notUsesIds);
            usersDto.AddRange(result);
        }

        return usersDto;
    }

    public Task<ResultDto<bool>> FriendStateAsync(Guid friendId)
    {
        throw new NotImplementedException();
    }

    public Task FriendRegistrationAsync(FriendRegistrationInput input)
    {
        throw new NotImplementedException();
    }
}