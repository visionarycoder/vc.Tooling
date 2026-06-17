namespace VisionaryCoder.Ifx;

public interface IConfigAdapter
{
    string? GetValue(string key);
    bool TryGetValue(string key, out string? value);
}
