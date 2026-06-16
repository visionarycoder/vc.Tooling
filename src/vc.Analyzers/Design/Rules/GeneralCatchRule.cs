using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Design.Rules;

internal sealed class GeneralCatchRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        DiagnosticIds.DesignExceptionSafetyBroadCatch,
        "Overly general catch",
        "Catching System.Exception or using a catch without an exception type is discouraged.",
        "ExceptionSafety",
        DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeCatchClause, SyntaxKind.CatchClause);
    }

    private static void AnalyzeCatchClause(SyntaxNodeAnalysisContext context)
    {
        var catchClause = (CatchClauseSyntax)context.Node;
        if (catchClause.Declaration is null)
        {
            context.ReportDiagnostic(Diagnostic.Create(descriptor, catchClause.CatchKeyword.GetLocation()));
            return;
        }

        var type = context.SemanticModel.GetTypeInfo(catchClause.Declaration.Type, context.CancellationToken).Type;
        if (type?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::System.Exception")
        {
            context.ReportDiagnostic(Diagnostic.Create(descriptor, catchClause.CatchKeyword.GetLocation()));
        }
    }
}