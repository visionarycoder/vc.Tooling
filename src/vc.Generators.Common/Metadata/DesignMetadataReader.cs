namespace VisionaryCoder.Generators.Common.Metadata;

public static class DesignMetadataReader
{
    public static bool IsDesignAnnotated(ISymbol symbol)
    {
        return symbol.GetAttributes().Any(predicate: a => a.AttributeClass?.Name.Contains(value: "Design") == true);
    }
}