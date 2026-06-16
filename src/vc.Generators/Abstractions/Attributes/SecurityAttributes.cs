namespace Vc.Generators.Abstractions.Attributes;

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcAuthorizationPolicyAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcAuthenticationFlowAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcSecretAccessWrapperAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcInputSanitizationAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcJwtValidationAttribute : System.Attribute { }

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed partial class VcAuthorizeAttribute : Attribute
{
    public string? Policy { get; }
    public VcAuthorizeAttribute(string? policy = null) => Policy = policy;
}

[AttributeUsage(AttributeTargets.Method)]
public sealed partial class VcSanitizeInputAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed partial class VcJwtRequiredAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Class)]
public sealed partial class VcSecretAttribute : Attribute {}

