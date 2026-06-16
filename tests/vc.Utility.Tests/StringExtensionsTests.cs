using VisionaryCoder.Utility;
using Xunit;

namespace VisionaryCoder.Utility.Tests;

public sealed class StringExtensionsTests
{
    [Fact]
    public void IsNullOrWhiteSpace_ShouldReturnTrue_WhenValueIsNull()
    {
        string? value = null;

        var result = value.IsNullOrWhiteSpace();

        Assert.True(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void IsNullOrWhiteSpace_ShouldReturnTrue_WhenValueIsWhitespace(string value)
    {
        var result = value.IsNullOrWhiteSpace();

        Assert.True(result);
    }

    [Fact]
    public void IsNullOrWhiteSpace_ShouldReturnFalse_WhenValueHasText()
    {
        var result = "vc".IsNullOrWhiteSpace();

        Assert.False(result);
    }

    [Fact]
    public void OrEmpty_ShouldReturnEmpty_WhenValueIsNull()
    {
        string? value = null;

        var result = value.OrEmpty();

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void OrEmpty_ShouldReturnOriginalValue_WhenValueIsNotNull()
    {
        var value = "value";

        var result = value.OrEmpty();

        Assert.Equal(value, result);
    }
}
