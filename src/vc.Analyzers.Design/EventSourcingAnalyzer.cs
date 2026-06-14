using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Vc.Analyzers.Distributed;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class EventSourcingAnalyzer : DiagnosticAnalyzer
{
    public const string MissingApplyMethodId = "VCDIST001";
    public const string UnusedEventId = "VCDIST002";
    public const string MutableStateId = "VCDIST003";

    private static readonly DiagnosticDescriptor MissingApplyMethodRule = new(
        MissingApplyMethodId,
        "Missing Apply method for event",
        "Aggregate '{0}' does not define an Apply({1}) method for event '{1}'.",
        "Distributed",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor UnusedEventRule = new(
        UnusedEventId,
        "Event is never applied",
        "Event '{0}' is created or raised but never applied in the aggregate.",
        "Distributed",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor MutableStateRule = new(
        MutableStateId,
        "Event-sourced aggregate must not mutate state outside Apply()",
        "State member '{0}' is mutated outside an Apply() method.",
        "Distributed",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(MissingApplyMethodRule, UnusedEventRule, MutableStateRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(AnalyzeAggregate, SymbolKind.NamedType);
    }

    private static void AnalyzeAggregate(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol typeSymbol)
        {
            return;
        }

        if (!IsAggregate(typeSymbol))
        {
            return;
        }

        var applyMethods = typeSymbol
            .GetMembers()
            .OfType<IMethodSymbol>()
            .Where(m => m.Name == "Apply" && m.Parameters.Length == 1)
            .ToList();

        var eventsRaised = FindRaisedEvents(context.Compilation, typeSymbol);
        var eventsApplied = new HashSet<ITypeSymbol>(
            applyMethods.Select(m => m.Parameters[0].Type),
            SymbolEqualityComparer.Default);

        foreach (var evt in eventsRaised)
        {
            if (!eventsApplied.Contains(evt))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        MissingApplyMethodRule,
                        typeSymbol.Locations.FirstOrDefault(),
                        typeSymbol.Name,
                        evt.Name));
            }
        }

        foreach (var evt in eventsRaised)
        {
            if (!eventsApplied.Contains(evt))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        UnusedEventRule,
                        typeSymbol.Locations.FirstOrDefault(),
                        evt.Name));
            }
        }

        DetectIllegalStateMutations(context, context.Compilation, typeSymbol, applyMethods);
    }

    private static bool IsAggregate(INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.Name.EndsWith("Aggregate") ||
               typeSymbol.Interfaces.Any(i => i.Name == "IAggregate");
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

            foreach (var objCreation in syntax.DescendantNodes().OfType<ObjectCreationExpressionSyntax>())
            {
                var model = compilation.GetSemanticModel(syntax.SyntaxTree);
                var type = model.GetTypeInfo(objCreation).Type as INamedTypeSymbol;

                if (type is not null && type.Name.EndsWith("Event"))
                {
                    builder.Add(type);
                }
            }
        }

        return builder.ToImmutable();
    }

    private static void DetectIllegalStateMutations(
        SymbolAnalysisContext context,
        Compilation compilation,
        INamedTypeSymbol aggregate,
        IEnumerable<IMethodSymbol> applyMethods)
    {
        var applySyntaxes = applyMethods
            .Where(m => m.DeclaringSyntaxReferences.Length > 0)
            .Select(m => m.DeclaringSyntaxReferences[0].GetSyntax())
            .OfType<MethodDeclarationSyntax>()
            .ToList();

        foreach (var method in aggregate.GetMembers().OfType<IMethodSymbol>())
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

            if (applySyntaxes.Contains(syntax))
            {
                continue;
            }

            foreach (var assignment in syntax.DescendantNodes().OfType<AssignmentExpressionSyntax>())
            {
                var model = compilation.GetSemanticModel(syntax.SyntaxTree);
                var symbol = model.GetSymbolInfo(assignment.Left).Symbol;

                if (symbol is IFieldSymbol or IPropertySymbol)
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            MutableStateRule,
                            assignment.GetLocation(),
                            symbol.Name));
                }
            }
        }
    }
}
