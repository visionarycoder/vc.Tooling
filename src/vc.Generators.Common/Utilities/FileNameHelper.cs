namespace VisionaryCoder.Generators.Common.Utilities;

public static class FileNameHelper
{
    public static string Normalize(string name)
    {
        return name.Replace(oldValue: "<", newValue: "").Replace(oldValue: ">", newValue: "").Replace(oldValue: ":", newValue: "_");
    }
}