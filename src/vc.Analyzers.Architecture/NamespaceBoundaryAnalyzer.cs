using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Vc.Analyzers.Architecture;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NamespaceBoundaryAnalyzer : DiagnosticAnalyzer
{
    public const string NamespaceBoundaryViolationId = "VCARCH003";

    private static readonly DiagnosticDescriptor NamespaceBoundaryViolationRule = new(
        NamespaceBoundaryViolationId,
        "Namespace boundary violation",
        "Namespace '{0}' must not reference namespace '{1}'.",
        "Architecture",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(NamespaceBoundaryViolationRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

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
            if (IsAllowedReference(fromNamespace, referencedNamespace))
            {
                continue;
            }

            context.ReportDiagnostic(
                Diagnostic.Create(
                    NamespaceBoundaryViolationRule,
                    typeSymbol.Locations.FirstOrDefault(),
                    fromNamespace,
                    referencedNamespace));
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
                    foreach (var p in method.Parameters)
                    {
                        AddNamespace(builder, p.Type);
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
        if (type?.ContainingNamespace is { } ns)
        {
            builder.Add(ns.ToDisplayString());
        }
    }

    private static bool IsAllowedReference(string from, string to)
    {
        // Allow self-references
        if (from == to)
        {
            return true;
        }

        // Allow references within the same root namespace
        var fromRoot = GetRoot(from);
        var toRoot = GetRoot(to);

        if (fromRoot == toRoot)
        {
            return true;
        }

        // Disallow cross-domain references:
        // Domain → Infrastructure
        // Domain → Api
        // Application → Api
        // Api → Infrastructure
        // etc.
        return false;
    }

    private static string GetRoot(string ns)
    {
        var index = ns.IndexOf('.');
        return index < 0 ? ns : ns.Substring(0, index);
    }
}
