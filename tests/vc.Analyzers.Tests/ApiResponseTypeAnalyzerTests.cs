using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Vc.Analyzers.Api;
using VisionaryCoder.Tooling.Analyzers.Common;
using Xunit;

namespace Vc.Analyzers.Tests;

public sealed class ApiResponseTypeAnalyzerTests
{
    [Fact]
    public async Task ApiResponseTypeRule_ShouldReportDiagnostic_WhenActionLacksResponseMetadata()
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

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.Contains(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.ApiResponseSerializationIssue);
    }

    [Fact]
    public async Task ApiResponseTypeRule_ShouldNotReportDiagnostic_WhenResponseMetadataExists()
    {
        var source = """
            using Microsoft.AspNetCore.Mvc;

            namespace SampleApp;

            public class WeatherController : ControllerBase
            {
                [HttpGet]
                [ProducesResponseType(typeof(string), 200)]
                public string Get() => "ok";
            }

            namespace Microsoft.AspNetCore.Mvc
            {
                public class ControllerBase { }
                public sealed class HttpGetAttribute : System.Attribute { }
                public sealed class ProducesResponseTypeAttribute : System.Attribute
                {
                    public ProducesResponseTypeAttribute(System.Type type, int statusCode) { }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.DoesNotContain(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.ApiResponseSerializationIssue);
    }

    private static async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(string source)
    {
        var tree = CSharpSyntaxTree.ParseText(source);
        var compilation = CSharpCompilation.Create(
            "AnalyzerTests",
            [tree],
            [
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location)
            ],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        return await compilation.WithAnalyzers([new ApiResponseTypeAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}