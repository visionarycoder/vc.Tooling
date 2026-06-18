using VisionaryCoder.Analyzers.Abstractions;
using VisionaryCoder.Analyzers.Api;
using Xunit;

namespace vc.Analyzers.Tests;

public sealed class ApiDesignAnalyzerTests
{
    [Fact]
    public async Task MissingApiControllerRule_ShouldReportDiagnostic_ForControllerWithoutAttribute()
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
                public sealed class ApiControllerAttribute : System.Attribute { }
                public sealed class HttpGetAttribute : System.Attribute { }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.Contains(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.ApiDesignControllerMissing);
    }

    [Fact]
    public async Task MissingApiControllerRule_ShouldNotReportDiagnostic_WhenAttributeExists()
    {
        var source = """
            using Microsoft.AspNetCore.Mvc;

            namespace SampleApp;

            [ApiController]
            public class WeatherController : ControllerBase
            {
                [HttpGet]
                public string Get() => "ok";
            }

            namespace Microsoft.AspNetCore.Mvc
            {
                public class ControllerBase { }
                public sealed class ApiControllerAttribute : System.Attribute { }
                public sealed class HttpGetAttribute : System.Attribute { }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.DoesNotContain(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.ApiDesignControllerMissing);
    }

    private static async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(string source)
    {
        var tree = CSharpSyntaxTree.ParseText(text: source);

        var references = new[]
        {
            MetadataReference.CreateFromFile(path: typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(path: typeof(System.Linq.Enumerable).Assembly.Location),
            MetadataReference.CreateFromFile(path: typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location)
        };

        var compilation = CSharpCompilation.Create(
            assemblyName: "AnalyzerTests",
            syntaxTrees: [tree],
            references: references,
            options: new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary));

        var analyzer = new ApiDesignAnalyzer();
        var diagnostics = await compilation.WithAnalyzers(analyzers: [analyzer]).GetAnalyzerDiagnosticsAsync();

        return diagnostics;
    }
}