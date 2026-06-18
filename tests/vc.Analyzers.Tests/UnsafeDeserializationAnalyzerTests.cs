using VisionaryCoder.Analyzers.Abstractions;
using VisionaryCoder.Analyzers.Security;
using Xunit;

namespace vc.Analyzers.Tests;

public sealed class UnsafeDeserializationAnalyzerTests
{
    [Fact]
    public void SupportedDiagnostics_ShouldContainUnsafeDeserializationRule()
    {
        var analyzer = new UnsafeDeserializationAnalyzer();
        Assert.Contains(collection: analyzer.SupportedDiagnostics, filter: d => d.Id == DiagnosticIds.SecurityUnsafeDeserialization);
    }

    [Fact]
    public async Task AnalyzerRuns_WithoutException()
    {
        var diagnostics = await GetDiagnosticsAsync(source: "namespace SampleApp; public class Sample {}");
        Assert.False(condition: diagnostics.IsDefault);
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
        return await compilation.WithAnalyzers(analyzers: [new UnsafeDeserializationAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}
