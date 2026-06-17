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

namespace Vc.CodeFixes.Design.Vbd;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(VbdEngineStateCodeFix))]
[Shared]
public sealed class VbdEngineStateCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticIds.VbdEngineVaultStateViolation);

    public override FixAllProvider GetFixAllProvider() =>
        WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics[0];
        const string title = "Add suppression with justification";

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

        var field = parent.AncestorsAndSelf().OfType<FieldDeclarationSyntax>().FirstOrDefault();
        if (field is null)
        {
            return document;
        }

        if (!HasSuppressMessage(field, diagnostic.Id))
        {
            var suppressionAttribute = BuildSuppressionAttribute(diagnostic.Id);
            root = root.ReplaceNode(field, field.AddAttributeLists(suppressionAttribute));
        }

        if (!HasUsingDirective(root, "System.Diagnostics.CodeAnalysis"))
        {
            root = AddUsingDirective(root, "System.Diagnostics.CodeAnalysis");
        }

        return document.WithSyntaxRoot(root.NormalizeWhitespace());
    }

    private static AttributeListSyntax BuildSuppressionAttribute(string diagnosticId)
    {
        var categoryArgument = SyntaxFactory.AttributeArgument(
            SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("VBD")));

        var idArgument = SyntaxFactory.AttributeArgument(
            SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(diagnosticId)));

        var justificationArgument = SyntaxFactory.AttributeArgument(
            SyntaxFactory.NameEquals("Justification"),
            null,
            SyntaxFactory.LiteralExpression(
                SyntaxKind.StringLiteralExpression,
                SyntaxFactory.Literal("Reviewed manually; field immutability refactor planned.")));

        var arguments = SyntaxFactory.SeparatedList(new[] { categoryArgument, idArgument, justificationArgument });
        var argumentList = SyntaxFactory.AttributeArgumentList(arguments);
        var attribute = SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("SuppressMessage"), argumentList);

        return SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(attribute));
    }

    private static bool HasSuppressMessage(FieldDeclarationSyntax field, string diagnosticId)
    {
        return field.AttributeLists
            .SelectMany(attributeList => attributeList.Attributes)
            .Any(attribute =>
            {
                var attributeName = attribute.Name.ToString();
                var isSuppressMessage =
                    string.Equals(attributeName, "SuppressMessage", System.StringComparison.Ordinal) ||
                    string.Equals(attributeName, "System.Diagnostics.CodeAnalysis.SuppressMessage", System.StringComparison.Ordinal);

                var hasDiagnosticId = attribute.ArgumentList?.Arguments.Any(
                    argument => argument.ToString().Contains(diagnosticId, System.StringComparison.Ordinal)) == true;

                return isSuppressMessage && hasDiagnosticId;
            });
    }

    private static bool HasUsingDirective(SyntaxNode root, string namespaceName)
    {
        return root.DescendantNodes()
            .OfType<UsingDirectiveSyntax>()
            .Any(usingDirective => usingDirective.Name?.ToString() == namespaceName);
    }

    private static SyntaxNode AddUsingDirective(SyntaxNode root, string namespaceName)
    {
        return ((CompilationUnitSyntax)root).AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(namespaceName)));
    }
}