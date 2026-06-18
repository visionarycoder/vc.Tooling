using FullImplementation.Domain;
using VisionaryCoder.Architecture;
using VisionaryCoder.Architecture.Vbd;
using VisionaryCoder.Runtime.Vbd;

namespace FullImplementation.Runtime;

/// <summary>
/// Wraps <see cref="VbdMessageBus"/> from vc.Runtime to publish order lifecycle events.
/// Operates at the runtime boundary, decoupled from the domain and integration vaults.
/// </summary>
[Boundary(boundaryType: BoundaryType.Runtime)]
[VbdBoundary(boundaryType: BoundaryType.Runtime)]
public sealed class OrderEventBus
{
    private readonly VbdMessageBus _bus = new();

    public const string OrderPlacedOperation = "order.placed";
    public const string OrderFailedOperation = "order.failed";

    public void OnOrderPlaced(Func<VbdMessageEnvelope<Order>, Task> handler) =>
        _bus.Subscribe(operationName: OrderPlacedOperation, handler: handler);

    public void OnOrderFailed(Func<VbdMessageEnvelope<Order?>, Task> handler) =>
        _bus.Subscribe(operationName: OrderFailedOperation, handler: handler);

    public Task PublishOrderPlacedAsync(Order order) =>
        _bus.PublishAsync(operationName: OrderPlacedOperation, payload: order);

    public Task PublishOrderFailedAsync(Order? order) =>
        _bus.PublishAsync(operationName: OrderFailedOperation, payload: order);

    public bool HasOrderPlacedSubscribers => _bus.HasSubscribers(operationName: OrderPlacedOperation);
}
