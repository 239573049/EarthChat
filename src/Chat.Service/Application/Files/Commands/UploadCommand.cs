namespace Chat.Service.Application.Files.Commands;

/// <summary>
/// 上传文件
/// </summary>
/// <param name="Stream"></param>
/// <param name="FileName"></param>
public record UploadCommand(Stream Stream, string FileName) : Command
{
    public string Result { get; set; }
}