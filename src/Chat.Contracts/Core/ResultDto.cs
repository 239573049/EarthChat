namespace Chat.Contracts.Core;

public class ResultDto<T> where T : class
{
    public ResultDto()
    {
    }

    public ResultDto(T data)
    {
        Data = data;
        Code = "200";
    }

    /// <summary>
    ///     状态码
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    ///     数据
    /// </summary>
    public T Data { get; set; }

    /// <summary>
    ///     如果有错误，返回错误信息
    /// </summary>
    public string Message { get; set; }

}

public class ResultDto
{
    public ResultDto()
    {
    }

    public ResultDto(object data)
    {
        Data = data;
        Code = "200";
    }

    /// <summary>
    ///     状态码
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    ///     数据
    /// </summary>
    public object Data { get; set; }

    /// <summary>
    ///     如果有错误，返回错误信息
    /// </summary>
    public string Message { get; set; }

}