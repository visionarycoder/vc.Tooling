using FullImplementation.Domain;
using VisionaryCoder.Utility;

namespace FullImplementation.Integration;

/// <summary>
/// In-process stub adapter for Scenario C/D testing — does not call a real HTTP endpoint.
/// Demonstrates the Ifx boundary by accepting a configurable failure mode.
/// </summary>
public sealed class StubOrderGateway(bool shouldFail = false) : IOrderGateway
{
    private readonly List<Order> _persisted = [];

    public IReadOnlyList<Order> Persisted => _persisted.AsReadOnly();

    public Task PersistAsync(Order order, CancellationToken cancellationToken = default)
    {
        Guard.NotNull(value: order, paramName: nameof(order));
        if (shouldFail)
            throw new InvalidOperationException(message: $"Simulated gateway failure for order {order.OrderId}.");
        _persisted.Add(item: order);
        return Task.CompletedTask;
    }
}