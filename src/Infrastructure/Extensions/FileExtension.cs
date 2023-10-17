namespace Infrastructure;

public static class FileExtension
{
    public static bool IsImage(this string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLower();
        switch (ext)
        {
            case ".jpg":
            case ".jpeg":
            case ".png":
            case ".bmp":
            case ".tiff":
            case ".webp":  // 这是另一种常见的图片格式
                return true;
            default:
                return false;
        }
    }
}