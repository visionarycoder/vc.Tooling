using VisionaryCoder.Architecture;
using VisionaryCoder.Architecture.Vbd;
using Xunit;

namespace VisionaryCoder.Architecture.Tests;

public sealed class BoundaryContractsTests
{
    [Fact]
    public void BoundaryAttribute_ShouldExposeConfiguredBoundaryType()
    {
        var attribute = new BoundaryAttribute(BoundaryType.Runtime);

        Assert.Equal(BoundaryType.Runtime, attribute.BoundaryType);
    }

    [Fact]
    public void ComponentAttribute_ShouldExposeConfiguredRole()
    {
        var attribute = new ComponentAttribute(ComponentRole.Engine);

        Assert.Equal(ComponentRole.Engine, attribute.Role);
    }

    [Fact]
    public void VbdBoundaryAttribute_ShouldExposeConfiguredBoundaryType()
    {
        var attribute = new VbdBoundaryAttribute(BoundaryType.Integration);

        Assert.Equal(BoundaryType.Integration, attribute.BoundaryType);
    }

    [Fact]
    public void VbdComponentAttribute_ShouldExposeConfiguredRole()
    {
        var attribute = new VbdComponentAttribute(ComponentRole.Access);

        Assert.Equal(ComponentRole.Access, attribute.Role);
    }
}
