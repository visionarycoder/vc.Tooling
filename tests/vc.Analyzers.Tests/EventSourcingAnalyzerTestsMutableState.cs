using System.Linq;
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

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.Contains(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.DistributedEventSourcingMutableState);
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

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.DoesNotContain(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.DistributedEventSourcingMutableState);
    }
}
