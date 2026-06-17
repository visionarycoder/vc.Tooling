using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.CodeFixes.Api;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ApiValidationCodeFixProvider))]
[Shared]
public sealed class ApiValidationCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticIds.ApiValidationMissing);

    public override FixAllProvider GetFixAllProvider() =>
        WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics[0];
        const string title = "Add [Required] validation attribute";

        context.RegisterCodeFix(
            CodeAction.Create(
                title,
                cancellationToken => ApplyFixAsync(context.Document, diagnostic, cancellationToken),
                equivalenceKey: title),
            diagnostic);
    }

    private static async Task<Document> ApplyFixAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

        if (root is null || semanticModel is null)
        {
            return document;
        }

        var parent = root.FindToken(diagnostic.Location.SourceSpan.Start).Parent;
        if (parent is null)
        {
            return document;
        }

        var method = parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().FirstOrDefault();
        if (method is null)
        {
            return document;
        }

        var methodSymbol = semanticModel.GetDeclaredSymbol(method, cancellationToken);
        if (methodSymbol is null)
        {
            return document;
        }

        var complexParameter = methodSymbol.Parameters.FirstOrDefault(IsComplexInput);
        if (complexParameter is null)
        {
            return document;
        }

        var parameterSyntax = method.ParameterList.Parameters
            .FirstOrDefault(parameter => parameter.Identifier.Text == complexParameter.Name);

        if (parameterSyntax is null)
        {
            return document;
        }

        if (!HasRequiredAttribute(parameterSyntax))
        {
            var attribute = SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("Required"));
            var attributeList = SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(attribute));

            root = root.ReplaceNode(
                parameterSyntax,
                parameterSyntax.AddAttributeLists(attributeList));
        }

        if (!HasUsingDirective(root, "System.ComponentModel.DataAnnotations"))
        {
            root = AddUsingDirective(root, "System.ComponentModel.DataAnnotations");
        }

        return document.WithSyntaxRoot(root.NormalizeWhitespace());
    }

    private static bool IsComplexInput(IParameterSymbol parameter)
    {
        if (parameter.Type.SpecialType != SpecialType.None || parameter.Type.TypeKind == TypeKind.Enum)
        {
            return false;
        }

        return !string.Equals(parameter.Type.Name, "String", System.StringComparison.Ordinal);
    }

    private static bool HasRequiredAttribute(ParameterSyntax parameterSyntax) =>
        parameterSyntax.AttributeLists
            .SelectMany(attributeList => attributeList.Attributes)
            .Any(attribute => attribute.Name.ToString() is "Required" or "System.ComponentModel.DataAnnotations.Required");

    private static bool HasUsingDirective(SyntaxNode root, string namespaceName) =>
        root.DescendantNodes()
            .OfType<UsingDirectiveSyntax>()
            .Any(usingDirective => usingDirective.Name.ToString() == namespaceName);

    private static SyntaxNode AddUsingDirective(SyntaxNode root, string namespaceName)
    {
        var usingDirective = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(namespaceName));
        return ((CompilationUnitSyntax)root).AddUsings(usingDirective);
    }
}