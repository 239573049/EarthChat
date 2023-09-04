using System.Net.Http.Json;
using System.Threading.Tasks;
using Chat.Contracts.Core;
using Chat.Contracts.Files;

namespace Chat.Client.Services;

public class FileService : IFileService
{
    public async Task<ResultDto<string>?> UploadBase64Async(UploadBase64Dto dto)
    {
        var response = await Caller.GetHttpClient().PostAsJsonAsync("Files/UploadBase64", dto);
        
        return await response.Content.ReadFromJsonAsync<ResultDto<string>>();
    }

    public Task DeleteAsync(string uri)
    {
        throw new NotImplementedException();
    }
}