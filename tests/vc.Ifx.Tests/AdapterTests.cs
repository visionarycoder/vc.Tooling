using VisionaryCoder.Ifx;
using Xunit;

namespace VisionaryCoder.Ifx.Tests;

public sealed class ConfigAdapterTests
{
    private static ConfigAdapter CreateAdapter(Dictionary<string, string>? values = null) =>
        new ConfigAdapter(values ?? new Dictionary<string, string>());

    [Fact]
    public void Constructor_ShouldThrow_WhenValuesIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new ConfigAdapter(null!));
    }

    [Fact]
    public void GetValue_ShouldReturnValue_WhenKeyExists()
    {
        var adapter = CreateAdapter(new Dictionary<string, string> { ["key"] = "value" });

        var result = adapter.GetValue("key");

        Assert.Equal("value", result);
    }

    [Fact]
    public void GetValue_ShouldReturnNull_WhenKeyMissing()
    {
        var adapter = CreateAdapter();

        var result = adapter.GetValue("missing");

        Assert.Null(result);
    }

    [Fact]
    public void GetValue_ShouldThrow_WhenKeyIsNull()
    {
        var adapter = CreateAdapter();

        Assert.Throws<ArgumentNullException>(() => adapter.GetValue(null!));
    }

    [Fact]
    public void TryGetValue_ShouldReturnTrueAndValue_WhenKeyExists()
    {
        var adapter = CreateAdapter(new Dictionary<string, string> { ["db"] = "server=localhost" });

        var found = adapter.TryGetValue("db", out var value);

        Assert.True(found);
        Assert.Equal("server=localhost", value);
    }

    [Fact]
    public void TryGetValue_ShouldReturnFalse_WhenKeyMissing()
    {
        var adapter = CreateAdapter();

        var found = adapter.TryGetValue("absent", out var value);

        Assert.False(found);
        Assert.Null(value);
    }
}

public sealed class HttpAdapterTests
{
    [Fact]
    public void Constructor_ShouldThrow_WhenClientIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new HttpAdapter(null!));
    }

    [Fact]
    public async Task GetAsync_ShouldThrow_WhenRequestUriIsNull()
    {
        var adapter = new HttpAdapter(new HttpClient());

        await Assert.ThrowsAsync<ArgumentNullException>(() => adapter.GetAsync(null!));
    }

    [Fact]
    public async Task PostAsync_ShouldThrow_WhenRequestUriIsNull()
    {
        var adapter = new HttpAdapter(new HttpClient());

        await Assert.ThrowsAsync<ArgumentNullException>(() => adapter.PostAsync(null!, "content"));
    }

    [Fact]
    public async Task PostAsync_ShouldThrow_WhenContentIsNull()
    {
        var adapter = new HttpAdapter(new HttpClient());

        await Assert.ThrowsAsync<ArgumentNullException>(() => adapter.PostAsync("http://example.com", null!));
    }
}
