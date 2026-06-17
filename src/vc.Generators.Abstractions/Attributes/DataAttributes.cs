namespace Vc.Generators.Abstractions.Data;

[AttributeUsage(AttributeTargets.Interface)]
public sealed class VcRepositoryAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Class)]
public sealed class VcCommandHandlerAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Class)]
public sealed class VcQueryHandlerAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Class)]
public sealed class VcProjectionAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Class)]
public sealed class VcAuditAttribute : Attribute {}
