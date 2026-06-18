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
        Console.WriteLine(value: "=== Scenario B: Edge Condition (zero-amount order) ===");

        var messages = new List<string>();
        var invoker = DefaultPipelineFactory.Create(
            log: msg => messages.Add(item: msg),
            logException: ex => messages.Add(item: $"ERROR: {ex.Message}"));

        var gateway = new StubOrderGateway(shouldFail: false);
        var manager = new OrderManager(gateway: gateway);

        var result = await invoker.InvokeAsync<Order>(operationName: "PlaceZeroAmountOrder", request: "ORD-ZERO",
            operation: () => manager.PlaceZeroAmountOrderAsync(customerId: "CUST-002"));

        Console.WriteLine(value: $"  Result: {result}");
        var amountLabel = result.Amount == 0m ? "zero (edge)" : result.Amount.ToString(format: "C");
        Console.WriteLine(value: $"  Amount: {amountLabel}");
        Console.WriteLine(value: $"  Status: {result.Status}");
        Console.WriteLine(value: $"  Pipeline log entries: {messages.Count}");
        Console.WriteLine(value: "  Status: PASS");
        Console.WriteLine();
    }
}
