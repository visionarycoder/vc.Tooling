using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Design.Rules;

internal sealed class GeneralCatchRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.DesignExceptionSafetyBroadCatch,
        title: "Overly general catch",
        messageFormat: "Catching System.Exception or using a catch without an exception type is discouraged.",
        category: "ExceptionSafety",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(action: AnalyzeCatchClause, syntaxKinds: SyntaxKind.CatchClause);
    }

    private static void AnalyzeCatchClause(SyntaxNodeAnalysisContext context)
    {
        var catchClause = (CatchClauseSyntax)context.Node;
        if (catchClause.Declaration is null)
        {
            context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: catchClause.CatchKeyword.GetLocation()));
            return;
        }

        var type = context.SemanticModel.GetTypeInfo(expression: catchClause.Declaration.Type, cancellationToken: context.CancellationToken).Type;
        if (type?.ToDisplayString(format: SymbolDisplayFormat.FullyQualifiedFormat) == "global::System.Exception")
        {
            context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: catchClause.CatchKeyword.GetLocation()));
        }
    }
}