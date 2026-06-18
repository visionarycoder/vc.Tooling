namespace VisionaryCoder.Architecture;

/// <summary>
/// Specifies the role of a component within the architecture.
/// This attribute is used to annotate classes, interfaces, or structs
/// with a specific <see cref="ComponentRole"/> to indicate their purpose or responsibility.
/// </summary>
/// <remarks>
/// The <see cref="ComponentAttribute"/> is designed to enhance the clarity of architectural intent
/// by categorizing components based on their roles, such as Manager, Engine, Access, Adapter, Service, Utility, etc.
/// </remarks>
[AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class ComponentAttribute(ComponentRole role) : Attribute
{
    /// <summary>
    /// Gets the role of the component.
    /// </summary>
    public ComponentRole Role { get; } = role;
}
