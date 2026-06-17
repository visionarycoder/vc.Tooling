using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Vc.Analyzers.Architecture;
using VisionaryCoder.Tooling.Analyzers.Common;
using Xunit;

namespace Vc.Analyzers.Tests;

public sealed class CyclicDependencyAnalyzerTests
{
    [Fact]
    public async Task CyclicDependencyAnalyzer_ShouldReportDiagnostic_WhenNamespacesFormCycle()
    {
        var source = """
            namespace Sample.B
            {
                public sealed class BType
                {
                }
            }

            namespace Sample.A
            {
                public sealed class AType
                {
                    private readonly Sample.B.BType _field = new Sample.B.BType();
                }
            }

            namespace Sample.B
            {
                public sealed class BConsumer
                {
                    private readonly Sample.A.AType _field = new Sample.A.AType();
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.Contains(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.ArchCyclicDependency);
    }

    [Fact]
    public async Task CyclicDependencyAnalyzer_ShouldNotReportDiagnostic_WhenNoCycleExists()
    {
        var source = """
            namespace Sample.B
            {
                public sealed class BType
                {
                }
            }

            namespace Sample.A
            {
                public sealed class AType
                {
                    private readonly Sample.B.BType _field = new Sample.B.BType();
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.DoesNotContain(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.ArchCyclicDependency);
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
                MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location)
            ],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        return await compilation.WithAnalyzers([new CyclicDependencyAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}