using VisionaryCoder.Ifx;
using Xunit;

namespace vc.Ifx.Tests;

/// <summary>
/// Tests for <see cref="HttpAdapter"/> argument validation.
/// </summary>
public sealed class HttpAdapterTests
{
    /// <summary>
    /// Verifies constructor throws when client is null.
    /// </summary>
    [Fact]
    public void Constructor_ShouldThrow_WhenClientIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new HttpAdapter(null!));
    }

    /// <summary>
    /// Verifies GetAsync throws when request URI is null.
    /// </summary>
    [Fact]
    public async Task GetAsync_ShouldThrow_WhenRequestUriIsNull()
    {
        var adapter = new HttpAdapter(new HttpClient());

        await Assert.ThrowsAsync<ArgumentNullException>(() => adapter.GetAsync(null!));
    }

    /// <summary>
    /// Verifies PostAsync throws when request URI is null.
    /// </summary>
    [Fact]
    public async Task PostAsync_ShouldThrow_WhenRequestUriIsNull()
    {
        var adapter = new HttpAdapter(new HttpClient());

        await Assert.ThrowsAsync<ArgumentNullException>(() => adapter.PostAsync(null!, "content"));
    }

    /// <summary>
    /// Verifies PostAsync throws when content is null.
    /// </summary>
    [Fact]
    public async Task PostAsync_ShouldThrow_WhenContentIsNull()
    {
        var adapter = new HttpAdapter(new HttpClient());

        await Assert.ThrowsAsync<ArgumentNullException>(() => adapter.PostAsync("http://example.com", null!));
    }
}
