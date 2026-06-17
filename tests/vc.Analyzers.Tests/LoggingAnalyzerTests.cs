using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Vc.Analyzers.Design;
using Xunit;

namespace Vc.Analyzers.Tests;

public sealed class LoggingAnalyzerTests
{
    [Fact]
    public void SupportedDiagnostics_ShouldNotBeEmpty()
    {
        var analyzer = new LoggingAnalyzer();
        Assert.NotEmpty(analyzer.SupportedDiagnostics);
    }

    [Fact]
    public async Task CatchBlockWithoutLoggingOrRethrow_ShouldReportDiagnostic()
    {
        var source = """
            namespace SampleApp;

            public class Service
            {
                public void DoWork()
                {
                    try { }
                    catch (System.Exception)
                    {
                        int x = 1;
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);
        Assert.NotEmpty(diagnostics);
    }

    [Fact]
    public async Task CatchBlockWithRethrow_ShouldNotReportDiagnostic()
    {
        var source = """
            namespace SampleApp;

            public class Service
            {
                public void DoWork()
                {
                    try { }
                    catch (System.Exception)
                    {
                        throw;
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);
        Assert.Empty(diagnostics);
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
        return await compilation.WithAnalyzers([new LoggingAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}
