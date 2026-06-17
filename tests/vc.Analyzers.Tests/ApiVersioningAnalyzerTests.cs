using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Vc.Analyzers.Api;
using VisionaryCoder.Tooling.Analyzers.Common;
using Xunit;

namespace Vc.Analyzers.Tests;

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

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.Contains(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.ApiVersioningMissing);
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

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.DoesNotContain(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.ApiVersioningMissing);
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

        return await compilation.WithAnalyzers([new ApiVersioningAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}