using VisionaryCoder.Analyzers.Abstractions;
using VisionaryCoder.Analyzers.Architecture;
using Xunit;

namespace vc.Analyzers.Tests;

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

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.Contains(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.ArchCyclicDependency);
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

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.DoesNotContain(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.ArchCyclicDependency);
    }

    private static async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(string source)
    {
        var tree = CSharpSyntaxTree.ParseText(text: source);
        var compilation = CSharpCompilation.Create(
            assemblyName: "AnalyzerTests",
            syntaxTrees: [tree],
            references:
            [
                MetadataReference.CreateFromFile(path: typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(path: typeof(System.Linq.Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(path: typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location)
            ],
            options: new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary));

        return await compilation.WithAnalyzers(analyzers: [new CyclicDependencyAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}