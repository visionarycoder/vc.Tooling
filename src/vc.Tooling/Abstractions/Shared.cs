using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class SecureAttribute : Attribute
{
    public string Permission { get; }
    public SecureAttribute(string permission) => Permission = permission;
}

[AttributeUsage(AttributeTargets.Class)]
public sealed class PermissionGroupAttribute : Attribute
{
    public string Name { get; }
    public PermissionGroupAttribute(string name) => Name = name;
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class RequiresRoleAttribute : Attribute
{
    public string Role { get; }
    public RequiresRoleAttribute(string role) => Role = role;
}
