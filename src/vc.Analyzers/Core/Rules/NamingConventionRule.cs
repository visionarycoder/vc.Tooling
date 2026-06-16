using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Core.Rules;

internal sealed class NamingConventionRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        DiagnosticIds.NamingConventionViolation,
        "Naming convention violation",
        "{0} '{1}' does not follow {2} naming convention",
        "Style",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeField, SyntaxKind.FieldDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeProperty, SyntaxKind.PropertyDeclaration);
    }

    private static void AnalyzeField(SyntaxNodeAnalysisContext context)
    {
        var field = (FieldDeclarationSyntax)context.Node;
        var isPrivate = field.Modifiers.Any(SyntaxKind.PrivateKeyword) || !field.Modifiers.Any();
        foreach (var variable in field.Declaration.Variables)
        {
            if (isPrivate && !variable.Identifier.Text.StartsWith("_", System.StringComparison.Ordinal))
            {
                context.ReportDiagnostic(Diagnostic.Create(descriptor, variable.GetLocation(), "Private field", variable.Identifier.Text, "_camelCase"));
            }
        }
    }

    private static void AnalyzeProperty(SyntaxNodeAnalysisContext context)
    {
        var property = (PropertyDeclarationSyntax)context.Node;
        var name = property.Identifier.Text;
        if (char.IsLower(name[0]))
        {
            context.ReportDiagnostic(Diagnostic.Create(descriptor, property.Identifier.GetLocation(), "Property", name, "PascalCase"));
        }
    }
}