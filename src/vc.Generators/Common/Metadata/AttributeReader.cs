using Microsoft.CodeAnalysis;

namespace Vc.Generators.Common.Metadata;

public static class AttributeReader
{
    public static IEnumerable<AttributeData> GetAttributes(ISymbol symbol)
    {
        return symbol.GetAttributes();
    }
}

public static class DesignMetadataReader
{
    public static bool IsDesignAnnotated(ISymbol symbol)
    {
        return symbol.GetAttributes().Any(a => a.AttributeClass?.Name.Contains("Design") == true);
    }
}

public static class SecurityMetadataReader
{
    public static bool IsSecurityAnnotated(ISymbol symbol)
    {
        return symbol.GetAttributes().Any(a => a.AttributeClass?.Name.Contains("Security") == true);
    }
}

public static class ObservabilityMetadataReader
{
    public static bool IsObservabilityAnnotated(ISymbol symbol)
    {
        return symbol.GetAttributes().Any(a => a.AttributeClass?.Name.Contains("Observe") == true);
    }
}

public static class ResilienceMetadataReader
{
    public static bool IsResilienceAnnotated(ISymbol symbol)
    {
        return symbol.GetAttributes().Any(a => a.AttributeClass?.Name.Contains("Retry") == true);
    }
}

public static class PerformanceMetadataReader
{
    public static bool IsPerformanceAnnotated(ISymbol symbol)
    {
        return symbol.GetAttributes().Any(a => a.AttributeClass?.Name.Contains("Perf") == true);
    }
}

public static class DistributedMetadataReader
{
    public static bool IsDistributedAnnotated(ISymbol symbol)
    {
        return symbol.GetAttributes().Any(a => a.AttributeClass?.Name.Contains("Distributed") == true);
    }
}

public static class ProxyMetadataReader
{
    public static bool IsProxyAnnotated(ISymbol symbol)
    {
        return symbol.GetAttributes().Any(a => a.AttributeClass?.Name.Contains("Proxy") == true);
    }
}