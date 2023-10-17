using Chat.Service.Application.Files.Commands;
using Chat.Service.Application.System.Commands;
using Chat.Service.Application.System.Queries;
using Chat.Service.Infrastructure.Helper;
using Infrastructure;

namespace Chat.Service.Application.Files;

public class CommandHandler
{
    private readonly ILogger<CommandHandler> _logger;
    private readonly IEventBus _eventBus;
    private readonly IUserContext _userContext;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public CommandHandler(IEventBus eventBus, IUserContext userContext,
        ILogger<CommandHandler> logger, IWebHostEnvironment webHostEnvironment)
    {
        _eventBus = eventBus;
        _userContext = userContext;
        _logger = logger;
        _webHostEnvironment = webHostEnvironment;
    }

    [EventHandler]
    public async Task LocalAsync(UploadCommand command)
    {
        // 生成一个唯一的文件名
        var fileName =
            $"files/{DateTime.Now:yyyyMMdd}/{StringHelper.RandomString(12)}/{command.FileName}";
        
        // 在这里使用webroot的目录
        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, fileName);
        var info = new FileInfo(filePath);
        try
        {
            var dayUploadQuantityQuery = new DayUploadQuantityQuery(_userContext.GetUserId<Guid>());
            await _eventBus.PublishAsync(dayUploadQuantityQuery);

            if (dayUploadQuantityQuery.Result > Constant.File.MaxUploadFile)
            {
                throw new UserFriendlyException("您已经超出当天限量。");
            }

            // 当文件夹不存在则创建
            if (info.Directory?.Exists == false)
            {
                info.Directory.Create();
            }

            // 创建文件
            await using var stream = new FileStream(filePath, FileMode.Create);
            // 直接copy Stream性能更好
            await command.Stream.CopyToAsync(stream);
            
            // 关闭stream，防止压缩图片出现文件被占用
            stream.Close();

            // 在这里会判断当前是否为图片
            if (fileName.IsImage())
            {
                // 压缩图片
                var ext = Path.GetExtension(filePath);
                // 在压缩文件的时候会按照规则生成 .compress后缀名
                ImageHelper.FitImage(filePath, filePath.Replace(ext, "") + ".compress" + ext, 256, 256);
            }

            command.Result = $"/{fileName}";
            var createFileSystemCommand =
                new CreateFileSystemCommand(info.Name, info.FullName, $"/{fileName}", info.Length);

            await _eventBus.PublishAsync(createFileSystemCommand);
        }
        catch (Exception exception)
        {
            _logger.LogError("上传文件发生异常：{Exception}", exception);
            if (info.Exists)
            {
                info.Delete();
            }
        }
    }
}