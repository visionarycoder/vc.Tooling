using VisionaryCoder.Analyzers.Abstractions;
using VisionaryCoder.Analyzers.Core;
using Xunit;

namespace vc.Analyzers.Tests;

public sealed class NamingConventionAnalyzerTests
{
    [Fact]
    public void SupportedDiagnostics_ShouldContainNamingRule()
    {
        var analyzer = new NamingConventionAnalyzer();
        Assert.Contains(collection: analyzer.SupportedDiagnostics, filter: d => d.Id == DiagnosticIds.NamingConventionViolation);
    }

    [Fact]
    public async Task PrivateField_NotStartingWithUnderscore_ShouldReportDiagnostic()
    {
        var source = """
            namespace SampleApp;

            public class Service
            {
                private int count = 0;
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);
        Assert.Contains(collection: diagnostics, filter: d => d.Id == DiagnosticIds.NamingConventionViolation);
    }

    [Fact]
    public async Task PrivateField_StartingWithUnderscore_ShouldNotReportDiagnostic()
    {
        var source = """
            namespace SampleApp;

            public class Service
            {
                private int _count = 0;
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);
        Assert.DoesNotContain(collection: diagnostics, filter: d => d.Id == DiagnosticIds.NamingConventionViolation);
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
        return await compilation.WithAnalyzers(analyzers: [new NamingConventionAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}
