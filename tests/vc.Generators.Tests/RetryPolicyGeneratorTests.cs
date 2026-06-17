using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace VisionaryCoder.Tooling.Generators.Tests;

public sealed class RetryPolicyGeneratorTests
{
    private static (Compilation, IEnumerable<string>) RunGenerator(string source)
    {
        const string attr = "namespace Vc.Generators.Abstractions.Resilience; [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)] public sealed class VcRetryPolicyAttribute : System.Attribute {}";
        var trees = ImmutableArray.Create(CSharpSyntaxTree.ParseText(attr), CSharpSyntaxTree.ParseText(source));
        var refs = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic && a.Location.Length > 0).Select(a => MetadataReference.CreateFromFile(a.Location)).Distinct().ToArray();
        var comp = CSharpCompilation.Create("Test", trees, refs, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var driver = CSharpGeneratorDriver.Create(new RetryPolicyGenerator());
        driver.RunGeneratorsAndUpdateCompilation(comp, out var outComp, out _);
        return (outComp, outComp.SyntaxTrees.Skip(2).Select(st => st.GetText().ToString()));
    }

    [Fact]
    public void GeneratesRetrySettings_ForDecoratedClass()
    {
        var (_, sources) = RunGenerator("using Vc.Generators.Abstractions.Resilience; namespace App; [VcRetryPolicy] public class OrderService {}");
        Assert.NotEmpty(sources);
        Assert.Contains(sources, s => s.Contains("OrderServiceRetrySettings"));
    }

    [Fact]
    public void GeneratedSettings_ContainsMaxAttempts()
    {
        var (_, sources) = RunGenerator("using Vc.Generators.Abstractions.Resilience; namespace App; [VcRetryPolicy] public class OrderService {}");
        var src = sources.First(s => s.Contains("RetrySettings"));
        Assert.Contains("MaxAttempts", src);
        Assert.Contains("BaseDelaySeconds", src);
    }

    [Fact]
    public void GeneratedSettings_ContainsRetryConfiguration()
    {
        var (_, sources) = RunGenerator("using Vc.Generators.Abstractions.Resilience; namespace App; [VcRetryPolicy] public class OrderService {}");
        var src = sources.First(s => s.Contains("RetrySettings"));
        Assert.Contains("OrderServiceRetryConfiguration", src);
    }

    [Fact]
    public void GeneratedOutput_ContainsAutoGenHeader()
    {
        var (_, sources) = RunGenerator("using Vc.Generators.Abstractions.Resilience; namespace App; [VcRetryPolicy] public class OrderService {}");
        Assert.Contains(sources, s => s.Contains("auto-generated"));
    }

    [Fact]
    public void NonDecoratedClass_ProducesNoOutput()
    {
        var (_, sources) = RunGenerator("namespace App; public class OrderService {}");
        Assert.Empty(sources);
    }
}
