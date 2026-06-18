using VisionaryCoder.Analyzers.Abstractions;
using VisionaryCoder.Analyzers.Architecture;
using Xunit;

namespace vc.Analyzers.Tests;

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

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.Contains(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.ArchNamespaceBoundaryViolation);
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

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.DoesNotContain(
            collection: diagnostics,
            filter: diagnostic => diagnostic.Id == DiagnosticIds.ArchNamespaceBoundaryViolation
                                  && diagnostic.GetMessage().Contains(value: "CompanyA.Services", comparisonType: System.StringComparison.Ordinal)
                                  && diagnostic.GetMessage().Contains(value: "CompanyA.Domain", comparisonType: System.StringComparison.Ordinal));
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

        return await compilation.WithAnalyzers(analyzers: [new NamespaceBoundaryAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}