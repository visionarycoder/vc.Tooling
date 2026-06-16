using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VisionaryCoder.Tooling.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NamingConventionAnalyzer : DiagnosticAnalyzer
{
    public static readonly DiagnosticDescriptor Rule = new(
        id: "VC4003",
        title: "Naming convention violation",
        messageFormat: "{0} '{1}' does not follow {2} naming convention",
        category: "Style",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeField, SyntaxKind.FieldDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeProperty, SyntaxKind.PropertyDeclaration);
    }

    private static void AnalyzeField(SyntaxNodeAnalysisContext ctx)
    {
        var field = (FieldDeclarationSyntax)ctx.Node;
        var isPrivate = field.Modifiers.Any(SyntaxKind.PrivateKeyword) || !field.Modifiers.Any();
        foreach (var variable in field.Declaration.Variables)
        {
            var name = variable.Identifier.Text;
            if (isPrivate && !name.StartsWith("_"))
            {
                ctx.ReportDiagnostic(Diagnostic.Create(Rule, variable.GetLocation(), "Private field", name, "_camelCase"));
            }
        }
    }

    private static void AnalyzeProperty(SyntaxNodeAnalysisContext ctx)
    {
        var property = (PropertyDeclarationSyntax)ctx.Node;
        var name = property.Identifier.Text;
        if (char.IsLower(name[0]))
        {
            ctx.ReportDiagnostic(Diagnostic.Create(Rule, property.Identifier.GetLocation(), "Property", name, "PascalCase"));
        }
    }
}