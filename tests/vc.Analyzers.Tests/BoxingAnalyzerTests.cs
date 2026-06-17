using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Vc.Analyzers.Performance;
using VisionaryCoder.Tooling.Analyzers.Common;
using Xunit;

namespace Vc.Analyzers.Tests;

public sealed class BoxingAnalyzerTests
{
    [Fact]
    public void SupportedDiagnostics_ShouldContainBoxingRule()
    {
        var analyzer = new BoxingAnalyzer();
        Assert.Contains(analyzer.SupportedDiagnostics, d => d.Id == DiagnosticIds.PerfBoxing);
    }

    [Fact]
    public async Task AnalyzerRuns_WithoutException()
    {
        var diagnostics = await GetDiagnosticsAsync("namespace SampleApp; public class Sample {}");
        Assert.NotNull(diagnostics);
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
        return await compilation.WithAnalyzers([new BoxingAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}
