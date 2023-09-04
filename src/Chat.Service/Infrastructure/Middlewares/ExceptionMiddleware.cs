namespace Chat.Service.Infrastructure.Middlewares;

public class ExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (UnauthorizedAccessException)
        {
            context.Response.StatusCode = 401;
        }
        catch (UserFriendlyException e)
        {
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
            context.Response.StatusCode = 200;
            await context.Response.Body.WriteAsync(
                JsonSerializer.SerializeToUtf8Bytes(e.CreateExceptionResult("400"), new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));
        }
    }
}