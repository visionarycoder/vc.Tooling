namespace VisionaryCoder.Generators.Common.Metadata;

public static class ProxyMetadataReader
{
    public static bool IsProxyAnnotated(ISymbol symbol)
    {
        return symbol.GetAttributes().Any(predicate: a => a.AttributeClass?.Name.Contains(value: "Proxy") == true);
    }
}