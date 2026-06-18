using VisionaryCoder.Analyzers.Abstractions;
using VisionaryCoder.Analyzers.Design.Vbd;
using Xunit;

namespace vc.Analyzers.Tests;

public sealed class VbdManagerAnalyzerTests
{
    [Fact]
    public async Task VbdManagerAnalyzer_ShouldReportUnstableContractDiagnostic_WhenMethodReturnsDto()
    {
        var source = """
            namespace SampleApp;

            public sealed class OrderManager
            {
                public OrderDto BuildOrder() => new();

                public void Orchestrate()
                {
                    BuildOrder();
                }
            }

            public sealed class OrderDto
            {
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.Contains(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.VbdManagerVaultUnstableContract);
    }

    [Fact]
    public async Task VbdManagerAnalyzer_ShouldNotReportUnstableContractDiagnostic_WhenMethodReturnsStableContract()
    {
        var source = """
            namespace SampleApp;

            public sealed class OrderManager
            {
                public IOrderContract BuildOrder() => new StableOrderContract();

                public void Orchestrate()
                {
                    BuildOrder();
                }
            }

            public interface IOrderContract
            {
            }

            public sealed class StableOrderContract : IOrderContract
            {
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.DoesNotContain(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.VbdManagerVaultUnstableContract);
    }

    [Fact]
    public async Task VbdManagerAnalyzer_ShouldReportFeatureSpecificLogicDiagnostic_WhenMethodNameContainsFeatureMarker()
    {
        var source = """
            namespace SampleApp;

            public sealed class OrderManager
            {
                public void ExecuteFeatureV2()
                {
                }

                public void Orchestrate()
                {
                    ExecuteFeatureV2();
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.Contains(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.VbdManagerVaultFeatureSpecificLogic);
    }

    [Fact]
    public async Task VbdManagerAnalyzer_ShouldNotReportFeatureSpecificLogicDiagnostic_WhenMethodNameIsGeneric()
    {
        var source = """
            namespace SampleApp;

            public sealed class OrderManager
            {
                public void Execute()
                {
                }

                public void Orchestrate()
                {
                    Execute();
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.DoesNotContain(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.VbdManagerVaultFeatureSpecificLogic);
    }

    [Fact]
    public async Task VbdManagerAnalyzer_ShouldReportMissingOrchestrationDiagnostic_WhenManagerHasNoInvocationCalls()
    {
        var source = """
            namespace SampleApp;

            public sealed class OrderManager
            {
                public void Handle()
                {
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.Contains(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.VbdManagerVaultMissingOrchestration);
    }

    [Fact]
    public async Task VbdManagerAnalyzer_ShouldNotReportMissingOrchestrationDiagnostic_WhenInvocationExists()
    {
        var source = """
            namespace SampleApp;

            public sealed class OrderManager
            {
                public void Handle()
                {
                    Compute();
                }

                private void Compute()
                {
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.DoesNotContain(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.VbdManagerVaultMissingOrchestration);
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

        return await compilation.WithAnalyzers(analyzers: [new VbdManagerAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}