using VisionaryCoder.Analyzers.Abstractions;
using VisionaryCoder.Analyzers.Api;
using Xunit;

namespace vc.Analyzers.Tests;

public sealed class ApiVersioningAnalyzerTests
{
    [Fact]
    public async Task ApiVersioningRule_ShouldReportDiagnostic_WhenControllerHasActionsWithoutVersion()
    {
        var source = """
            using Microsoft.AspNetCore.Mvc;

            namespace SampleApp;

            public class WeatherController : ControllerBase
            {
                [HttpGet]
                public string Get() => "ok";
            }

            namespace Microsoft.AspNetCore.Mvc
            {
                public class ControllerBase { }
                public sealed class HttpGetAttribute : System.Attribute { }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.Contains(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.ApiVersioningMissing);
    }

    [Fact]
    public async Task ApiVersioningRule_ShouldNotReportDiagnostic_WhenControllerHasVersionAttribute()
    {
        var source = """
            using Microsoft.AspNetCore.Mvc;

            namespace SampleApp;

            [ApiVersion("1.0")]
            public class WeatherController : ControllerBase
            {
                [HttpGet]
                public string Get() => "ok";
            }

            namespace Microsoft.AspNetCore.Mvc
            {
                public class ControllerBase { }
                public sealed class HttpGetAttribute : System.Attribute { }
                public sealed class ApiVersionAttribute : System.Attribute
                {
                    public ApiVersionAttribute(string version) { }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.DoesNotContain(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.ApiVersioningMissing);
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

        return await compilation.WithAnalyzers(analyzers: [new ApiVersioningAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}