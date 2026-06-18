using VisionaryCoder.Utility;
using Xunit;

namespace vc.Utility.Tests;

/// <summary>
/// Tests for <see cref="StringExtensions"/> helpers.
/// </summary>
public sealed class StringExtensionsTests
{
    /// <summary>
    /// Verifies IsNullOrWhiteSpace returns true for null values.
    /// </summary>
    [Fact]
    public void IsNullOrWhiteSpace_ShouldReturnTrue_WhenValueIsNull()
    {
        string? value = null;

        var result = value.IsNullOrWhiteSpace();

        Assert.True(result);
    }

    /// <summary>
    /// Verifies IsNullOrWhiteSpace returns true for whitespace values.
    /// </summary>
    /// <param name="value">Input value under test.</param>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void IsNullOrWhiteSpace_ShouldReturnTrue_WhenValueIsWhitespace(string value)
    {
        var result = value.IsNullOrWhiteSpace();

        Assert.True(result);
    }

    /// <summary>
    /// Verifies IsNullOrWhiteSpace returns false when text exists.
    /// </summary>
    [Fact]
    public void IsNullOrWhiteSpace_ShouldReturnFalse_WhenValueHasText()
    {
        var result = "vc".IsNullOrWhiteSpace();

        Assert.False(result);
    }

    /// <summary>
    /// Verifies OrEmpty returns empty string for null values.
    /// </summary>
    [Fact]
    public void OrEmpty_ShouldReturnEmpty_WhenValueIsNull()
    {
        string? value = null;

        var result = value.OrEmpty();

        Assert.Equal(string.Empty, result);
    }

    /// <summary>
    /// Verifies OrEmpty returns the original value when non-null.
    /// </summary>
    [Fact]
    public void OrEmpty_ShouldReturnOriginalValue_WhenValueIsNotNull()
    {
        var value = "value";

        var result = value.OrEmpty();

        Assert.Equal(value, result);
    }
}
