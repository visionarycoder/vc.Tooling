using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Vc.Analyzers.Distributed;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RepositoryAnalyzer : DiagnosticAnalyzer
{
    public const string MissingAsyncId = "VCDIST004";
    public const string IQueryableLeakId = "VCDIST005";
    public const string DomainReturnTypeId = "VCDIST006";

    private static readonly DiagnosticDescriptor MissingAsyncRule = new(
        MissingAsyncId,
        "Repository methods should be async",
        "Repository method '{0}' should be asynchronous.",
        "Distributed",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor IQueryableLeakRule = new(
        IQueryableLeakId,
        "Repository must not expose IQueryable",
        "Repository method '{0}' exposes IQueryable, which leaks internal query details.",
        "Distributed",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor DomainReturnTypeRule = new(
        DomainReturnTypeId,
        "Repository should not return domain entities directly",
        "Repository method '{0}' returns domain type '{1}'. Repositories should return aggregates or DTOs.",
        "Distributed",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(MissingAsyncRule, IQueryableLeakRule, DomainReturnTypeRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(AnalyzeRepository, SymbolKind.NamedType);
    }

    private static void AnalyzeRepository(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol typeSymbol)
        {
            return;
        }

        if (!IsRepository(typeSymbol))
        {
            return;
        }

        foreach (var method in typeSymbol.GetMembers().OfType<IMethodSymbol>())
        {
            if (method.MethodKind != MethodKind.Ordinary)
            {
                continue;
            }

            AnalyzeAsync(method, context);
            AnalyzeIQueryable(method, context);
            AnalyzeDomainReturnType(method, context);
        }
    }

    private static bool IsRepository(INamedTypeSymbol typeSymbol)
    {
        if (typeSymbol.Name.EndsWith("Repository"))
        {
            return true;
        }

        if (typeSymbol.Interfaces.Any(i => i.Name == "IRepository"))
        {
            return true;
        }

        return false;
    }

    private static void AnalyzeAsync(IMethodSymbol method, SymbolAnalysisContext context)
    {
        if (method.ReturnType is INamedTypeSymbol returnType)
        {
            if (returnType.Name == "Task" || returnType.Name == "ValueTask")
            {
                return;
            }
        }

        context.ReportDiagnostic(
            Diagnostic.Create(
                MissingAsyncRule,
                method.Locations.FirstOrDefault(),
                method.Name));
    }

    private static void AnalyzeIQueryable(IMethodSymbol method, SymbolAnalysisContext context)
    {
        if (method.ReturnType is not INamedTypeSymbol returnType)
        {
            return;
        }

        if (returnType.Name == "IQueryable")
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    IQueryableLeakRule,
                    method.Locations.FirstOrDefault(),
                    method.Name));
        }
    }

    private static void AnalyzeDomainReturnType(IMethodSymbol method, SymbolAnalysisContext context)
    {
        if (method.ReturnType is not INamedTypeSymbol returnType)
        {
            return;
        }

        if (returnType.Name.EndsWith("Entity") || returnType.Name.EndsWith("Model") || returnType.Name.EndsWith("Domain"))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    DomainReturnTypeRule,
                    method.Locations.FirstOrDefault(),
                    method.Name,
                    returnType.Name));
        }
    }
}
