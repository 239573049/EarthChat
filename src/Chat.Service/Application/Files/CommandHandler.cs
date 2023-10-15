using Chat.Service.Application.Files.Commands;
using Chat.Service.Application.System.Commands;
using Chat.Service.Application.System.Queries;
using Chat.Service.Infrastructure.Helper;
using Infrastructure;
using SkiaSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using SkiaSharp;

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

            if (fileName.IsImage())
            {
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

            stream.Close();
            // 压缩图片
            var ext = Path.GetExtension(filePath);
            await CompressImage(filePath, filePath.Replace(ext, "") + "_compress" + ext);

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

    public static async Task CompressImage(string inputPath, string outputPath, int quality = 50)
    {
        using var inputStream = new SKFileStream(inputPath);
        using var original = SKBitmap.Decode(inputStream);
        var imageInfo = new SKImageInfo(original.Width, original.Height);

        using var surface = SKSurface.Create(imageInfo);
        surface.Canvas.DrawBitmap(original, 0, 0);
        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, quality);
        await using var fileStream = File.Create(outputPath);
        await data.AsStream().CopyToAsync(fileStream);
    }

    // [EventHandler]
    // public async Task MinIoAsync(UploadCommand command)
    // {
    //     
    // }
}