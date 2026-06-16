namespace Vc.Generators.Abstractions.Data;

[AttributeUsage(AttributeTargets.Interface)]
public sealed partial class VcRepositoryAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Class)]
public sealed partial class VcCommandHandlerAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Class)]
public sealed partial class VcQueryHandlerAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Class)]
public sealed partial class VcProjectionAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Class)]
public sealed partial class VcAuditAttribute : Attribute {}

[System.AttributeUsage(System.AttributeTargets.Interface | System.AttributeTargets.Class)]
public sealed class VcRepositoryAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class)]
public sealed class VcCommandHandlerAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class)]
public sealed class VcQueryHandlerAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class)]
public sealed class VcProjectionAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class)]
public sealed class VcAuditTrailAttribute : System.Attribute { }
