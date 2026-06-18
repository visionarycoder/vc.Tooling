namespace VisionaryCoder.Generators.Common.Extensions;

public static class RoslynExtensions
{
    public static string GetFullName(this INamedTypeSymbol symbol)
    {
        return symbol.ToDisplayString(format: SymbolDisplayFormat.FullyQualifiedFormat);
    }
}