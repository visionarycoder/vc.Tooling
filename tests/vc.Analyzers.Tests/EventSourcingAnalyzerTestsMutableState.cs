using VisionaryCoder.Analyzers.Abstractions;
using Xunit;

namespace vc.Analyzers.Tests;

public sealed partial class EventSourcingAnalyzerTests
{
    [Fact]
    public async Task EventSourcingAnalyzer_ShouldReportDiagnostic_WhenAggregateHasMutableState()
    {
        var source = """
            namespace Sample.Domain
            {
                public sealed class OrderAggregate
                {
                    private int quantity;

                    public void UpdateQuantity(int newQuantity)
                    {
                        quantity = newQuantity;
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.Contains(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.DistributedEventSourcingMutableState);
    }

    [Fact]
    public async Task EventSourcingAnalyzer_ShouldNotReportDiagnostic_WhenAggregateHasImmutableState()
    {
        var source = """
            namespace Sample.Domain
            {
                public sealed class OrderAggregate
                {
                    private readonly int quantity;

                    public OrderAggregate(int qty)
                    {
                        quantity = qty;
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source: source);

        Assert.DoesNotContain(collection: diagnostics, filter: diagnostic => diagnostic.Id == DiagnosticIds.DistributedEventSourcingMutableState);
    }
}
