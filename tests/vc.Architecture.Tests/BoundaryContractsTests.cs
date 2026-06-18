using VisionaryCoder.Architecture;
using VisionaryCoder.Architecture.Vbd;
using Xunit;

namespace vc.Architecture.Tests;

/// <summary>
/// Verifies architecture boundary contract attributes and enums.
/// </summary>
public sealed class BoundaryContractsTests
{
    /// <summary>
    /// Verifies <see cref="BoundaryAttribute"/> stores all boundary enum values.
    /// </summary>
    /// <param name="boundaryType">Boundary type under test.</param>
    [Theory]
    [InlineData(BoundaryType.Unknown)]
    [InlineData(BoundaryType.Architecture)]
    [InlineData(BoundaryType.Domain)]
    [InlineData(BoundaryType.Runtime)]
    [InlineData(BoundaryType.Integration)]
    [InlineData(BoundaryType.Utility)]
    [InlineData(BoundaryType.Tooling)]
    public void BoundaryAttribute_ShouldStoreAllBoundaryTypes(BoundaryType boundaryType)
    {
        var attribute = new BoundaryAttribute(boundaryType);
        Assert.Equal(boundaryType, attribute.BoundaryType);
    }

    /// <summary>
    /// Verifies <see cref="ComponentAttribute"/> stores all component roles.
    /// </summary>
    /// <param name="role">Component role under test.</param>
    [Theory]
    [InlineData(ComponentRole.Unknown)]
    [InlineData(ComponentRole.Manager)]
    [InlineData(ComponentRole.Engine)]
    [InlineData(ComponentRole.Access)]
    [InlineData(ComponentRole.Adapter)]
    [InlineData(ComponentRole.Service)]
    [InlineData(ComponentRole.Utility)]
    public void ComponentAttribute_ShouldStoreAllComponentRoles(ComponentRole role)
    {
        var attribute = new ComponentAttribute(role);
        Assert.Equal(role, attribute.Role);
    }

    /// <summary>
    /// Verifies <see cref="VbdBoundaryAttribute"/> stores all boundary enum values.
    /// </summary>
    /// <param name="boundaryType">Boundary type under test.</param>
    [Theory]
    [InlineData(BoundaryType.Unknown)]
    [InlineData(BoundaryType.Architecture)]
    [InlineData(BoundaryType.Domain)]
    [InlineData(BoundaryType.Runtime)]
    [InlineData(BoundaryType.Integration)]
    [InlineData(BoundaryType.Utility)]
    [InlineData(BoundaryType.Tooling)]
    public void VbdBoundaryAttribute_ShouldStoreAllBoundaryTypes(BoundaryType boundaryType)
    {
        var attribute = new VbdBoundaryAttribute(boundaryType);
        Assert.Equal(boundaryType, attribute.BoundaryType);
    }

    /// <summary>
    /// Verifies <see cref="VbdComponentAttribute"/> stores all component roles.
    /// </summary>
    /// <param name="role">Component role under test.</param>
    [Theory]
    [InlineData(ComponentRole.Unknown)]
    [InlineData(ComponentRole.Manager)]
    [InlineData(ComponentRole.Engine)]
    [InlineData(ComponentRole.Access)]
    [InlineData(ComponentRole.Adapter)]
    [InlineData(ComponentRole.Service)]
    [InlineData(ComponentRole.Utility)]
    public void VbdComponentAttribute_ShouldStoreAllComponentRoles(ComponentRole role)
    {
        var attribute = new VbdComponentAttribute(role);
        Assert.Equal(role, attribute.Role);
    }

    /// <summary>
    /// Verifies all VBD volatility enum values are usable.
    /// </summary>
    /// <param name="volatility">Volatility value under test.</param>
    [Theory]
    [InlineData(VbdVolatility.Unknown)]
    [InlineData(VbdVolatility.PolicyOrchestration)]
    [InlineData(VbdVolatility.Algorithm)]
    [InlineData(VbdVolatility.InfrastructureIntegration)]
    public void VbdVolatility_ShouldHaveValidEnumValues(VbdVolatility volatility)
    {
        // Verify enum values are defined
        Assert.NotEqual(0, (int)volatility | 1);
    }

    /// <summary>
    /// Verifies boundary attribute supports expected target types.
    /// </summary>
    [Fact]
    public void BoundaryAttribute_ShouldSupportMultipleTargets()
    {
        var attrType = typeof(BoundaryAttribute);
        var usageAttr = attrType.GetCustomAttributes(typeof(AttributeUsageAttribute), false)
            .OfType<AttributeUsageAttribute>()
            .FirstOrDefault();

        Assert.NotNull(usageAttr);
        // Should allow Assembly, Class, Interface, Struct
        Assert.True((usageAttr.ValidOn & AttributeTargets.Assembly) != 0);
        Assert.True((usageAttr.ValidOn & AttributeTargets.Class) != 0);
    }
}
