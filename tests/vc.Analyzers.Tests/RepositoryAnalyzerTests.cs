using VisionaryCoder.Analyzers.Abstractions;
using VisionaryCoder.Analyzers.Distributed;
using Xunit;

namespace vc.Analyzers.Tests;

public sealed class RepositoryAnalyzerTests
{
    [Fact]
    public async Task RepositoryAnalyzer_ShouldReportDiagnostic_WhenRepositoryMethodIsSynchronous()
    {
        var source = """
            namespace Sample.Data
            {
                public sealed class OrderRepository
                {
                    public int GetCount()
                    {
                        return 1;
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.Contains(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.DistributedRepositoryAsyncMissing);
    }

    [Fact]
    public async Task RepositoryAnalyzer_ShouldNotReportDiagnostic_WhenRepositoryMethodReturnsTask()
    {
        var source = """
            using System.Threading.Tasks;

            namespace Sample.Data
            {
                public sealed class OrderRepository
                {
                    public Task<int> GetCountAsync()
                    {
                        return Task.FromResult(1);
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.DoesNotContain(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.DistributedRepositoryAsyncMissing);
    }

    [Fact]
    public async Task RepositoryAnalyzer_ShouldReportDiagnostic_WhenRepositoryExposesIQueryable()
    {
        var source = """
            using System.Linq;

            namespace Sample.Data
            {
                public sealed class OrderRepository
                {
                    public IQueryable<int> GetQuery()
                    {
                        return Enumerable.Empty<int>().AsQueryable();
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.Contains(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.DistributedRepositoryQueryableLeakage);
    }

    [Fact]
    public async Task RepositoryAnalyzer_ShouldNotReportQueryableDiagnostic_WhenRepositoryReturnsTaskList()
    {
        var source = """
            using System.Collections.Generic;
            using System.Threading.Tasks;

            namespace Sample.Data
            {
                public sealed class OrderRepository
                {
                    public Task<List<int>> GetItemsAsync()
                    {
                        return Task.FromResult(new List<int>());
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.DoesNotContain(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.DistributedRepositoryQueryableLeakage);
    }

    [Fact]
    public async Task RepositoryAnalyzer_ShouldReportDiagnostic_WhenRepositoryReturnsDomainEntity()
    {
        var source = """
            using System.Threading.Tasks;

            namespace Sample.Domain
            {
                public sealed class OrderEntity
                {
                }
            }

            namespace Sample.Data
            {
                public sealed class OrderRepository
                {
                    public Sample.Domain.OrderEntity GetEntity()
                    {
                        return new Sample.Domain.OrderEntity();
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.Contains(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.DistributedRepositoryContractViolation);
    }

    [Fact]
    public async Task RepositoryAnalyzer_ShouldNotReportDomainReturnDiagnostic_WhenRepositoryReturnsDto()
    {
        var source = """
            using System.Threading.Tasks;

            namespace Sample.Contracts
            {
                public sealed class OrderDto
                {
                }
            }

            namespace Sample.Data
            {
                public sealed class OrderRepository
                {
                    public Task<Sample.Contracts.OrderDto> GetDtoAsync()
                    {
                        return Task.FromResult(new Sample.Contracts.OrderDto());
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.DoesNotContain(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.DistributedRepositoryContractViolation);
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
                MetadataReference.CreateFromFile(path: typeof(System.Threading.Tasks.Task).Assembly.Location),
                MetadataReference.CreateFromFile(path: typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location)
            ],
            options: new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary));

        return await compilation.WithAnalyzers(analyzers: [new RepositoryAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}