namespace EarthChat.Core.Models;

public class ResultDto
{
    public bool Success { get; set; }

    public string? Message { get; set; }

    public object? Data { get; set; }

    public static ResultDto SuccessResult(string message = "操作成功", object? data = null)
    {
        return new ResultDto
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ResultDto FailResult(Exception exception)
    {
        return new ResultDto
        {
            Success = false,
            Message = exception.Message
        };
    }

    public static ResultDto FailResult(string message)
    {
        return new ResultDto
        {
            Success = false,
            Message = message
        };
    }
}

public class ResultDto<TResult>
{
    public bool Success { get; set; }

    public string? Message { get; set; }

    public TResult? Data { get; set; }

    public static ResultDto<TResult> SuccessResult(TResult? data, string message = "操作成功")
    {
        return new ResultDto<TResult>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ResultDto<TResult> FailResult(Exception exception)
    {
        return new ResultDto<TResult>
        {
            Success = false,
            Message = exception.Message
        };
    }

    public static ResultDto<TResult> FailResult(string message)
    {
        return new ResultDto<TResult>
        {
            Success = false,
            Message = message
        };
    }
}