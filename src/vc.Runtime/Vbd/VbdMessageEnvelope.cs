namespace VisionaryCoder.Runtime.Vbd;

public sealed class VbdMessageEnvelope<TPayload>(string operationName, TPayload? payload)
{
    public Guid MessageId { get; } = Guid.NewGuid();
    public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
    public string OperationName { get; } = operationName ?? throw new ArgumentNullException(paramName: nameof(operationName));
    public TPayload? Payload { get; } = payload;
}
