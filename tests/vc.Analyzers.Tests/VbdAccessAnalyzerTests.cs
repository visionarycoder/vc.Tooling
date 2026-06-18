using VisionaryCoder.Analyzers.Abstractions;
using VisionaryCoder.Analyzers.Design.Vbd;
using Xunit;

namespace vc.Analyzers.Tests;

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

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.Contains(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.VbdAccessVaultBusinessLogicLeakage);
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

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.DoesNotContain(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.VbdAccessVaultBusinessLogicLeakage);
    }

    [Fact]
    public async Task VbdAccessAnalyzer_ShouldReportBoundaryViolation_WhenAccessComponentDependsOnManager()
    {
        var source = """
            namespace SampleApp;

            public sealed class OrderRepository
            {
                private readonly OrderManager _manager = new();
            }

            public sealed class OrderManager
            {
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.Contains(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.VbdAccessVaultBoundaryViolation);
    }

    [Fact]
    public async Task VbdAccessAnalyzer_ShouldNotReportBoundaryViolation_WhenNoManagerOrEngineDependencyExists()
    {
        var source = """
            namespace SampleApp;

            public sealed class OrderRepository
            {
                private readonly int _count = 0;
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.DoesNotContain(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.VbdAccessVaultBoundaryViolation);
    }

    [Fact]
    public async Task VbdAccessAnalyzer_ShouldReportSchemaMappingMissing_WhenEntityIsReturnedWithoutMappingMethod()
    {
        var source = """
            namespace SampleApp;

            public sealed class OrderRepository
            {
                public OrderEntity GetEntity() => new();
            }

            public sealed class OrderEntity
            {
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.Contains(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.VbdAccessVaultSchemaMappingMissing);
    }

    [Fact]
    public async Task VbdAccessAnalyzer_ShouldNotReportSchemaMappingMissing_WhenMappingMethodExists()
    {
        var source = """
            namespace SampleApp;

            public sealed class OrderRepository
            {
                public OrderEntity GetEntity() => new();
                public Order ToDomain(OrderEntity entity) => new();
            }

            public sealed class OrderEntity
            {
            }

            public sealed class Order
            {
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.DoesNotContain(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.VbdAccessVaultSchemaMappingMissing);
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

        return await compilation.WithAnalyzers(analyzers: [new VbdAccessAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}