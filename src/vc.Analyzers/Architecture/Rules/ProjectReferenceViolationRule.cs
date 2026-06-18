using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Architecture.Rules;

internal sealed class ProjectReferenceViolationRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.ArchProjectReferenceViolation,
        title: "Project reference violation",
        messageFormat: "Project '{0}' must not reference project '{1}'.",
        category: "Architecture",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterCompilationStartAction(action: Start);
    }

    private static void Start(CompilationStartAnalysisContext context)
    {
        var currentAssembly = context.Compilation.Assembly;
        var currentLayer = GetLayer(assemblyName: currentAssembly.Name);

        context.RegisterSymbolAction(action: symbolContext =>
        {
            if (symbolContext.Symbol is not INamedTypeSymbol typeSymbol)
            {
                return;
            }

            foreach (var referencedAssembly in typeSymbol.ContainingAssembly.Modules.SelectMany(selector: module => module.ReferencedAssemblySymbols))
            {
                var referencedLayer = GetLayer(assemblyName: referencedAssembly.Name);
                if (referencedLayer != Layer.Unknown && !IsAllowedReference(from: currentLayer, to: referencedLayer))
                {
                    symbolContext.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: typeSymbol.Locations.FirstOrDefault(), messageArgs: [currentAssembly.Name, referencedAssembly.Name]));
                }
            }
        }, symbolKinds: SymbolKind.NamedType);
    }

    private static Layer GetLayer(string assemblyName)
    {
        if (assemblyName.EndsWith(value: ".Api", comparisonType: System.StringComparison.Ordinal))
        {
            return Layer.Api;
        }

        if (assemblyName.EndsWith(value: ".Application", comparisonType: System.StringComparison.Ordinal))
        {
            return Layer.Application;
        }

        if (assemblyName.EndsWith(value: ".Domain", comparisonType: System.StringComparison.Ordinal))
        {
            return Layer.Domain;
        }

        if (assemblyName.EndsWith(value: ".Infrastructure", comparisonType: System.StringComparison.Ordinal))
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