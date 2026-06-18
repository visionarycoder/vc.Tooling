using VisionaryCoder.Analyzers.Abstractions;
using VisionaryCoder.Analyzers.Api;
using Xunit;

namespace vc.Analyzers.Tests;

public sealed class ApiValidationAnalyzerTests
{
    [Fact]
    public async Task ApiValidationRule_ShouldReportDiagnostic_WhenComplexInputLacksValidation()
    {
        var source = """
            using Microsoft.AspNetCore.Mvc;

            namespace SampleApp;

            public class WeatherController : ControllerBase
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
                public class ControllerBase { }
                public sealed class HttpPostAttribute : System.Attribute { }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.Contains(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.ApiValidationMissing);
    }

    [Fact]
    public async Task ApiValidationRule_ShouldNotReportDiagnostic_WhenInputIsValidated()
    {
        var source = """
            using Microsoft.AspNetCore.Mvc;
            using System.ComponentModel.DataAnnotations;

            namespace SampleApp;

            public class WeatherController : ControllerBase
            {
                [HttpPost]
                public string Post([Required] OrderRequest request) => "ok";
            }

            public sealed class OrderRequest
            {
                public string Id { get; set; } = string.Empty;
            }

            namespace Microsoft.AspNetCore.Mvc
            {
                public class ControllerBase { }
                public sealed class HttpPostAttribute : System.Attribute { }
            }

            namespace System.ComponentModel.DataAnnotations
            {
                public sealed class RequiredAttribute : System.Attribute { }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.DoesNotContain(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.ApiValidationMissing);
    }

    private static async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(string source)
    {
        var tree = CSharpSyntaxTree.ParseText(text: source);
        var compilation = CSharpCompilation.Create(
            assemblyName: "AnalyzerTests",
            syntaxTrees: [tree],
            references:
            [
                MetadataReference.CreateFromFile(path: typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(path: typeof(System.Linq.Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(path: typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location)
            ],
            options: new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary));

        return await compilation.WithAnalyzers(analyzers: [new ApiValidationAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}