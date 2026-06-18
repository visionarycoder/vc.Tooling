using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Architecture.Rules;

internal sealed class NamespaceBoundaryViolationRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.ArchNamespaceBoundaryViolation,
        title: "Namespace boundary violation",
        messageFormat: "Namespace '{0}' must not reference namespace '{1}'.",
        category: "Architecture",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSymbolAction(action: AnalyzeType, symbolKinds: SymbolKind.NamedType);
    }

    private static void AnalyzeType(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol typeSymbol)
        {
            return;
        }

        var fromNamespace = typeSymbol.ContainingNamespace?.ToDisplayString();
        if (fromNamespace is null || fromNamespace.Length == 0)
        {
            return;
        }

        var sourceNamespace = fromNamespace;
        foreach (var referencedNamespace in GetReferencedNamespaces(typeSymbol: typeSymbol))
        {
            if (!IsAllowedReference(from: sourceNamespace, to: referencedNamespace))
            {
                context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: typeSymbol.Locations.FirstOrDefault(), messageArgs: [sourceNamespace, referencedNamespace]));
            }
        }
    }

    private static ImmutableHashSet<string> GetReferencedNamespaces(INamedTypeSymbol typeSymbol)
    {
        var builder = ImmutableHashSet.CreateBuilder<string>();

        foreach (var member in typeSymbol.GetMembers())
        {
            switch (member)
            {
                case IMethodSymbol method:
                    AddNamespace(builder: builder, type: method.ReturnType);
                    foreach (var parameter in method.Parameters)
                    {
                        AddNamespace(builder: builder, type: parameter.Type);
                    }
                    break;
                case IPropertySymbol property:
                    AddNamespace(builder: builder, type: property.Type);
                    break;
                case IFieldSymbol field:
                    AddNamespace(builder: builder, type: field.Type);
                    break;
            }
        }

        return builder.ToImmutable();
    }

    private static void AddNamespace(ImmutableHashSet<string>.Builder builder, ITypeSymbol? type)
    {
        if (type?.ContainingNamespace is { } typeNamespace)
        {
            builder.Add(item: typeNamespace.ToDisplayString());
        }
    }

    private static bool IsAllowedReference(string from, string to)
    {
        if (from == to)
        {
            return true;
        }

        return GetRoot(value: from) == GetRoot(value: to);
    }

    private static string GetRoot(string value)
    {
        var index = value.IndexOf(value: '.');
        return index < 0 ? value : value.Substring(startIndex: 0, length: index);
    }
}