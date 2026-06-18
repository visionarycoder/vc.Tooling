namespace VisionaryCoder.Utility;

public static class Guard
{
    public static T NotNull<T>(T? value, string paramName) where T : class
    {
        ArgumentNullException.ThrowIfNull(argument: value, paramName: paramName);
        return value;
    }

    public static string NotNullOrWhiteSpace(string? value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value: value))
        {
            throw new ArgumentException(message: "Value cannot be null, empty, or whitespace.", paramName: paramName);
        }

        return value;
    }
}
