using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Vc.Analyzers.Api;
using Vc.CodeFixes.Api;
using VisionaryCoder.Tooling.Analyzers.Common;
using Xunit;

namespace Vc.CodeFixes.Tests;

public sealed class ApiResponseTypeCodeFixProviderTests
{
    [Fact]
    public async Task RegisterCodeFixesAsync_ShouldAddProducesResponseTypeAttributeAndUsingDirective()
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
                public sealed class ProducesResponseTypeAttribute : System.Attribute
                {
                    public ProducesResponseTypeAttribute(System.Type type, int statusCode) { }
                }
            }
            """;

        var document = await CreateDocumentAsync(source);
        var diagnostics = await GetAnalyzerDiagnosticsAsync(document);
        var diagnostic = Assert.Single(diagnostics.Where(d => d.Id == DiagnosticIds.ApiResponseSerializationIssue));

        var provider = new ApiResponseTypeCodeFixProvider();
        CodeAction? codeAction = null;

        var context = new CodeFixContext(document, diagnostic, (action, _) => codeAction = action, CancellationToken.None);
        await provider.RegisterCodeFixesAsync(context);

        var operations = await codeAction!.GetOperationsAsync(CancellationToken.None);
        var applyOperation = Assert.IsType<ApplyChangesOperation>(operations.Single());
        var updatedDocument = applyOperation.ChangedSolution.GetDocument(document.Id)!;
        var updatedText = await updatedDocument.GetTextAsync();

        Assert.Contains("using Microsoft.AspNetCore.Mvc;", updatedText.ToString());
        Assert.Contains("[ProducesResponseType(typeof(string), 200)]", updatedText.ToString());
    }

    private static async Task<Document> CreateDocumentAsync(string source)
    {
        var workspace = new AdhocWorkspace();
        var project = workspace.AddProject("CodeFixTests", LanguageNames.CSharp)
            .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        project = project.AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
        project = project.AddMetadataReference(MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location));
        project = project.AddMetadataReference(MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location));

        return workspace.AddDocument(project.Id, "WeatherController.cs", SourceText.From(source));
    }

    private static async Task<IReadOnlyList<Diagnostic>> GetAnalyzerDiagnosticsAsync(Document document)
    {
        var compilation = await document.Project.GetCompilationAsync();
        return await compilation!.WithAnalyzers([new ApiResponseTypeAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}