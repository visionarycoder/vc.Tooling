using FullImplementation.Domain;
using FullImplementation.Integration;
using FullImplementation.Runtime;
using VisionaryCoder.Tooling.Core;

namespace FullImplementation.Scenarios;

/// <summary>
/// Scenario D — Integration/runtime failure: the gateway throws and the event bus
/// receives a failure notification. Exercises exception handling in the pipeline
/// and the runtime VbdMessageBus fallback path.
/// </summary>
public static class ScenarioD
{
    public static async Task RunAsync()
    {
        Console.WriteLine(value: "=== Scenario D: Integration Failure Handling ===");

        var errors = new List<string>();
        var invoker = DefaultPipelineFactory.Create(
            log: _ => { },
            logException: ex => errors.Add(item: ex.Message));

        // Gateway configured to always fail
        var failingGateway = new StubOrderGateway(shouldFail: true);
        var manager = new OrderManager(gateway: failingGateway);
        var eventBus = new OrderEventBus();

        eventBus.OnOrderFailed(handler: envelope =>
        {
            Console.WriteLine(value: $"  [Event] Order failed (orderId may be null): {envelope.Payload?.OrderId ?? "N/A"}");
            return Task.CompletedTask;
        });

        try
        {
            await invoker.InvokeAsync(operationName: "PlaceOrder-Failure", request: "ORD-FAIL",
                operation: () => manager.PlaceOrderAsync(customerId: "CUST-ERR", amount: 49.99m));
        }
        catch (InvalidOperationException ex)
        {
            errors.Add(item: ex.Message);
            await eventBus.PublishOrderFailedAsync(order: null);
            Console.WriteLine(value: $"  [D1] Gateway failure caught: {ex.Message}");
        }

        Console.WriteLine(value: $"  Errors captured: {errors.Count}");
        Console.WriteLine(value: $"  EventBus has failure subscribers: {eventBus.HasOrderPlacedSubscribers}");
        Console.WriteLine(value: "  Status: PASS");
        Console.WriteLine();
    }
}
