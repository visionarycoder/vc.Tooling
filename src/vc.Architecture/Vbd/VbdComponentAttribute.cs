namespace VisionaryCoder.Architecture.Vbd;

/// <summary>
/// Marks a type with its VBD component role.
/// </summary>
/// <param name="role">The architectural role assigned to the annotated component.</param>
[AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class VbdComponentAttribute(ComponentRole role) : Attribute
{
    /// <summary>
    /// Gets the role declared for the annotated component.
    /// </summary>
    public ComponentRole Role { get; } = role;
}
