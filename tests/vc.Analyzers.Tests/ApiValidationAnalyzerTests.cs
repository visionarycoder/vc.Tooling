using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Vc.Analyzers.Api;
using VisionaryCoder.Tooling.Analyzers.Common;
using Xunit;

namespace Vc.Analyzers.Tests;

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

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.Contains(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.ApiValidationMissing);
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

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.DoesNotContain(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.ApiValidationMissing);
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

        return await compilation.WithAnalyzers([new ApiValidationAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}