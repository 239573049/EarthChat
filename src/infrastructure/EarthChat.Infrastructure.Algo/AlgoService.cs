using EarthChat.Infrastructure.Algo.Internal;
using EarthChat.Infrastructure.Algo.Options;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace EarthChat.Infrastructure.Algo;

/// <summary>
/// 算法服务
/// 提供需要算法解决的基础组件
/// </summary>
public sealed class AlgoService : IAlgoService
{
    private readonly IConnectionMultiplexer _connection;
    private readonly RedisOptions _redisOptions;

    public AlgoService(IConnectionMultiplexer connection, IOptions<RedisOptions> redisOptions)
    {
        _connection = connection;
        _redisOptions = redisOptions.Value;
    }

    private IDatabase Db
    {
        get
        {
            if (_connection.IsConnected || _connection.IsConnecting)
                return _connection.GetDatabase();

            throw new NotSupportedException(
                "Redis service has been disconnected, please wait for reconnection and try again");
        }
    }

    /// <inheritdoc />
    public async Task<bool> IdempotentAsync(
        string key,
        TimeSpan expire, IdempotentOptions? options = null)
    {
        // 生成幂等键，根据幂等键判断是否已经处理过，如果处理过则返回true，设置过期时间
        key = $"{_redisOptions.Prefix}:{key}";

        var db = Db;

        if (options?.WaitTime != null)
        {
            // 等待超时时间
            var waitTime = options.WaitTime;

            await using var distributedLock = new DistributedLock(db, key, waitTime.Value);

            return await distributedLock.AcquireAsync(waitTime.Value, options.IsThrow).ConfigureAwait(false);
        }

        var value = await db.StringGetAsync(key).ConfigureAwait(false);

        if (value.HasValue)
        {
            if (options?.IsThrow == true)
                throw new IdempotentException("幂等处理中");

            return false;
        }

        await db.StringSetAsync(key, key, expire).ConfigureAwait(false);

        return true;
    }

    /// <inheritdoc />
    public async Task<bool> LockAsync(string key, TimeSpan waitExpire, bool isThrow = false)
    {
        key = $"{_redisOptions.Prefix}:{key}";

        var db = Db;

        await using var distributedLock = new DistributedLock(db, key, waitExpire);

        return await distributedLock.AcquireAsync(waitExpire, isThrow).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<DistributedLock> CreateLockAsync(string key, TimeSpan waitExpire, bool isThrow = false)
    {
        key = $"{_redisOptions.Prefix}:{key}";

        var db = Db;

        var distributedLock = new DistributedLock(db, key, waitExpire);
        
        await distributedLock.AcquireAsync(waitExpire, isThrow).ConfigureAwait(false);
        
        return distributedLock;
    }
}