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
[Boundary(BoundaryType.Integration)]
[VbdBoundary(BoundaryType.Integration)]
[Component(ComponentRole.Access)]
[VbdComponent(ComponentRole.Access)]
public sealed class OrderHttpGateway : IOrderGateway
{
    private readonly IHttpAdapter _http;
    private readonly IConfigAdapter _config;

    public OrderHttpGateway(IHttpAdapter http, IConfigAdapter config)
    {
        _http = Guard.NotNull(http, nameof(http));
        _config = Guard.NotNull(config, nameof(config));
    }

    public async Task PersistAsync(Order order, CancellationToken cancellationToken = default)
    {
        Guard.NotNull(order, nameof(order));

        var endpoint = _config.GetValue("Orders:Endpoint").OrEmpty();
        if (endpoint.IsNullOrWhiteSpace())
            endpoint = "https://api.example.com/orders";

        var payload = $"{{\"orderId\":\"{order.OrderId}\",\"customerId\":\"{order.CustomerId}\",\"amount\":{order.Amount}}}";
        await _http.PostAsync(endpoint, payload, cancellationToken);
    }
}

/// <summary>
/// In-process stub adapter for Scenario C/D testing — does not call a real HTTP endpoint.
/// Demonstrates the Ifx boundary by accepting a configurable failure mode.
/// </summary>
public sealed class StubOrderGateway : IOrderGateway
{
    private readonly bool _shouldFail;
    private readonly List<Order> _persisted = [];

    public StubOrderGateway(bool shouldFail = false) => _shouldFail = shouldFail;

    public IReadOnlyList<Order> Persisted => _persisted.AsReadOnly();

    public Task PersistAsync(Order order, CancellationToken cancellationToken = default)
    {
        Guard.NotNull(order, nameof(order));
        if (_shouldFail)
            throw new InvalidOperationException($"Simulated gateway failure for order {order.OrderId}.");
        _persisted.Add(order);
        return Task.CompletedTask;
    }
}
