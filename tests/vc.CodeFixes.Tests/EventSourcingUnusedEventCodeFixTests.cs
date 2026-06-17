using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Vc.CodeFixes.Distributed;
using VisionaryCoder.Tooling.Analyzers.Common;
using Xunit;

namespace Vc.CodeFixes.Tests;

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

        var document = await CreateDocumentAsync(source);
        var root = await document.GetSyntaxRootAsync();
        var classNode = root!
            .DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .First(node => node.Identifier.Text == "OrderAggregate");

        var diagnostic = Diagnostic.Create(
            new DiagnosticDescriptor(
                DiagnosticIds.DistributedEventSourcingUnusedEvent,
                "Event is never applied",
                "Event '{0}' is created or raised but never applied in the aggregate.",
                "Distributed",
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true),
            classNode.Identifier.GetLocation(),
            "UnusedEvent");

        var provider = new EventSourcingUnusedEventCodeFix();
        CodeAction? codeAction = null;

        var context = new CodeFixContext(document, diagnostic, (action, _) => codeAction = action, CancellationToken.None);
        await provider.RegisterCodeFixesAsync(context);

        var operations = await codeAction!.GetOperationsAsync(CancellationToken.None);
        var applyOperation = Assert.IsType<ApplyChangesOperation>(operations.Single());
        var updatedDocument = applyOperation.ChangedSolution.GetDocument(document.Id)!;
        var updatedText = await updatedDocument.GetTextAsync();

        Assert.Contains("using System.Diagnostics.CodeAnalysis;", updatedText.ToString());
        Assert.Contains("SuppressMessage", updatedText.ToString());
        Assert.Contains(DiagnosticIds.DistributedEventSourcingUnusedEvent, updatedText.ToString());
    }

    private static async Task<Document> CreateDocumentAsync(string source)
    {
        var workspace = new AdhocWorkspace();
        var project = workspace.AddProject("CodeFixTests", LanguageNames.CSharp)
            .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        project = project.AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
        project = project.AddMetadataReference(MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location));
        project = project.AddMetadataReference(MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location));

        return workspace.AddDocument(project.Id, "EventSourcingUnusedEvent.cs", SourceText.From(source));
    }
}
