using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Vc.Analyzers.Resilience;
using VisionaryCoder.Tooling.Analyzers.Common;
using Xunit;

namespace Vc.Analyzers.Tests;

public sealed class TimeoutAnalyzerTests
{
    [Fact]
    public void SupportedDiagnostics_ShouldContainTimeoutRule()
    {
        var analyzer = new TimeoutAnalyzer();
        Assert.Contains(analyzer.SupportedDiagnostics, d => d.Id == DiagnosticIds.ResilienceTimeoutMissing);
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
        return await compilation.WithAnalyzers([new TimeoutAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}
