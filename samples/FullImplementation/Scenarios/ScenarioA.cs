using FullImplementation.Domain;
using FullImplementation.Integration;
using FullImplementation.Runtime;
using VisionaryCoder.Tooling.Core;
using VisionaryCoder.Utility;

namespace FullImplementation.Scenarios;

/// <summary>
/// Scenario A — Happy path: place a valid order end-to-end through the pipeline.
/// Exercises: vc.Tooling pipeline, vc.Runtime message bus, vc.Utility Guard.
/// </summary>
public static class ScenarioA
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Scenario A: Happy Path ===");

        var messages = new List<string>();
        var invoker = DefaultPipelineFactory.Create(
            log: msg => messages.Add(msg),
            logException: ex => messages.Add($"ERROR: {ex.Message}"));

        var gateway = new StubOrderGateway(shouldFail: false);
        var manager = new OrderManager(gateway);
        var eventBus = new OrderEventBus();

        eventBus.OnOrderPlaced(envelope =>
        {
            Console.WriteLine($"  [Event] Order placed: {envelope.Payload}");
            return Task.CompletedTask;
        });

        var result = await invoker.InvokeAsync<Order>("PlaceOrder", request: "ORD-001",
            async () =>
            {
                var order = await manager.PlaceOrderAsync("CUST-001", 99.99m);
                await eventBus.PublishOrderPlacedAsync(order);
                return order;
            });

        Guard.NotNull(result, nameof(result));
        Console.WriteLine($"  Result: {result}");
        Console.WriteLine($"  Pipeline log entries: {messages.Count}");
        Console.WriteLine($"  Persisted: {gateway.Persisted.Count} order(s)");
        Console.WriteLine("  Status: PASS");
        Console.WriteLine();
    }
}
