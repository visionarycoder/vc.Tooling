using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Core.Rules;

internal sealed class MissingAsyncSuffixRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.AsyncMissingNameSuffix,
        title: "Async method should end with 'Async'",
        messageFormat: "Async method '{0}' should be suffixed with 'Async'.",
        category: "AsyncCorrectness",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(action: AnalyzeMethodDeclaration, syntaxKinds: SyntaxKind.MethodDeclaration);
    }

    private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;
        if (method.Modifiers.Any(kind: SyntaxKind.AsyncKeyword) && !method.Identifier.Text.EndsWith(value: "Async", comparisonType: StringComparison.Ordinal))
        {
            context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: method.Identifier.GetLocation(), messageArgs: method.Identifier.Text));
        }
    }
}