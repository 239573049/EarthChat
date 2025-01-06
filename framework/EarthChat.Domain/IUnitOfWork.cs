namespace EarthChat.Domain;

public interface IUnitOfWork
{
    /// <summary>
    /// 开启事务
    /// </summary>
    /// <returns></returns>
    Task BeginTransactionAsync();

    /// <summary>
    /// 提交事务
    /// </summary>
    /// <returns></returns>
    Task CommitTransactionAsync();

    /// <summary>
    /// 回滚事务
    /// </summary>
    /// <returns></returns>
    Task RollbackTransactionAsync();

    /// <summary>
    /// 保存更改
    /// </summary>
    /// <returns></returns>
    Task<int> SaveChangesAsync();
}