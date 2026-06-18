namespace VisionaryCoder.Generators.Common.Metadata;

public static class ObservabilityMetadataReader
{
    public static bool IsObservabilityAnnotated(ISymbol symbol)
    {
        return symbol.GetAttributes().Any(predicate: a => a.AttributeClass?.Name.Contains(value: "Observe") == true);
    }
}