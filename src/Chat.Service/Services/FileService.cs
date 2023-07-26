
using Chat.Service.Application.Files.Commands;

namespace Chat.Service.Services;

public class FileService : BaseService<FileService>
{
    public async Task<ResultDto<string>> UploadAsync(IFormFile file)
    {
        // 判断当前文件大小
        if (file.Length > 1024 * 1024 * 5)
        {
            return "文件大小不能超过2M".Fail<string>();
        }
        
        var command = new UploadCommand(file.OpenReadStream(), file.FileName);
        await _eventBus.PublishAsync(command);
        return command.Result.CreateResult();
    }
}