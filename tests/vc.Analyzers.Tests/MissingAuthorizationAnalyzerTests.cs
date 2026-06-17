using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Vc.Analyzers.Security;
using VisionaryCoder.Tooling.Analyzers.Common;
using Xunit;

namespace Vc.Analyzers.Tests;

public sealed class MissingAuthorizationAnalyzerTests
{
    [Fact]
    public void SupportedDiagnostics_ShouldContainAuthorizationRule()
    {
        var analyzer = new MissingAuthorizationAnalyzer();
        Assert.Contains(analyzer.SupportedDiagnostics, d => d.Id == DiagnosticIds.SecurityAuthorizationMissing);
    }

    [Fact]
    public async Task HttpGetMethod_WithoutAuthorize_ShouldReportDiagnostic()
    {
        var source = """
            namespace SampleApp;

            public class UserController
            {
                [HttpGet]
                public string GetUser() => "user";
            }

            public sealed class HttpGetAttribute : System.Attribute { }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);
        Assert.Contains(diagnostics, d => d.Id == DiagnosticIds.SecurityAuthorizationMissing);
    }

    [Fact]
    public async Task HttpGetMethod_WithAuthorize_ShouldNotReportDiagnostic()
    {
        var source = """
            namespace SampleApp;

            public class UserController
            {
                [HttpGet]
                [Authorize]
                public string GetUser() => "user";
            }

            public sealed class HttpGetAttribute : System.Attribute { }
            public sealed class AuthorizeAttribute : System.Attribute { }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);
        Assert.DoesNotContain(diagnostics, d => d.Id == DiagnosticIds.SecurityAuthorizationMissing);
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
        var compilation = CSharpCompilation.Create("AnalyzerTests", [tree], references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        return await compilation.WithAnalyzers([new MissingAuthorizationAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}
