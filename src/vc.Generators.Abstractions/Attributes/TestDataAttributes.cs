namespace Vc.Generators.Abstractions.TestData;

[AttributeUsage(AttributeTargets.Class)]
public sealed class VcTestDataAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Class)]
public sealed class VcFakeAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Class)]
public sealed class VcStubAttribute : Attribute {}

[System.AttributeUsage(System.AttributeTargets.Class)]
public sealed class VcFakeBuilderAttribute : System.Attribute { }
