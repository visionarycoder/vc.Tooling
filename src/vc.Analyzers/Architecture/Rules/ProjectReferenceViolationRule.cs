using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Architecture.Rules;

internal sealed class ProjectReferenceViolationRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        DiagnosticIds.ArchProjectReferenceViolation,
        "Project reference violation",
        "Project '{0}' must not reference project '{1}'.",
        "Architecture",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterCompilationStartAction(Start);
    }

    private static void Start(CompilationStartAnalysisContext context)
    {
        var currentAssembly = context.Compilation.Assembly;
        var currentLayer = GetLayer(currentAssembly.Name);

        context.RegisterSymbolAction(symbolContext =>
        {
            if (symbolContext.Symbol is not INamedTypeSymbol typeSymbol)
            {
                return;
            }

            foreach (var referencedAssembly in typeSymbol.ContainingAssembly.Modules.SelectMany(module => module.ReferencedAssemblySymbols))
            {
                var referencedLayer = GetLayer(referencedAssembly.Name);
                if (referencedLayer != Layer.Unknown && !IsAllowedReference(currentLayer, referencedLayer))
                {
                    symbolContext.ReportDiagnostic(Diagnostic.Create(descriptor, typeSymbol.Locations.FirstOrDefault(), currentAssembly.Name, referencedAssembly.Name));
                }
            }
        }, SymbolKind.NamedType);
    }

    private static Layer GetLayer(string assemblyName)
    {
        if (assemblyName.EndsWith(".Api", System.StringComparison.Ordinal))
        {
            return Layer.Api;
        }

        if (assemblyName.EndsWith(".Application", System.StringComparison.Ordinal))
        {
            return Layer.Application;
        }

        if (assemblyName.EndsWith(".Domain", System.StringComparison.Ordinal))
        {
            return Layer.Domain;
        }

        if (assemblyName.EndsWith(".Infrastructure", System.StringComparison.Ordinal))
        {
            return Layer.Infrastructure;
        }

        return Layer.Unknown;
    }

    private static bool IsAllowedReference(Layer from, Layer to)
    {
        if (from == to)
        {
            return true;
        }

        return (from, to) switch
        {
            (Layer.Api, Layer.Application) => true,
            (Layer.Api, Layer.Domain) => true,
            (Layer.Application, Layer.Domain) => true,
            (Layer.Infrastructure, Layer.Domain) => true,
            _ => false
        };
    }

    private enum Layer
    {
        Unknown,
        Api,
        Application,
        Domain,
        Infrastructure
    }
}