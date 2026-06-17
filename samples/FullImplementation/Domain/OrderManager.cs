using VisionaryCoder.Architecture;
using VisionaryCoder.Architecture.Vbd;

namespace FullImplementation.Domain;

/// <summary>Manager vault: coordinates order lifecycle policy.</summary>
[Component(ComponentRole.Manager)]
[VbdComponent(ComponentRole.Manager)]
public sealed class OrderManager
{
    private readonly IOrderGateway _gateway;

    public OrderManager(IOrderGateway gateway) =>
        _gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));

    /// <summary>Scenario A: happy path — creates and confirms an order.</summary>
    public async Task<Order> PlaceOrderAsync(
        string customerId, decimal amount, CancellationToken cancellationToken = default)
    {
        var orderId = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        var order = new Order(orderId, customerId, amount);
        await _gateway.PersistAsync(order, cancellationToken);
        order.Complete();
        return order;
    }

    /// <summary>Scenario B: edge — handles zero-amount orders gracefully.</summary>
    public async Task<Order> PlaceZeroAmountOrderAsync(
        string customerId, CancellationToken cancellationToken = default)
    {
        var orderId = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        var order = new Order(orderId, customerId, 0m);
        await _gateway.PersistAsync(order, cancellationToken);
        order.Complete();
        return order;
    }
}

/// <summary>Access vault contract for order persistence.</summary>
[Component(ComponentRole.Access)]
public interface IOrderGateway
{
    Task PersistAsync(Order order, CancellationToken cancellationToken = default);
}
