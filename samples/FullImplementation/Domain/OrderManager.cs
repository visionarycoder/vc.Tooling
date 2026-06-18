using VisionaryCoder.Architecture;
using VisionaryCoder.Architecture.Vbd;

namespace FullImplementation.Domain;

/// <summary>Manager vault: coordinates order lifecycle policy.</summary>
[Component(role: ComponentRole.Manager)]
[VbdComponent(role: ComponentRole.Manager)]
public sealed class OrderManager(IOrderGateway gateway)
{
    private readonly IOrderGateway _gateway = gateway ?? throw new ArgumentNullException(paramName: nameof(gateway));

    /// <summary>Scenario A: happy path — creates and confirms an order.</summary>
    public async Task<Order> PlaceOrderAsync(
        string customerId, decimal amount, CancellationToken cancellationToken = default)
    {
        var orderId = Guid.NewGuid().ToString(format: "N")[..8].ToUpperInvariant();
        var order = new Order(orderId: orderId, customerId: customerId, amount: amount);
        await _gateway.PersistAsync(order: order, cancellationToken: cancellationToken);
        order.Complete();
        return order;
    }

    /// <summary>Scenario B: edge — handles zero-amount orders gracefully.</summary>
    public async Task<Order> PlaceZeroAmountOrderAsync(
        string customerId, CancellationToken cancellationToken = default)
    {
        var orderId = Guid.NewGuid().ToString(format: "N")[..8].ToUpperInvariant();
        var order = new Order(orderId: orderId, customerId: customerId, amount: 0m);
        await _gateway.PersistAsync(order: order, cancellationToken: cancellationToken);
        order.Complete();
        return order;
    }
}