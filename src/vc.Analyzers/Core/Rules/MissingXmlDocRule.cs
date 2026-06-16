using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Core.Rules;

internal sealed class MissingXmlDocRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        DiagnosticIds.DocumentationMissingXmlDoc,
        "Public API missing XML documentation",
        "Public member '{0}' does not have XML documentation. Add a <summary> comment.",
        "Documentation",
        DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration, SyntaxKind.PropertyDeclaration, SyntaxKind.ClassDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var symbol = context.SemanticModel.GetDeclaredSymbol(context.Node);
        if (symbol is null || symbol.DeclaredAccessibility != Accessibility.Public || symbol.GetDocumentationCommentXml() is { Length: > 0 })
        {
            return;
        }

        var location = context.Node switch
        {
            MethodDeclarationSyntax method => method.Identifier.GetLocation(),
            PropertyDeclarationSyntax property => property.Identifier.GetLocation(),
            ClassDeclarationSyntax type => type.Identifier.GetLocation(),
            _ => context.Node.GetLocation()
        };

        context.ReportDiagnostic(Diagnostic.Create(descriptor, location, symbol.Name));
    }
}