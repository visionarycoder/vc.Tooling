namespace VisionaryCoder.Ifx;

public sealed class ConfigAdapter : IConfigAdapter
{
    private readonly IReadOnlyDictionary<string, string> _values;

    public ConfigAdapter(IReadOnlyDictionary<string, string> values)
    {
        _values = values ?? throw new ArgumentNullException(nameof(values));
    }

    public string? GetValue(string key)
    {
        ArgumentNullException.ThrowIfNull(key);
        return _values.TryGetValue(key, out var value) ? value : null;
    }

    public bool TryGetValue(string key, out string? value)
    {
        ArgumentNullException.ThrowIfNull(key);
        return _values.TryGetValue(key, out value);
    }
}
