using Chat.Service.Application.Users.Commands;
using Microsoft.AspNetCore.Authorization;

namespace Chat.Service.Services;

public class EmojiService : BaseService<EmojiService>
{
    [Authorize]
    public async Task<ResultDto> CreateAsync(string path)
    {
        var command = new AddEmojiCommand(path);
        await PublishAsync(command);
        return new ResultDto();
    }

    [Authorize]
    public async Task<ResultDto> DeleteAsync(Guid id)
    {
        var command = new DeleteEmojiCommand(id);
        await PublishAsync(command);
        return new ResultDto();
    }

    [Authorize]
    public async Task<ResultDto<IReadOnlyList<EmojiDto>>> GetListAsync()
    {
        var query = new GetEmojiQuery();
        await PublishAsync(query);

        return query.Result.CreateResult();
    }
}