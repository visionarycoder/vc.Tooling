namespace VisionaryCoder.Generators.Common.Metadata;

public static class ResilienceMetadataReader
{
    public static bool IsResilienceAnnotated(ISymbol symbol)
    {
        return symbol.GetAttributes().Any(predicate: a => a.AttributeClass?.Name.Contains(value: "Retry") == true);
    }
}