using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Vc.Analyzers.Architecture;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ProjectReferenceAnalyzer : DiagnosticAnalyzer
{
    public const string ProjectReferenceViolationId = "VCARCH004";

    private static readonly DiagnosticDescriptor ProjectReferenceViolationRule = new(
        ProjectReferenceViolationId,
        "Project reference violation",
        "Project '{0}' must not reference project '{1}'.",
        "Architecture",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(ProjectReferenceViolationRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(Start);
    }

    private static void Start(CompilationStartAnalysisContext context)
    {
        var compilation = context.Compilation;
        var currentAssembly = compilation.Assembly;

        var currentLayer = GetLayer(currentAssembly.Name);

        context.RegisterSymbolAction(symbolContext =>
        {
            if (symbolContext.Symbol is not INamedTypeSymbol typeSymbol)
            {
                return;
            }

            foreach (var referencedAssembly in typeSymbol.ContainingAssembly.Modules
                .SelectMany(m => m.ReferencedAssemblySymbols))
            {
                var referencedLayer = GetLayer(referencedAssembly.Name);

                if (referencedLayer == Layer.Unknown)
                {
                    continue;
                }

                if (!IsAllowedReference(currentLayer, referencedLayer))
                {
                    symbolContext.ReportDiagnostic(
                        Diagnostic.Create(
                            ProjectReferenceViolationRule,
                            typeSymbol.Locations.FirstOrDefault(),
                            currentAssembly.Name,
                            referencedAssembly.Name));
                }
            }
        }, SymbolKind.NamedType);
    }

    private static Layer GetLayer(string assemblyName)
    {
        if (assemblyName.EndsWith(".Api"))
        {
            return Layer.Api;
        }

        if (assemblyName.EndsWith(".Application"))
        {
            return Layer.Application;
        }

        if (assemblyName.EndsWith(".Domain"))
        {
            return Layer.Domain;
        }

        if (assemblyName.EndsWith(".Infrastructure"))
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
        Unknown = 0,
        Api,
        Application,
        Domain,
        Infrastructure
    }
}
