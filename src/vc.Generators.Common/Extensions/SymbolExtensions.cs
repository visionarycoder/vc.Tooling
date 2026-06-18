namespace VisionaryCoder.Generators.Common.Extensions;

file static class SymbolExtensions
{
    public static bool HasAttribute(this ISymbol symbol, string name)
    {
        return symbol.GetAttributes().Any(predicate: a => a.AttributeClass?.Name == name);
    }

    public static AttributeData? GetAttribute(this ISymbol symbol, string name)
    {
        return symbol.GetAttributes().FirstOrDefault(predicate: a => a.AttributeClass?.Name == name);
    }

    public static string FullName(this INamedTypeSymbol symbol)
    {
        return symbol.ToDisplayString(format: SymbolDisplayFormat.FullyQualifiedFormat);
    }
}