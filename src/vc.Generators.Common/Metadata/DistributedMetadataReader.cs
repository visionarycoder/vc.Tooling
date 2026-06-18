namespace VisionaryCoder.Generators.Common.Metadata;

public static class DistributedMetadataReader
{
    public static bool IsDistributedAnnotated(ISymbol symbol)
    {
        return symbol.GetAttributes().Any(predicate: a => a.AttributeClass?.Name.Contains(value: "Distributed") == true);
    }
}