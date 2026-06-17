using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Vc.Analyzers.Api;
using VisionaryCoder.Tooling.Analyzers.Common;
using Xunit;

namespace Vc.Analyzers.Tests;

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

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.Contains(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.ApiDesignControllerMissing);
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

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.DoesNotContain(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.ApiDesignControllerMissing);
    }

    private static async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(string source)
    {
        var tree = CSharpSyntaxTree.ParseText(source);

        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location)
        };

        var compilation = CSharpCompilation.Create(
            "AnalyzerTests",
            [tree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var analyzer = new ApiDesignAnalyzer();
        var diagnostics = await compilation.WithAnalyzers([analyzer]).GetAnalyzerDiagnosticsAsync();

        return diagnostics;
    }
}