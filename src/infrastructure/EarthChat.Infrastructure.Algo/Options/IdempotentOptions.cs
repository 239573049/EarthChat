namespace EarthChat.Infrastructure.Algo.Options;

/// <summary>
/// 幂等输入
/// </summary>
/// <param name="WaitTime">
/// 如果不为空则等待时间，如果存在则等待
/// </param>
/// <param name="IsThrow">
/// 如果存在是否抛出异常，如果设置了等待时间，但是等待完成后还是存在则抛出异常
/// </param>
/// <param name="CancellationToken"></param>
public record IdempotentOptions(
    TimeSpan? WaitTime = null,
    bool IsThrow = false,
    CancellationToken CancellationToken = default);