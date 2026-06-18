using VisionaryCoder.Analyzers.Abstractions;
using VisionaryCoder.Analyzers.Security;
using Xunit;

namespace vc.Analyzers.Tests;

public sealed class MissingAuthorizationAnalyzerTests
{
    [Fact]
    public void SupportedDiagnostics_ShouldContainAuthorizationRule()
    {
        var analyzer = new MissingAuthorizationAnalyzer();
        Assert.Contains(collection: analyzer.SupportedDiagnostics, filter: d => d.Id == DiagnosticIds.SecurityAuthorizationMissing);
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

        var diagnostics = await GetDiagnosticsAsync(source: source);
        Assert.Contains(collection: diagnostics, filter: d => d.Id == DiagnosticIds.SecurityAuthorizationMissing);
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

        var diagnostics = await GetDiagnosticsAsync(source: source);
        Assert.DoesNotContain(collection: diagnostics, filter: d => d.Id == DiagnosticIds.SecurityAuthorizationMissing);
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
        var compilation = CSharpCompilation.Create(assemblyName: "AnalyzerTests", syntaxTrees: [tree], references: references,
            options: new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary));
        return await compilation.WithAnalyzers(analyzers: [new MissingAuthorizationAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}
