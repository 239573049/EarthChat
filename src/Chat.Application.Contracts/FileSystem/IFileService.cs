using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Chat.FileSystem;

public interface IFileService
{
    Task<string> UploadingAsync(IFormFile file);
}