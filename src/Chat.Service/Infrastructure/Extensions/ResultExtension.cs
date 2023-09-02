namespace Chat.Service.Infrastructure.Extensions;

public static class ResultExtension
{
    public static ResultDto CreateExceptionResult(this Exception exception, string code) 
    {
        return new()
        {
            Code = code,
            Message = exception.Message
        };
    }
    
    public static ResultDto<T> CreateResult<T>(this T data) where T : class
    {
        return new(data);
    }
    
    public static ResultDto<T> Fail<T>(this T data, string code = "400") where T : class
    {
        return new()
        {
            Data = data,
            Code = code
        };
    }

    public static ResultDto<T> CreateResult<T>(this T data, string code) where T : class
    {
        return new()
        {
            Data = data,
            Code = code
        };
    }
    
    public static ResultDto<T> CreateResult<T>(this T data, string code, string message) where T : class
    {
        return new()
        {
            Data = data,
            Code = code,
            Message = message
        };
    }
    
}