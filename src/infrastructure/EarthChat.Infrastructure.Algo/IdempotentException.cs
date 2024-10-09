namespace EarthChat.Infrastructure.Algo;

/// <summary>
///     幂等异常
/// </summary>
public class IdempotentException : Exception
{
    public IdempotentException(string message) : base(message)
    {
    }
}