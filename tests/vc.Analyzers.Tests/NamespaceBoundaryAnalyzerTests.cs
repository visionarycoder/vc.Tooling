using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Vc.Analyzers.Architecture;
using VisionaryCoder.Tooling.Analyzers.Common;
using Xunit;

namespace Vc.Analyzers.Tests;

public sealed class NamespaceBoundaryAnalyzerTests
{
    [Fact]
    public async Task NamespaceBoundaryAnalyzer_ShouldReportDiagnostic_WhenTypeReferencesDifferentRootNamespace()
    {
        var source = """
            namespace CompanyA.Domain
            {
                public sealed class ExternalType
                {
                }
            }

            namespace CompanyB.Services
            {
                public sealed class Consumer
                {
                    private readonly CompanyA.Domain.ExternalType _dependency = new CompanyA.Domain.ExternalType();
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.Contains(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.ArchNamespaceBoundaryViolation);
    }

    [Fact]
    public async Task NamespaceBoundaryAnalyzer_ShouldNotReportDiagnostic_WhenTypeReferencesSameRootNamespace()
    {
        var source = """
            namespace CompanyA.Domain
            {
                public sealed class LocalType
                {
                }
            }

            namespace CompanyA.Services
            {
                public sealed class Consumer
                {
                    private readonly CompanyA.Domain.LocalType _dependency = new CompanyA.Domain.LocalType();
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.DoesNotContain(
            diagnostics,
            diagnostic => diagnostic.Id == DiagnosticIds.ArchNamespaceBoundaryViolation
                && diagnostic.GetMessage().Contains("CompanyA.Services", System.StringComparison.Ordinal)
                && diagnostic.GetMessage().Contains("CompanyA.Domain", System.StringComparison.Ordinal));
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

        return await compilation.WithAnalyzers([new NamespaceBoundaryAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}