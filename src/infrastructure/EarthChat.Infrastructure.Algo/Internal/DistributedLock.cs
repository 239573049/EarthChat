using StackExchange.Redis;

namespace EarthChat.Infrastructure.Algo.Internal;


/// <summary>
/// 分布式锁
/// </summary>
public sealed class DistributedLock : IAsyncDisposable
{
    private readonly IDatabase _database;
    private readonly string _lockKey;
    private readonly TimeSpan _expiry;
    private readonly string _lockValue;
    private bool _isLocked;

    public DistributedLock(IDatabase database, string lockKey, TimeSpan expiry)
    {
        _database = database;
        _lockKey = lockKey;
        _expiry = expiry;
        _lockValue = Guid.NewGuid().ToString();
    }

    public async Task<bool> AcquireAsync(TimeSpan timeout, bool isThrow = false)
    {
        var endTime = DateTime.UtcNow.Add(timeout);
        int retryCount = 0;

        while (DateTime.UtcNow < endTime)
        {
            // 尝试获取锁
            if (await _database.StringSetAsync(_lockKey, _lockValue, _expiry, When.NotExists).ConfigureAwait(false))
            {
                _isLocked = true;
                return true;
            }

            // 使用指数退避策略进行等待
            var waitTime = TimeSpan.FromMilliseconds(Math.Pow(2, retryCount) * 10);
            if (waitTime > TimeSpan.FromMilliseconds(100)) // 可设置最大等待时间
            {
                waitTime = TimeSpan.FromMilliseconds(100);
            }

            await Task.Delay(waitTime).ConfigureAwait(false);

            retryCount++;
        }

        if (isThrow)
        {
            throw new IdempotentException("获取锁超时");
        }

        return false; // 超时未获取到锁
    }

    public async ValueTask ReleaseAsync()
    {
        if (_isLocked)
        {
            // 使用 Lua 脚本进行原子释放锁
            var script = @"
                if redis.call('get', KEYS[1]) == ARGV[1] then
                    return redis.call('del', KEYS[1])
                else
                    return 0
                end";

            await _database.ScriptEvaluateAsync(script, [_lockKey], [_lockValue]);
            _isLocked = false;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await ReleaseAsync();
    }
}