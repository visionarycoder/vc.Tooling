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

        var document = await CreateDocumentAsync(source);
        var diagnostics = await GetAnalyzerDiagnosticsAsync(document);
        var diagnostic = Assert.Single(diagnostics.Where(d => d.Id == DiagnosticIds.ApiValidationMissing));

        var provider = new ApiValidationCodeFixProvider();
        CodeAction? codeAction = null;

        var context = new CodeFixContext(document, diagnostic, (action, _) => codeAction = action, CancellationToken.None);
        await provider.RegisterCodeFixesAsync(context);

        var operations = await codeAction!.GetOperationsAsync(CancellationToken.None);
        var applyOperation = Assert.IsType<ApplyChangesOperation>(operations.Single());
        var updatedDocument = applyOperation.ChangedSolution.GetDocument(document.Id)!;
        var updatedText = await updatedDocument.GetTextAsync();

        Assert.Contains("using System.ComponentModel.DataAnnotations;", updatedText.ToString());
        Assert.Contains("[Required]", updatedText.ToString());
        Assert.Contains("OrderRequest request", updatedText.ToString());
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
        var analyzerDiagnostics = await compilation!.WithAnalyzers([new ApiValidationAnalyzer()]).GetAnalyzerDiagnosticsAsync();
        return analyzerDiagnostics;
    }
}