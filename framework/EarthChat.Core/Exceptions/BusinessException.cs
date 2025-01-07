namespace EarthChat.Core.Exceptions;

public class BusinessException : Exception
{
    public readonly string? Code;

    public BusinessException(string message) : base(message)
    {
    }

    public BusinessException(string code, string message) : base(message)
    {
        Code = code;
    }
}