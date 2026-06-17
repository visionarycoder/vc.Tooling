using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Vc.Analyzers.Design.Vbd;
using VisionaryCoder.Tooling.Analyzers.Common;
using Xunit;

namespace Vc.Analyzers.Tests;

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

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.Contains(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.VbdManagerVaultUnstableContract);
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

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.DoesNotContain(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.VbdManagerVaultUnstableContract);
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

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.Contains(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.VbdManagerVaultFeatureSpecificLogic);
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

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.DoesNotContain(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.VbdManagerVaultFeatureSpecificLogic);
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

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.Contains(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.VbdManagerVaultMissingOrchestration);
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

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.DoesNotContain(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.VbdManagerVaultMissingOrchestration);
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

        return await compilation.WithAnalyzers([new VbdManagerAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}