using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Vc.Analyzers.Architecture;
using VisionaryCoder.Tooling.Analyzers.Common;
using Xunit;

namespace Vc.Analyzers.Tests;

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

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.Contains(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.ArchLayeringViolation);
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

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.DoesNotContain(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.ArchLayeringViolation);
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

        return await compilation.WithAnalyzers([new LayeringAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}