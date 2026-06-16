using System;
using VisionaryCoder.Utility;
using Xunit;

namespace VisionaryCoder.Utility.Tests;

public sealed class GuardTests
{
    [Fact]
    public void NotNull_ShouldReturnValue_WhenValueIsNotNull()
    {
        var value = "abc";

        var result = Guard.NotNull(value, nameof(value));

        Assert.Same(value, result);
    }

    [Fact]
    public void NotNull_ShouldThrowArgumentNullException_WhenValueIsNull()
    {
        string? value = null;

        var exception = Assert.Throws<ArgumentNullException>(() => Guard.NotNull(value, nameof(value)));

        Assert.Equal(nameof(value), exception.ParamName);
    }

    [Fact]
    public void NotNullOrWhiteSpace_ShouldReturnValue_WhenValueIsValid()
    {
        var value = "valid";

        var result = Guard.NotNullOrWhiteSpace(value, nameof(value));

        Assert.Equal(value, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NotNullOrWhiteSpace_ShouldThrowArgumentException_WhenValueIsEmptyOrWhitespace(string value)
    {
        var exception = Assert.Throws<ArgumentException>(() => Guard.NotNullOrWhiteSpace(value, nameof(value)));

        Assert.Equal(nameof(value), exception.ParamName);
    }

    [Fact]
    public void NotNullOrWhiteSpace_ShouldThrowArgumentException_WhenValueIsNull()
    {
        string? value = null;

        var exception = Assert.Throws<ArgumentException>(() => Guard.NotNullOrWhiteSpace(value, nameof(value)));

        Assert.Equal(nameof(value), exception.ParamName);
    }
}
