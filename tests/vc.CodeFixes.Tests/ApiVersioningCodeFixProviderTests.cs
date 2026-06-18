using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Vc.CodeFixes.Api;
using VisionaryCoder.Analyzers.Abstractions;
using VisionaryCoder.Analyzers.Api;
using Xunit;

namespace vc.CodeFixes.Tests;

public sealed class ApiVersioningCodeFixProviderTests
{
    [Fact]
    public async Task RegisterCodeFixesAsync_ShouldAddApiVersionAttributeAndUsingDirective()
    {
        var source = """
            namespace SampleApp;

            public class WeatherController
            {
                [HttpGet]
                public string Get() => "ok";
            }

            namespace Microsoft.AspNetCore.Mvc
            {
                public sealed class HttpGetAttribute : System.Attribute { }
                public class ControllerBase { }
            }
            """;

        var document = await CreateDocumentAsync(source: source);
        var diagnostics = await GetAnalyzerDiagnosticsAsync(document: document);
        var diagnostic = Assert.Single(collection: diagnostics.Where(predicate: d => d.Id == DiagnosticIds.ApiVersioningMissing));

        var provider = new ApiVersioningCodeFixProvider();
        CodeAction? codeAction = null;

        var context = new CodeFixContext(document: document, diagnostic: diagnostic, registerCodeFix: (action, _) => codeAction = action, cancellationToken: CancellationToken.None);
        await provider.RegisterCodeFixesAsync(context: context);

        var operations = await codeAction!.GetOperationsAsync(cancellationToken: CancellationToken.None);
        var applyOperation = Assert.IsType<ApplyChangesOperation>(@object: operations.Single());
        var updatedDocument = applyOperation.ChangedSolution.GetDocument(documentId: document.Id)!;
        var updatedText = await updatedDocument.GetTextAsync();

        Assert.Contains(expectedSubstring: "using Microsoft.AspNetCore.Mvc;", actualString: updatedText.ToString());
        Assert.Contains(expectedSubstring: "[ApiVersion(\"1.0\")]", actualString: updatedText.ToString());
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
        return await compilation!.WithAnalyzers(analyzers: [new ApiVersioningAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}