using Infrastructure;

namespace Chat.Service.Infrastructure.Middlewares;

/// <summary>
/// 压缩中间件
/// </summary>
public class ReductionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Query.TryGetValue("reduction", out var reduction) && reduction == "true")
        {
            // 检查请求路径，看是否是图片请求，如果是则处理
            var imagePath = context.Request.Path.Value;
            if (imagePath?.IsImage() == true) //您可以根据需要添加其他图片格式
            {
                var ext = Path.GetExtension(context.Request.Path);

                context.Request.Path = new PathString(imagePath.Replace(ext, ".compress" + ext)); // 示例逻辑
            }

            await next(context);

            // 如果没找到压缩图片就用原文件
            if (context.Response.StatusCode == 404)
            {
                var ext = Path.GetExtension(context.Request.Path);
                var newExt = ext.Replace(".compress", "");

                context.Request.Path = new PathString(imagePath?.TrimEnd(ext) + newExt); // 示例逻辑
                
                // 由于状态码是404所以在此请求不会出现异常！
                await next(context);
            }
        }
        else
        {
            await next(context);
        }
    }
}