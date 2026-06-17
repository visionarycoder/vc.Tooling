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

namespace Vc.CodeFixes.Performance;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PerformanceAllocationCodeFix))]
[Shared]
public sealed class PerformanceAllocationCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticIds.PerfAllocationInHotPath);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

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
        if (root is null) return document;

        var parent = root.FindToken(diagnostic.Location.SourceSpan.Start).Parent;
        if (parent is null) return document;

        var methodNode = parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().FirstOrDefault();
        if (methodNode is null) return document;

        if (!HasSuppressMessage(methodNode, diagnostic.Id))
        {
            var suppressionAttribute = BuildSuppressionAttribute(diagnostic.Id);
            root = root.ReplaceNode(methodNode, methodNode.AddAttributeLists(suppressionAttribute));
        }

        if (!HasUsingDirective(root, "System.Diagnostics.CodeAnalysis"))
            root = AddUsingDirective(root, "System.Diagnostics.CodeAnalysis");

        return document.WithSyntaxRoot(root.NormalizeWhitespace());
    }

    private static AttributeListSyntax BuildSuppressionAttribute(string diagnosticId)
    {
        var args = SyntaxFactory.SeparatedList(new[]
        {
            SyntaxFactory.AttributeArgument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("Performance"))),
            SyntaxFactory.AttributeArgument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(diagnosticId))),
            SyntaxFactory.AttributeArgument(SyntaxFactory.NameEquals("Justification"), null,
                SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression,
                    SyntaxFactory.Literal("Allocation is intentional or this code path is not on the critical performance path.")))
        });
        return SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(
            SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("SuppressMessage"), SyntaxFactory.AttributeArgumentList(args))));
    }

    private static bool HasSuppressMessage(MethodDeclarationSyntax node, string id) =>
        node.AttributeLists.SelectMany(l => l.Attributes).Any(a =>
            (a.Name.ToString() is "SuppressMessage" or "System.Diagnostics.CodeAnalysis.SuppressMessage") &&
            a.ArgumentList?.Arguments.Any(arg => arg.ToString().Contains(id, System.StringComparison.Ordinal)) == true);

    private static bool HasUsingDirective(SyntaxNode root, string ns) =>
        root.DescendantNodes().OfType<UsingDirectiveSyntax>().Any(u => u.Name?.ToString() == ns);

    private static SyntaxNode AddUsingDirective(SyntaxNode root, string ns) =>
        ((CompilationUnitSyntax)root).AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(ns)));
}
