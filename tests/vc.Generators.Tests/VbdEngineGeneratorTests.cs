using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VisionaryCoder.Generators_Design_Vbd;
using Xunit;

namespace VisionaryCoder.Tooling.Generators.Tests;

public sealed class VbdEngineGeneratorTests
{
    private static (Compilation, IEnumerable<string>) RunGenerator(string source)
    {
        var tree = CSharpSyntaxTree.ParseText(source);
        var refs = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && a.Location.Length > 0)
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Distinct()
            .ToArray();
        var comp = CSharpCompilation.Create("Test", [tree], refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var driver = CSharpGeneratorDriver.Create(new VbdEngineGenerator());
        driver.RunGeneratorsAndUpdateCompilation(comp, out var outComp, out _);
        var generated = outComp.SyntaxTrees.Skip(comp.SyntaxTrees.Count())
            .Select(st => st.GetText().ToString())
            .ToList();
        return (outComp, generated);
    }

    [Fact]
    public void GeneratesInterface_ForEngineClass()
    {
        var (_, sources) = RunGenerator("""
            namespace Domain;
            public class RiskEngine
            {
                public decimal ComputeScore(int age, decimal income) => 0m;
            }
            """);

        Assert.NotEmpty(sources);
        Assert.Contains(sources, s => s.Contains("interface IRiskEngine"));
    }

    [Fact]
    public void GeneratedInterface_ContainsCapabilityComment()
    {
        var (_, sources) = RunGenerator("""
            namespace Algo;
            public class PricingEngine
            {
                public decimal Calculate(decimal basePrice) => basePrice;
            }
            """);

        Assert.NotEmpty(sources);
        var source = sources.First(s => s.Contains("IPricingEngine"));
        Assert.Contains("VBD0006", source);
        Assert.Contains("capability contract", source);
    }

    [Fact]
    public void GeneratedInterface_ContainsPublicMethods()
    {
        var (_, sources) = RunGenerator("""
            namespace Core;
            public class ValidationEngine
            {
                public bool Validate(string input) => true;
                private void Log(string msg) { }
            }
            """);

        Assert.NotEmpty(sources);
        var source = sources.First(s => s.Contains("IValidationEngine"));
        Assert.Contains("Validate", source);
        Assert.DoesNotContain("Log(", source);
    }

    [Fact]
    public void NonEngineClass_ProducesNoOutput()
    {
        var (_, sources) = RunGenerator("""
            namespace App;
            public class OrderService
            {
                public void Process() { }
            }
            """);

        Assert.Empty(sources);
    }

    [Fact]
    public void GeneratorRuns_WithoutException()
    {
        var (_, _) = RunGenerator("namespace App; public class Sample {}");
        Assert.True(true);
    }
}
