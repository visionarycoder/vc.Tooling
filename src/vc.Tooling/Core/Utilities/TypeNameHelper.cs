namespace Vc.Tooling.Core.Utilities;

file static class TypeNameHelper
{
    public static string Simplify(string fullName)
        => fullName.Split('.').Last();
}

file sealed class FastStringPool
{
    private readonly Dictionary<string, string> _pool = new(StringComparer.Ordinal);

    public string Intern(string value)
        => _pool.TryGetValue(value, out var existing)
            ? existing
            : (_pool[value] = value);
}
