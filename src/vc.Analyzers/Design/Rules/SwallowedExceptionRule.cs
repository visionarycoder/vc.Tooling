using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Design.Rules;

internal sealed class SwallowedExceptionRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        DiagnosticIds.DesignExceptionSafetySwallowed,
        "Exception is swallowed",
        "Exception is caught but not logged, wrapped, or rethrown.",
        "ExceptionSafety",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeCatchClause, SyntaxKind.CatchClause);
    }

    private static void AnalyzeCatchClause(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not CatchClauseSyntax catchClause || catchClause.Block is null)
        {
            return;
        }

        if (!ContainsThrow(catchClause.Block) && !ContainsLogging(context.SemanticModel, catchClause.Block, context.CancellationToken))
        {
            context.ReportDiagnostic(Diagnostic.Create(descriptor, catchClause.CatchKeyword.GetLocation()));
        }
    }

    private static bool ContainsThrow(BlockSyntax block)
    {
        return block.DescendantNodes().OfType<ThrowStatementSyntax>().Any();
    }

    private static bool ContainsLogging(SemanticModel semanticModel, BlockSyntax block, System.Threading.CancellationToken cancellationToken)
    {
        foreach (var invocation in block.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            var symbol = semanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol as IMethodSymbol;
            if (symbol is not null)
            {
                var name = symbol.Name;
                if (name.Contains("Log", StringComparison.OrdinalIgnoreCase) || name.Contains("Trace", StringComparison.OrdinalIgnoreCase) || name.Contains("Error", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }

        return false;
    }
}