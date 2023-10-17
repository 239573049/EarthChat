namespace Chat.Service.Infrastructure.Middlewares;

/// <summary>
/// 异常处理中间件
/// </summary>
public class ExceptionMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            if (context.Request.Path == "/")
            {
                context.Request.Path = "/index.html";
            }

            await next(context);

            if (context.Response.StatusCode == 404)
            {
                // 获取WebHost的环境变量
                var webHost = context.RequestServices.GetRequiredService<IWebHostEnvironment>();
                // WebRootPath则是静态目录映射地址，默认提供index.html文件
                var index = Path.Combine(webHost.WebRootPath, "index.html");
                if (File.Exists(index))
                {
                    // 存在则读取
                    await using var file = File.OpenRead(index);
                    await file.CopyToAsync(context.Response.Body);
                }
            }
        }
        catch (UnauthorizedAccessException unauthorizedAccessException)
        {
            // 返回401前端会自动定位到登录界面
            _logger.LogError(unauthorizedAccessException.Message);
            context.Response.StatusCode = 401;
        }
        catch (UserFriendlyException e)
        {
            // 统一包装状态码为200，将状态码添加到json对象中
            _logger.LogError(e.Message);
            context.Response.StatusCode = 200;
            await context.Response.Body.WriteAsync(JsonSerializer.SerializeToUtf8Bytes(e.CreateExceptionResult("400"),
                new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));
        }
        catch (Exception e)
        {
            // 统一包装状态码为200，将状态码添加到json对象中
            _logger.LogError("{e}", e);
            context.Response.StatusCode = 200;
            await context.Response.Body.WriteAsync(
                JsonSerializer.SerializeToUtf8Bytes(e.CreateExceptionResult("500"), new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));
        }
    }
}