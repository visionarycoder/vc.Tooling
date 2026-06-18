using VisionaryCoder.Architecture;
using VisionaryCoder.Architecture.Vbd;

namespace FullImplementation.Domain;

/// <summary>Domain entity representing a customer order.</summary>
[Boundary(boundaryType: BoundaryType.Domain)]
[VbdBoundary(boundaryType: BoundaryType.Domain)]
public sealed class Order(string orderId, string customerId, decimal amount)
{
    public string OrderId { get; } = orderId;
    public string CustomerId { get; } = customerId;
    public decimal Amount { get; } = amount;
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;

    public void Complete() => Status = OrderStatus.Completed;
    public void Fail() => Status = OrderStatus.Failed;

    public override string ToString() =>
        $"Order[{OrderId}] Customer={CustomerId} Amount={Amount:C} Status={Status}";
}