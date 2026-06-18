namespace VisionaryCoder.Tooling.Core.Utilities;

file static class TypeNameHelper
{
    public static string Simplify(string fullName)
        => fullName.Split(separator: '.').Last();
}