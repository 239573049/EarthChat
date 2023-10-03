using Chat.Contracts.Core;

namespace Chat.Contracts.Users;

public interface IEmojiService
{
    Task<ResultDto> CreateAsync(string path);

    Task<ResultDto> DeleteAsync(Guid id);

    Task<ResultDto<IReadOnlyList<EmojiDto>>> GetListAsync();
}