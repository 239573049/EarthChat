namespace EarthChat.Core.System.Extensions;

public static class StringExtensions
{
    public static bool IsNullOrWhiteSpace(this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    public static bool IsNullOrEmpty(this string? value)
    {
        return string.IsNullOrEmpty(value);
    }

    public static bool IsNotNullOrEmpty(this string? value)
    {
        return !string.IsNullOrEmpty(value);
    }

    public static bool IsNotNullOrWhiteSpace(this string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    public static bool IsNullOrEmptyOrWhiteSpace(this string? value)
    {
        return string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);
    }

    public static bool IsNotNullOrEmptyOrWhiteSpace(this string? value)
    {
        return !string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value);
    }

    public static string? ToNullOrWhiteSpace(this string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    public static string? ToNullOrEmpty(this string? value)
    {
        return string.IsNullOrEmpty(value) ? null : value;
    }

    public static string? TrimToNull(this string? value)
    {
        return value.IsNullOrWhiteSpace() ? null : value?.Trim();
    }

    public static string TrimToEmpty(this string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
    }

    public static string? TrimToEmptyOrNull(this string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    public static string TrimEnd(this string? value, string trimString)
    {
        return value?.TrimEnd(trimString.ToCharArray()) ?? string.Empty;
    }

    public static string TrimStart(this string? value, string trimString)
    {
        return value?.TrimStart(trimString.ToCharArray()) ?? string.Empty;
    }

    public static string Trim(this string? value, string trimString)
    {
        return value?.Trim(trimString.ToCharArray()) ?? string.Empty;
    }

    public static string TrimEnd(this string? value, char trimChar)
    {
        return value?.TrimEnd(trimChar) ?? string.Empty;
    }

    public static string TrimStart(this string? value, char trimChar)
    {
        return value?.TrimStart(trimChar) ?? string.Empty;
    }

    public static string Trim(this string? value, char trimChar)
    {
        return value?.Trim(trimChar) ?? string.Empty;
    }

    public static string TrimEnd(this string? value, char[] trimChars)
    {
        return value?.TrimEnd(trimChars) ?? string.Empty;
    }

    public static string TrimStart(this string? value, char[] trimChars)
    {
        return value?.TrimStart(trimChars) ?? string.Empty;
    }

    public static string Trim(this string? value, char[] trimChars)
    {
        return value?.Trim(trimChars) ?? string.Empty;
    }

    /// <summary>
    /// Convert a string to a byte array
    /// </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static byte[] FromHexString(this string hex)
    {
        if (hex == null)
        {
            throw new ArgumentNullException(nameof(hex));
        }

        if (hex.Length % 2 != 0)
        {
            throw new ArgumentException("Hex string must have an even number of characters");
        }

        var bytes = new byte[hex.Length / 2];
        for (int i = 0; i < hex.Length; i += 2)
        {
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        }

        return bytes;
    }


    /// <summary>
    /// 将 Base64 字符串转换为字节数组
    /// </summary>
    /// <param name="base64"></param>
    /// <returns></returns>
    public static byte[] FromBase64String(this string base64)
    {
        return Convert.FromBase64String(base64);
    }
}