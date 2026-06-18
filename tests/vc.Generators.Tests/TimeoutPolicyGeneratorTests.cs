using VisionaryCoder.Generators.Resilience;
using Xunit;

namespace vc.Generators.Tests;

public sealed class TimeoutPolicyGeneratorTests
{
    private static (Compilation, IEnumerable<string>) RunGenerator(string source)
    {
        const string attr = "namespace Vc.Generators.Abstractions.Resilience; [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)] public sealed class VcTimeoutPolicyAttribute : System.Attribute {}";
        var trees = ImmutableArray.Create(item1: CSharpSyntaxTree.ParseText(text: attr), item2: CSharpSyntaxTree.ParseText(text: source));
        var refs = AppDomain.CurrentDomain.GetAssemblies().Where(predicate: a => !a.IsDynamic && a.Location.Length > 0).Select(selector: a => MetadataReference.CreateFromFile(path: a.Location)).Distinct().ToArray();
        var comp = CSharpCompilation.Create(assemblyName: "Test", syntaxTrees: trees, references: refs, options: new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary));
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: new TimeoutPolicyGenerator());
        driver.RunGeneratorsAndUpdateCompilation(compilation: comp, outputCompilation: out var outComp, diagnostics: out _);
        return (outComp, outComp.SyntaxTrees.Skip(count: 2).Select(selector: st => st.GetText().ToString()));
    }

    [Fact]
    public void GeneratesTimeoutSettings_ForDecoratedClass()
    {
        var (_, sources) = RunGenerator(source: "using Vc.Generators.Abstractions.Resilience; namespace App; [VcTimeoutPolicy] public class PaymentService {}");
        Assert.NotEmpty(collection: sources);
        Assert.Contains(collection: sources, filter: s => s.Contains(value: "PaymentServiceTimeoutSettings"));
    }

    [Fact]
    public void GeneratedSettings_ContainsTimeoutValues()
    {
        var (_, sources) = RunGenerator(source: "using Vc.Generators.Abstractions.Resilience; namespace App; [VcTimeoutPolicy] public class PaymentService {}");
        var src = sources.First(predicate: s => s.Contains(value: "TimeoutSettings"));
        Assert.Contains(expectedSubstring: "DefaultTimeoutSeconds", actualString: src);
        Assert.Contains(expectedSubstring: "LongRunningTimeoutSeconds", actualString: src);
    }

    [Fact]
    public void GeneratedSettings_ContainsTimeoutConfiguration()
    {
        var (_, sources) = RunGenerator(source: "using Vc.Generators.Abstractions.Resilience; namespace App; [VcTimeoutPolicy] public class PaymentService {}");
        var src = sources.First(predicate: s => s.Contains(value: "TimeoutSettings"));
        Assert.Contains(expectedSubstring: "PaymentServiceTimeoutConfiguration", actualString: src);
    }

    [Fact]
    public void GeneratedOutput_ContainsAutoGenHeader()
    {
        var (_, sources) = RunGenerator(source: "using Vc.Generators.Abstractions.Resilience; namespace App; [VcTimeoutPolicy] public class PaymentService {}");
        Assert.Contains(collection: sources, filter: s => s.Contains(value: "auto-generated"));
    }

    [Fact]
    public void NonDecoratedClass_ProducesNoOutput()
    {
        var (_, sources) = RunGenerator(source: "namespace App; public class PaymentService {}");
        Assert.Empty(collection: sources);
    }
}
