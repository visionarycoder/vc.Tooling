using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VisionaryCoder.Tooling.SecurityGenerators;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MissingAuthorizationAnalyzer : DiagnosticAnalyzer
{
    public static readonly DiagnosticDescriptor Rule = new(
        id: "VC3003",
        title: "HTTP endpoint missing authorization",
        messageFormat: "Controller or action '{0}' has an HTTP method attribute but no [Authorize] or [AllowAnonymous]. Add explicit authorization.",
        category: "Security",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly ImmutableHashSet<string> HttpAttributes = ImmutableHashSet.Create(
        "HttpGet", "HttpPost", "HttpPut", "HttpDelete", "HttpPatch",
        "HttpGetAttribute", "HttpPostAttribute", "HttpPutAttribute", "HttpDeleteAttribute", "HttpPatchAttribute",
        "Route", "RouteAttribute");

    private static readonly ImmutableHashSet<string> AuthAttributes = ImmutableHashSet.Create(
        "Authorize", "AuthorizeAttribute", "AllowAnonymous", "AllowAnonymousAttribute",
        "SecureEndpoint", "SecureEndpointAttribute");

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
        var attrs = method.AttributeLists.SelectMany(a => a.Attributes).Select(a => a.Name.ToString()).ToList();

        if (!attrs.Any(a => HttpAttributes.Contains(a))) return;
        if (attrs.Any(a => AuthAttributes.Contains(a))) return;

        // Check parent class for auth attributes
        if (method.Parent is ClassDeclarationSyntax classDecl)
        {
            var classAttrs = classDecl.AttributeLists.SelectMany(a => a.Attributes).Select(a => a.Name.ToString());
            if (classAttrs.Any(a => AuthAttributes.Contains(a))) return;
        }

        ctx.ReportDiagnostic(Diagnostic.Create(Rule, method.Identifier.GetLocation(), method.Identifier.Text));
    }
}