using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Vc.CodeFixes.Api;
using VisionaryCoder.Analyzers.Abstractions;
using Xunit;

namespace vc.CodeFixes.Tests;

public sealed class ApiNamingCodeFixTests
{
    [Fact]
    public async Task RegisterCodeFixesAsync_ShouldAddSuppressMessageAttributeForNamingViolation()
    {
        var source = """
            namespace Sample.Api
            {
                public sealed class ProductEndpoint
                {
                    public void GetProductById(int id)
                    {
                    }
                }
            }
            """;

        var document = await CreateDocumentAsync(source: source);
        var root = await document.GetSyntaxRootAsync();
        var methodNode = root!
            .DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .First(predicate: node => node.Identifier.Text == "GetProductById");

        var diagnostic = Diagnostic.Create(
            descriptor: new DiagnosticDescriptor(
                id: DiagnosticIds.ApiDesignNaming,
                title: "API naming violation",
                messageFormat: "Method '{0}' does not follow API naming convention.",
                category: "Api",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true),
            location: methodNode.Identifier.GetLocation(),
            messageArgs: "GetProductById");

        var provider = new ApiNamingCodeFix();
        CodeAction? codeAction = null;

        var context = new CodeFixContext(document: document, diagnostic: diagnostic, registerCodeFix: (action, _) => codeAction = action, cancellationToken: CancellationToken.None);
        await provider.RegisterCodeFixesAsync(context: context);

        var operations = await codeAction!.GetOperationsAsync(cancellationToken: CancellationToken.None);
        var applyOperation = Assert.IsType<ApplyChangesOperation>(@object: operations.Single());
        var updatedDocument = applyOperation.ChangedSolution.GetDocument(documentId: document.Id)!;
        var updatedText = await updatedDocument.GetTextAsync();

        Assert.Contains(expectedSubstring: "using System.Diagnostics.CodeAnalysis;", actualString: updatedText.ToString());
        Assert.Contains(expectedSubstring: "SuppressMessage", actualString: updatedText.ToString());
        Assert.Contains(expectedSubstring: DiagnosticIds.ApiDesignNaming, actualString: updatedText.ToString());
    }

    private static async Task<Document> CreateDocumentAsync(string source)
    {
        var workspace = new AdhocWorkspace();
        var project = workspace.AddProject(name: "CodeFixTests", language: LanguageNames.CSharp)
            .WithCompilationOptions(options: new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary));

        project = project.AddMetadataReference(metadataReference: MetadataReference.CreateFromFile(path: typeof(object).Assembly.Location));
        project = project.AddMetadataReference(metadataReference: MetadataReference.CreateFromFile(path: typeof(System.Linq.Enumerable).Assembly.Location));
        project = project.AddMetadataReference(metadataReference: MetadataReference.CreateFromFile(path: typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location));

        return workspace.AddDocument(projectId: project.Id, name: "ApiNaming.cs", text: SourceText.From(text: source));
    }
}
