namespace Vc.Generators.Abstractions.Dx;

[AttributeUsage(AttributeTargets.Class)]
public sealed class VcNotifyPropertyChangedAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Class)]
public sealed class VcBuilderAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Class)]
public sealed class VcMapperAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Class)]
public sealed class VcOptionsAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Class)]
public sealed class VcBuilderPatternAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class)]
public sealed class VcEqualityAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class)]
public sealed class VcOptionsBindingAttribute : Attribute { }
