using VisionaryCoder.Runtime.Vbd;
using Xunit;

namespace VisionaryCoder.Runtime.Tests;

public sealed class VbdMessageEnvelopeTests
{
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

    [Fact]
    public void Constructor_ShouldThrow_WhenOperationNameIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new VbdMessageEnvelope<string>(null!, "payload"));
    }

    [Fact]
    public void Constructor_ShouldAcceptNullPayload()
    {
        var envelope = new VbdMessageEnvelope<string>("op", null);

        Assert.Null(envelope.Payload);
    }

    [Fact]
    public void TwoEnvelopes_ShouldHaveDifferentMessageIds()
    {
        var a = new VbdMessageEnvelope<string>("op", "x");
        var b = new VbdMessageEnvelope<string>("op", "x");

        Assert.NotEqual(a.MessageId, b.MessageId);
    }
}

public sealed class VbdMessageBusTests
{
    [Fact]
    public async Task PublishAsync_ShouldInvokeSubscribedHandler()
    {
        var bus = new VbdMessageBus();
        VbdMessageEnvelope<string>? received = null;

        bus.Subscribe<string>("order.created", env =>
        {
            received = env;
            return Task.CompletedTask;
        });

        await bus.PublishAsync("order.created", "payload-value");

        Assert.NotNull(received);
        Assert.Equal("payload-value", received.Payload);
        Assert.Equal("order.created", received.OperationName);
    }

    [Fact]
    public async Task PublishAsync_ShouldNotThrow_WhenNoSubscribersExist()
    {
        var bus = new VbdMessageBus();

        // Should complete with no error
        await bus.PublishAsync("unknown.event", "data");
    }

    [Fact]
    public async Task PublishAsync_ShouldInvokeMultipleSubscribers()
    {
        var bus = new VbdMessageBus();
        var calls = 0;

        bus.Subscribe<string>("evt", _ => { calls++; return Task.CompletedTask; });
        bus.Subscribe<string>("evt", _ => { calls++; return Task.CompletedTask; });

        await bus.PublishAsync("evt", "x");

        Assert.Equal(2, calls);
    }

    [Fact]
    public void HasSubscribers_ShouldReturnFalse_WhenNoneRegistered()
    {
        var bus = new VbdMessageBus();

        Assert.False(bus.HasSubscribers("order.created"));
    }

    [Fact]
    public void HasSubscribers_ShouldReturnTrue_AfterSubscription()
    {
        var bus = new VbdMessageBus();
        bus.Subscribe<string>("order.created", _ => Task.CompletedTask);

        Assert.True(bus.HasSubscribers("order.created"));
    }

    [Fact]
    public void Subscribe_ShouldThrow_WhenOperationNameIsNull()
    {
        var bus = new VbdMessageBus();

        Assert.Throws<ArgumentNullException>(() =>
            bus.Subscribe<string>(null!, _ => Task.CompletedTask));
    }

    [Fact]
    public void Subscribe_ShouldThrow_WhenHandlerIsNull()
    {
        var bus = new VbdMessageBus();

        Assert.Throws<ArgumentNullException>(() =>
            bus.Subscribe<string>("op", null!));
    }
}
