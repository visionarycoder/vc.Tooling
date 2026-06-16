using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Distributed.Rules;

internal sealed class MissingApplyMethodRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        DiagnosticIds.DistributedEventSourcingMissingApply,
        "Missing Apply method for event",
        "Aggregate '{0}' does not define an Apply({1}) method for event '{1}'.",
        "Distributed",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSymbolAction(AnalyzeAggregate, SymbolKind.NamedType);
    }

    private static void AnalyzeAggregate(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol typeSymbol || !IsAggregate(typeSymbol))
        {
            return;
        }

        var applyMethods = typeSymbol.GetMembers().OfType<IMethodSymbol>()
            .Where(method => method.Name == "Apply" && method.Parameters.Length == 1)
            .ToList();

        var eventsRaised = FindRaisedEvents(context.Compilation, typeSymbol);
        var eventsApplied = new HashSet<ITypeSymbol>(applyMethods.Select(method => method.Parameters[0].Type), SymbolEqualityComparer.Default);

        foreach (var eventType in eventsRaised)
        {
            if (!eventsApplied.Contains(eventType))
            {
                context.ReportDiagnostic(Diagnostic.Create(descriptor, typeSymbol.Locations.FirstOrDefault(), typeSymbol.Name, eventType.Name));
            }
        }
    }

    private static bool IsAggregate(INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.Name.EndsWith("Aggregate", System.StringComparison.Ordinal) ||
               typeSymbol.Interfaces.Any(interfaceSymbol => interfaceSymbol.Name == "IAggregate");
    }

    private static ImmutableHashSet<INamedTypeSymbol> FindRaisedEvents(Compilation compilation, INamedTypeSymbol typeSymbol)
    {
        var builder = ImmutableHashSet.CreateBuilder<INamedTypeSymbol>(SymbolEqualityComparer.Default);

        foreach (var method in typeSymbol.GetMembers().OfType<IMethodSymbol>())
        {
            if (method.DeclaringSyntaxReferences.Length == 0)
            {
                continue;
            }

            var syntax = method.DeclaringSyntaxReferences[0].GetSyntax() as MethodDeclarationSyntax;
            if (syntax is null)
            {
                continue;
            }

            var model = compilation.GetSemanticModel(syntax.SyntaxTree);
            foreach (var creation in syntax.DescendantNodes().OfType<ObjectCreationExpressionSyntax>())
            {
                if (model.GetTypeInfo(creation).Type is INamedTypeSymbol eventType && eventType.Name.EndsWith("Event", System.StringComparison.Ordinal))
                {
                    builder.Add(eventType);
                }
            }
        }

        return builder.ToImmutable();
    }
}