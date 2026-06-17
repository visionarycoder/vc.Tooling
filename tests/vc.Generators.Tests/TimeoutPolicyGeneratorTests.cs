using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace VisionaryCoder.Tooling.Generators.Tests;

public sealed class TimeoutPolicyGeneratorTests
{
    private static (Compilation, IEnumerable<string>) RunGenerator(string source)
    {
        const string attr = "namespace Vc.Generators.Abstractions.Resilience; [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)] public sealed class VcTimeoutPolicyAttribute : System.Attribute {}";
        var trees = ImmutableArray.Create(CSharpSyntaxTree.ParseText(attr), CSharpSyntaxTree.ParseText(source));
        var refs = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic && a.Location.Length > 0).Select(a => MetadataReference.CreateFromFile(a.Location)).Distinct().ToArray();
        var comp = CSharpCompilation.Create("Test", trees, refs, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var driver = CSharpGeneratorDriver.Create(new TimeoutPolicyGenerator());
        driver.RunGeneratorsAndUpdateCompilation(comp, out var outComp, out _);
        return (outComp, outComp.SyntaxTrees.Skip(2).Select(st => st.GetText().ToString()));
    }

    [Fact]
    public void GeneratesTimeoutSettings_ForDecoratedClass()
    {
        var (_, sources) = RunGenerator("using Vc.Generators.Abstractions.Resilience; namespace App; [VcTimeoutPolicy] public class PaymentService {}");
        Assert.NotEmpty(sources);
        Assert.Contains(sources, s => s.Contains("PaymentServiceTimeoutSettings"));
    }

    [Fact]
    public void GeneratedSettings_ContainsTimeoutValues()
    {
        var (_, sources) = RunGenerator("using Vc.Generators.Abstractions.Resilience; namespace App; [VcTimeoutPolicy] public class PaymentService {}");
        var src = sources.First(s => s.Contains("TimeoutSettings"));
        Assert.Contains("DefaultTimeoutSeconds", src);
        Assert.Contains("LongRunningTimeoutSeconds", src);
    }

    [Fact]
    public void GeneratedSettings_ContainsTimeoutConfiguration()
    {
        var (_, sources) = RunGenerator("using Vc.Generators.Abstractions.Resilience; namespace App; [VcTimeoutPolicy] public class PaymentService {}");
        var src = sources.First(s => s.Contains("TimeoutSettings"));
        Assert.Contains("PaymentServiceTimeoutConfiguration", src);
    }

    [Fact]
    public void GeneratedOutput_ContainsAutoGenHeader()
    {
        var (_, sources) = RunGenerator("using Vc.Generators.Abstractions.Resilience; namespace App; [VcTimeoutPolicy] public class PaymentService {}");
        Assert.Contains(sources, s => s.Contains("auto-generated"));
    }

    [Fact]
    public void NonDecoratedClass_ProducesNoOutput()
    {
        var (_, sources) = RunGenerator("namespace App; public class PaymentService {}");
        Assert.Empty(sources);
    }
}
