using VisionaryCoder.Generators.Design.Vbd;
using Xunit;

namespace vc.Generators.Tests;

public sealed class VbdAccessGeneratorTests
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
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: new VbdAccessGenerator());
        driver.RunGeneratorsAndUpdateCompilation(compilation: comp, outputCompilation: out var outComp, diagnostics: out _);
        var generated = outComp.SyntaxTrees.Skip(count: comp.SyntaxTrees.Count())
            .Select(selector: st => st.GetText().ToString())
            .ToList();
        return (outComp, generated);
    }

    [Fact]
    public void GeneratesInterface_ForRepositoryClass()
    {
        var (_, sources) = RunGenerator(source: """
                                                namespace Orders;
                                                public class OrderRepository
                                                {
                                                    public Order? GetById(int id) => null;
                                                    public void Save(Order order) { }
                                                }
                                                public class Order { }
                                                """);

        Assert.NotEmpty(collection: sources);
        Assert.Contains(collection: sources, filter: s => s.Contains(value: "interface IOrderRepository"));
    }

    [Fact]
    public void GeneratedInterface_ContainsBoundaryComment()
    {
        var (_, sources) = RunGenerator(source: """
                                                namespace Data;
                                                public class UserStore
                                                {
                                                    public void Add(string name) { }
                                                }
                                                """);

        Assert.NotEmpty(collection: sources);
        var source = sources.First(predicate: s => s.Contains(value: "IUserStore"));
        Assert.Contains(expectedSubstring: "VBD0006", actualString: source);
        Assert.Contains(expectedSubstring: "auto-generated", actualString: source);
    }

    [Fact]
    public void GeneratedInterface_ContainsPublicMethods()
    {
        var (_, sources) = RunGenerator(source: """
                                                namespace Infra;
                                                public class PaymentGateway
                                                {
                                                    public bool ProcessPayment(decimal amount) => true;
                                                    private void Log(string msg) { }
                                                }
                                                """);

        Assert.NotEmpty(collection: sources);
        var source = sources.First(predicate: s => s.Contains(value: "IPaymentGateway"));
        Assert.Contains(expectedSubstring: "ProcessPayment", actualString: source);
        Assert.DoesNotContain(expectedSubstring: "Log(", actualString: source);
    }

    [Fact]
    public void NonAccessClass_ProducesNoOutput()
    {
        var (_, sources) = RunGenerator(source: """
                                                namespace App;
                                                public class UserService
                                                {
                                                    public void DoWork() { }
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
