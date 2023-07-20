using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Chat.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;
using FileOptions = Chat.Options.FileOptions;

namespace Chat.FileSystem;

public class FileService : IFileService
{
    private readonly FileOptions _fileOptions;
    private readonly IHttpContextAccessor _httpContext;
    private readonly IServiceProvider _serviceProvider;
    private readonly MinIOOptions _minIOOptions;

    public FileService(IOptions<FileOptions> fileOptions, IHttpContextAccessor httpContext,
        IServiceProvider serviceProvider, IOptions<MinIOOptions> minIoOptions)
    {
        _httpContext = httpContext;
        _serviceProvider = serviceProvider;
        _minIOOptions = minIoOptions.Value;
        _fileOptions = fileOptions.Value;
    }

    public async Task<string> UploadingAsync(IFormFile file)
    {
        if (_fileOptions.Type == UploadingFileType.Local)
        {
            var name = Path.Combine("file", DateTime.Now.ToString("yyyyMMdd"), $"{Guid.NewGuid():N}" + file.FileName);
            var path = new FileInfo(Path.Combine(AppContext.BaseDirectory, "wwwroot", name));
            if (!path.Directory.Exists)
            {
                path.Directory.Create();
            }

            await using var fileStream = path.Create();
            await file.OpenReadStream().CopyToAsync(fileStream);

            // 获取当前请求的域名
            var host = _httpContext.HttpContext.Request.Host;

            return $"{(_httpContext.HttpContext.Request.IsHttps ? "https" : "http")}://{host}/{name}";
        }

        if (_fileOptions.Type == UploadingFileType.MinIO)
        {
            var name = Path.Combine("file", DateTime.Now.ToString("yyyyMMdd"), $"{Guid.NewGuid():N}" + file.FileName);
            var client = _serviceProvider.GetRequiredService<MinioClient>();
            var args = new PutObjectArgs();
            args.WithBucket(_minIOOptions.Bucket);
            args.WithStreamData(file.OpenReadStream());
            args.WithFileName(name);
            var result = await client.PutObjectAsync(args);
            return  $"{result.ObjectName}";
        }

        return "";
    }
}