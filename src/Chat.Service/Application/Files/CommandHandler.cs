using Chat.Service.Application.Files.Commands;
using Chat.Service.Application.System.Commands;
using Chat.Service.Infrastructure.Helper;

namespace Chat.Service.Application.Files;

public class CommandHandler
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IEventBus _eventBus;

    public CommandHandler(IHttpContextAccessor contextAccessor, IEventBus eventBus)
    {
        _contextAccessor = contextAccessor;
        _eventBus = eventBus;
    }

    [EventHandler]
    public async Task LocalAsync(UploadCommand command)
    {
        var fileName =
            $"files/{DateTime.Now:yyyyMMdd}/{StringHelper.RandomString(8)}{Path.GetExtension(command.FileName)}";
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileName);
        var info = new FileInfo(filePath);
        try
        {
            if (!info.Directory.Exists)
            {
                info.Directory.Create();
            }

            await using var stream = new FileStream(filePath, FileMode.Create);
            await command.Stream.CopyToAsync(stream);

            // 回去当前请求的域名
            var host = _contextAccessor.HttpContext!.Request.Host.Value;

            // 判断是否https
            if (_contextAccessor.HttpContext!.Request.IsHttps)
            {
                host = $"https://{host}";
            }
            else
            {
                host = $"http://{host}";
            }

            command.Result = $"{host}/{fileName}";
            var createFileSystemCommand =
                new CreateFileSystemCommand(info.Name, info.FullName, $"{host}/{fileName}", info.Length);
            
            await _eventBus.PublishAsync(createFileSystemCommand);
        }
        catch (Exception exception)
        {
            if (info.Exists)
            {
                info.Delete();
            }
        }
    }

    // [EventHandler]
    // public async Task MinIoAsync(UploadCommand command)
    // {
    //     
    // }
}