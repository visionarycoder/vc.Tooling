namespace VisionaryCoder.Tooling.Abstractions;

[AttributeUsage(validOn: AttributeTargets.Method)]
public sealed class RequiresRoleAttribute(string role) : Attribute
{
    public string Role { get; } = role;
}