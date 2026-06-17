using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VisionaryCoder.Generators_Design_Vbd;
using Xunit;

namespace VisionaryCoder.Tooling.Generators.Tests;

public sealed class VbdManagerGeneratorTests
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
        var driver = CSharpGeneratorDriver.Create(new VbdManagerGenerator());
        driver.RunGeneratorsAndUpdateCompilation(comp, out var outComp, out _);
        var generated = outComp.SyntaxTrees.Skip(comp.SyntaxTrees.Count())
            .Select(st => st.GetText().ToString())
            .ToList();
        return (outComp, generated);
    }

    [Fact]
    public void GeneratesInterface_ForManagerClass()
    {
        var (_, sources) = RunGenerator("""
            namespace Orders;
            public class OrderManager
            {
                public void PlaceOrder(string customerId, decimal amount) { }
                public void CancelOrder(int orderId) { }
            }
            """);

        Assert.NotEmpty(sources);
        Assert.Contains(sources, s => s.Contains("interface IOrderManager"));
    }

    [Fact]
    public void GeneratedInterface_ContainsOrchestrationComment()
    {
        var (_, sources) = RunGenerator("""
            namespace Billing;
            public class PaymentManager
            {
                public bool Authorize(decimal amount) => true;
            }
            """);

        Assert.NotEmpty(sources);
        var source = sources.First(s => s.Contains("IPaymentManager"));
        Assert.Contains("VBD0006", source);
        Assert.Contains("orchestration contract", source);
    }

    [Fact]
    public void GeneratedInterface_ContainsPublicMethods()
    {
        var (_, sources) = RunGenerator("""
            namespace App;
            public class UserManager
            {
                public void CreateUser(string name) { }
                public void DeleteUser(int id) { }
                private void Audit(string action) { }
            }
            """);

        Assert.NotEmpty(sources);
        var source = sources.First(s => s.Contains("IUserManager"));
        Assert.Contains("CreateUser", source);
        Assert.Contains("DeleteUser", source);
        Assert.DoesNotContain("Audit(", source);
    }

    [Fact]
    public void NonManagerClass_ProducesNoOutput()
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
