using VisionaryCoder.Analyzers.Abstractions;
using VisionaryCoder.Analyzers.Architecture;
using Xunit;

namespace vc.Analyzers.Tests;

public sealed class LayeringAnalyzerTests
{
    [Fact]
    public async Task LayeringAnalyzer_ShouldReportDiagnostic_WhenDomainDependsOnInfrastructure()
    {
        var source = """
            namespace Sample.Infrastructure
            {
                public sealed class SqlRepository
                {
                }
            }

            namespace Sample.Domain
            {
                public sealed class Order
                {
                    private readonly Sample.Infrastructure.SqlRepository _repo = new Sample.Infrastructure.SqlRepository();
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.Contains(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.ArchLayeringViolation);
    }

    [Fact]
    public async Task LayeringAnalyzer_ShouldNotReportDiagnostic_WhenApiDependsOnApplication()
    {
        var source = """
            namespace Sample.Application
            {
                public sealed class Service
                {
                }
            }

            namespace Sample.Api
            {
                public sealed class Controller
                {
                    private readonly Sample.Application.Service _service = new Sample.Application.Service();
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.DoesNotContain(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.ArchLayeringViolation);
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

        return await compilation.WithAnalyzers(analyzers: [new LayeringAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}