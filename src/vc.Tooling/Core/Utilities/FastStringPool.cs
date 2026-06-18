namespace VisionaryCoder.Tooling.Core.Utilities;

file sealed class FastStringPool
{
    private readonly Dictionary<string, string> _pool = new(comparer: StringComparer.Ordinal);

    public string Intern(string value)
        => _pool.TryGetValue(key: value, value: out var existing)
            ? existing
            : (_pool[key: value] = value);
}