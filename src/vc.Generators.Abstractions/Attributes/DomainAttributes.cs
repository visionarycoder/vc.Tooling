namespace Vc.Generators.Abstractions.Domain;

[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
public sealed partial class VcStrongIdAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Class)]
public sealed partial class VcValueObjectAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Class)]
public sealed partial class VcUnionAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Class)]
public sealed partial class VcResultAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Class)]
public sealed partial class VcDomainEventAttribute : Attribute {}

[System.AttributeUsage(System.AttributeTargets.Struct | System.AttributeTargets.Class)]
public sealed class VcStrongIdAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class)]
public sealed class VcValueObjectAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class)]
public sealed class VcUnionTypeAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Struct | System.AttributeTargets.Class)]
public sealed class VcResultTypeAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class)]
public sealed class VcDomainEventAttribute : System.Attribute { }
