using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Distributed.Rules;

internal sealed class DomainReturnTypeRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        DiagnosticIds.DistributedRepositoryContractViolation,
        "Repository should not return domain entities directly",
        "Repository method '{0}' returns domain type '{1}'. Repositories should return aggregates or DTOs.",
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

            if (method.ReturnType is not INamedTypeSymbol returnType)
            {
                continue;
            }

            if (returnType.Name.EndsWith("Entity", System.StringComparison.Ordinal) ||
                returnType.Name.EndsWith("Model", System.StringComparison.Ordinal) ||
                returnType.Name.EndsWith("Domain", System.StringComparison.Ordinal))
            {
                context.ReportDiagnostic(Diagnostic.Create(descriptor, method.Locations.FirstOrDefault(), method.Name, returnType.Name));
            }
        }
    }

    private static bool LooksLikeRepository(INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.Name.EndsWith("Repository", System.StringComparison.Ordinal) ||
               typeSymbol.Interfaces.Any(interfaceSymbol => interfaceSymbol.Name == "IRepository");
    }
}