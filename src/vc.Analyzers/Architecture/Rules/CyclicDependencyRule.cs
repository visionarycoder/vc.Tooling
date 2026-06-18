using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Architecture.Rules;

internal sealed class CyclicDependencyRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.ArchCyclicDependency,
        title: "Cyclic namespace dependency detected",
        messageFormat: "Namespace '{0}' depends on '{1}', forming a cyclic dependency.",
        category: "Architecture",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterCompilationStartAction(action: Start);
    }

    private static void Start(CompilationStartAnalysisContext context)
    {
        var graph = new DependencyGraph();

        context.RegisterSymbolAction(action: symbolContext =>
        {
            if (symbolContext.Symbol is not INamedTypeSymbol typeSymbol)
            {
                return;
            }

            var fromNamespace = typeSymbol.ContainingNamespace?.ToDisplayString();
            if (fromNamespace is null || fromNamespace.Length == 0)
            {
                return;
            }

            var sourceNamespace = fromNamespace;
            foreach (var referenced in GetReferencedNamespaces(typeSymbol: typeSymbol))
            {
                if (referenced != sourceNamespace)
                {
                    graph.AddEdge(from: sourceNamespace, to: referenced, type: typeSymbol);
                }
            }
        }, symbolKinds: SymbolKind.NamedType);

        context.RegisterCompilationEndAction(action: endContext =>
        {
            foreach (var cycle in graph.FindCycles())
            {
                var (from, to, typeSymbol) = cycle;
                endContext.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: typeSymbol.Locations.FirstOrDefault(), messageArgs: [from, to]));
            }
        });
    }

    private static IEnumerable<string> GetReferencedNamespaces(INamedTypeSymbol typeSymbol)
    {
        foreach (var member in typeSymbol.GetMembers())
        {
            switch (member)
            {
                case IMethodSymbol method:
                    if (method.ReturnType?.ContainingNamespace is { } returnNamespace)
                    {
                        yield return returnNamespace.ToDisplayString();
                    }

                    foreach (var parameter in method.Parameters)
                    {
                        if (parameter.Type.ContainingNamespace is { } parameterNamespace)
                        {
                            yield return parameterNamespace.ToDisplayString();
                        }
                    }
                    break;

                case IPropertySymbol property:
                    if (property.Type.ContainingNamespace is { } propertyNamespace)
                    {
                        yield return propertyNamespace.ToDisplayString();
                    }
                    break;

                case IFieldSymbol field:
                    if (field.Type.ContainingNamespace is { } fieldNamespace)
                    {
                        yield return fieldNamespace.ToDisplayString();
                    }
                    break;
            }
        }
    }
}