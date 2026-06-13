using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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

        var eventsRaised = FindRaisedEvents(typeSymbol);
        var eventsApplied = applyMethods.Select(m => m.Parameters[0].Type).ToHashSet(SymbolEqualityComparer.Default);

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

        DetectIllegalStateMutations(context, typeSymbol, applyMethods);
    }

    private static bool IsAggregate(INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.Name.EndsWith("Aggregate") ||
               typeSymbol.Interfaces.Any(i => i.Name == "IAggregate");
    }

    private static ImmutableHashSet<INamedTypeSymbol> FindRaisedEvents(INamedTypeSymbol typeSymbol)
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
                var model = method.ContainingAssembly.GetSemanticModel(syntax.SyntaxTree);
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
        INamedTypeSymbol aggregate,
        IEnumerable<IMethodSymbol> applyMethods)
    {
        var applySyntaxes = applyMethods
            .Where(m => m.DeclaringSyntaxReferences.Length > 0)
            .Select(m => m.DeclaringSyntaxReferences[0].GetSyntax())
            .OfType<MethodDeclarationSyntax>()
            .ToHashSet();

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
                var model = method.ContainingAssembly.GetSemanticModel(syntax.SyntaxTree);
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

        if (returnType.Name.EndsWith("Entity") ||
            returnType.Name.EndsWith("Model") ||
            returnType.Name.EndsWith("Domain"))
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

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class CircuitBreakerAnalyzer : DiagnosticAnalyzer
{
    public const string MissingCircuitBreakerId = "VCRES001";
    public const string UnusedCircuitBreakerId = "VCRES002";

    private static readonly DiagnosticDescriptor MissingCircuitBreakerRule = new(
        MissingCircuitBreakerId,
        "Missing circuit breaker",
        "External call '{0}' is not wrapped in a circuit breaker policy.",
        "Resilience",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor UnusedCircuitBreakerRule = new(
        UnusedCircuitBreakerId,
        "Circuit breaker declared but never used",
        "Circuit breaker policy '{0}' is declared but never applied.",
        "Resilience",
        DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(MissingCircuitBreakerRule, UnusedCircuitBreakerRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        context.RegisterSymbolAction(AnalyzeFieldOrProperty, SymbolKind.Field, SymbolKind.Property);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not InvocationExpressionSyntax invocation)
        {
            return;
        }

        var semanticModel = context.SemanticModel;
        var cancellationToken = context.CancellationToken;

        var symbol = semanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol as IMethodSymbol;
        if (symbol is null)
        {
            return;
        }

        if (!IsExternalCall(symbol))
        {
            return;
        }

        if (IsInsideCircuitBreaker(invocation))
        {
            return;
        }

        context.ReportDiagnostic(
            Diagnostic.Create(
                MissingCircuitBreakerRule,
                invocation.GetLocation(),
                symbol.Name));
    }

    private static bool IsExternalCall(IMethodSymbol methodSymbol)
    {
        var ns = methodSymbol.ContainingNamespace?.ToDisplayString();

        if (ns is null)
        {
            return false;
        }

        if (ns.StartsWith("System.Net.Http"))
        {
            return true;
        }

        if (ns.Contains("SqlClient"))
        {
            return true;
        }

        if (ns.Contains("Redis") || ns.Contains("Mongo") || ns.Contains("Cosmos"))
        {
            return true;
        }

        return false;
    }

    private static bool IsInsideCircuitBreaker(SyntaxNode node)
    {
        var current = node.Parent;

        while (current is not null)
        {
            if (current is InvocationExpressionSyntax invocation)
            {
                if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
                {
                    var name = memberAccess.Name.Identifier.Text;

                    if (name.Contains("ExecuteAsync") ||
                        name.Contains("Execute") ||
                        name.Contains("WrapAsync") ||
                        name.Contains("Wrap"))
                    {
                        return true;
                    }
                }
            }

            current = current.Parent;
        }

        return false;
    }

    private static void AnalyzeFieldOrProperty(SymbolAnalysisContext context)
    {
        var symbol = context.Symbol;

        if (!IsCircuitBreakerPolicy(symbol))
        {
            return;
        }

        if (!IsUsed(symbol))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    UnusedCircuitBreakerRule,
                    symbol.Locations.FirstOrDefault(),
                    symbol.Name));
        }
    }

    private static bool IsCircuitBreakerPolicy(ISymbol symbol)
    {
        var type = symbol switch
        {
            IFieldSymbol f => f.Type,
            IPropertySymbol p => p.Type,
            _ => null
        };

        if (type is null)
        {
            return false;
        }

        return type.Name.Contains("CircuitBreaker") ||
               type.Name.Contains("AsyncPolicy") ||
               type.Name.Contains("Policy");
    }

    private static bool IsUsed(ISymbol symbol)
    {
        foreach (var reference in symbol.DeclaringSyntaxReferences)
        {
            var syntax = reference.GetSyntax();

            var root = syntax.SyntaxTree.GetRoot();

            var invocations = root.DescendantNodes().OfType<InvocationExpressionSyntax>();

            foreach (var invocation in invocations)
            {
                if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
                {
                    if (memberAccess.Expression is IdentifierNameSyntax id &&
                        id.Identifier.Text == symbol.Name)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
