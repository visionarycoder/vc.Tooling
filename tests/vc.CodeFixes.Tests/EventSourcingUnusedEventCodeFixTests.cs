using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Vc.CodeFixes.Distributed;
using VisionaryCoder.Analyzers.Abstractions;
using Xunit;

namespace vc.CodeFixes.Tests;

public sealed class EventSourcingUnusedEventCodeFixTests
{
    [Fact]
    public async Task RegisterCodeFixesAsync_ShouldAddSuppressMessageAttributeForUnusedEvent()
    {
        var source = """
            namespace Sample.Domain
            {
                public sealed class UnusedEvent
                {
                }

                public sealed class OrderAggregate
                {
                    public void RaiseEvent()
                    {
                        var evt = new UnusedEvent();
                    }
                }
            }
            """;

        var document = await CreateDocumentAsync(source: source);
        var root = await document.GetSyntaxRootAsync();
        var classNode = root!
            .DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .First(predicate: node => node.Identifier.Text == "OrderAggregate");

        var diagnostic = Diagnostic.Create(
            descriptor: new DiagnosticDescriptor(
                id: DiagnosticIds.DistributedEventSourcingUnusedEvent,
                title: "Event is never applied",
                messageFormat: "Event '{0}' is created or raised but never applied in the aggregate.",
                category: "Distributed",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true),
            location: classNode.Identifier.GetLocation(),
            messageArgs: "UnusedEvent");

        var provider = new EventSourcingUnusedEventCodeFix();
        CodeAction? codeAction = null;

        var context = new CodeFixContext(document: document, diagnostic: diagnostic, registerCodeFix: (action, _) => codeAction = action, cancellationToken: CancellationToken.None);
        await provider.RegisterCodeFixesAsync(context: context);

        var operations = await codeAction!.GetOperationsAsync(cancellationToken: CancellationToken.None);
        var applyOperation = Assert.IsType<ApplyChangesOperation>(@object: operations.Single());
        var updatedDocument = applyOperation.ChangedSolution.GetDocument(documentId: document.Id)!;
        var updatedText = await updatedDocument.GetTextAsync();

        Assert.Contains(expectedSubstring: "using System.Diagnostics.CodeAnalysis;", actualString: updatedText.ToString());
        Assert.Contains(expectedSubstring: "SuppressMessage", actualString: updatedText.ToString());
        Assert.Contains(expectedSubstring: DiagnosticIds.DistributedEventSourcingUnusedEvent, actualString: updatedText.ToString());
    }

    private static async Task<Document> CreateDocumentAsync(string source)
    {
        var workspace = new AdhocWorkspace();
        var project = workspace.AddProject(name: "CodeFixTests", language: LanguageNames.CSharp)
            .WithCompilationOptions(options: new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary));

        project = project.AddMetadataReference(metadataReference: MetadataReference.CreateFromFile(path: typeof(object).Assembly.Location));
        project = project.AddMetadataReference(metadataReference: MetadataReference.CreateFromFile(path: typeof(System.Linq.Enumerable).Assembly.Location));
        project = project.AddMetadataReference(metadataReference: MetadataReference.CreateFromFile(path: typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location));

        return workspace.AddDocument(projectId: project.Id, name: "EventSourcingUnusedEvent.cs", text: SourceText.From(text: source));
    }
}
