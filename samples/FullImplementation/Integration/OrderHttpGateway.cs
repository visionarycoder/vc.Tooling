using FullImplementation.Domain;
using VisionaryCoder.Architecture;
using VisionaryCoder.Architecture.Vbd;
using VisionaryCoder.Ifx;
using VisionaryCoder.Utility;

namespace FullImplementation.Integration;

/// <summary>
/// Access vault: bridges the domain order gateway to an HTTP integration boundary.
/// Uses <see cref="IHttpAdapter"/> from vc.Ifx to persist orders to an external service.
/// </summary>
[Boundary(boundaryType: BoundaryType.Integration)]
[VbdBoundary(boundaryType: BoundaryType.Integration)]
[Component(role: ComponentRole.Access)]
[VbdComponent(role: ComponentRole.Access)]
public sealed class OrderHttpGateway(IHttpAdapter http, IConfigAdapter config) : IOrderGateway
{
    private readonly IHttpAdapter _http = Guard.NotNull(value: http, paramName: nameof(http));
    private readonly IConfigAdapter _config = Guard.NotNull(value: config, paramName: nameof(config));

    public async Task PersistAsync(Order order, CancellationToken cancellationToken = default)
    {
        Guard.NotNull(value: order, paramName: nameof(order));

        var endpoint = _config.GetValue(key: "Orders:Endpoint").OrEmpty();
        if (endpoint.IsNullOrWhiteSpace())
            endpoint = "https://api.example.com/orders";

        var payload = $"{{\"orderId\":\"{order.OrderId}\",\"customerId\":\"{order.CustomerId}\",\"amount\":{order.Amount}}}";
        await _http.PostAsync(requestUri: endpoint, content: payload, cancellationToken: cancellationToken);
    }
}