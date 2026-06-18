using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Security.Rules;

internal sealed class MissingAuthorizationRule : IAnalyzerRule
{
    private static readonly ImmutableHashSet<string> HttpAttributes = ImmutableHashSet.Create(
        "HttpGet", "HttpPost", "HttpPut", "HttpDelete", "HttpPatch",
        "HttpGetAttribute", "HttpPostAttribute", "HttpPutAttribute", "HttpDeleteAttribute", "HttpPatchAttribute",
        "Route", "RouteAttribute");

    private static readonly ImmutableHashSet<string> AuthAttributes = ImmutableHashSet.Create(
        "Authorize", "AuthorizeAttribute", "AllowAnonymous", "AllowAnonymousAttribute",
        "SecureEndpoint", "SecureEndpointAttribute");

    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.SecurityAuthorizationMissing,
        title: "HTTP endpoint missing authorization",
        messageFormat: "Controller or action '{0}' has an HTTP method attribute but no [Authorize] or [AllowAnonymous]. Add explicit authorization.",
        category: "Security",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
    }

    private static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;
        var attributes = method.AttributeLists.SelectMany(list => list.Attributes).Select(attribute => attribute.Name.ToString()).ToList();

        if (!attributes.Any(HttpAttributes.Contains) || attributes.Any(AuthAttributes.Contains))
        {
            return;
        }

        if (method.Parent is ClassDeclarationSyntax classDeclaration)
        {
            var classAttributes = classDeclaration.AttributeLists.SelectMany(list => list.Attributes).Select(attribute => attribute.Name.ToString());
            if (classAttributes.Any(AuthAttributes.Contains))
            {
                return;
            }
        }

        context.ReportDiagnostic(Diagnostic.Create(descriptor, method.Identifier.GetLocation(), method.Identifier.Text));
    }
}