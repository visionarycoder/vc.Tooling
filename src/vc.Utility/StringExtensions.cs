namespace VisionaryCoder.Utility;

public static class StringExtensions
{
    public static bool IsNullOrWhiteSpace(this string? value)
    {
        return string.IsNullOrWhiteSpace(value: value);
    }

    public static string OrEmpty(this string? value)
    {
        return value ?? string.Empty;
    }
}
