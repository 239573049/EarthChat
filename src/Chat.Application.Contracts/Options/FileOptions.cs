namespace Chat.Options;

public class FileOptions
{
    /// <summary>
    /// 上传最大文件大小
    /// </summary>
    public int MaxSize { get; set; }

    /// <summary>
    /// 当前上传类型
    /// </summary>
    public UploadingFileType Type { get; set; }

    /// <summary>
    /// 上传文件返回的前缀
    /// </summary>
    public string Prefix { get; set; }
}