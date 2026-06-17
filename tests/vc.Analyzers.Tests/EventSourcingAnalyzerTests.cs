using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Vc.Analyzers.Distributed;
using VisionaryCoder.Tooling.Analyzers.Common;
using Xunit;

namespace Vc.Analyzers.Tests;

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

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.Contains(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.DistributedEventSourcingMissingApply);
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

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.DoesNotContain(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.DistributedEventSourcingMissingApply);
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

        return await compilation.WithAnalyzers([new EventSourcingAnalyzer()]).GetAnalyzerDiagnosticsAsync();
    }
}
