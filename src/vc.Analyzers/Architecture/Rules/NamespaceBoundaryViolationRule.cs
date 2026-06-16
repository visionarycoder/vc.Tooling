using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Architecture.Rules;

internal sealed class NamespaceBoundaryViolationRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        DiagnosticIds.ArchNamespaceBoundaryViolation,
        "Namespace boundary violation",
        "Namespace '{0}' must not reference namespace '{1}'.",
        "Architecture",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSymbolAction(AnalyzeType, SymbolKind.NamedType);
    }

    private static void AnalyzeType(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol typeSymbol)
        {
            return;
        }

        var fromNamespace = typeSymbol.ContainingNamespace?.ToDisplayString();
        if (string.IsNullOrEmpty(fromNamespace))
        {
            return;
        }

        foreach (var referencedNamespace in GetReferencedNamespaces(typeSymbol))
        {
            if (!IsAllowedReference(fromNamespace, referencedNamespace))
            {
                context.ReportDiagnostic(Diagnostic.Create(descriptor, typeSymbol.Locations.FirstOrDefault(), fromNamespace, referencedNamespace));
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
                    AddNamespace(builder, method.ReturnType);
                    foreach (var parameter in method.Parameters)
                    {
                        AddNamespace(builder, parameter.Type);
                    }
                    break;
                case IPropertySymbol property:
                    AddNamespace(builder, property.Type);
                    break;
                case IFieldSymbol field:
                    AddNamespace(builder, field.Type);
                    break;
            }
        }

        return builder.ToImmutable();
    }

    private static void AddNamespace(ImmutableHashSet<string>.Builder builder, ITypeSymbol? type)
    {
        if (type?.ContainingNamespace is { } typeNamespace)
        {
            builder.Add(typeNamespace.ToDisplayString());
        }
    }

    private static bool IsAllowedReference(string from, string to)
    {
        if (from == to)
        {
            return true;
        }

        return GetRoot(from) == GetRoot(to);
    }

    private static string GetRoot(string value)
    {
        var index = value.IndexOf('.');
        return index < 0 ? value : value.Substring(0, index);
    }
}