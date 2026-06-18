using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Resilience.Rules;

internal sealed class ExcessiveTimeoutRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.ResilienceTimeoutConfigurationIssue,
        title: "Timeout value too high",
        messageFormat: "Timeout '{0}' is set to {1} seconds, which exceeds recommended limits",
        category: "Resilience",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSymbolAction(action: AnalyzeTimeoutDeclaration, symbolKinds: new[]{SymbolKind.Field, SymbolKind.Property});
    }

    private static void AnalyzeTimeoutDeclaration(SymbolAnalysisContext context)
    {
        var seconds = GetTimeoutSeconds(symbol: context.Symbol);
        if (seconds > 30)
        {
            context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: context.Symbol.Locations.FirstOrDefault(), messageArgs: [context.Symbol.Name, seconds]));
        }
    }

    private static int GetTimeoutSeconds(ISymbol symbol)
    {
        foreach (var reference in symbol.DeclaringSyntaxReferences)
        {
            foreach (var literal in reference.GetSyntax().SyntaxTree.GetRoot().DescendantNodes().OfType<LiteralExpressionSyntax>())
            {
                if (literal.Token.Value is int seconds)
                {
                    return seconds;
                }
            }
        }

        return 0;
    }
}