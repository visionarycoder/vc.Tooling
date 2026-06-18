namespace VisionaryCoder.Generators.Common.Extensions;

public static class AttributeExtensions
{
    public static T? GetConstructorValue<T>(this AttributeData attr, int index)
    {
        if (attr.ConstructorArguments.Length <= index)
            return default;

        return (T?)attr.ConstructorArguments[index: index].Value;
    }
}