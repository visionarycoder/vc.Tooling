namespace VisionaryCoder.Generators.Common.Metadata;

public static class SecurityMetadataReader
{
    public static bool IsSecurityAnnotated(ISymbol symbol)
    {
        return symbol.GetAttributes().Any(predicate: a => a.AttributeClass?.Name.Contains(value: "Security") == true);
    }
}