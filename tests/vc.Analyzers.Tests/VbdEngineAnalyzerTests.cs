using VisionaryCoder.Analyzers.Abstractions;
using VisionaryCoder.Analyzers.Design.Vbd;
using Xunit;

namespace vc.Analyzers.Tests;

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

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.Contains(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.VbdEngineVaultInfrastructureAccess);
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

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.DoesNotContain(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.VbdEngineVaultInfrastructureAccess);
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

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.Contains(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.VbdEngineVaultNondeterminism);
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

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.DoesNotContain(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.VbdEngineVaultNondeterminism);
    }

    [Fact]
    public async Task VbdEngineAnalyzer_ShouldReportStateViolationDiagnostic_WhenEngineContainsMutableField()
    {
        var source = """
            namespace SampleApp;

            public sealed class RiskEngine
            {
                private int _count = 0;
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.Contains(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.VbdEngineVaultStateViolation);
    }

    [Fact]
    public async Task VbdEngineAnalyzer_ShouldNotReportStateViolationDiagnostic_WhenFieldIsReadonly()
    {
        var source = """
            namespace SampleApp;

            public sealed class RiskEngine
            {
                private readonly int _count = 0;
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.DoesNotContain(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.VbdEngineVaultStateViolation);
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
                MetadataReference.CreateFromFile(path: typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(path: typeof(System.Net.Http.HttpClient).Assembly.Location)
            ],
            options: new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary));

        return await compilation.WithAnalyzers(analyzers: [new VbdEngineAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}