using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Vc.CodeFixes.Resilience;
using VisionaryCoder.Analyzers.Abstractions;
using Xunit;

namespace vc.CodeFixes.Tests;

public sealed class ResilienceRetryCodeFixTests
{
    [Fact]
    public async Task RegisterCodeFixesAsync_ShouldAddSuppressMessageForMissingRetry()
    {
        var source = """
            namespace SampleApp;
            public sealed class PaymentService
            {
                public void ChargeCard(decimal amount) { }
            }
            """;

        var document = await CreateDocumentAsync(source: source);
        var root = await document.GetSyntaxRootAsync();
        var methodNode = root!.DescendantNodes().OfType<MethodDeclarationSyntax>().Single();

        var diagnostic = Diagnostic.Create(
            descriptor: new DiagnosticDescriptor(id: DiagnosticIds.ResilienceRetryPolicyMissing, title: "Missing retry policy",
                messageFormat: "External call '{0}' is not wrapped in a retry policy.", category: "Resilience",
                defaultSeverity: DiagnosticSeverity.Warning, isEnabledByDefault: true),
            location: methodNode.Identifier.GetLocation(), messageArgs: "ChargeCard");

        var provider = new ResilienceRetryCodeFix();
        CodeAction? codeAction = null;
        var context = new CodeFixContext(document: document, diagnostic: diagnostic, registerCodeFix: (action, _) => codeAction = action, cancellationToken: CancellationToken.None);
        await provider.RegisterCodeFixesAsync(context: context);

        var operations = await codeAction!.GetOperationsAsync(cancellationToken: CancellationToken.None);
        var applyOperation = Assert.IsType<ApplyChangesOperation>(@object: operations.Single());
        var updatedText = await applyOperation.ChangedSolution.GetDocument(documentId: document.Id)!.GetTextAsync();

        Assert.Contains(expectedSubstring: "SuppressMessage", actualString: updatedText.ToString());
        Assert.Contains(expectedSubstring: DiagnosticIds.ResilienceRetryPolicyMissing, actualString: updatedText.ToString());
    }

    private static async Task<Document> CreateDocumentAsync(string source)
    {
        var workspace = new AdhocWorkspace();
        var project = workspace.AddProject(name: "CodeFixTests", language: LanguageNames.CSharp)
            .WithCompilationOptions(options: new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary));
        project = project.AddMetadataReference(metadataReference: MetadataReference.CreateFromFile(path: typeof(object).Assembly.Location));
        project = project.AddMetadataReference(metadataReference: MetadataReference.CreateFromFile(path: typeof(System.Linq.Enumerable).Assembly.Location));
        project = project.AddMetadataReference(metadataReference: MetadataReference.CreateFromFile(path: typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location));
        return workspace.AddDocument(projectId: project.Id, name: "Sample.cs", text: SourceText.From(text: source));
    }
}
