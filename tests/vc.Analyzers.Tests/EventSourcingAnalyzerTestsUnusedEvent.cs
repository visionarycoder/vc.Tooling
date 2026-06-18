using VisionaryCoder.Analyzers.Abstractions;
using Xunit;

namespace vc.Analyzers.Tests;

public sealed partial class EventSourcingAnalyzerTests
{
    [Fact]
    public async Task EventSourcingAnalyzer_ShouldReportDiagnostic_WhenEventIsNeverApplied()
    {
        var source = """
            namespace Sample.Domain
            {
                public sealed class UnusedEvent
                {
                }

                public sealed class OrderAggregate
                {
                    public void RaiseEvent()
                    {
                        var evt = new UnusedEvent();
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.Contains(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.DistributedEventSourcingUnusedEvent);
    }

    [Fact]
    public async Task EventSourcingAnalyzer_ShouldNotReportDiagnostic_WhenEventIsApplied()
    {
        var source = """
            namespace Sample.Domain
            {
                public sealed class UsedEvent
                {
                }

                public sealed class OrderAggregate
                {
                    public void RaiseEvent()
                    {
                        var evt = new UsedEvent();
                    }

                    public void Apply(UsedEvent evt)
                    {
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.DoesNotContain(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.DistributedEventSourcingUnusedEvent);
    }
}
