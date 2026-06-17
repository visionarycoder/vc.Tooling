using FullImplementation.Domain;
using FullImplementation.Integration;
using VisionaryCoder.Tooling.Core;

namespace FullImplementation.Scenarios;

/// <summary>
/// Scenario B — Edge condition: place an order with a zero amount (boundary value).
/// Verifies that the system handles zero-value orders without error.
/// </summary>
public static class ScenarioB
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Scenario B: Edge Condition (zero-amount order) ===");

        var messages = new List<string>();
        var invoker = DefaultPipelineFactory.Create(
            log: msg => messages.Add(msg),
            logException: ex => messages.Add($"ERROR: {ex.Message}"));

        var gateway = new StubOrderGateway(shouldFail: false);
        var manager = new OrderManager(gateway);

        var result = await invoker.InvokeAsync<Order>("PlaceZeroAmountOrder", request: "ORD-ZERO",
            () => manager.PlaceZeroAmountOrderAsync("CUST-002"));

        Console.WriteLine($"  Result: {result}");
        var amountLabel = result.Amount == 0m ? "zero (edge)" : result.Amount.ToString("C");
        Console.WriteLine($"  Amount: {amountLabel}");
        Console.WriteLine($"  Status: {result.Status}");
        Console.WriteLine($"  Pipeline log entries: {messages.Count}");
        Console.WriteLine("  Status: PASS");
        Console.WriteLine();
    }
}
