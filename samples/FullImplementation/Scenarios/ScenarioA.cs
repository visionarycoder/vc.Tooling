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
        Console.WriteLine(value: "=== Scenario A: Happy Path ===");

        var messages = new List<string>();
        var invoker = DefaultPipelineFactory.Create(
            log: msg => messages.Add(item: msg),
            logException: ex => messages.Add(item: $"ERROR: {ex.Message}"));

        var gateway = new StubOrderGateway(shouldFail: false);
        var manager = new OrderManager(gateway: gateway);
        var eventBus = new OrderEventBus();

        eventBus.OnOrderPlaced(handler: envelope =>
        {
            Console.WriteLine(value: $"  [Event] Order placed: {envelope.Payload}");
            return Task.CompletedTask;
        });

        var result = await invoker.InvokeAsync<Order>(operationName: "PlaceOrder", request: "ORD-001",
            operation: async () =>
            {
                var order = await manager.PlaceOrderAsync(customerId: "CUST-001", amount: 99.99m);
                await eventBus.PublishOrderPlacedAsync(order: order);
                return order;
            });

        Guard.NotNull(value: result, paramName: nameof(result));
        Console.WriteLine(value: $"  Result: {result}");
        Console.WriteLine(value: $"  Pipeline log entries: {messages.Count}");
        Console.WriteLine(value: $"  Persisted: {gateway.Persisted.Count} order(s)");
        Console.WriteLine(value: "  Status: PASS");
        Console.WriteLine();
    }
}
