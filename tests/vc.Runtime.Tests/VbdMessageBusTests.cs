using VisionaryCoder.Runtime.Vbd;
using Xunit;

namespace vc.Runtime.Tests;

/// <summary>
/// Tests for <see cref="VbdMessageBus"/> behavior.
/// </summary>
public sealed class VbdMessageBusTests
{
    /// <summary>
    /// Verifies publishing invokes the subscribed handler.
    /// </summary>
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

    /// <summary>
    /// Verifies publishing with no subscribers completes without error.
    /// </summary>
    [Fact]
    public async Task PublishAsync_ShouldNotThrow_WhenNoSubscribersExist()
    {
        var bus = new VbdMessageBus();

        // Should complete with no error
        await bus.PublishAsync("unknown.event", "data");
    }

    /// <summary>
    /// Verifies publishing invokes all subscribed handlers.
    /// </summary>
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

    /// <summary>
    /// Verifies subscriber existence check is false when no handlers are registered.
    /// </summary>
    [Fact]
    public void HasSubscribers_ShouldReturnFalse_WhenNoneRegistered()
    {
        var bus = new VbdMessageBus();

        Assert.False(bus.HasSubscribers("order.created"));
    }

    /// <summary>
    /// Verifies subscriber existence check is true after registration.
    /// </summary>
    [Fact]
    public void HasSubscribers_ShouldReturnTrue_AfterSubscription()
    {
        var bus = new VbdMessageBus();
        bus.Subscribe<string>("order.created", _ => Task.CompletedTask);

        Assert.True(bus.HasSubscribers("order.created"));
    }

    /// <summary>
    /// Verifies subscribing throws for null operation name.
    /// </summary>
    [Fact]
    public void Subscribe_ShouldThrow_WhenOperationNameIsNull()
    {
        var bus = new VbdMessageBus();

        Assert.Throws<ArgumentNullException>(() =>
            bus.Subscribe<string>(null!, _ => Task.CompletedTask));
    }

    /// <summary>
    /// Verifies subscribing throws for null handler.
    /// </summary>
    [Fact]
    public void Subscribe_ShouldThrow_WhenHandlerIsNull()
    {
        var bus = new VbdMessageBus();

        Assert.Throws<ArgumentNullException>(() =>
            bus.Subscribe<string>("op", null!));
    }
}
