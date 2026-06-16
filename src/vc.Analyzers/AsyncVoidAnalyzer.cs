using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VisionaryCoder.Tooling.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AsyncVoidAnalyzer : DiagnosticAnalyzer
{
    public static readonly DiagnosticDescriptor Rule = new(
        id: "VC1001",
        title: "Avoid async void methods",
        messageFormat: "Method '{0}' is async void. Use async Task instead unless this is an event handler.",
        category: "Reliability",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "async void methods cannot be awaited and swallow exceptions. Convert to async Task.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
    }

    private static void AnalyzeMethod(SyntaxNodeAnalysisContext ctx)
    {
        var method = (MethodDeclarationSyntax)ctx.Node;
        if (!method.Modifiers.Any(SyntaxKind.AsyncKeyword)) return;
        if (method.ReturnType.ToString() != "void") return;

        // Allow event handlers (object sender, EventArgs e)
        var parameters = method.ParameterList.Parameters;
        if (parameters.Count == 2)
        {
            var p0 = ctx.SemanticModel.GetTypeInfo(parameters[0].Type!).Type;
            var p1 = ctx.SemanticModel.GetTypeInfo(parameters[1].Type!).Type;
            if (p0?.SpecialType == SpecialType.System_Object &&
                (p1?.Name.EndsWith("EventArgs") ?? false))
                return;
        }

        ctx.ReportDiagnostic(Diagnostic.Create(Rule, method.Identifier.GetLocation(), method.Identifier.Text));
    }
}