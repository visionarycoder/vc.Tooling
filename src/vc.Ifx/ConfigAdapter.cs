namespace VisionaryCoder.Ifx;

public sealed class ConfigAdapter(IReadOnlyDictionary<string, string> values) : IConfigAdapter
{
    private readonly IReadOnlyDictionary<string, string> _values = values ?? throw new ArgumentNullException(paramName: nameof(values));

    public string? GetValue(string key)
    {
        ArgumentNullException.ThrowIfNull(argument: key);
        return _values.TryGetValue(key: key, value: out var value) ? value : null;
    }

    public bool TryGetValue(string key, out string? value)
    {
        ArgumentNullException.ThrowIfNull(argument: key);
        return _values.TryGetValue(key: key, value: out value);
    }
}
