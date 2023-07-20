namespace Chat.Options;

public enum UploadingFileType
{
    /// <summary>
    /// 本地存储
    /// </summary>
    Local,

    /// <summary>
    /// MinIO存储
    /// </summary>
    MinIO,

    /// <summary>
    /// OSS存储
    /// </summary>
    OSS
}