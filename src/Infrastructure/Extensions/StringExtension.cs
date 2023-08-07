namespace Infrastructure;

public static class StringExtension
{
    public static bool IsNullOrEmpty(this string? str)
    {
        return string.IsNullOrEmpty(str);
    }
    
    public static bool IsNullOrWhiteSpace(this string? str)
    {
        return string.IsNullOrWhiteSpace(str);
    }
    
    public static string Trim(this string? str)
    {
        return str?.Trim() ?? string.Empty;
    }
    
    public static string Trim(this string? str, char trimChar)
    {
        return str?.Trim(trimChar) ?? string.Empty;
    }
    
    public static string Trim(this string? str, params char[] trimChars)
    {
        return str?.Trim(trimChars) ?? string.Empty;
    }
    
    public static string TrimStart(this string? str)
    {
        return str?.TrimStart() ?? string.Empty;
    }
    
    public static string TrimStart(this string? str, char trimChar)
    {
        return str?.TrimStart(trimChar) ?? string.Empty;
    }
    
    public static string TrimStart(this string? str, params char[] trimChars)
    {
        return str?.TrimStart(trimChars) ?? string.Empty;
    }
    
    public static string TrimEnd(this string? str)
    {
        return str?.TrimEnd() ?? string.Empty;
    }
    
    public static string TrimEnd(this string? str, char trimChar)
    {
        return str?.TrimEnd(trimChar) ?? string.Empty;
    }
    
    public static string TrimEnd(this string? str, params char[] trimChars)
    {
        return str?.TrimEnd(trimChars) ?? string.Empty;
    }
    
    public static string ToUpper(this string? str)
    {
        return str?.ToUpper() ?? string.Empty;
    }
    
    public static string ToLower(this string? str)
    {
        return str?.ToLower() ?? string.Empty;
    }
    
    public static string ToUpperInvariant(this string? str)
    {
        return str?.ToUpperInvariant() ?? string.Empty;
    }
    
    public static string ToLowerInvariant(this string? str)
    {
        return str?.ToLowerInvariant() ?? string.Empty;
    }
    
    public static string Replace(this string? str, char oldChar, char newChar)
    {
        return str?.Replace(oldChar, newChar) ?? string.Empty;
    }
    
    public static string Replace(this string? str, string oldValue, string newValue)
    {
        return str?.Replace(oldValue, newValue) ?? string.Empty;
    }
    
    public static string[] Split(this string? str, char separator)
    {
        return str?.Split(separator) ?? Array.Empty<string>();
    }
    
    public static string[] Split(this string? str, char separator, StringSplitOptions options)
    {
        return str?.Split(separator, options) ?? Array.Empty<string>();
    }
    
    public static string[] Split(this string? str, char separator, int count)
    {
        return str?.Split(separator, count) ?? Array.Empty<string>();
    }
}