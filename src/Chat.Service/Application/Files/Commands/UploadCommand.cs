namespace Chat.Service.Application.Files.Commands;

public record UploadCommand(Stream Stream, string FileName) : Command
{
    public string Result { get; set; }
}