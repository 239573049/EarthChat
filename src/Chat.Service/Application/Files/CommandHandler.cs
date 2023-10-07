using Chat.Service.Application.Files.Commands;
using Chat.Service.Application.System.Commands;
using Chat.Service.Application.System.Queries;
using Chat.Service.Infrastructure.Helper;

namespace Chat.Service.Application.Files;

public class CommandHandler
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IEventBus _eventBus;
    private readonly IUserContext _userContext;

    public CommandHandler(IHttpContextAccessor contextAccessor, IEventBus eventBus, IUserContext userContext)
    {
        _contextAccessor = contextAccessor;
        _eventBus = eventBus;
        _userContext = userContext;
    }

    [EventHandler]
    public async Task LocalAsync(UploadCommand command)
    {
        var fileName =
            $"files/{DateTime.Now:yyyyMMdd}/{StringHelper.RandomString(12)}/{command.FileName}";
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileName);
        var info = new FileInfo(filePath);
        try
        {
            var dayUploadQuantityQuery = new DayUploadQuantityQuery(_userContext.GetUserId<Guid>());
            await _eventBus.PublishAsync(dayUploadQuantityQuery);

            if (dayUploadQuantityQuery.Result > 500)
            {
                throw new UserFriendlyException("您已经超出当天限量。");
            }
            
            
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