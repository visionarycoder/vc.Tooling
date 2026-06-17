namespace VisionaryCoder.Runtime.Vbd;

public sealed class VbdMessageBus
{
    private readonly Dictionary<string, List<Func<object, Task>>> _handlers =
        new(StringComparer.Ordinal);

    public void Subscribe<TPayload>(
        string operationName,
        Func<VbdMessageEnvelope<TPayload>, Task> handler)
    {
        ArgumentNullException.ThrowIfNull(operationName);
        ArgumentNullException.ThrowIfNull(handler);

        if (!_handlers.TryGetValue(operationName, out var list))
        {
            list = [];
            _handlers[operationName] = list;
        }

        list.Add(msg => handler((VbdMessageEnvelope<TPayload>)msg));
    }

    public async Task PublishAsync<TPayload>(string operationName, TPayload? payload)
    {
        ArgumentNullException.ThrowIfNull(operationName);

        var envelope = new VbdMessageEnvelope<TPayload>(operationName, payload);

        if (_handlers.TryGetValue(operationName, out var handlers))
        {
            foreach (var handler in handlers)
                await handler(envelope);
        }
    }

    public bool HasSubscribers(string operationName) =>
        _handlers.ContainsKey(operationName);
}
