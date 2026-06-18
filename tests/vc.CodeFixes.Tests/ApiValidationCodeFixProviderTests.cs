using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Vc.CodeFixes.Api;
using VisionaryCoder.Analyzers.Abstractions;
using VisionaryCoder.Analyzers.Api;
using Xunit;

namespace vc.CodeFixes.Tests;

public sealed class ApiValidationCodeFixProviderTests
{
    [Fact]
    public async Task RegisterCodeFixesAsync_ShouldAddRequiredAttributeAndUsingDirective()
    {
        var source = """
            namespace SampleApp;

            public class WeatherController
            {
                [HttpPost]
                public string Post(OrderRequest request) => "ok";
            }

            public sealed class OrderRequest
            {
                public string Id { get; set; } = string.Empty;
            }

            namespace Microsoft.AspNetCore.Mvc
            {
                public sealed class HttpPostAttribute : System.Attribute { }
            }
            """;

        var document = await CreateDocumentAsync(source: source);
        var diagnostics = await GetAnalyzerDiagnosticsAsync(document: document);
        var diagnostic = Assert.Single(collection: diagnostics.Where(predicate: d => d.Id == DiagnosticIds.ApiValidationMissing));

        var provider = new ApiValidationCodeFixProvider();
        CodeAction? codeAction = null;

        var context = new CodeFixContext(document: document, diagnostic: diagnostic, registerCodeFix: (action, _) => codeAction = action, cancellationToken: CancellationToken.None);
        await provider.RegisterCodeFixesAsync(context: context);

        var operations = await codeAction!.GetOperationsAsync(cancellationToken: CancellationToken.None);
        var applyOperation = Assert.IsType<ApplyChangesOperation>(@object: operations.Single());
        var updatedDocument = applyOperation.ChangedSolution.GetDocument(documentId: document.Id)!;
        var updatedText = await updatedDocument.GetTextAsync();

        Assert.Contains(expectedSubstring: "using System.ComponentModel.DataAnnotations;", actualString: updatedText.ToString());
        Assert.Contains(expectedSubstring: "[Required]", actualString: updatedText.ToString());
        Assert.Contains(expectedSubstring: "OrderRequest request", actualString: updatedText.ToString());
    }

    private static async Task<Document> CreateDocumentAsync(string source)
    {
        var workspace = new AdhocWorkspace();
        var project = workspace.AddProject(name: "CodeFixTests", language: LanguageNames.CSharp)
            .WithCompilationOptions(options: new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary));

        project = project.AddMetadataReference(metadataReference: MetadataReference.CreateFromFile(path: typeof(object).Assembly.Location));
        project = project.AddMetadataReference(metadataReference: MetadataReference.CreateFromFile(path: typeof(System.Linq.Enumerable).Assembly.Location));
        project = project.AddMetadataReference(metadataReference: MetadataReference.CreateFromFile(path: typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location));

        return workspace.AddDocument(projectId: project.Id, name: "WeatherController.cs", text: SourceText.From(text: source));
    }

    private static async Task<IReadOnlyList<Diagnostic>> GetAnalyzerDiagnosticsAsync(Document document)
    {
        var compilation = await document.Project.GetCompilationAsync();
        var analyzerDiagnostics = await compilation!.WithAnalyzers(analyzers: [new ApiValidationAnalyzer()]).GetAnalyzerDiagnosticsAsync();
        return analyzerDiagnostics;
    }
}