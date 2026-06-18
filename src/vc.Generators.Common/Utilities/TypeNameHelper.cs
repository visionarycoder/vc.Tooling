namespace VisionaryCoder.Generators.Common.Utilities;

public static class TypeNameHelper
{
    public static string Sanitize(string name)
    {
        return name.Replace(oldValue: ".", newValue: "_").Replace(oldValue: "+", newValue: "_");
    }
}