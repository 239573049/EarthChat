namespace Chat.Service.Domain.System.Aggregates;

public class FileSystem  : AuditAggregateRoot<Guid, Guid>
{
    /// <summary>
    /// 文件名
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// 文件实际目录
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// 文件实际大小
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// 访问地址
    /// </summary>
    public string Uri { get; set; }
}