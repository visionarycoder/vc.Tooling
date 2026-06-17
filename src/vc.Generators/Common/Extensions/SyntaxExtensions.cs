using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vc.Generators.Common.Extensions;

file static class SyntaxExtensions
{
    public static bool IsPartial(this TypeDeclarationSyntax type)
    {
        return type.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));
    }

    public static string IdentifierText(this TypeDeclarationSyntax type)
    {
        return type.Identifier.Text;
    }
}

file static class SymbolExtensions
{
    public static bool HasAttribute(this ISymbol symbol, string name)
    {
        return symbol.GetAttributes().Any(a => a.AttributeClass?.Name == name);
    }

    public static AttributeData? GetAttribute(this ISymbol symbol, string name)
    {
        return symbol.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == name);
    }

    public static string FullName(this INamedTypeSymbol symbol)
    {
        return symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    }
}

public static class AttributeExtensions
{
    public static T? GetConstructorValue<T>(this AttributeData attr, int index)
    {
        if (attr.ConstructorArguments.Length <= index)
            return default;

        return (T?)attr.ConstructorArguments[index].Value;
    }
}

public static class RoslynExtensions
{
    public static string GetFullName(this INamedTypeSymbol symbol)
    {
        return symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    }
}

