using EarthChat.Infrastructure.Algo.Internal;
using EarthChat.Infrastructure.Algo.Options;

namespace EarthChat.Infrastructure.Algo;


public interface IAlgoService
{
    /// <summary>
    /// 接口幂等处理
    /// </summary>
    /// <param name="expire">
    /// 过期时间
    /// </param>
    /// <param name="options"></param>
    /// <param name="key">
    /// 幂等Key
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// 当设置了等待，但是等待时间为空时抛出
    /// </exception>
    /// <returns></returns>
    /// <exception cref="IdempotentException">
    /// 当存在或等待超时还未释放时抛出，需要先设置 IsThrow 为 true
    /// </exception>
    Task<bool> IdempotentAsync(
        string key,
        TimeSpan expire, IdempotentOptions? options = null);

    /// <summary>
    /// 分布式锁
    /// 提供分布式锁功能，如果锁已经被占用则等待，等待时间为 waitExpire，如果等待时间超时则返回 false，也可以设置 isThrow 为 true 抛出异常
    /// </summary>
    /// <param name="key">
    ///     锁的Key
    /// </param>
    /// <param name="waitExpire">
    ///     等待时间
    /// </param>
    /// <param name="isThrow">
    ///     是否抛出异常，默认为 false
    /// </param>
    /// <returns></returns>
    Task<bool> LockAsync(string key, TimeSpan waitExpire, bool isThrow = false);

    /// <summary>
    /// 创建分布式锁
    /// </summary>
    /// <param name="key"></param>
    /// <param name="waitExpire"></param>
    /// <param name="isThrow"></param>
    /// <returns></returns>
    Task<DistributedLock> CreateLockAsync(string key, TimeSpan waitExpire, bool isThrow = false);
}