using VisionaryCoder.Runtime.Vbd;
using Xunit;

namespace vc.Runtime.Tests;

/// <summary>
/// Tests for <see cref="VbdMessageEnvelope{TPayload}"/>.
/// </summary>
public sealed class VbdMessageEnvelopeTests
{
    /// <summary>
    /// Verifies constructor populates envelope properties.
    /// </summary>
    [Fact]
    public void Constructor_ShouldPopulateAllProperties()
    {
        const string operationName = "TestOperation";
        const int payload = 42;

        var envelope = new VbdMessageEnvelope<int>(operationName, payload);

        Assert.Equal(operationName, envelope.OperationName);
        Assert.Equal(payload, envelope.Payload);
        Assert.NotEqual(Guid.Empty, envelope.MessageId);
        Assert.True(envelope.Timestamp <= DateTimeOffset.UtcNow);
    }

    /// <summary>
    /// Verifies constructor throws when operation name is null.
    /// </summary>
    [Fact]
    public void Constructor_ShouldThrow_WhenOperationNameIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new VbdMessageEnvelope<string>(null!, "payload"));
    }

    /// <summary>
    /// Verifies constructor accepts null payload.
    /// </summary>
    [Fact]
    public void Constructor_ShouldAcceptNullPayload()
    {
        var envelope = new VbdMessageEnvelope<string>("op", null);

        Assert.Null(envelope.Payload);
    }

    /// <summary>
    /// Verifies each envelope instance gets a unique message identifier.
    /// </summary>
    [Fact]
    public void TwoEnvelopes_ShouldHaveDifferentMessageIds()
    {
        var a = new VbdMessageEnvelope<string>("op", "x");
        var b = new VbdMessageEnvelope<string>("op", "x");

        Assert.NotEqual(a.MessageId, b.MessageId);
    }
}
