namespace VisionaryCoder.Generators.Common.Metadata;

public static class PerformanceMetadataReader
{
    public static bool IsPerformanceAnnotated(ISymbol symbol)
    {
        return symbol.GetAttributes().Any(predicate: a => a.AttributeClass?.Name.Contains(value: "Perf") == true);
    }
}