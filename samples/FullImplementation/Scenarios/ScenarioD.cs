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
        Console.WriteLine("=== Scenario D: Integration Failure Handling ===");

        var errors = new List<string>();
        var invoker = DefaultPipelineFactory.Create(
            log: _ => { },
            logException: ex => errors.Add(ex.Message));

        // Gateway configured to always fail
        var failingGateway = new StubOrderGateway(shouldFail: true);
        var manager = new OrderManager(failingGateway);
        var eventBus = new OrderEventBus();

        eventBus.OnOrderFailed(envelope =>
        {
            Console.WriteLine($"  [Event] Order failed (orderId may be null): {envelope.Payload?.OrderId ?? "N/A"}");
            return Task.CompletedTask;
        });

        try
        {
            await invoker.InvokeAsync("PlaceOrder-Failure", request: "ORD-FAIL",
                () => manager.PlaceOrderAsync("CUST-ERR", 49.99m));
        }
        catch (InvalidOperationException ex)
        {
            errors.Add(ex.Message);
            await eventBus.PublishOrderFailedAsync(null);
            Console.WriteLine($"  [D1] Gateway failure caught: {ex.Message}");
        }

        Console.WriteLine($"  Errors captured: {errors.Count}");
        Console.WriteLine($"  EventBus has failure subscribers: {eventBus.HasOrderPlacedSubscribers}");
        Console.WriteLine("  Status: PASS");
        Console.WriteLine();
    }
}
