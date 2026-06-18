using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Vc.CodeFixes.Async;
using VisionaryCoder.Analyzers.Abstractions;
using Xunit;

namespace vc.CodeFixes.Tests;

public sealed class AsyncVoidCodeFixTests
{
    [Fact]
    public async Task RegisterCodeFixesAsync_ShouldAddSuppressMessageAttributeForAsyncVoid()
    {
        var source = """
            namespace Sample.Async
            {
                public sealed class EventHandler
                {
                    public async void OnButtonClick()
                    {
                        await Task.Delay(100);
                    }
                }
            }
            """;

        var document = await CreateDocumentAsync(source: source);
        var root = await document.GetSyntaxRootAsync();
        var methodNode = root!
            .DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .First(predicate: node => node.Identifier.Text == "OnButtonClick");

        var diagnostic = Diagnostic.Create(
            descriptor: new DiagnosticDescriptor(
                id: DiagnosticIds.AsyncVoidMethod,
                title: "Async void method",
                messageFormat: "Method '{0}' returns void but is async.",
                category: "Async",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true),
            location: methodNode.Identifier.GetLocation(),
            messageArgs: "OnButtonClick");

        var provider = new AsyncVoidCodeFix();
        CodeAction? codeAction = null;

        var context = new CodeFixContext(document: document, diagnostic: diagnostic, registerCodeFix: (action, _) => codeAction = action, cancellationToken: CancellationToken.None);
        await provider.RegisterCodeFixesAsync(context: context);

        var operations = await codeAction!.GetOperationsAsync(cancellationToken: CancellationToken.None);
        var applyOperation = Assert.IsType<ApplyChangesOperation>(@object: operations.Single());
        var updatedDocument = applyOperation.ChangedSolution.GetDocument(documentId: document.Id)!;
        var updatedText = await updatedDocument.GetTextAsync();

        Assert.Contains(expectedSubstring: "using System.Diagnostics.CodeAnalysis;", actualString: updatedText.ToString());
        Assert.Contains(expectedSubstring: "SuppressMessage", actualString: updatedText.ToString());
        Assert.Contains(expectedSubstring: DiagnosticIds.AsyncVoidMethod, actualString: updatedText.ToString());
    }

    private static async Task<Document> CreateDocumentAsync(string source)
    {
        var workspace = new AdhocWorkspace();
        var project = workspace.AddProject(name: "CodeFixTests", language: LanguageNames.CSharp)
            .WithCompilationOptions(options: new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary));

        project = project.AddMetadataReference(metadataReference: MetadataReference.CreateFromFile(path: typeof(object).Assembly.Location));
        project = project.AddMetadataReference(metadataReference: MetadataReference.CreateFromFile(path: typeof(System.Linq.Enumerable).Assembly.Location));
        project = project.AddMetadataReference(metadataReference: MetadataReference.CreateFromFile(path: typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location));

        return workspace.AddDocument(projectId: project.Id, name: "AsyncVoid.cs", text: SourceText.From(text: source));
    }
}
