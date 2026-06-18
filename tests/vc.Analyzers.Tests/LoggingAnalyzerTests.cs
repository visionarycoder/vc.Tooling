using VisionaryCoder.Analyzers.Design;
using Xunit;

namespace vc.Analyzers.Tests;

public sealed class LoggingAnalyzerTests
{
    [Fact]
    public void SupportedDiagnostics_ShouldNotBeEmpty()
    {
        var analyzer = new LoggingAnalyzer();
        Assert.NotEmpty(collection: analyzer.SupportedDiagnostics);
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

        var diagnostics = await GetDiagnosticsAsync(source: source);
        Assert.NotEmpty(collection: diagnostics);
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

        var diagnostics = await GetDiagnosticsAsync(source: source);
        Assert.Empty(collection: diagnostics);
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
        return await compilation.WithAnalyzers(analyzers: [new LoggingAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}
