using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VisionaryCoder.Tooling.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MissingXmlDocAnalyzer : DiagnosticAnalyzer
{
    public static readonly DiagnosticDescriptor Rule = new(
        id: "VC4002",
        title: "Public API missing XML documentation",
        messageFormat: "Public member '{0}' does not have XML documentation. Add a <summary> comment.",
        category: "Documentation",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration, SyntaxKind.PropertyDeclaration, SyntaxKind.ClassDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext ctx)
    {
        var symbol = ctx.SemanticModel.GetDeclaredSymbol(ctx.Node);
        if (symbol is null || symbol.DeclaredAccessibility != Accessibility.Public) return;
        if (symbol.GetDocumentationCommentXml() is { Length: > 0 }) return;

        var location = ctx.Node switch
        {
            MethodDeclarationSyntax m => m.Identifier.GetLocation(),
            PropertyDeclarationSyntax p => p.Identifier.GetLocation(),
            ClassDeclarationSyntax c => c.Identifier.GetLocation(),
            _ => ctx.Node.GetLocation()
        };

        ctx.ReportDiagnostic(Diagnostic.Create(Rule, location, symbol.Name));
    }
}