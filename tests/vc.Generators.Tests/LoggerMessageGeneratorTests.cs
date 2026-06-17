using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace VisionaryCoder.Tooling.Generators.Tests;

public sealed class LoggerMessageGeneratorTests
{
    private static (Compilation, IEnumerable<string>) RunGenerator(string source)
    {
        const string attr = "namespace Vc.Generators.Abstractions.Attributes; [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)] public sealed class VcLoggingContextAttribute : System.Attribute {}";
        var trees = ImmutableArray.Create(CSharpSyntaxTree.ParseText(attr), CSharpSyntaxTree.ParseText(source));
        var refs = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic && a.Location.Length > 0).Select(a => MetadataReference.CreateFromFile(a.Location)).Distinct().ToArray();
        var comp = CSharpCompilation.Create("Test", trees, refs, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var driver = CSharpGeneratorDriver.Create(new LoggerMessageGenerator());
        driver.RunGeneratorsAndUpdateCompilation(comp, out var outComp, out _);
        return (outComp, outComp.SyntaxTrees.Skip(2).Select(st => st.GetText().ToString()));
    }

    [Fact]
    public void GeneratesLogClass_ForDecoratedClass()
    {
        var (_, sources) = RunGenerator("using Vc.Generators.Abstractions.Attributes; namespace App; [VcLoggingContext] public class OrderService {}");
        Assert.NotEmpty(sources);
        Assert.Contains(sources, s => s.Contains("OrderServiceLog"));
    }

    [Fact]
    public void GeneratedLog_ContainsLoggerMessageAttributes()
    {
        var (_, sources) = RunGenerator("using Vc.Generators.Abstractions.Attributes; namespace App; [VcLoggingContext] public class OrderService {}");
        var src = sources.First(s => s.Contains("OrderServiceLog"));
        Assert.Contains("[LoggerMessage", src);
    }

    [Fact]
    public void GeneratedLog_ContainsStartedAndCompletedMethods()
    {
        var (_, sources) = RunGenerator("using Vc.Generators.Abstractions.Attributes; namespace App; [VcLoggingContext] public class OrderService {}");
        var src = sources.First(s => s.Contains("OrderServiceLog"));
        Assert.Contains("Started", src);
        Assert.Contains("Completed", src);
        Assert.Contains("Failed", src);
    }

    [Fact]
    public void GeneratedLog_ContainsPublicMethodLogging()
    {
        var (_, sources) = RunGenerator("using Vc.Generators.Abstractions.Attributes; namespace App; [VcLoggingContext] public class OrderService { public void PlaceOrder() {} }");
        var src = sources.First(s => s.Contains("OrderServiceLog"));
        Assert.Contains("PlaceOrderCalled", src);
    }

    [Fact]
    public void GeneratedOutput_ContainsAutoGenHeader()
    {
        var (_, sources) = RunGenerator("using Vc.Generators.Abstractions.Attributes; namespace App; [VcLoggingContext] public class OrderService {}");
        Assert.Contains(sources, s => s.Contains("auto-generated"));
    }

    [Fact]
    public void NonDecoratedClass_ProducesNoOutput()
    {
        var (_, sources) = RunGenerator("namespace App; public class OrderService {}");
        Assert.Empty(sources);
    }
}
