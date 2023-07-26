using Chat.Service.Application.Files.Commands;
using Chat.Service.Infrastructure.Helper;

namespace Chat.Service.Application.Files;

public class CommandHandler
{
    private readonly IHttpContextAccessor _contextAccessor;

    public CommandHandler(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    [EventHandler]
    public async Task LocalAsync(UploadCommand command)
    {
        var fileName =
            $"files/{DateTime.Now:yyyyMMdd}/{StringHelper.RandomString(8)}{Path.GetExtension(command.FileName)}";
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileName);
        var info = new FileInfo(filePath);
        if (!info.Directory.Exists)
        {
            info.Directory.Create();
        }

        await using var stream = new FileStream(filePath, FileMode.Create);
        await command.Stream.CopyToAsync(stream);

        // 回去当前请求的域名
        var host = _contextAccessor.HttpContext.Request.Host.Value;

        // 判断是否https
        if (_contextAccessor.HttpContext.Request.IsHttps)
        {
            host = $"https://{host}";
        }
        else
        {
            host = $"http://{host}";
        }

        command.Result = $"{host}/{fileName}";
    }

    // [EventHandler]
    // public async Task MinIoAsync(UploadCommand command)
    // {
    //     
    // }
}