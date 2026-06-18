using VisionaryCoder.Generators.Resilience;
using Xunit;

namespace vc.Generators.Tests;

public sealed class RetryPolicyGeneratorTests
{
    private static (Compilation, IEnumerable<string>) RunGenerator(string source)
    {
        const string attr = "namespace Vc.Generators.Abstractions.Resilience; [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)] public sealed class VcRetryPolicyAttribute : System.Attribute {}";
        var trees = ImmutableArray.Create(item1: CSharpSyntaxTree.ParseText(text: attr), item2: CSharpSyntaxTree.ParseText(text: source));
        var refs = AppDomain.CurrentDomain.GetAssemblies().Where(predicate: a => !a.IsDynamic && a.Location.Length > 0).Select(selector: a => MetadataReference.CreateFromFile(path: a.Location)).Distinct().ToArray();
        var comp = CSharpCompilation.Create(assemblyName: "Test", syntaxTrees: trees, references: refs, options: new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary));
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: new RetryPolicyGenerator());
        driver.RunGeneratorsAndUpdateCompilation(compilation: comp, outputCompilation: out var outComp, diagnostics: out _);
        return (outComp, outComp.SyntaxTrees.Skip(count: 2).Select(selector: st => st.GetText().ToString()));
    }

    [Fact]
    public void GeneratesRetrySettings_ForDecoratedClass()
    {
        var (_, sources) = RunGenerator(source: "using Vc.Generators.Abstractions.Resilience; namespace App; [VcRetryPolicy] public class OrderService {}");
        Assert.NotEmpty(collection: sources);
        Assert.Contains(collection: sources, filter: s => s.Contains(value: "OrderServiceRetrySettings"));
    }

    [Fact]
    public void GeneratedSettings_ContainsMaxAttempts()
    {
        var (_, sources) = RunGenerator(source: "using Vc.Generators.Abstractions.Resilience; namespace App; [VcRetryPolicy] public class OrderService {}");
        var src = sources.First(predicate: s => s.Contains(value: "RetrySettings"));
        Assert.Contains(expectedSubstring: "MaxAttempts", actualString: src);
        Assert.Contains(expectedSubstring: "BaseDelaySeconds", actualString: src);
    }

    [Fact]
    public void GeneratedSettings_ContainsRetryConfiguration()
    {
        var (_, sources) = RunGenerator(source: "using Vc.Generators.Abstractions.Resilience; namespace App; [VcRetryPolicy] public class OrderService {}");
        var src = sources.First(predicate: s => s.Contains(value: "RetrySettings"));
        Assert.Contains(expectedSubstring: "OrderServiceRetryConfiguration", actualString: src);
    }

    [Fact]
    public void GeneratedOutput_ContainsAutoGenHeader()
    {
        var (_, sources) = RunGenerator(source: "using Vc.Generators.Abstractions.Resilience; namespace App; [VcRetryPolicy] public class OrderService {}");
        Assert.Contains(collection: sources, filter: s => s.Contains(value: "auto-generated"));
    }

    [Fact]
    public void NonDecoratedClass_ProducesNoOutput()
    {
        var (_, sources) = RunGenerator(source: "namespace App; public class OrderService {}");
        Assert.Empty(collection: sources);
    }
}
