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