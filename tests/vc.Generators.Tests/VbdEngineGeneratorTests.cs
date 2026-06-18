using VisionaryCoder.Generators.Design.Vbd;
using Xunit;

namespace vc.Generators.Tests;

public sealed class VbdEngineGeneratorTests
{
    private static (Compilation, IEnumerable<string>) RunGenerator(string source)
    {
        var tree = CSharpSyntaxTree.ParseText(text: source);
        var refs = AppDomain.CurrentDomain.GetAssemblies()
            .Where(predicate: a => !a.IsDynamic && a.Location.Length > 0)
            .Select(selector: a => MetadataReference.CreateFromFile(path: a.Location))
            .Distinct()
            .ToArray();
        var comp = CSharpCompilation.Create(assemblyName: "Test", syntaxTrees: [tree], references: refs,
            options: new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary));
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: new VbdEngineGenerator());
        driver.RunGeneratorsAndUpdateCompilation(compilation: comp, outputCompilation: out var outComp, diagnostics: out _);
        var generated = outComp.SyntaxTrees.Skip(count: comp.SyntaxTrees.Count())
            .Select(selector: st => st.GetText().ToString())
            .ToList();
        return (outComp, generated);
    }

    [Fact]
    public void GeneratesInterface_ForEngineClass()
    {
        var (_, sources) = RunGenerator(source: """
                                                namespace Domain;
                                                public class RiskEngine
                                                {
                                                    public decimal ComputeScore(int age, decimal income) => 0m;
                                                }
                                                """);

        Assert.NotEmpty(collection: sources);
        Assert.Contains(collection: sources, filter: s => s.Contains(value: "interface IRiskEngine"));
    }

    [Fact]
    public void GeneratedInterface_ContainsCapabilityComment()
    {
        var (_, sources) = RunGenerator(source: """
                                                namespace Algo;
                                                public class PricingEngine
                                                {
                                                    public decimal Calculate(decimal basePrice) => basePrice;
                                                }
                                                """);

        Assert.NotEmpty(collection: sources);
        var source = sources.First(predicate: s => s.Contains(value: "IPricingEngine"));
        Assert.Contains(expectedSubstring: "VBD0006", actualString: source);
        Assert.Contains(expectedSubstring: "capability contract", actualString: source);
    }

    [Fact]
    public void GeneratedInterface_ContainsPublicMethods()
    {
        var (_, sources) = RunGenerator(source: """
                                                namespace Core;
                                                public class ValidationEngine
                                                {
                                                    public bool Validate(string input) => true;
                                                    private void Log(string msg) { }
                                                }
                                                """);

        Assert.NotEmpty(collection: sources);
        var source = sources.First(predicate: s => s.Contains(value: "IValidationEngine"));
        Assert.Contains(expectedSubstring: "Validate", actualString: source);
        Assert.DoesNotContain(expectedSubstring: "Log(", actualString: source);
    }

    [Fact]
    public void NonEngineClass_ProducesNoOutput()
    {
        var (_, sources) = RunGenerator(source: """
                                                namespace App;
                                                public class OrderService
                                                {
                                                    public void Process() { }
                                                }
                                                """);

        Assert.Empty(collection: sources);
    }

    [Fact]
    public void GeneratorRuns_WithoutException()
    {
        var (_, _) = RunGenerator(source: "namespace App; public class Sample {}");
        Assert.True(condition: true);
    }
}
