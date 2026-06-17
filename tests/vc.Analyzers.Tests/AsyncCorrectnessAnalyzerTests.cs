using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Vc.Analyzers.Core;
using VisionaryCoder.Tooling.Analyzers.Common;
using Xunit;

namespace Vc.Analyzers.Tests;

public sealed class AsyncCorrectnessAnalyzerTests
{
    [Fact]
    public void SupportedDiagnostics_ShouldContainBlockingCallRule()
    {
        var analyzer = new AsyncCorrectnessAnalyzer();
        Assert.Contains(analyzer.SupportedDiagnostics, d => d.Id == DiagnosticIds.AsyncBlockingCall);
    }

    [Fact]
    public void SupportedDiagnostics_ShouldContainFireAndForgetRule()
    {
        var analyzer = new AsyncCorrectnessAnalyzer();
        Assert.Contains(analyzer.SupportedDiagnostics, d => d.Id == DiagnosticIds.AsyncFireAndForget);
    }

    [Fact]
    public async Task BlockingWait_ShouldReportDiagnostic_InAsyncMethod()
    {
        var source = """
            using System.Threading.Tasks;

            namespace SampleApp;

            public class Service
            {
                public async Task BadMethodAsync()
                {
                    var task = Task.FromResult(1);
                    task.Wait();
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);
        Assert.Contains(diagnostics, d => d.Id == DiagnosticIds.AsyncBlockingCall);
    }

    [Fact]
    public async Task AwaitedCall_ShouldNotReportBlockingDiagnostic()
    {
        var source = """
            using System.Threading.Tasks;

            namespace SampleApp;

            public class Service
            {
                public async Task GoodMethodAsync()
                {
                    await Task.FromResult(1);
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);
        Assert.DoesNotContain(diagnostics, d => d.Id == DiagnosticIds.AsyncBlockingCall);
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
        return await compilation.WithAnalyzers([new AsyncCorrectnessAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}
