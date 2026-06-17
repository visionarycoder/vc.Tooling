using VisionaryCoder.Architecture;
using VisionaryCoder.Architecture.Vbd;

namespace FullImplementation.Domain;

/// <summary>Domain entity representing a customer order.</summary>
[Boundary(BoundaryType.Domain)]
[VbdBoundary(BoundaryType.Domain)]
public sealed class Order
{
    public Order(string orderId, string customerId, decimal amount)
    {
        OrderId = orderId;
        CustomerId = customerId;
        Amount = amount;
        Status = OrderStatus.Pending;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public string OrderId { get; }
    public string CustomerId { get; }
    public decimal Amount { get; }
    public OrderStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; }

    public void Complete() => Status = OrderStatus.Completed;
    public void Fail() => Status = OrderStatus.Failed;

    public override string ToString() =>
        $"Order[{OrderId}] Customer={CustomerId} Amount={Amount:C} Status={Status}";
}

public enum OrderStatus { Pending, Completed, Failed }
