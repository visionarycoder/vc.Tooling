namespace Vc.Generators.Abstractions.Domain;

[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
public sealed class VcStrongIdAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Class)]
public sealed class VcValueObjectAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Class)]
public sealed class VcUnionAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Class)]
public sealed class VcResultAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Class)]
public sealed class VcDomainEventAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Interface)]
public sealed class VcEventPublisherAttribute : Attribute {}
