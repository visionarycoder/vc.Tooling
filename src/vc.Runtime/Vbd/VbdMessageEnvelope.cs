namespace VisionaryCoder.Runtime.Vbd;

public sealed class VbdMessageEnvelope<TPayload>
{
    public VbdMessageEnvelope(string operationName, TPayload? payload)
    {
        MessageId = Guid.NewGuid();
        Timestamp = DateTimeOffset.UtcNow;
        OperationName = operationName ?? throw new ArgumentNullException(nameof(operationName));
        Payload = payload;
    }

    public Guid MessageId { get; }
    public DateTimeOffset Timestamp { get; }
    public string OperationName { get; }
    public TPayload? Payload { get; }
}
