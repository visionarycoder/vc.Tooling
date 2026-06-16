using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Distributed.Rules;

internal sealed class MissingAsyncRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        DiagnosticIds.DistributedRepositoryAsyncMissing,
        "Repository methods should be async",
        "Repository method '{0}' should be asynchronous.",
        "Distributed",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSymbolAction(AnalyzeRepository, SymbolKind.NamedType);
    }

    private static void AnalyzeRepository(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol typeSymbol || !LooksLikeRepository(typeSymbol))
        {
            return;
        }

        foreach (var method in typeSymbol.GetMembers().OfType<IMethodSymbol>())
        {
            if (method.MethodKind != MethodKind.Ordinary)
            {
                continue;
            }

            if (method.ReturnType is INamedTypeSymbol returnType && (returnType.Name == "Task" || returnType.Name == "ValueTask"))
            {
                continue;
            }

            context.ReportDiagnostic(Diagnostic.Create(descriptor, method.Locations.FirstOrDefault(), method.Name));
        }
    }

    private static bool LooksLikeRepository(INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.Name.EndsWith("Repository", System.StringComparison.Ordinal) ||
               typeSymbol.Interfaces.Any(interfaceSymbol => interfaceSymbol.Name == "IRepository");
    }
}