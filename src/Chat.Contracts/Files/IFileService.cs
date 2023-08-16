using Chat.Contracts.Core;

namespace Chat.Contracts.Files;

public interface IFileService
{
    Task<ResultDto<string>?> UploadBase64Async(UploadBase64Dto dto);
}