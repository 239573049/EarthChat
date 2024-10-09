namespace EarthChat.EntityFrameworkCore.Options;

public class EarthDbContextOptions
{
    /// <summary>
    /// 是否启用软删除
    /// </summary>
    public bool EnableSoftDelete { get; set; } = false;

    /// <summary>
    /// 是否启用多租户
    /// </summary>
    public bool EnableMultiTenancy { get; set; } = false;

    /// <summary>
    /// 是否启用审计
    /// </summary>
    public bool EnableAudit { get; set; } = false;
}