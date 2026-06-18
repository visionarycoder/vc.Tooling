using VisionaryCoder.Utility;
using Xunit;

namespace vc.Utility.Tests;

/// <summary>
/// Tests for <see cref="VisionaryCoder.Utility.Guard"/> helpers.
/// </summary>
public sealed class GuardTests
{
    /// <summary>
    /// Verifies NotNull returns value for non-null input.
    /// </summary>
    [Fact]
    public void NotNull_ShouldReturnValue_WhenValueIsNotNull()
    {
        var value = "abc";

        var result = Guard.NotNull(value, nameof(value));

        Assert.Same(value, result);
    }

    /// <summary>
    /// Verifies NotNull throws for null input.
    /// </summary>
    [Fact]
    public void NotNull_ShouldThrowArgumentNullException_WhenValueIsNull()
    {
        string? value = null;

        var exception = Assert.Throws<ArgumentNullException>(() => Guard.NotNull(value, nameof(value)));

        Assert.Equal(nameof(value), exception.ParamName);
    }

    /// <summary>
    /// Verifies NotNullOrWhiteSpace returns value for valid input.
    /// </summary>
    [Fact]
    public void NotNullOrWhiteSpace_ShouldReturnValue_WhenValueIsValid()
    {
        var value = "valid";

        var result = Guard.NotNullOrWhiteSpace(value, nameof(value));

        Assert.Equal(value, result);
    }

    /// <summary>
    /// Verifies NotNullOrWhiteSpace throws for empty or whitespace input.
    /// </summary>
    /// <param name="value">Input value under test.</param>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NotNullOrWhiteSpace_ShouldThrowArgumentException_WhenValueIsEmptyOrWhitespace(string value)
    {
        var exception = Assert.Throws<ArgumentException>(() => Guard.NotNullOrWhiteSpace(value, nameof(value)));

        Assert.Equal(nameof(value), exception.ParamName);
    }

    /// <summary>
    /// Verifies NotNullOrWhiteSpace throws for null input.
    /// </summary>
    [Fact]
    public void NotNullOrWhiteSpace_ShouldThrowArgumentException_WhenValueIsNull()
    {
        string? value = null;

        var exception = Assert.Throws<ArgumentException>(() => Guard.NotNullOrWhiteSpace(value, nameof(value)));

        Assert.Equal(nameof(value), exception.ParamName);
    }
}
