using System.Text;

namespace EarthChat.Core.System.Extensions;

public static class ByteExtensions
{
    /// <summary>
    /// 将字节数组转换为十六进制字符串
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string ToHexString(this byte[] bytes)
    {
        if (bytes == null)
        {
            throw new ArgumentNullException(nameof(bytes));
        }

        var hex = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes)
        {
            hex.AppendFormat("{0:x2}", b);
        }

        return hex.ToString();
    }

    /// <summary>
    /// 将字节数组转换为 Base64 字符串
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static string ToBase64String(this byte[] bytes)
    {
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// 将字节数组的一部分复制到另一个字节数组
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <param name="length"></param>
    public static void CopyTo(this byte[] source, byte[] destination, int length)
    {
        Array.Copy(source, destination, length);
    }

    /// <summary>
    /// 将字节数组的一部分复制到另一个字节数组的指定位置
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <param name="sourceOffset"></param>
    /// <param name="destinationOffset"></param>
    /// <param name="length"></param>
    public static void CopyTo(this byte[] source, byte[] destination, int sourceOffset, int destinationOffset,
        int length)
    {
        Array.Copy(source, sourceOffset, destination, destinationOffset, length);
    }

    /// <summary>
    /// 反转字节数组
    /// </summary>
    /// <param name="array"></param>
    public static void Reverse(this byte[] array)
    {
        Array.Reverse(array);
    }
}