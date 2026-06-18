using VisionaryCoder.Ifx;
using Xunit;

namespace vc.Ifx.Tests;

/// <summary>
/// Tests for <see cref="ConfigAdapter"/>.
/// </summary>
public sealed class ConfigAdapterTests
{
    private static ConfigAdapter CreateAdapter(Dictionary<string, string>? values = null) =>
        new ConfigAdapter(values ?? new Dictionary<string, string>());

    /// <summary>
    /// Verifies constructor throws when values are null.
    /// </summary>
    [Fact]
    public void Constructor_ShouldThrow_WhenValuesIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new ConfigAdapter(null!));
    }

    /// <summary>
    /// Verifies GetValue returns configured value for an existing key.
    /// </summary>
    [Fact]
    public void GetValue_ShouldReturnValue_WhenKeyExists()
    {
        var adapter = CreateAdapter(new Dictionary<string, string> { ["key"] = "value" });

        var result = adapter.GetValue("key");

        Assert.Equal("value", result);
    }

    /// <summary>
    /// Verifies GetValue returns null for a missing key.
    /// </summary>
    [Fact]
    public void GetValue_ShouldReturnNull_WhenKeyMissing()
    {
        var adapter = CreateAdapter();

        var result = adapter.GetValue("missing");

        Assert.Null(result);
    }

    /// <summary>
    /// Verifies GetValue throws when key is null.
    /// </summary>
    [Fact]
    public void GetValue_ShouldThrow_WhenKeyIsNull()
    {
        var adapter = CreateAdapter();

        Assert.Throws<ArgumentNullException>(() => adapter.GetValue(null!));
    }

    /// <summary>
    /// Verifies TryGetValue returns true and value for existing key.
    /// </summary>
    [Fact]
    public void TryGetValue_ShouldReturnTrueAndValue_WhenKeyExists()
    {
        var adapter = CreateAdapter(new Dictionary<string, string> { ["db"] = "server=localhost" });

        var found = adapter.TryGetValue("db", out var value);

        Assert.True(found);
        Assert.Equal("server=localhost", value);
    }

    /// <summary>
    /// Verifies TryGetValue returns false for missing key.
    /// </summary>
    [Fact]
    public void TryGetValue_ShouldReturnFalse_WhenKeyMissing()
    {
        var adapter = CreateAdapter();

        var found = adapter.TryGetValue("absent", out var value);

        Assert.False(found);
        Assert.Null(value);
    }
}
