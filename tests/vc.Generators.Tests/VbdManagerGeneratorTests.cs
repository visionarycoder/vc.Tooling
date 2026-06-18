using VisionaryCoder.Generators.Design.Vbd;
using Xunit;

namespace vc.Generators.Tests;

public sealed class VbdManagerGeneratorTests
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
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: new VbdManagerGenerator());
        driver.RunGeneratorsAndUpdateCompilation(compilation: comp, outputCompilation: out var outComp, diagnostics: out _);
        var generated = outComp.SyntaxTrees.Skip(count: comp.SyntaxTrees.Count())
            .Select(selector: st => st.GetText().ToString())
            .ToList();
        return (outComp, generated);
    }

    [Fact]
    public void GeneratesInterface_ForManagerClass()
    {
        var (_, sources) = RunGenerator(source: """
                                                namespace Orders;
                                                public class OrderManager
                                                {
                                                    public void PlaceOrder(string customerId, decimal amount) { }
                                                    public void CancelOrder(int orderId) { }
                                                }
                                                """);

        Assert.NotEmpty(collection: sources);
        Assert.Contains(collection: sources, filter: s => s.Contains(value: "interface IOrderManager"));
    }

    [Fact]
    public void GeneratedInterface_ContainsOrchestrationComment()
    {
        var (_, sources) = RunGenerator(source: """
                                                namespace Billing;
                                                public class PaymentManager
                                                {
                                                    public bool Authorize(decimal amount) => true;
                                                }
                                                """);

        Assert.NotEmpty(collection: sources);
        var source = sources.First(predicate: s => s.Contains(value: "IPaymentManager"));
        Assert.Contains(expectedSubstring: "VBD0006", actualString: source);
        Assert.Contains(expectedSubstring: "orchestration contract", actualString: source);
    }

    [Fact]
    public void GeneratedInterface_ContainsPublicMethods()
    {
        var (_, sources) = RunGenerator(source: """
                                                namespace App;
                                                public class UserManager
                                                {
                                                    public void CreateUser(string name) { }
                                                    public void DeleteUser(int id) { }
                                                    private void Audit(string action) { }
                                                }
                                                """);

        Assert.NotEmpty(collection: sources);
        var source = sources.First(predicate: s => s.Contains(value: "IUserManager"));
        Assert.Contains(expectedSubstring: "CreateUser", actualString: source);
        Assert.Contains(expectedSubstring: "DeleteUser", actualString: source);
        Assert.DoesNotContain(expectedSubstring: "Audit(", actualString: source);
    }

    [Fact]
    public void NonManagerClass_ProducesNoOutput()
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
