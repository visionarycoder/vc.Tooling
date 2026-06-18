using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Vc.CodeFixes.Data;
using VisionaryCoder.Analyzers.Abstractions;
using Xunit;

namespace vc.CodeFixes.Tests;

public sealed class DtoDesignCodeFixTests
{
    [Fact]
    public async Task RegisterCodeFixesAsync_ShouldAddSuppressMessageAttributeForDtoMissing()
    {
        var source = """
            namespace Sample.Data
            {
                public sealed class User
                {
                    public int Id { get; set; }
                    public string Name { get; set; }
                }
            }
            """;

        var document = await CreateDocumentAsync(source: source);
        var root = await document.GetSyntaxRootAsync();
        var classNode = root!
            .DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .First(predicate: node => node.Identifier.Text == "User");

        var diagnostic = Diagnostic.Create(
            descriptor: new DiagnosticDescriptor(
                id: DiagnosticIds.DataDesignDtoMissing,
                title: "DTO missing",
                messageFormat: "Type '{0}' should be transferred as DTO.",
                category: "Data",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true),
            location: classNode.Identifier.GetLocation(),
            messageArgs: "User");

        var provider = new DtoDesignCodeFix();
        CodeAction? codeAction = null;

        var context = new CodeFixContext(document: document, diagnostic: diagnostic, registerCodeFix: (action, _) => codeAction = action, cancellationToken: CancellationToken.None);
        await provider.RegisterCodeFixesAsync(context: context);

        var operations = await codeAction!.GetOperationsAsync(cancellationToken: CancellationToken.None);
        var applyOperation = Assert.IsType<ApplyChangesOperation>(@object: operations.Single());
        var updatedDocument = applyOperation.ChangedSolution.GetDocument(documentId: document.Id)!;
        var updatedText = await updatedDocument.GetTextAsync();

        Assert.Contains(expectedSubstring: "using System.Diagnostics.CodeAnalysis;", actualString: updatedText.ToString());
        Assert.Contains(expectedSubstring: "SuppressMessage", actualString: updatedText.ToString());
        Assert.Contains(expectedSubstring: DiagnosticIds.DataDesignDtoMissing, actualString: updatedText.ToString());
    }

    private static async Task<Document> CreateDocumentAsync(string source)
    {
        var workspace = new AdhocWorkspace();
        var project = workspace.AddProject(name: "CodeFixTests", language: LanguageNames.CSharp)
            .WithCompilationOptions(options: new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary));

        project = project.AddMetadataReference(metadataReference: MetadataReference.CreateFromFile(path: typeof(object).Assembly.Location));
        project = project.AddMetadataReference(metadataReference: MetadataReference.CreateFromFile(path: typeof(System.Linq.Enumerable).Assembly.Location));
        project = project.AddMetadataReference(metadataReference: MetadataReference.CreateFromFile(path: typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location));

        return workspace.AddDocument(projectId: project.Id, name: "DtoDesign.cs", text: SourceText.From(text: source));
    }
}
