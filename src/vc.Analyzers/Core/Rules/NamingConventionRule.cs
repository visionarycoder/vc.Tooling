using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Core.Rules;

internal sealed class NamingConventionRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.NamingConventionViolation,
        title: "Naming convention violation",
        messageFormat: "{0} '{1}' does not follow {2} naming convention",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(action: AnalyzeField, syntaxKinds: SyntaxKind.FieldDeclaration);
        context.RegisterSyntaxNodeAction(action: AnalyzeProperty, syntaxKinds: SyntaxKind.PropertyDeclaration);
    }

    private static void AnalyzeField(SyntaxNodeAnalysisContext context)
    {
        var field = (FieldDeclarationSyntax)context.Node;
        var isPrivate = field.Modifiers.Any(kind: SyntaxKind.PrivateKeyword) || !field.Modifiers.Any();
        foreach (var variable in field.Declaration.Variables)
        {
            if (isPrivate && !variable.Identifier.Text.StartsWith(value: "_", comparisonType: System.StringComparison.Ordinal))
            {
                context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: variable.GetLocation(), messageArgs: ["Private field", variable.Identifier.Text, "_camelCase"]));
            }
        }
    }

    private static void AnalyzeProperty(SyntaxNodeAnalysisContext context)
    {
        var property = (PropertyDeclarationSyntax)context.Node;
        var name = property.Identifier.Text;
        if (char.IsLower(c: name[index: 0]))
        {
            context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: property.Identifier.GetLocation(), messageArgs: ["Property", name, "PascalCase"]));
        }
    }
}