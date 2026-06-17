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

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ApiVersioningCodeFixProvider))]
[Shared]
public sealed class ApiVersioningCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticIds.ApiVersioningMissing);

    public override FixAllProvider GetFixAllProvider() =>
        WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics[0];
        const string title = "Add [ApiVersion(\"1.0\")] attribute";

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
        if (root is null)
        {
            return document;
        }

        var parent = root.FindToken(diagnostic.Location.SourceSpan.Start).Parent;
        if (parent is null)
        {
            return document;
        }

        var node = parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        if (node is null)
        {
            return document;
        }

        if (!HasApiVersionAttribute(node))
        {
            var attribute = SyntaxFactory.Attribute(
                SyntaxFactory.IdentifierName("ApiVersion"),
                SyntaxFactory.AttributeArgumentList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.AttributeArgument(
                            SyntaxFactory.LiteralExpression(
                                SyntaxKind.StringLiteralExpression,
                                SyntaxFactory.Literal("1.0"))))));

            root = root.ReplaceNode(
                node,
                node.WithAttributeLists(node.AttributeLists.Insert(0, SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(attribute)))));
        }

        if (!HasUsingDirective(root, "Microsoft.AspNetCore.Mvc"))
        {
            root = AddUsingDirective(root, "Microsoft.AspNetCore.Mvc");
        }

        return document.WithSyntaxRoot(root.NormalizeWhitespace());
    }

    private static bool HasApiVersionAttribute(ClassDeclarationSyntax node) =>
        node.AttributeLists
            .SelectMany(attributeList => attributeList.Attributes)
            .Any(attribute => attribute.Name.ToString() is "ApiVersion" or "Microsoft.AspNetCore.Mvc.ApiVersion");

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