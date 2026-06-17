using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Vc.Analyzers.Distributed;
using VisionaryCoder.Tooling.Analyzers.Common;
using Xunit;

namespace Vc.Analyzers.Tests;

public sealed class RepositoryAnalyzerTests
{
    [Fact]
    public async Task RepositoryAnalyzer_ShouldReportDiagnostic_WhenRepositoryMethodIsSynchronous()
    {
        var source = """
            namespace Sample.Data
            {
                public sealed class OrderRepository
                {
                    public int GetCount()
                    {
                        return 1;
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.Contains(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.DistributedRepositoryAsyncMissing);
    }

    [Fact]
    public async Task RepositoryAnalyzer_ShouldNotReportDiagnostic_WhenRepositoryMethodReturnsTask()
    {
        var source = """
            using System.Threading.Tasks;

            namespace Sample.Data
            {
                public sealed class OrderRepository
                {
                    public Task<int> GetCountAsync()
                    {
                        return Task.FromResult(1);
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.DoesNotContain(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.DistributedRepositoryAsyncMissing);
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
                MetadataReference.CreateFromFile(typeof(System.Threading.Tasks.Task).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location)
            ],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        return await compilation.WithAnalyzers([new RepositoryAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}