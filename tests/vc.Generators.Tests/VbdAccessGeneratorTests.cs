using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VisionaryCoder.Generators_Design_Vbd;
using Xunit;

namespace VisionaryCoder.Tooling.Generators.Tests;

public sealed class VbdAccessGeneratorTests
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
        var driver = CSharpGeneratorDriver.Create(new VbdAccessGenerator());
        driver.RunGeneratorsAndUpdateCompilation(comp, out var outComp, out _);
        var generated = outComp.SyntaxTrees.Skip(comp.SyntaxTrees.Count())
            .Select(st => st.GetText().ToString())
            .ToList();
        return (outComp, generated);
    }

    [Fact]
    public void GeneratesInterface_ForRepositoryClass()
    {
        var (_, sources) = RunGenerator("""
            namespace Orders;
            public class OrderRepository
            {
                public Order? GetById(int id) => null;
                public void Save(Order order) { }
            }
            public class Order { }
            """);

        Assert.NotEmpty(sources);
        Assert.Contains(sources, s => s.Contains("interface IOrderRepository"));
    }

    [Fact]
    public void GeneratedInterface_ContainsBoundaryComment()
    {
        var (_, sources) = RunGenerator("""
            namespace Data;
            public class UserStore
            {
                public void Add(string name) { }
            }
            """);

        Assert.NotEmpty(sources);
        var source = sources.First(s => s.Contains("IUserStore"));
        Assert.Contains("VBD0006", source);
        Assert.Contains("auto-generated", source);
    }

    [Fact]
    public void GeneratedInterface_ContainsPublicMethods()
    {
        var (_, sources) = RunGenerator("""
            namespace Infra;
            public class PaymentGateway
            {
                public bool ProcessPayment(decimal amount) => true;
                private void Log(string msg) { }
            }
            """);

        Assert.NotEmpty(sources);
        var source = sources.First(s => s.Contains("IPaymentGateway"));
        Assert.Contains("ProcessPayment", source);
        Assert.DoesNotContain("Log(", source);
    }

    [Fact]
    public void NonAccessClass_ProducesNoOutput()
    {
        var (_, sources) = RunGenerator("""
            namespace App;
            public class UserService
            {
                public void DoWork() { }
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
