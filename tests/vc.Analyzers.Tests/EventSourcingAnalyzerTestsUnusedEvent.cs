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

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.Contains(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.DistributedEventSourcingUnusedEvent);
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

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.DoesNotContain(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.DistributedEventSourcingUnusedEvent);
    }
}
