using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Vc.Analyzers.Design.Vbd;
using VisionaryCoder.Tooling.Analyzers.Common;
using Xunit;

namespace Vc.Analyzers.Tests;

public sealed class VbdAccessAnalyzerTests
{
    [Fact]
    public async Task VbdAccessAnalyzer_ShouldReportBusinessLogicLeakage_WhenRepositoryContainsComputeMethod()
    {
        var source = """
            namespace SampleApp;

            public sealed class OrderRepository
            {
                public int ComputeRiskScore() => 42;
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.Contains(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.VbdAccessVaultBusinessLogicLeakage);
    }

    [Fact]
    public async Task VbdAccessAnalyzer_ShouldNotReportBusinessLogicLeakage_WhenRepositoryHasOnlyDataAccessMethods()
    {
        var source = """
            namespace SampleApp;

            public sealed class OrderRepository
            {
                public string GetById(int id) => id.ToString();
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.DoesNotContain(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.VbdAccessVaultBusinessLogicLeakage);
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

        return await compilation.WithAnalyzers([new VbdAccessAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}