using SkiaSharp;

namespace Chat.Service.Infrastructure.Helper;

/// <summary>
/// 操作图片工具类
/// </summary>
public class ImageHelper
{
    
    /// <summary>
    /// 压缩图片
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    /// <param name="quality"></param>
    public static async Task CompressImage(string inputPath, string outputPath, int quality = 30)
    {
        using var inputStream = new SKFileStream(inputPath);
        using var original = SKBitmap.Decode(inputStream);
        var imageInfo = new SKImageInfo(original.Width, original.Height);

        using var surface = SKSurface.Create(imageInfo);
        surface.Canvas.DrawBitmap(original, 0, 0);
        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, quality);
        await using var fileStream = File.Create(outputPath);
        await data.AsStream().CopyToAsync(fileStream);
    }
}