# 图片压缩

在聊天的时候会存在很多图片消息，但是每一张图片都会非常大，如果聊天用户过多的话会导致大量宽带浪费在图片上，并且很多用户并不会去看图片内容，所以下面会对于用户聊天的时候的图片进行压缩优化，也可以利用浏览器缓存优化。

## 上传文件时压缩图片

在`src/Chat>service/Application/Files/CommandHandler.cs`中，`LocalAsync`方法，这是用户上传文件的处理方法，在代代码里面会先判断当前的文件是否为图片，如果是图片则进行压缩并且在后缀名和文件中间增加`.compress`名称，

```csharp

        // 在这里会判断当前是否为图片
        if (fileName.IsImage())
        {
            // 压缩图片
            var ext = Path.GetExtension(filePath);
            // 在压缩文件的时候会按照规则生成 .compress后缀名
            ImageHelper.FitImage(filePath, filePath.Replace(ext, "") + ".compress" + ext, 256, 256);
        }

```

下面我们看看`ImageHelper`方法
首先安装一下NuGet包,`SkiaSharp.NativeAssets.Linux.NoDependencies`是在Linux环境下使用的依赖包，`NoDependencies`则标识无需依赖。

```xml
<PackageReference Include="SkiaSharp" Version="2.88.6" />
<PackageReference Include="SkiaSharp.NativeAssets.Linux.NoDependencies" Version="2.88.6" />
```

```csharp
/// <summary>
/// 操作图片工具类
/// </summary>
public class ImageHelper
{
    /// <summary>
    /// 对于图片进行缩放，使其宽度和高度都不超过指定的最大值。
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="outputPath"></param>
    /// <param name="maxWidth"></param>
    /// <param name="maxHeight"></param>
    public static void FitImage(string fileName,string outputPath, int maxWidth, int maxHeight)
    {
        var stream = File.OpenRead(fileName);
        using var originalBitmap = SKBitmap.Decode(stream);
        var widthRatio = (double)maxWidth / originalBitmap.Width;
        var heightRatio = (double)maxHeight / originalBitmap.Height;
        var minRatio = Math.Min(widthRatio, heightRatio);

        if (!(minRatio < 1.0)) return; // 仅当图像大于指定的最大尺寸时才调整大小
        int newWidth = (int)(originalBitmap.Width * minRatio);
        int newHeight = (int)(originalBitmap.Height * minRatio);

        using var resizedImage = originalBitmap.Resize(new SKImageInfo(newWidth, newHeight), SKFilterQuality.High);
        if (resizedImage == null) return;

        using var image = SKImage.FromBitmap(resizedImage);
        using var outputStream = File.Create(outputPath);
        var data = image.Encode(SKEncodedImageFormat.Jpeg, 70); // JPEG的质量设置为90
        data.SaveTo(outputStream);
    }
}
```

总结上面代码，在上传文件的时候我们会还会创建一份压缩的图片文件，相当于双份文件，并且会在后缀名和名称中间增加`.compress`,用于辨认压缩图片文件，

## 如何使用压缩图片

虽然上面步骤完成了压缩图片的方式，但是我们还需要支持更简单的使用压缩图片，下面将使用默认的静态文件中间件+自定义中间件完成。

添加中间件,需要注册自定义的中间件，然后使用默认提供的中间件并且天啊及啊响应头，增加浏览器缓存，减少宽带占用。

```csharp

builder.Services.AddScoped<ReductionMiddleware>();

....

app.UseMiddleware<ReductionMiddleware>();

var cacheMaxAgeOneWeek = (60 * 60 * 24 * 7).ToString();

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // 给所有文件加上浏览器缓存 7天
        ctx.Context.Response.Headers.Append(
            "Cache-Control", $"public, max-age={cacheMaxAgeOneWeek}");
    }
});

```

`ReductionMiddleware.cs`

下面中间件会从url的query参数中获取`reduction`参数如果`reduction`参数为true，则使用压缩图片，然而我们使用了微软的静态 文件夹中间件我们只需要处理`context.Request.Path`，将文件名拆分然后给后缀名和文件中间加上`.compress`,这样静态文件中间件就会搜索`文件名.compress.后缀名`的文件，还需要注意的就是有些图片并没有压缩，我们就需要处理一下然后的404，如果是404，则去掉`.compress`使用原图。

```csharp
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
```
