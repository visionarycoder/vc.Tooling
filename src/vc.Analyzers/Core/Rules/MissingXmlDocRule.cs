using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Core.Rules;

internal sealed class MissingXmlDocRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.DocumentationMissingXmlDoc,
        title: "Public API missing XML documentation",
        messageFormat: "Public member '{0}' does not have XML documentation. Add a <summary> comment.",
        category: "Documentation",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(action: Analyze, syntaxKinds: new[]{SyntaxKind.MethodDeclaration, SyntaxKind.PropertyDeclaration, SyntaxKind.ClassDeclaration});
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var symbol = context.SemanticModel.GetDeclaredSymbol(declaration: context.Node);
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

        context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: location, messageArgs: symbol.Name));
    }
}