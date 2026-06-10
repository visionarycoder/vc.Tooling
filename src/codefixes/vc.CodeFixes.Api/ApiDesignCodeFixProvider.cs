using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Vc.CodeFixes.Api;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ApiDesignCodeFixProvider))]
[Shared]
public sealed class ApiDesignCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create("VCAPI001");

    public override FixAllProvider GetFixAllProvider() =>
        WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        // TODO: Implement code fix logic for VCAPI001.
        var diagnostic = context.Diagnostics[0];
        var title = "Apply API design guideline";

        context.RegisterCodeFix(
            CodeAction.Create(
                title,
                cancellationToken => ApplyFixAsync(context.Document, diagnostic, cancellationToken),
                equivalenceKey: title),
            diagnostic);
    }

    private static Task<Document> ApplyFixAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
    {
        // TODO: Implement document transformation.
        return Task.FromResult(document);
    }
}
