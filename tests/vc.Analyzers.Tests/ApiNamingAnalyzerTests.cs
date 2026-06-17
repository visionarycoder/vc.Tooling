using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Vc.Analyzers.Api;
using VisionaryCoder.Tooling.Analyzers.Common;
using Xunit;

namespace Vc.Analyzers.Tests;

public sealed class ApiNamingAnalyzerTests
{
    [Fact]
    public async Task ApiDesignAnalyzer_ShouldReportDiagnostic_ForNonRestfulActionName()
    {
        var source = """
            using Microsoft.AspNetCore.Mvc;

            namespace SampleApp;

            [ApiController]
            public class WeatherController : ControllerBase
            {
                [HttpGet]
                public string FetchWeather() => "ok";
            }

            namespace Microsoft.AspNetCore.Mvc
            {
                public class ControllerBase { }
                public sealed class ApiControllerAttribute : System.Attribute { }
                public sealed class HttpGetAttribute : System.Attribute { }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.Contains(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.ApiDesignNaming);
    }

    [Fact]
    public async Task ApiDesignAnalyzer_ShouldNotReportDiagnostic_ForRestfulActionName()
    {
        var source = """
            using Microsoft.AspNetCore.Mvc;

            namespace SampleApp;

            [ApiController]
            public class WeatherController : ControllerBase
            {
                [HttpGet]
                public string GetWeather() => "ok";
            }

            namespace Microsoft.AspNetCore.Mvc
            {
                public class ControllerBase { }
                public sealed class ApiControllerAttribute : System.Attribute { }
                public sealed class HttpGetAttribute : System.Attribute { }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.DoesNotContain(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.ApiDesignNaming);
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

        return await compilation.WithAnalyzers([new ApiDesignAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}