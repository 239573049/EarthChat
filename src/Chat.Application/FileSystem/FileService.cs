using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Chat.FileSystem;

public class FileService : IFileService
{
    public Task UploadingAsync(IFormFile file)
    {
        throw new System.NotImplementedException();
    }
}