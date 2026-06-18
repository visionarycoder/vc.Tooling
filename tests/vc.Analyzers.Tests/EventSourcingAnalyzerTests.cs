using VisionaryCoder.Analyzers.Abstractions;
using VisionaryCoder.Analyzers.Distributed;
using Xunit;

namespace vc.Analyzers.Tests;

public sealed partial class EventSourcingAnalyzerTests
{
    [Fact]
    public async Task EventSourcingAnalyzer_ShouldReportDiagnostic_WhenAggregateHasNoApplyForRaisedEvent()
    {
        var source = """
            namespace Sample.Domain
            {
                public sealed class OrderEvent
                {
                }

                public sealed class OrderAggregate
                {
                    public void Place()
                    {
                        var evt = new OrderEvent();
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.Contains(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.DistributedEventSourcingMissingApply);
    }

    [Fact]
    public async Task EventSourcingAnalyzer_ShouldNotReportDiagnostic_WhenAggregateHasApplyForAllEvents()
    {
        var source = """
            namespace Sample.Domain
            {
                public sealed class OrderEvent
                {
                }

                public sealed class OrderAggregate
                {
                    public void Place()
                    {
                        var evt = new OrderEvent();
                    }

                    public void Apply(OrderEvent evt)
                    {
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.DoesNotContain(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.DistributedEventSourcingMissingApply);
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

        return await compilation.WithAnalyzers(analyzers: [new EventSourcingAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}
