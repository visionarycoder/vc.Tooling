using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Vc.Analyzers.Design.Vbd;
using VisionaryCoder.Tooling.Analyzers.Common;
using Xunit;

namespace Vc.Analyzers.Tests;

public sealed class VbdEngineAnalyzerTests
{
    [Fact]
    public async Task VbdEngineAnalyzer_ShouldReportInfrastructureAccessDiagnostic_WhenEngineReferencesHttpClient()
    {
        var source = """
            namespace SampleApp;

            public sealed class RiskEngine
            {
                private readonly System.Net.Http.HttpClient _client = new();
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.Contains(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.VbdEngineVaultInfrastructureAccess);
    }

    [Fact]
    public async Task VbdEngineAnalyzer_ShouldNotReportInfrastructureAccessDiagnostic_WhenEngineUsesPureTypes()
    {
        var source = """
            namespace SampleApp;

            public sealed class RiskEngine
            {
                private readonly int _threshold = 5;
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.DoesNotContain(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.VbdEngineVaultInfrastructureAccess);
    }

    [Fact]
    public async Task VbdEngineAnalyzer_ShouldReportNondeterminismDiagnostic_WhenEngineUsesGuidNewGuid()
    {
        var source = """
            namespace SampleApp;

            public sealed class RiskEngine
            {
                public string ComputeId() => System.Guid.NewGuid().ToString();
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.Contains(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.VbdEngineVaultNondeterminism);
    }

    [Fact]
    public async Task VbdEngineAnalyzer_ShouldNotReportNondeterminismDiagnostic_WhenMethodIsDeterministic()
    {
        var source = """
            namespace SampleApp;

            public sealed class RiskEngine
            {
                public int ComputeFixedValue() => 42;
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.DoesNotContain(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.VbdEngineVaultNondeterminism);
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
                MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Net.Http.HttpClient).Assembly.Location)
            ],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        return await compilation.WithAnalyzers([new VbdEngineAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}