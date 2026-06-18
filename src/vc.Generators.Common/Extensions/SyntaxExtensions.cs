namespace VisionaryCoder.Generators.Common.Extensions;

file static class SyntaxExtensions
{
    public static bool IsPartial(this TypeDeclarationSyntax type)
    {
        return type.Modifiers.Any(predicate: m => m.IsKind(kind: SyntaxKind.PartialKeyword));
    }

    public static string IdentifierText(this TypeDeclarationSyntax type)
    {
        return type.Identifier.Text;
    }
}