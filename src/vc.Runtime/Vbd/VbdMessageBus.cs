namespace VisionaryCoder.Runtime.Vbd;

public sealed class VbdMessageBus
{
    private readonly Dictionary<string, List<Func<object, Task>>> _handlers =
        new(comparer: StringComparer.Ordinal);

    public void Subscribe<TPayload>(
        string operationName,
        Func<VbdMessageEnvelope<TPayload>, Task> handler)
    {
        ArgumentNullException.ThrowIfNull(argument: operationName);
        ArgumentNullException.ThrowIfNull(argument: handler);

        if (!_handlers.TryGetValue(key: operationName, value: out var list))
        {
            list = [];
            _handlers[key: operationName] = list;
        }

        list.Add(item: msg => handler(arg: (VbdMessageEnvelope<TPayload>)msg));
    }

    public async Task PublishAsync<TPayload>(string operationName, TPayload? payload)
    {
        ArgumentNullException.ThrowIfNull(argument: operationName);

        var envelope = new VbdMessageEnvelope<TPayload>(operationName: operationName, payload: payload);

        if (_handlers.TryGetValue(key: operationName, value: out var handlers))
        {
            foreach (var handler in handlers)
                await handler(arg: envelope);
        }
    }

    public bool HasSubscribers(string operationName) =>
        _handlers.ContainsKey(key: operationName);
}
